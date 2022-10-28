terraform {
  
}


variable "rg_name" {
  type = string
  description = "Resource group name that the solution is deployed to"
  
}
variable "solution_prefix" {}
variable "keyvault_name" {}
variable "fec_access_key"{}
variable "table_affix" {}
variable "process_queues" {}
variable "functions" {}
#variable "state_twoletter" {}
variable "keyvault_id" {}
data azurerm_client_config "current" { }
data azurerm_subscription "current" {}
data "azurerm_key_vault" "keyvault" {
  name                = "${var.keyvault_name}"
  resource_group_name = "BatchRendering"
}
locals{
  appconfigkeys = jsondecode(file("./appconfigurationsettingsv1.json"))    
state_twoletter = jsondecode(file("./states.json"))    
configkeys = {for kvpair in local.appconfigkeys.confsettings:kvpair.key => kvpair}
confreferences =flatten([for ref, value in local.appconfigkeys: flatten([for data in value:{
"${data.key}"="${data.value}"


}])])


functionappsettingsbase = tomap({
"AzureWebJobsStorage__credential":"managedidentity",
  "AzureWebJobsStorage__clientId":"${azurerm_user_assigned_identity.solution_worker.client_id}",
  "APPLICATIONINSIGHTS_CONNECTION_STRING":"${data.azurerm_key_vault_secret.appinsightcs.value}",
  "AZURE_CLIENT_ID":"${azurerm_user_assigned_identity.solution_worker.client_id}",
  "netFrameworkVersion":"6.0",
  "FUNCTIONS_WORKER_RUNTIME":"dotnet"

}) 
#application settings for functions include references to app config settings in app configuration store
#each function gets this block of settings when built, as well as base settings so the function can connect to application insights using shared user created managed id
functionappsettingflat = {for setting in local.confreferences: keys(setting)[0] =>values(setting)[0]}
mergedappsettings = merge(local.functionappsettingsbase,local.functionappsettingflat)
} 

  
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


#access to app configuration store
resource "azurerm_role_assignment" "appconfigfunctionaccess" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "App Configuration Data Reader"
  principal_id       = azurerm_user_assigned_identity.solution_worker.principal_id
}


resource "azurerm_role_assignment" "appconfigapimaccess" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "App Configuration Data Reader"
  principal_id       = azurerm_user_assigned_identity.apim_worker.principal_id
}

#roles necessary for data storage access
resource "azurerm_role_assignment" "tableaccessfunctions" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Table Data Contributor"
  principal_id       = azurerm_user_assigned_identity.solution_worker.principal_id
}

resource "azurerm_role_assignment" "blobaccessfunctions" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Blob Data Owner"
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
  role_definition_name = "Storage Table Data Contributor"
  principal_id       = azurerm_user_assigned_identity.apim_worker.principal_id
}

resource "azurerm_role_assignment" "storageaccountapim" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Account Contributor"
  principal_id       = azurerm_user_assigned_identity.apim_worker.principal_id
}

resource "azurerm_key_vault_access_policy" "candidatewatchaccess" {
  key_vault_id = var.keyvault_id
  tenant_id    = "${data.azurerm_client_config.current.tenant_id}"
  object_id    = "${azurerm_user_assigned_identity.solution_worker.principal_id}"

  key_permissions = [
    "Get",
  ]
secret_permissions = [
    "Get",
  ]
}

/*
resource "azurerm_key_vault_access_policy" "terraformaccess" {
  key_vault_id = var.keyvault_id
  tenant_id    = "${data.azurerm_client_config.current.tenant_id}"
  object_id    = "8f637ebd-d24d-43b6-9b63-2f5a8bbb9d21"
  #application_id = "bcfa59e0-364b-450d-ae1b-a04ee5ae5a89"
  key_permissions = [
    "Get",
    "Create",
    "Delete",
    "Update",
    "List"
  ]

  secret_permissions = [
    "Get",
    "Set",
    "Delete",
    "List"
    
  ]
}
*/

### END SECURITY

#tables for solution
resource "azurerm_storage_table" "defaulttables" {
  for_each=toset(local.state_twoletter)

  
  name= "${each.key}${var.table_affix}"
  
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
  
}

