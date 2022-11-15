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
data "azurerm_key_vault_certificate" "cloudflarecert" {
  name                = "cloudflare"
  key_vault_id = var.keyvault_id
}



locals{
  appconfigkeys = jsondecode(file("./appconfigurationsettingsv1.json"))    
state_twoletter = jsondecode(file("./states.json"))    
configkeys = {for kvpair in local.appconfigkeys.confsettings:kvpair.key => kvpair}
confreferences =flatten([for ref, value in local.appconfigkeys: flatten([for data in value:{
"${data.key}"="${data.value}"


}])])


apibackendbase = tomap({
"AzureWebJobsStorage__credential":"managedidentity",
  "AzureWebJobsStorage__clientId":"${azurerm_user_assigned_identity.apim_worker.client_id}",
  "APPLICATIONINSIGHTS_CONNECTION_STRING":"${data.azurerm_key_vault_secret.appinsightcs.value}",
  "AZURE_CLIENT_ID":"${azurerm_user_assigned_identity.apim_worker.client_id}",
  "netFrameworkVersion":"6.0",


})

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
mergedapiappsettings = merge(local.apibackendbase,local.functionappsettingflat)
} 

  
resource "azurerm_resource_group" "CandidateWatchRG" {
  name     = var.rg_name
  location = "eastus"
}

### NETWORK CONFIGURATION

resource "azurerm_network_security_group" "edgensg"{
name = "EdgeNSG"
location = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
}
resource "azurerm_network_security_rule" "edgerules"{
 
  for_each                    = local.nsgrules 
  name                        = each.key
  direction                   = each.value.direction
  access                      = each.value.access
  priority                    = each.value.priority
  protocol                    = each.value.protocol
  source_port_range           = each.value.source_port_range
  destination_port_range      = each.value.destination_port_range
  source_address_prefix       = each.value.source_address_prefix
  destination_address_prefix  = each.value.destination_address_prefix
  resource_group_name         = azurerm_resource_group.CandidateWatchRG.name
  network_security_group_name = azurerm_network_security_group.edgensg.name 
}



resource "azurerm_virtual_network" "defaultvnet" {
  name = "defaultnetwork"
  location = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  address_space=["10.0.0.0/16"]
  
}
resource azurerm_subnet subnet1 {
    resource_group_name = azurerm_resource_group.CandidateWatchRG.name
    virtual_network_name = azurerm_virtual_network.defaultvnet.name
    name="subnet1"
    address_prefixes=["10.0.1.0/24"]
  
  }
resource "azurerm_subnet_network_security_group_association" "subnetnsgassociation" {
  subnet_id = azurerm_subnet.subnet1.id
  network_security_group_id = azurerm_network_security_group.edgensg.id
}



### END NETWORK



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

