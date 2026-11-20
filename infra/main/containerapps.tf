# Workload-profiles environment with only the (free) Consumption profile:
# Consumption pricing, VNet integration, TCP ingress and NFS volumes.
resource "azurerm_container_app_environment" "main" {
  name                           = "cae-${local.name}"
  location                       = local.location
  resource_group_name            = local.rg_name
  log_analytics_workspace_id     = azurerm_log_analytics_workspace.main.id
  infrastructure_subnet_id       = azurerm_subnet.aca.id
  internal_load_balancer_enabled = false

  workload_profile {
    name                  = "Consumption"
    workload_profile_type = "Consumption"
  }

  tags = local.tags
}

resource "azurerm_container_app_environment_storage" "pgdata" {
  name                         = "pgdata"
  container_app_environment_id = azurerm_container_app_environment.main.id
  nfs_server_url               = azurerm_storage_account.nfs.primary_file_host
  share_name                   = "/${azurerm_storage_account.nfs.name}/${azurerm_storage_share.pgdata.name}"
  access_mode                  = "ReadWrite"
}

# ---------------------------------------------------------------------------
# Postgres
#
# Container Apps starts a new revision before it stops the old one, which would
# put two postmasters on one data directory. So:
#   * Terraform creates this app once and then ignores its template, so
#     `terraform apply` never creates a new revision.
#   * The app pipeline never touches it.
#   * Updates go through infra/scripts/postgres-update.sh (stop, update, start).
#   * The image's entrypoint refuses to start while another postmaster still
#     answers on the app's address (db/postgres/wait-for-previous.sh).
# ---------------------------------------------------------------------------

resource "azurerm_container_app" "postgres" {
  name                         = local.app_names.postgres
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = local.rg_name
  revision_mode                = "Single"
  workload_profile_name        = "Consumption"

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.this["db"].id]
  }

  registry {
    server   = local.acr
    identity = azurerm_user_assigned_identity.this["db"].id
  }

  secret {
    name                = "postgres-password"
    key_vault_secret_id = azurerm_key_vault_secret.postgres_password.versionless_id
    identity            = azurerm_user_assigned_identity.this["db"].id
  }

  ingress {
    external_enabled = false
    transport        = "tcp"
    target_port      = 5432
    exposed_port     = 5432

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  template {
    min_replicas = 1
    max_replicas = 1

    # Postgres image stops with SIGINT ("fast" shutdown); give it time to
    # finish the shutdown checkpoint on NFS before a SIGKILL.
    termination_grace_period_seconds = 60

    container {
      name   = "postgres"
      image  = "${local.acr}/syntax-postgres:${var.postgres_image_tag}"
      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "POSTGRES_USER"
        value = local.db_user
      }

      env {
        name  = "POSTGRES_DB"
        value = local.db_name
      }

      env {
        name        = "POSTGRES_PASSWORD"
        secret_name = "postgres-password"
      }

      # A subdirectory of the mount, so initdb gets an empty directory it owns.
      env {
        name  = "PGDATA"
        value = "/var/lib/postgresql/data/pgdata"
      }

      # Where wait-for-previous.sh looks for a still-running old revision.
      env {
        name  = "PEER_HOST"
        value = local.app_names.postgres
      }

      volume_mounts {
        name = "pgdata"
        path = "/var/lib/postgresql/data"
      }

      startup_probe {
        transport               = "TCP"
        port                    = 5432
        initial_delay           = 10
        interval_seconds        = 20
        timeout                 = 5
        failure_count_threshold = 10
      }

      liveness_probe {
        transport               = "TCP"
        port                    = 5432
        interval_seconds        = 30
        timeout                 = 5
        failure_count_threshold = 5
      }
    }

    volume {
      name         = "pgdata"
      storage_type = "NfsAzureFile"
      storage_name = azurerm_container_app_environment_storage.pgdata.name
    }
  }

  lifecycle {
    ignore_changes = [template]
  }

  tags = local.tags

  depends_on = [time_sleep.rbac_propagation]
}

