resource "azurerm_key_vault" "main" {
  name                       = "kv-${var.project}-${var.environment}-${local.suffix}"
  location                   = local.location
  resource_group_name        = local.rg_name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  rbac_authorization_enabled = true
  purge_protection_enabled   = false
  soft_delete_retention_days = 7

  # Public endpoint on, gated by Entra ID + RBAC, so GitHub runners can write
  # the generated secret. The CI identity's Key Vault roles come from bootstrap.
  public_network_access_enabled = true

  network_acls {
    default_action = "Allow"
    bypass         = "AzureServices"
  }

  tags = local.tags
}

# Note: this value is also stored in Terraform state, which is why the state
# storage account only accepts Entra ID sign-in.
resource "random_password" "postgres" {
  length  = 32
  special = false
}

resource "azurerm_key_vault_secret" "postgres_password" {
  name         = "postgres-password"
  value        = random_password.postgres.result
  key_vault_id = azurerm_key_vault.main.id
  content_type = "text/plain"
}

resource "azurerm_key_vault_secret" "db_connection_string" {
  name         = "db-connection-string"
  value        = "Host=${local.app_names.postgres};Port=5432;Database=${local.db_name};Username=${local.db_user};Password=${random_password.postgres.result};SSL Mode=Disable"
  key_vault_id = azurerm_key_vault.main.id
  content_type = "text/plain"
}

# Wraps the ASP.NET Core Data Protection key ring stored in Blob storage.
resource "azurerm_key_vault_key" "dataprotection" {
  name         = "dataprotection"
  key_vault_id = azurerm_key_vault.main.id
  key_type     = "RSA"
  key_size     = 2048
  key_opts     = ["wrapKey", "unwrapKey"]
}
