terraform {
  
}


variable "rg_name" {
  type = string
  description = "Resource group name that the solution is deployed to"
  
}
variable "solution_prefix" {}
variable "keyvault" {}
variable "fec_access_key"{}
variable "table_affix" {}
variable "process_queues" {}
variable "functions" {}
variable "state_twoletter" {}

data azurerm_client_config "current" { }
data azurerm_subscription "current" {}
resource "azurerm_resource_group" "CandidateWatchRG" {
  name     = var.rg_name
  location = "eastus"
}


resource "azurerm_storage_account" "SolutionStorageAccount" {
  name="st${var.solution_prefix}data01"
 resource_group_name = azurerm_resource_group.CandidateWatchRG.name
 location = azurerm_resource_group.CandidateWatchRG.location
  account_replication_type = "LRS"
  account_tier = "Standard"
}

### USER AND ROLE DEFINITIONS
#managed identity for static site to use to access storage for the solution
resource "azurerm_user_assigned_identity" "solution_worker" {
  location            = azurerm_resource_group.CandidateWatchRG.location
  name                = "solutionworker"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
}

#managed identity APIM
resource "azurerm_user_assigned_identity" "apim_worker" {
  location            = azurerm_resource_group.CandidateWatchRG.location
  name                = "apim_worker"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
}

#roles necessary for data storage access
resource "azurerm_role_assignment" "tableaccessfunctions" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Table Data Contributor"
  principal_id       = azurerm_user_assigned_identity.solution_worker.principal_id
}
resource "azurerm_role_assignment" "storageaccountfunctions" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Account Contributor"
  principal_id       = azurerm_user_assigned_identity.solution_worker.principal_id
}

resource "azurerm_role_assignment" "queueaccessfunctions" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Queue Data Contributor"
  principal_id       = azurerm_user_assigned_identity.solution_worker.principal_id
}

resource "azurerm_role_assignment" "tableaccessapim" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Table Data Reader"
  principal_id       = azurerm_user_assigned_identity.apim_worker.principal_id
}

resource "azurerm_role_assignment" "storageaccountapim" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Account Contributor"
  principal_id       = azurerm_user_assigned_identity.apim_worker.principal_id
}

resource "azurerm_key_vault_access_policy" "candidatewatchaccess" {
  key_vault_id = "/subscriptions/782918b5-c24a-4ef2-8042-1b90bf912ae3/resourceGroups/BatchRendering/providers/Microsoft.KeyVault/vaults/SecVaultPrimary"
  tenant_id    = "${data.azurerm_client_config.current.tenant_id}"
  object_id    = "${azurerm_user_assigned_identity.solution_worker.principal_id}"

  key_permissions = [
    "Get",
  ]

  secret_permissions = [
    "Get",
  ]
}

### END SECURITY

#tables for solution
resource "azurerm_storage_table" "defaulttables" {
  for_each=toset(var.state_twoletter)

  
  name= "${each.key}${var.table_affix}"
  
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
  
}

#queues for data ingestion process
resource "azurerm_storage_queue" "committeeprocess" {
 for_each = toset(var.process_queues)
  name = each.key
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
}

#central app settings, used by solution code to reference certain exteral resources such as table and queue names
resource "azurerm_app_configuration" "solutionConf" {
  name = "conf${var.solution_prefix}"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location = azurerm_resource_group.CandidateWatchRG.location
}
resource "azurerm_api_management" "restapi" {
  name                = "${var.solution_prefix}apim"
  location            = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  publisher_name      = "John McMahon"
  publisher_email     = "john@johnmcmahonart.com"

  sku_name = "Consumption_0"
  }

  resource "azurerm_static_site" "frontend" {
  name                = "frontend"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = "eastus2"
sku_tier = "Standard"
sku_size = "Standard"
  identity {
    type = "UserAssigned"
    identity_ids = ["${azurerm_user_assigned_identity.solution_worker.id}"]
  }

}
resource "azurerm_service_plan" "appserviceplan" {
  name                = "defaultserviceplan"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_windows_function_app" "functionworkers" {
  for_each = toset(var.functions)
  name = each.key
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location

  storage_account_name       = azurerm_storage_account.SolutionStorageAccount.name
  storage_uses_managed_identity = true
  service_plan_id            = azurerm_service_plan.appserviceplan.id
identity {
    type = "UserAssigned"
    identity_ids = ["${azurerm_user_assigned_identity.solution_worker.id}"]
  }

  site_config {}
}

resource "azurerm_public_ip" "frontendip" {
  name                = "frontendip"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  allocation_method   = "Static"

  tags = {
    environment = "Production"
  }
}