# ---------------------------------------------------------------------------
# API (internal only, reached through the frontend's nginx)
# ---------------------------------------------------------------------------

resource "azurerm_container_app" "api" {
  name                         = local.app_names.api
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = local.rg_name
  revision_mode                = "Single"
  workload_profile_name        = "Consumption"

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.this["api"].id]
  }

  registry {
    server   = local.acr
    identity = azurerm_user_assigned_identity.this["api"].id
  }

  secret {
    name                = "db-connection-string"
    key_vault_secret_id = azurerm_key_vault_secret.db_connection_string.versionless_id
    identity            = azurerm_user_assigned_identity.this["api"].id
  }

  ingress {
    external_enabled = false
    # Plain HTTP inside the environment (nginx -> api); TLS ends at the
    # frontend's external ingress.
    allow_insecure_connections = true
    target_port                = 8080
    transport                  = "auto"

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  template {
    min_replicas = 0
    # SignalR groups live in memory, so there must never be a second replica.
    max_replicas = 1

    container {
      name   = "api"
      image  = "${local.acr}/syntax-api:${var.image_tag}"
      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }

      env {
        name        = "ConnectionStrings__DefaultConnection"
        secret_name = "db-connection-string"
      }

      # User-assigned identity used by DefaultAzureCredential.
      env {
        name  = "AZURE_CLIENT_ID"
        value = azurerm_user_assigned_identity.this["api"].client_id
      }

      env {
        name  = "Storage__BlobServiceUri"
        value = azurerm_storage_account.blob.primary_blob_endpoint
      }

      env {
        name  = "Storage__AvatarContainer"
        value = azurerm_storage_container.this["avatars"].name
      }

      env {
        name  = "DataProtection__BlobUri"
        value = "${azurerm_storage_account.blob.primary_blob_endpoint}${azurerm_storage_container.this["dataprotection"].name}/keys.xml"
      }

      env {
        name  = "DataProtection__KeyVaultKeyId"
        value = azurerm_key_vault_key.dataprotection.versionless_id
      }

      startup_probe {
        transport               = "HTTP"
        port                    = 8080
        path                    = "/healthz"
        interval_seconds        = 5
        timeout                 = 3
        failure_count_threshold = 10
      }

      liveness_probe {
        transport               = "HTTP"
        port                    = 8080
        path                    = "/healthz"
        interval_seconds        = 30
        timeout                 = 5
        failure_count_threshold = 3
      }
    }
  }

  lifecycle {
    # The pipeline rolls out new images after migrations have run.
    ignore_changes = [template[0].container[0].image]
  }

  tags = local.tags

  depends_on = [time_sleep.rbac_propagation]
}

# ---------------------------------------------------------------------------
# Frontend: nginx serving the Vite build; the only external ingress
# ---------------------------------------------------------------------------

resource "azurerm_container_app" "frontend" {
  name                         = local.app_names.frontend
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = local.rg_name
  revision_mode                = "Single"
  workload_profile_name        = "Consumption"

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.this["frontend"].id]
  }

  registry {
    server   = local.acr
    identity = azurerm_user_assigned_identity.this["frontend"].id
  }

  ingress {
    external_enabled           = true
    allow_insecure_connections = false
    target_port                = 8080
    transport                  = "auto"

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  template {
    min_replicas = 0
    max_replicas = 1

    container {
      name   = "frontend"
      image  = "${local.acr}/syntax-frontend:${var.image_tag}"
      cpu    = 0.25
      memory = "0.5Gi"

      # nginx proxies /api, /notification and /users here.
      env {
        name  = "API_UPSTREAM"
        value = "http://${azurerm_container_app.api.name}"
      }

      liveness_probe {
        transport               = "HTTP"
        port                    = 8080
        path                    = "/healthz"
        interval_seconds        = 30
        timeout                 = 5
        failure_count_threshold = 3
      }
    }
  }

  lifecycle {
    ignore_changes = [template[0].container[0].image]
  }

  tags = local.tags

  depends_on = [time_sleep.rbac_propagation]
}
