locals { 
nsgrules = {
   
    rdp = {
      name                       = "httpsin"
      priority                   = 100
      direction                  = "Inbound"
      access                     = "Allow"
      protocol                   = "Tcp"
      source_port_range          = "*"
      destination_port_range    = "443"
      source_address_prefix      = "Internet"
      destination_address_prefix = "VirtualNetwork"
    }
 
    sql = {
      name                       = "management"
      priority                   = 101
      direction                  = "Inbound"
      access                     = "Allow"
      protocol                   = "Tcp"
      source_port_range          = "*"
      destination_port_range     = "3443"
      source_address_prefix      = "ApiManagement"
      destination_address_prefix = "VirtualNetwork"
    }
 
    apps = {
      name                       = "apps"
      priority                   = 201
      direction                  = "Outbound"
      access                     = "Allow"
      protocol                   = "Tcp"
      source_port_range          = "*"
      destination_port_range     = "*"
      source_address_prefix      = "VirtualNetwork"
      destination_address_prefix = "AppService"
    }
  }
 
}