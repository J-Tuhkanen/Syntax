variable "subscription_id" {
  description = "Azure subscription to deploy into. Get it with: az account show --query id -o tsv"
  type        = string
}

variable "project" {
  description = "Short project name used in resource names."
  type        = string
  default     = "syntax"

  validation {
    condition     = can(regex("^[a-z][a-z0-9]{1,9}$", var.project))
    error_message = "Use 2-10 lowercase letters or digits (it ends up in storage account names)."
  }
}

variable "location" {
  description = "Azure region."
  type        = string
  default     = "swedencentral"
}

variable "environments" {
  description = "Environments to prepare. Each gets rg-<project>-<env>, CI role assignments and a GitHub environment federated credential. Adding one here plus a .tfvars file in infra/main is all a new environment needs."
  type        = list(string)
  default     = ["dev"]
}

variable "github_repository" {
  description = "GitHub repository in owner/name form."
  type        = string
  default     = "J-Tuhkanen/Syntax"
}

variable "github_default_branch" {
  description = "Branch that deploys."
  type        = string
  default     = "master"
}

variable "github_oidc_subject_prefix" {
  description = <<-EOT
    Prefix of the OIDC 'sub' claim GitHub issues for this repository. Leave null for the
    name-based format (repo:owner/name), which repositories created before 15 July 2026 use.
    If the repository uses immutable subject claims, set it to
    "repo:<owner>@<owner_id>/<repo>@<repo_id>" (see infra/README.md).
  EOT
  type        = string
  default     = null
}

variable "tags" {
  description = "Extra tags for every resource."
  type        = map(string)
  default     = {}
}
