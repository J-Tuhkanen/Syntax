# One user-assigned identity per workload, so each only gets what it uses:
#   frontend - pull its image
#   api      - pull, read its connection string, avatars + Data Protection blobs, wrap/unwrap with the DP key
#   db       - pull (postgres, flyway, backup/restore jobs), read the Postgres password, write backups
resource "azurerm_user_assigned_identity" "this" {
  for_each = toset(["frontend", "api", "db"])

  name                = "id-${var.project}-${each.key}-${var.environment}"
  location            = local.location
  resource_group_name = local.rg_name
  tags                = local.tags
}

# The CI identity may only create these roles, and only for service principals
# (bootstrap's RBAC Administrator condition), hence principal_type everywhere.
resource "azurerm_role_assignment" "acr_pull" {
  for_each = azurerm_user_assigned_identity.this

  scope                = data.azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = each.value.principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "api_blob" {
  for_each = toset(["avatars", "dataprotection"])

  scope                = azurerm_storage_container.this[each.key].id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.this["api"].principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "db_backup_blob" {
  scope                = azurerm_storage_container.this["pg-backups"].id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.this["db"].principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "api_connection_string" {
  scope                = azurerm_key_vault_secret.db_connection_string.resource_versionless_id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_user_assigned_identity.this["api"].principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "db_postgres_password" {
  scope                = azurerm_key_vault_secret.postgres_password.resource_versionless_id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_user_assigned_identity.this["db"].principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "api_dataprotection_key" {
  scope                = azurerm_key_vault_key.dataprotection.resource_versionless_id
  role_definition_name = "Key Vault Crypto User"
  principal_id         = azurerm_user_assigned_identity.this["api"].principal_id
  principal_type       = "ServicePrincipal"
}

# Container Apps checks Key Vault references and registry access when a
# revision is created, and new role assignments take a while to propagate.
# Waiting here avoids a failed first apply.
resource "time_sleep" "rbac_propagation" {
  create_duration = "90s"

  triggers = {
    assignments = join(",", concat(
      [for ra in azurerm_role_assignment.acr_pull : ra.id],
      [for ra in azurerm_role_assignment.api_blob : ra.id],
      [
        azurerm_role_assignment.db_backup_blob.id,
        azurerm_role_assignment.api_connection_string.id,
        azurerm_role_assignment.db_postgres_password.id,
        azurerm_role_assignment.api_dataprotection_key.id,
      ],
    ))
  }
}
