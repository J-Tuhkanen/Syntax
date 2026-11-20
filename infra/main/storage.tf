# ---------------------------------------------------------------------------
# Blob storage: avatars, Data Protection keys, database backups
# ---------------------------------------------------------------------------

resource "azurerm_storage_account" "blob" {
  name                            = "st${var.project}${var.environment}${local.suffix}"
  resource_group_name             = local.rg_name
  location                        = local.location
  account_kind                    = "StorageV2"
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  access_tier                     = "Hot"
  min_tls_version                 = "TLS1_2"
  https_traffic_only_enabled      = true
  allow_nested_items_to_be_public = false
  shared_access_key_enabled       = false
  default_to_oauth_authentication = true

  # "Enabled from selected networks": the firewall denies everything except the
  # Container Apps subnet (service endpoint). Fully disabling public network
  # access would also block service endpoints, which only private endpoints
  # get around; private endpoints are out of scope for this project.
  public_network_access_enabled = true

  network_rules {
    default_action             = "Deny"
    bypass                     = ["AzureServices"]
    virtual_network_subnet_ids = [azurerm_subnet.aca.id]
  }

  tags = local.tags
}

resource "azurerm_storage_container" "this" {
  for_each = toset(["avatars", "dataprotection", "pg-backups"])

  name                  = each.key
  storage_account_id    = azurerm_storage_account.blob.id
  container_access_type = "private"
}

resource "azurerm_storage_management_policy" "blob" {
  storage_account_id = azurerm_storage_account.blob.id

  rule {
    name    = "expire-pg-backups"
    enabled = true

    filters {
      blob_types   = ["blockBlob"]
      prefix_match = ["pg-backups/"]
    }

    actions {
      base_blob {
        delete_after_days_since_modification_greater_than = var.backup_retention_days
      }
    }
  }
}

# ---------------------------------------------------------------------------
# NFS share for the Postgres data directory (SSD, provisioned v2)
# ---------------------------------------------------------------------------

resource "azurerm_storage_account" "nfs" {
  name                              = "st${var.project}nfs${var.environment}${local.suffix}"
  resource_group_name               = local.rg_name
  location                          = local.location
  account_kind                      = "FileStorage"
  account_tier                      = "Premium"
  account_replication_type          = "LRS"
  provisioned_billing_model_version = "V2"
  min_tls_version                   = "TLS1_2"
  allow_nested_items_to_be_public   = false
  shared_access_key_enabled         = false

  # NFS has no encryption in transit here, so Azure requires "secure transfer
  # required" to be off. Access is limited to the Container Apps subnet below.
  https_traffic_only_enabled = false

  public_network_access_enabled = true

  network_rules {
    default_action             = "Deny"
    bypass                     = ["AzureServices"]
    virtual_network_subnet_ids = [azurerm_subnet.aca.id]
  }

  tags = local.tags
}

resource "azurerm_storage_share" "pgdata" {
  name               = "pgdata"
  storage_account_id = azurerm_storage_account.nfs.id
  quota              = var.postgres_share_size_gib
  enabled_protocol   = "NFS"
}
