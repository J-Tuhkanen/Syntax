locals {
  github_variables = {
    AZURE_CLIENT_ID          = azurerm_user_assigned_identity.github.client_id
    AZURE_TENANT_ID          = data.azurerm_client_config.current.tenant_id
    AZURE_SUBSCRIPTION_ID    = data.azurerm_subscription.current.subscription_id
    TF_STATE_RESOURCE_GROUP  = azurerm_resource_group.shared.name
    TF_STATE_STORAGE_ACCOUNT = azurerm_storage_account.tfstate.name
    TF_STATE_CONTAINER       = azurerm_storage_container.tfstate.name
    ACR_NAME                 = azurerm_container_registry.acr.name
  }
}

output "github_variables" {
  description = "Repository variables (not secrets) for GitHub Actions."
  value       = local.github_variables
}

output "gh_variable_commands" {
  description = "Run these with the GitHub CLI from the repository folder to set the variables."
  value = join("\n", [
    for key, value in local.github_variables : "gh variable set ${key} --body \"${value}\""
  ])
}

output "acr_login_server" {
  value = azurerm_container_registry.acr.login_server
}

output "environment_resource_groups" {
  value = { for env, rg in azurerm_resource_group.env : env => rg.name }
}
