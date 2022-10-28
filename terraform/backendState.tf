terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.8.0"
    }
  }
    backend "azurerm" {
        resource_group_name  = "BatchRendering"
        storage_account_name = "ty7zoiyyzpckistorage"
        container_name       = "tfstate"
        key                  = "$ARM_ACCESS_KEY"
    }

}

provider "azurerm" {
  features {
    key_vault {
      recover_soft_deleted_secrets=false
      purge_soft_deleted_secrets_on_destroy=true
    }
  }
    
  
}