resource "azurerm_storage_table" "updatelog" {
  

  
  name= "UpdateLog${var.table_affix}"
  
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
  
}

resource "azurerm_storage_table" "solutionconfigurationtable" {
  

  
  name= "SolutionConfiguration${var.table_affix}"
  
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
  
}

resource "azurerm_storage_table_entity" "statesentity" {
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
  table_name           = "${azurerm_storage_table.solutionconfigurationtable.name}"

  partition_key = "Config"
  row_key       = "states"

  entity = {
    allStatesJson = file("./states.json")
  }
}


#queues for data ingestion process
resource "azurerm_storage_queue" "committeeprocess" {
 for_each = toset(var.process_queues)
  name = each.key
  storage_account_name = azurerm_storage_account.SolutionStorageAccount.name
}

#central app settings, used by solution code to reference certain exteral resources such as table and queue names
resource "azurerm_app_configuration" "solutionConf" {
  name = "conffree${var.solution_prefix}"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location = azurerm_resource_group.CandidateWatchRG.location
  #sku = "free"
}

/*
due to the design of the app, there are a lot of hits to the configuration store, so it does not make sense from a cost perspective to store commonly used
config values in the conf store

#add key/value pairs used internally in app code to configuration store

resource "azurerm_app_configuration_key" "configkeys" {
  configuration_store_id = azurerm_app_configuration.solutionConf.id
  for_each = {for kvpair in local.appconfigkeys.confsettings:kvpair.key => kvpair}
  type = "kv"
  key                    = each.key
  label                  = each.key
  value                  = each.value.value

}
*/
resource "azurerm_service_plan" "appserviceplan" {
  name                = "defaultserviceplan"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_api_management" "restapi" {
  name                = "${var.solution_prefix}apim"
  location            = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  publisher_name      = "John McMahon"
  publisher_email     = "john@johnmcmahonart.com"

  sku_name = "Consumption_0"
  identity {
    type = "UserAssigned"
    identity_ids = ["${azurerm_user_assigned_identity.apim_worker.id}"]
  }
  }

/*
resource "azurerm_windows_web_app" "restapiapp" {
  name                = "restapiapp"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  service_plan_id     = azurerm_service_plan.appserviceplan.id

  site_config {}
}
*/
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

resource "azurerm_log_analytics_workspace" "solutionlogs" {
  name                = "cwlogs01"
  location            = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  sku                 = "PerGB2018"
  retention_in_days   = 45
}

resource "azurerm_application_insights" "functionappinsights" {
  name                = "cwfunctioninsights"
  location            = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  workspace_id = azurerm_log_analytics_workspace.solutionlogs.id
  application_type    = "other"
}

resource "azurerm_key_vault_secret" "applicationInsights_ConnectionString" {
  name         = "applicationinsightsConnectionString"
  value        = "${azurerm_application_insights.functionappinsights.connection_string}"
  key_vault_id = var.keyvault_id
depends_on = [
  azurerm_application_insights.functionappinsights
] 

}

data "azurerm_key_vault_secret" "appinsightcs" {
  name         = "applicationinsightsConnectionString"
  key_vault_id = "${data.azurerm_key_vault.keyvault.id}"

depends_on = [
  azurerm_key_vault_secret.applicationInsights_ConnectionString,

]
}



resource "azurerm_windows_function_app" "functionworkers" {
for_each = toset(var.functions)
#output "test" {value=each.value[0]}
#for_each = var.functions


  name = "${each.key}"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location

  storage_account_name       = azurerm_storage_account.SolutionStorageAccount.name
  storage_uses_managed_identity = true
  service_plan_id            = azurerm_service_plan.appserviceplan.id
  
functions_extension_version = "~4"
identity {
    type = "UserAssigned"
    identity_ids = ["${azurerm_user_assigned_identity.solution_worker.id}"]
  }

  site_config {
worker_count = "1"
app_scale_limit = "2"



}
#https://learn.microsoft.com/en-us/azure/azure-functions/functions-reference?tabs=azurewebjobsstorage#common-properties-for-identity-based-connections
app_settings = local.mergedappsettings
  
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