resource "azurerm_role_assignment" "blobaccessapim" {
  scope              = data.azurerm_subscription.current.id
  role_definition_name = "Storage Blob Data Owner"
  principal_id       = azurerm_user_assigned_identity.apim_worker.principal_id
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


resource "azurerm_key_vault_access_policy" "apim_said" {
  key_vault_id       = var.keyvault_id
  object_id          = azurerm_api_management.solutionapim.identity.0.principal_id
  tenant_id          = data.azurerm_client_config.current.tenant_id
  certificate_permissions = [
    "Get",
    "List"
  ]
secret_permissions = [
  "Get",
  "List"
]
depends_on = [
  azurerm_api_management.solutionapim
]
}



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

### DEFINE API INFRASTRUCTURE
resource "azurerm_service_plan" "backendserviceplan" {
  name                = "backendserviceplan"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  os_type             = "Windows"
  sku_name            = "B1"
}

/*
resource "azurerm_api_management_certificate" "cloudflarecert" {
  name                = "cloudflare"
  api_management_name = azurerm_api_management.solutionapim.name
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name

  key_vault_secret_id = data.azurerm_key_vault_certificate.cloudflarecert.id
}


resource "azurerm_api_management_custom_domain" "defaultdomain" {
  api_management_id = azurerm_api_management.solutionapim.id

  gateway {
    host_name    = "uscandidatewatch.org"
    key_vault_id = data.azurerm_key_vault_certificate.cloudflarecert.secret_id
  }

  
}
*/
resource "azurerm_api_management" "solutionapim" {
  name                = "${var.solution_prefix}apim"
  location            = azurerm_resource_group.CandidateWatchRG.location
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  publisher_name      = "John McMahon"
  publisher_email     = "john@johnmcmahonart.com"

  
  
  sku_name = "Developer_1"
  identity {
    type = "SystemAssigned"
    
  }
  virtual_network_type = "External"
  virtual_network_configuration {
    subnet_id= azurerm_subnet.subnet1.id
  }
  
  public_ip_address_id = azurerm_public_ip.frontendip.id
  }


resource "azurerm_windows_web_app" "restapiapp" {
    name                = "CW-restapiapp"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  service_plan_id     = azurerm_service_plan.backendserviceplan.id
  
identity {
 type = "UserAssigned"
    identity_ids = ["${azurerm_user_assigned_identity.apim_worker.id}"] 
}
app_settings = local.mergedapiappsettings
  
  site_config {
  always_on = "false"
  use_32_bit_worker = "true"
  application_stack{
    current_stack = "dotnet"
    dotnet_version = "v6.0"
  }  
  }


}

resource "azurerm_api_management_api" "restapi" {
  name                = "restapi"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  api_management_name = azurerm_api_management.solutionapim.name
  revision            = "1"
  display_name        = "RESTAPI"
  path                = "getdata"
  protocols = ["http","https"]
subscription_required = false
service_url = "https://cw-restapiapp.azurewebsites.net"
  import {
    content_format = "openapi+json"
    content_value  = file("./restapiV1definition.json")
  }
}

resource "azurerm_api_management_backend" "apibackend" {
  name                = "defaultbackend"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  api_management_name = azurerm_api_management.solutionapim.name
  protocol            = "http"
  url                 = "https://${azurerm_windows_web_app.restapiapp.name}.azurewebsites.net"

  
}
resource "azurerm_api_management_api_policy" "backendpolicy" {
  api_name            = azurerm_api_management_api.restapi.name
  api_management_name = azurerm_api_management_api.restapi.api_management_name
  resource_group_name = azurerm_api_management_api.restapi.resource_group_name

  xml_content = <<XML
<policies>
  <inbound>
    
    <set-backend-service backend-id="defaultbackend" />
  </inbound>
</policies>
XML
}

resource "azurerm_api_management_api" "frontendroute" {
  name                = "frontend"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  api_management_name = azurerm_api_management.solutionapim.name
  revision            = "1"
  display_name        = "frontend"
  path                = ""
  protocols = ["https"]
subscription_required = false
service_url = "https://${azurerm_static_site.frontend.default_host_name}"
import {
    content_format = "openapi+json"
    content_value  = file("./frontendapiv1definition.json")
      }
}
resource "azurerm_api_management_backend" "frontend" {
  name                = "frontend"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  api_management_name = azurerm_api_management.solutionapim.name
  protocol            = "http"
  url                 = "https://${azurerm_static_site.frontend.default_host_name}"

  
}
resource "azurerm_api_management_api_policy" "frontendpolicy" {
  api_name            = azurerm_api_management_api.frontendroute.name
  api_management_name = azurerm_api_management_api.frontendroute.api_management_name
  resource_group_name = azurerm_api_management_api.frontendroute.resource_group_name

  xml_content = <<XML
<policies>
  <inbound>
    
    
    <set-backend-service backend-id="frontend" />
  </inbound>
</policies>
XML
}




### END API INFRASTRUCTURE

### FRONTEND
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



### END FRONTEND
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

app_settings = local.mergedappsettings
  
}


resource "azurerm_public_ip" "frontendip" {
  name                = "frontendip"
  resource_group_name = azurerm_resource_group.CandidateWatchRG.name
  location            = azurerm_resource_group.CandidateWatchRG.location
  allocation_method   = "Static"
domain_name_label="edge-dns"
sku = "Standard"
  tags = {
    environment = "Production"
  }
}

