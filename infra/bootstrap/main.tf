data "azurerm_client_config" "current" {}

data "azurerm_subscription" "current" {}

locals {
  tags = merge({
    project    = var.project
    managed-by = "terraform"
    stack      = "bootstrap"
  }, var.tags)

  subject_prefix = coalesce(var.github_oidc_subject_prefix, "repo:${var.github_repository}")


  # Roles the CI identity may hand out (to managed identities only) in infra/main.
  assignable_builtin_roles = [
    "AcrPull",
    "Storage Blob Data Contributor",
    "Key Vault Secrets User",
    "Key Vault Crypto User",
  ]
  assignable_role_ids = concat(
    [for name in local.assignable_builtin_roles : data.azurerm_role_definition.assignable[name].role_definition_id],
    [azurerm_role_definition.apps_power_operator.role_definition_id],
  )
  assignable_role_list = join(", ", local.assignable_role_ids)

  # "Constrain roles and principal types" condition: role assignments may only be
  # created or deleted for the roles above, and only for service principals
  # (managed identities). See https://learn.microsoft.com/azure/role-based-access-control/delegate-role-assignments-examples
  rbac_admin_condition = <<-EOT
    (
     (
      !(ActionMatches{'Microsoft.Authorization/roleAssignments/write'})
     )
     OR
     (
      @Request[Microsoft.Authorization/roleAssignments:RoleDefinitionId] ForAnyOfAnyValues:GuidEquals {${local.assignable_role_list}}
      AND
      @Request[Microsoft.Authorization/roleAssignments:PrincipalType] ForAnyOfAnyValues:StringEqualsIgnoreCase {'ServicePrincipal'}
     )
    )
    AND
    (
     (
      !(ActionMatches{'Microsoft.Authorization/roleAssignments/delete'})
     )
     OR
     (
      @Resource[Microsoft.Authorization/roleAssignments:RoleDefinitionId] ForAnyOfAnyValues:GuidEquals {${local.assignable_role_list}}
      AND
      @Resource[Microsoft.Authorization/roleAssignments:PrincipalType] ForAnyOfAnyValues:StringEqualsIgnoreCase {'ServicePrincipal'}
     )
    )
  EOT

  ci_scopes = merge(
    { shared = azurerm_resource_group.shared.id },
    { for env, rg in azurerm_resource_group.env : env => rg.id },
  )
}

data "azurerm_role_definition" "assignable" {
  for_each = toset(local.assignable_builtin_roles)
  name     = each.value
}

resource "random_string" "suffix" {
  length  = 4
  upper   = false
  special = false
}

# ---------------------------------------------------------------------------
# Resource groups
# ---------------------------------------------------------------------------

resource "azurerm_resource_group" "shared" {
  name     = "rg-${var.project}-shared"
  location = var.location
  tags     = local.tags
}

# Environment resource groups are created here (not in infra/main) so the CI
# identity can be scoped to them without any subscription-level rights.
resource "azurerm_resource_group" "env" {
  for_each = toset(var.environments)

  name     = "rg-${var.project}-${each.key}"
  location = var.location
  tags     = merge(local.tags, { environment = each.key })
}

# ---------------------------------------------------------------------------
# Remote state for infra/main
# ---------------------------------------------------------------------------

resource "azurerm_storage_account" "tfstate" {
  name                            = "sttfstate${var.project}${random_string.suffix.result}"
  resource_group_name             = azurerm_resource_group.shared.name
  location                        = azurerm_resource_group.shared.location
  account_kind                    = "StorageV2"
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  min_tls_version                 = "TLS1_2"
  https_traffic_only_enabled      = true
  allow_nested_items_to_be_public = false

  # Entra ID sign-in only. State contains the generated Postgres password, so no
  # account keys or SAS tokens should be able to read it.
  shared_access_key_enabled       = false
  default_to_oauth_authentication = true

  # GitHub-hosted runners have no fixed IP, so the endpoint stays public and
  # access is controlled by RBAC alone.
  public_network_access_enabled = true

  blob_properties {
    versioning_enabled = true

    delete_retention_policy {
      days = 30
    }

    container_delete_retention_policy {
      days = 30
    }
  }

  tags = local.tags
}

resource "azurerm_storage_container" "tfstate" {
  name                  = "tfstate"
  storage_account_id    = azurerm_storage_account.tfstate.id
  container_access_type = "private"
}

# ---------------------------------------------------------------------------
# Container registry (shared by all environments)
# ---------------------------------------------------------------------------

resource "azurerm_container_registry" "acr" {
  name                = "acr${var.project}${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.shared.name
  location            = azurerm_resource_group.shared.location
  sku                 = "Basic"
  admin_enabled       = false
  tags                = local.tags
}

