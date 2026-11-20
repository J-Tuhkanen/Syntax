output "resource_group_name" {
  value = local.rg_name
}

output "frontend_url" {
  description = "Public URL of the app."
  value       = "https://${azurerm_container_app.frontend.ingress[0].fqdn}"
}

output "frontend_app_name" {
  value = azurerm_container_app.frontend.name
}

output "api_app_name" {
  value = azurerm_container_app.api.name
}

output "postgres_app_name" {
  value = azurerm_container_app.postgres.name
}

output "migrate_job_name" {
  value = azurerm_container_app_job.migrate.name
}

output "backup_job_name" {
  value = azurerm_container_app_job.pg_tools["backup"].name
}

output "restore_job_name" {
  value = azurerm_container_app_job.pg_tools["restore"].name
}

output "acr_login_server" {
  value = local.acr
}

output "key_vault_name" {
  value = azurerm_key_vault.main.name
}

output "blob_storage_account_name" {
  value = azurerm_storage_account.blob.name
}
