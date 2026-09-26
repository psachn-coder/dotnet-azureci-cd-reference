@description('Azure region for resources.')
param location string

@description('App Service plan name.')
param planName string

@description('Web App name.')
param webAppName string

@description('App Service Plan SKU name (e.g. B1, P1v3).')
param planSkuName string = 'B1'

@description('App Service Plan tier (e.g. Basic, PremiumV3).')
param planSkuTier string = 'Basic'

@description('User-assigned managed identity resource ID.')
param userAssignedIdentityId string

@description('Application Insights connection string.')
param appInsightsConnectionString string

@description('Key Vault URI for configuration references.')
param keyVaultUri string

@description('Container image (optional). When empty, uses .NET code deployment.')
param containerImage string = ''

@description('Linux runtime stack when not using a container image.')
param linuxFxVersion string = 'DOTNETCORE|10.0'

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: planName
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: planSkuName
    tier: planSkuTier
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentityId}': {}
    }
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: empty(containerImage) ? linuxFxVersion : 'DOCKER|${containerImage}'
      alwaysOn: planSkuTier != 'Free' && planSkuTier != 'F1'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'KeyVault__Uri'
          value: keyVaultUri
        }
        {
          name: 'OpenTelemetry__ServiceName'
          value: webAppName
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: empty(containerImage) ? 'false' : 'false'
        }
      ]
    }
  }
}

output webAppId string = webApp.id
output webAppName string = webApp.name
output defaultHostName string = webApp.properties.defaultHostName