# ---------------------------------------------------------------------------
# GitHub Actions identity (OIDC, no secrets)
# ---------------------------------------------------------------------------

resource "azurerm_user_assigned_identity" "github" {
  name                = "id-${var.project}-github"
  resource_group_name = azurerm_resource_group.shared.name
  location            = azurerm_resource_group.shared.location
  tags                = local.tags
}

# One federated credential per token subject GitHub Actions presents: pushes to
# the default branch, pull requests, and each deployment environment (a job with
# `environment: dev` gets ...:environment:dev, not the branch ref).
#
# Azure rejects concurrent credential writes on one identity, so they are
# created one after another. When adding several environments at once, run
# apply with -parallelism=1.
resource "azurerm_federated_identity_credential" "github_branch" {
  name                = "github-branch-${var.github_default_branch}"
  resource_group_name = azurerm_resource_group.shared.name
  parent_id           = azurerm_user_assigned_identity.github.id
  audience            = ["api://AzureADTokenExchange"]
  issuer              = "https://token.actions.githubusercontent.com"
  subject             = "${local.subject_prefix}:ref:refs/heads/${var.github_default_branch}"
}

resource "azurerm_federated_identity_credential" "github_pull_request" {
  name                = "github-pull-request"
  resource_group_name = azurerm_resource_group.shared.name
  parent_id           = azurerm_user_assigned_identity.github.id
  audience            = ["api://AzureADTokenExchange"]
  issuer              = "https://token.actions.githubusercontent.com"
  subject             = "${local.subject_prefix}:pull_request"

  depends_on = [azurerm_federated_identity_credential.github_branch]
}

resource "azurerm_federated_identity_credential" "github_environment" {
  for_each = toset(var.environments)

  name                = "github-environment-${each.key}"
  resource_group_name = azurerm_resource_group.shared.name
  parent_id           = azurerm_user_assigned_identity.github.id
  audience            = ["api://AzureADTokenExchange"]
  issuer              = "https://token.actions.githubusercontent.com"
  subject             = "${local.subject_prefix}:environment:${each.key}"

  depends_on = [azurerm_federated_identity_credential.github_pull_request]
}

# Lets the budget shutdown runbook in each environment stop container apps and
# nothing else. Contributor would also work, but is far broader than needed.
resource "azurerm_role_definition" "apps_power_operator" {
  name        = "${title(var.project)} Container Apps Power Operator"
  scope       = data.azurerm_subscription.current.id
  description = "Read, start and stop Azure Container Apps. Used by the budget shutdown runbook."

  permissions {
    actions = [
      "Microsoft.App/containerApps/read",
      "Microsoft.App/containerApps/start/action",
      "Microsoft.App/containerApps/stop/action",
      "Microsoft.Resources/subscriptions/resourceGroups/read",
    ]
  }

  assignable_scopes = [for rg in azurerm_resource_group.env : rg.id]
}

# Contributor on the shared group (push to ACR) and on every environment group.
resource "azurerm_role_assignment" "github_contributor" {
  for_each = local.ci_scopes

  scope                = each.value
  role_definition_name = "Contributor"
  principal_id         = azurerm_user_assigned_identity.github.principal_id
  principal_type       = "ServicePrincipal"
}

# Role assignments limited to the roles in local.assignable_role_ids, for managed
# identities only. Needed on the shared group too, for AcrPull on the registry.
resource "azurerm_role_assignment" "github_rbac_admin" {
  for_each = local.ci_scopes

  scope                = each.value
  role_definition_name = "Role Based Access Control Administrator"
  principal_id         = azurerm_user_assigned_identity.github.principal_id
  principal_type       = "ServicePrincipal"
  condition_version    = "2.0"
  condition            = local.rbac_admin_condition
}

# Key Vault data-plane rights in the environment groups, so CI can write the
# generated Postgres password and create the Data Protection key. Granted here
# rather than in infra/main to avoid waiting for RBAC propagation mid-apply.
resource "azurerm_role_assignment" "github_kv_secrets_officer" {
  for_each = azurerm_resource_group.env

  scope                = each.value.id
  role_definition_name = "Key Vault Secrets Officer"
  principal_id         = azurerm_user_assigned_identity.github.principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "github_kv_crypto_officer" {
  for_each = azurerm_resource_group.env

  scope                = each.value.id
  role_definition_name = "Key Vault Crypto Officer"
  principal_id         = azurerm_user_assigned_identity.github.principal_id
  principal_type       = "ServicePrincipal"
}

# State access for CI and for whoever ran bootstrap (to run infra/main locally).
resource "azurerm_role_assignment" "github_tfstate" {
  scope                = azurerm_storage_account.tfstate.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.github.principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_role_assignment" "operator_tfstate" {
  scope                = azurerm_storage_account.tfstate.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = data.azurerm_client_config.current.object_id
}
