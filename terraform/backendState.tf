terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=2.46.0"
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
  features {}
    use_msi=true
  subscription_id = "782918b5-c24a-4ef2-8042-1b90bf912ae3"
  tenant_id = "25244e77-0dac-4267-a423-e69100dc62aa"
}

