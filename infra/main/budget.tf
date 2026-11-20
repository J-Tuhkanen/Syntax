# Monthly budget on the environment resource group:
#   50 % and 80 % of actual spend  -> email
#   100 % of forecast spend        -> email
#   100 % of actual spend          -> email + Action Group -> Automation runbook
#                                     that stops every container app in the group.
# Cost data lags actual usage by several hours, so the shutdown is a safety net,
# not a hard cap. Bring things back by re-running the pipeline or with
# infra/scripts/start-apps.sh.

locals {
  alert_emails = var.alert_email == "" ? [] : [var.alert_email]
}

resource "azurerm_automation_account" "main" {
  name                = "aa-${local.name}"
  location            = local.location
  resource_group_name = local.rg_name
  sku_name            = "Basic"

  identity {
    type = "SystemAssigned"
  }

  tags = local.tags
}

# Custom role created by infra/bootstrap: read, start and stop container apps.
resource "azurerm_role_assignment" "automation_apps_operator" {
  scope                = data.azurerm_resource_group.env.id
  role_definition_name = "${title(var.project)} Container Apps Power Operator"
  principal_id         = azurerm_automation_account.main.identity[0].principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_automation_runbook" "stop_apps" {
  name                    = "Stop-ContainerApps"
  location                = local.location
  resource_group_name     = local.rg_name
  automation_account_name = azurerm_automation_account.main.name
  runbook_type            = "PowerShell"
  log_progress            = false
  log_verbose             = false
  description             = "Stops every container app in the resource group (Postgres last). Triggered by the budget Action Group."
  content                 = file("${path.module}/runbooks/Stop-ContainerApps.ps1")
  tags                    = local.tags
}

resource "time_offset" "webhook_expiry" {
  offset_years = 5
}

resource "azurerm_automation_webhook" "stop_apps" {
  name                    = "budget-shutdown"
  resource_group_name     = local.rg_name
  automation_account_name = azurerm_automation_account.main.name
  runbook_name            = azurerm_automation_runbook.stop_apps.name
  expiry_time             = time_offset.webhook_expiry.rfc3339
  enabled                 = true

  # Fixed parameters; the runbook ignores anything in the request body.
  parameters = {
    subscriptionid    = data.azurerm_client_config.current.subscription_id
    resourcegroupname = local.rg_name
  }
}

resource "azurerm_monitor_action_group" "budget_shutdown" {
  name                = "ag-${local.name}-budget-shutdown"
  resource_group_name = local.rg_name
  short_name          = "budgetstop"

  automation_runbook_receiver {
    name                    = "stop-container-apps"
    automation_account_id   = azurerm_automation_account.main.id
    runbook_name            = azurerm_automation_runbook.stop_apps.name
    webhook_resource_id     = azurerm_automation_webhook.stop_apps.id
    is_global_runbook       = false
    service_uri             = azurerm_automation_webhook.stop_apps.uri
    use_common_alert_schema = true
  }

  tags = local.tags
}

resource "azurerm_consumption_budget_resource_group" "main" {
  name              = "budget-${local.name}"
  resource_group_id = data.azurerm_resource_group.env.id
  amount            = var.budget_amount
  time_grain        = "Monthly"

  time_period {
    # First day of the month the budget is created in.
    start_date = formatdate("YYYY-MM-01'T'00:00:00'Z'", timestamp())
  }

  # Without alert_email, notifications go to the Owners of the resource group
  # (the subscription Owner inherits that role).
  notification {
    enabled        = true
    threshold      = 50
    threshold_type = "Actual"
    operator       = "GreaterThanOrEqualTo"
    contact_emails = local.alert_emails
    contact_roles  = ["Owner"]
  }

  notification {
    enabled        = true
    threshold      = 80
    threshold_type = "Actual"
    operator       = "GreaterThanOrEqualTo"
    contact_emails = local.alert_emails
    contact_roles  = ["Owner"]
  }

  notification {
    enabled        = true
    threshold      = 100
    threshold_type = "Forecasted"
    operator       = "GreaterThanOrEqualTo"
    contact_emails = local.alert_emails
    contact_roles  = ["Owner"]
  }

  notification {
    enabled        = true
    threshold      = 100
    threshold_type = "Actual"
    operator       = "GreaterThanOrEqualTo"
    contact_emails = local.alert_emails
    contact_roles  = ["Owner"]
    contact_groups = [azurerm_monitor_action_group.budget_shutdown.id]
  }

  lifecycle {
    ignore_changes = [time_period]
  }
}
