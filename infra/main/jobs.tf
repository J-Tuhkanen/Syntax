# Database migrations. Started by the pipeline after `terraform apply` and
# before the API rollout; runs db/Migrations with Flyway.
resource "azurerm_container_app_job" "migrate" {
  name                         = local.job_names.migrate
  location                     = local.location
  resource_group_name          = local.rg_name
  container_app_environment_id = azurerm_container_app_environment.main.id
  workload_profile_name        = "Consumption"
  replica_timeout_in_seconds   = 600
  replica_retry_limit          = 1

  manual_trigger_config {
    parallelism              = 1
    replica_completion_count = 1
  }

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

  template {
    container {
      name   = "flyway"
      image  = "${local.acr}/syntax-db-migrate:${var.image_tag}"
      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "DB_HOST"
        value = local.app_names.postgres
      }

      env {
        name  = "DB_PORT"
        value = "5432"
      }

      env {
        name  = "DB_NAME"
        value = local.db_name
      }

      env {
        name  = "FLYWAY_USER"
        value = local.db_user
      }

      env {
        name        = "FLYWAY_PASSWORD"
        secret_name = "postgres-password"
      }
    }
  }

  tags = local.tags

  depends_on = [time_sleep.rbac_propagation, azurerm_container_app.postgres]
}

# Nightly pg_dump to the pg-backups container (expired after
# var.backup_retention_days by the storage lifecycle rule), and a manual job
# that restores the newest dump. Same image, different arguments.
locals {
  pg_tools_jobs = {
    backup = {
      name    = local.job_names.backup
      args    = ["backup"]
      timeout = 1800
    }
    restore = {
      name    = local.job_names.restore
      args    = ["restore", "latest"]
      timeout = 3600
    }
  }
}

resource "azurerm_container_app_job" "pg_tools" {
  for_each = local.pg_tools_jobs

  name                         = each.value.name
  location                     = local.location
  resource_group_name          = local.rg_name
  container_app_environment_id = azurerm_container_app_environment.main.id
  workload_profile_name        = "Consumption"
  replica_timeout_in_seconds   = each.value.timeout
  replica_retry_limit          = 0

  dynamic "schedule_trigger_config" {
    for_each = each.key == "backup" ? [1] : []

    content {
      cron_expression          = var.backup_cron
      parallelism              = 1
      replica_completion_count = 1
    }
  }

  dynamic "manual_trigger_config" {
    for_each = each.key == "restore" ? [1] : []

    content {
      parallelism              = 1
      replica_completion_count = 1
    }
  }

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

  template {
    container {
      name   = "pg-tools"
      image  = "${local.acr}/syntax-pg-backup:${var.image_tag}"
      args   = each.value.args
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "PGHOST"
        value = local.app_names.postgres
      }

      env {
        name  = "PGPORT"
        value = "5432"
      }

      env {
        name  = "PGDATABASE"
        value = local.db_name
      }

      env {
        name  = "PGUSER"
        value = local.db_user
      }

      env {
        name        = "PGPASSWORD"
        secret_name = "postgres-password"
      }

      env {
        name  = "BACKUP_CONTAINER_URL"
        value = "${azurerm_storage_account.blob.primary_blob_endpoint}${azurerm_storage_container.this["pg-backups"].name}"
      }

      env {
        name  = "AZURE_CLIENT_ID"
        value = azurerm_user_assigned_identity.this["db"].client_id
      }
    }
  }

  tags = local.tags

  depends_on = [time_sleep.rbac_propagation, azurerm_container_app.postgres]
}
