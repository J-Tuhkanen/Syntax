data "azurerm_client_config" "current" {}

data "azurerm_resource_group" "env" {
  name = "rg-${var.project}-${var.environment}"
}

data "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = var.acr_resource_group_name
}

# Suffix for globally unique names (Key Vault, storage accounts).
resource "random_string" "suffix" {
  length  = 4
  upper   = false
  special = false
}

locals {
  name     = "${var.project}-${var.environment}"
  location = data.azurerm_resource_group.env.location
  rg_name  = data.azurerm_resource_group.env.name
  suffix   = random_string.suffix.result

  acr = data.azurerm_container_registry.acr.login_server

  app_names = {
    frontend = "ca-${var.project}-frontend-${var.environment}"
    api      = "ca-${var.project}-api-${var.environment}"
    postgres = "ca-${var.project}-postgres-${var.environment}"
  }

  job_names = {
    migrate = "caj-${var.project}-flyway-${var.environment}"
    backup  = "caj-${var.project}-pgbackup-${var.environment}"
    restore = "caj-${var.project}-pgrestore-${var.environment}"
  }

  # The app connects as the database owner created by the postgres image
  # (POSTGRES_USER / POSTGRES_DB). One role keeps things simple for a hobby
  # project; see infra/README.md for how to add a least-privilege role later.
  db_name = var.project
  db_user = var.project

  tags = merge({
    project     = var.project
    environment = var.environment
    managed-by  = "terraform"
  }, var.tags)
}

resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-${local.name}"
  location            = local.location
  resource_group_name = local.rg_name
  sku                 = "PerGB2018"
  retention_in_days   = var.log_retention_days
  daily_quota_gb      = var.log_daily_quota_gb
  tags                = local.tags
}
