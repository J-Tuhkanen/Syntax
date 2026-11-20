variable "environment" {
  description = "Environment name. The resource group rg-<project>-<environment> must already exist (infra/bootstrap creates it)."
  type        = string
}

variable "project" {
  description = "Short project name used in resource names. Must match infra/bootstrap."
  type        = string
  default     = "syntax"
}

variable "acr_name" {
  description = "Name of the container registry created by infra/bootstrap (output ACR_NAME). CI passes it as TF_VAR_acr_name."
  type        = string
}

variable "acr_resource_group_name" {
  description = "Resource group of the container registry."
  type        = string
  default     = "rg-syntax-shared"
}

variable "image_tag" {
  description = "Tag of the frontend, api, db-migrate and pg-backup images (the git SHA in CI). The frontend and api only use it when they are first created; after that the pipeline rolls them out after migrations."
  type        = string
}

variable "postgres_image_tag" {
  description = "Tag of the syntax-postgres image: <postgres version>-<wrapper revision>. Only used when the Postgres app is first created; later changes go through infra/scripts/postgres-update.sh."
  type        = string
  default     = "17.12-1"
}

variable "alert_email" {
  description = "Address for budget alerts. Leave empty to notify only the subscription Owners (role-based recipients)."
  type        = string
  default     = ""

  validation {
    condition     = var.alert_email == "" || can(regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", var.alert_email))
    error_message = "alert_email must be empty or a valid email address."
  }
}

variable "budget_amount" {
  description = "Monthly budget for the environment resource group, in the billing currency of the subscription (EUR for a Finnish subscription)."
  type        = number
  default     = 30
}

variable "vnet_address_space" {
  type    = list(string)
  default = ["10.20.0.0/16"]
}

variable "aca_subnet_prefix" {
  description = "Subnet for the Container Apps environment (delegated to Microsoft.App/environments)."
  type        = string
  default     = "10.20.0.0/23"
}

variable "log_retention_days" {
  type    = number
  default = 30
}

variable "log_daily_quota_gb" {
  description = "Log Analytics daily ingestion cap. Logs stop being collected for the rest of the day once it is reached."
  type        = number
  default     = 0.2
}

variable "postgres_share_size_gib" {
  description = "Provisioned size of the NFS share holding the Postgres data directory (SSD provisioned v2 minimum is 32 GiB)."
  type        = number
  default     = 32
}

variable "backup_cron" {
  description = "Schedule of the pg_dump job, in UTC."
  type        = string
  default     = "30 1 * * *"
}

variable "backup_retention_days" {
  type    = number
  default = 14
}

variable "tags" {
  type    = map(string)
  default = {}
}
