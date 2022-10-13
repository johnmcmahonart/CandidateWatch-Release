terraform {
  
}


variable "rg_name" {
  type = string
  description = "Resource group name that the solution is deployed to"
  
}
variable "solution_prefix" {}
variable "keyvault" {}
variable "fec_access_key"{}
variable "committeeprocessQueue"{}
variable "financetotalsQueue" {}
variable "schedulebdetailprocessQueue" {}
variable "schedulebpageprocessQueue"{}


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

#managed identity for static site to use to access storage for the solution
resource "azurerm_user_assigned_identity" "solution_worker" {
  location            = azurerm_resource_group.CandidateWatchRG.location
  name                = "solutionworker"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
}

#roles necessary for data storage access
resource "azurerm_role_assignment" "tableaccess" {
  scope              = azurerm_resource_group.CandidateWatchRG.name
  role_definition_name = "Storage Table Data Contributor"
  principal_id       = azurerm_user_assigned_identity.solution_worker.principal_id
}
resource "azurerm_role_assignment" "queueaccess" {
  scope              = azurerm_resource_group.CandidateWatchRG.name
  role_definition_name = "Storage Queue Data Contributor"
  principal_id       = azurerm_user_assigned_identity.solution_worker.principal_id
}


#default table for solution, more will be created later
resource "azurerm_storage_table" "maryland" {
  name = "MDTable"
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
  
}

#necessary queues for data ingestion process
resource "azurerm_storage_queue" "committeeprocess" {
  name = "${var.committeeprocessQueue}"
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
}
resource "azurerm_storage_queue" "financetotalsprocess" {
  name = "${var.financetotalsQueue}"
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
}
resource "azurerm_storage_queue" "schedulebdetailprocessQueue" {
  name = "${var.schedulebdetailprocessQueue}"
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
}
resource "azurerm_storage_queue" "schedulebpageprocess" {
  name = "${var.schedulebpageprocessQueue}"
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
}

#central app settings, used by solution code to reference certain exteral resources such as table and queue names
resource "azurerm_app_configuration" "solutionConf" {
  name = "conf${var.solution_prefix}"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location = azurerm_resource_group.CandidateWatchRG.location
}
resource "azurerm_api_management" "restapi" {
  name                = "conf${var.solution_prefix}apim"
  location            = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  publisher_name      = "John McMahon"
  publisher_email     = "john@johnmcmahonart.com"

  sku_name = "Consumption_0"
  }

  resource "azurerm_static_site" "frontend" {
  name                = "frontend"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  identity {
    type = "UserAssigned"
    identity_ids = azurerm_user_assigned_identity.solution_worker.id
  }

}
resource "azurerm_service_plan" "appserviceplan" {
  name                = "defaultserviceplan"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  os_type             = "Windows"
  sku_name            = "B1"
}

resource "azurerm_windows_function_app" "example" {
  name                = "example-windows-function-app"
  resource_group_name = azurerm_resource_group.example.name
  location            = azurerm_resource_group.example.location

  storage_account_name       = azurerm_storage_account.example.name
  storage_account_access_key = azurerm_storage_account.example.primary_access_key
  service_plan_id            = azurerm_service_plan.appserviceplan.id

  site_config {}
}