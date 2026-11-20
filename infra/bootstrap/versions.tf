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
  }

  # Local state on purpose: this configuration creates the storage account that
  # holds the remote state of infra/main. terraform.tfstate is gitignored; keep a
  # copy somewhere safe (it holds no secrets, only resource IDs).
}

provider "azurerm" {
  subscription_id = var.subscription_id

  # Storage accounts here have shared keys disabled, so any data-plane call must
  # use Entra ID. The provider is also told not to make data-plane calls at all,
  # because the person running bootstrap is Owner (control plane only).
  storage_use_azuread = true

  # Bootstrap runs as subscription Owner, so it registers every resource
  # provider that infra/main needs. infra/main (CI) cannot register providers.
  resource_provider_registrations = "core"
  resource_providers_to_register = [
    "Microsoft.App",
    "Microsoft.Automation",
    "Microsoft.Consumption",
    "Microsoft.ContainerRegistry",
    "Microsoft.CostManagement",
    "Microsoft.Insights",
    "Microsoft.KeyVault",
    "Microsoft.ManagedIdentity",
    "Microsoft.Network",
    "Microsoft.OperationalInsights",
    "Microsoft.Storage",
  ]

  features {
    storage {
      data_plane_available = false
    }
  }
}
