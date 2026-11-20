resource "azurerm_virtual_network" "main" {
  name                = "vnet-${local.name}"
  location            = local.location
  resource_group_name = local.rg_name
  address_space       = var.vnet_address_space
  tags                = local.tags
}

# No NSG, NAT gateway or private endpoints: outbound traffic uses the
# environment's default outbound path, and Storage / Key Vault are reached
# through service endpoints on this subnet.
resource "azurerm_subnet" "aca" {
  name                 = "snet-aca"
  resource_group_name  = local.rg_name
  virtual_network_name = azurerm_virtual_network.main.name
  address_prefixes     = [var.aca_subnet_prefix]
  service_endpoints    = ["Microsoft.Storage", "Microsoft.KeyVault"]

  delegation {
    name = "aca-environment"

    service_delegation {
      name    = "Microsoft.App/environments"
      actions = ["Microsoft.Network/virtualNetworks/subnets/join/action"]
    }
  }
}
