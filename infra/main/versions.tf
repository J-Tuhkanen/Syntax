terraform {
  required_version = ">= 1.9, < 2.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.81"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.7"
    }
    time = {
      source  = "hashicorp/time"
      version = "~> 0.13"
    }
  }

  # Partial configuration: resource_group_name, storage_account_name,
  # container_name and key are passed with -backend-config (see infra/README.md).
  # Entra ID auth only; the state storage account has shared keys disabled.
  backend "azurerm" {
    use_azuread_auth = true
  }
}

provider "azurerm" {
  # subscription_id comes from ARM_SUBSCRIPTION_ID; in CI, OIDC sign-in comes
  # from ARM_USE_OIDC / ARM_CLIENT_ID / ARM_TENANT_ID.
  storage_use_azuread = true

  # The CI identity has no subscription-level rights; bootstrap registered the
  # resource providers.
  resource_provider_registrations = "none"

  features {
    key_vault {
      # Purging a vault needs subscription-level rights that CI does not have.
      # A destroyed vault stays soft-deleted for 7 days instead.
      purge_soft_delete_on_destroy    = false
      recover_soft_deleted_key_vaults = true
    }

    storage {
      # The storage firewalls only admit the Container Apps subnet, so GitHub
      # runners must never touch the data plane. Everything here (containers,
      # shares, lifecycle rules) goes through the management plane.
      data_plane_available = false
    }
  }
}
