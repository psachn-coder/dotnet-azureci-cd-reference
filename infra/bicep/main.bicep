@description('Deployment environment label (dev, prod).')
@allowed([
  'dev'
  'prod'
])
param environmentName string

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Short application name used in resource naming.')
@minLength(3)
@maxLength(10)
param applicationName string = 'orders'

@description('Optional container image for App Service (leave empty for zip/deploy).')
param containerImage string = ''

@description('App Service Plan SKU name.')
param appServicePlanSkuName string

@description('App Service Plan SKU tier.')
param appServicePlanSkuTier string

var uniqueSuffix = uniqueString(resourceGroup().id, environmentName, applicationName)
var namePrefix = '${applicationName}-${environmentName}-${uniqueSuffix}'

module managedIdentity 'modules/managedIdentity.bicep' = {
  name: 'identity-${environmentName}'
  params: {
    location: location
    name: 'id-${namePrefix}'
  }
}

module logAnalytics 'modules/logAnalytics.bicep' = {
  name: 'log-${environmentName}'
  params: {
    location: location
    name: 'log-${namePrefix}'
    retentionInDays: environmentName == 'prod' ? 90 : 30
  }
}

module appInsights 'modules/appInsights.bicep' = {
  name: 'ai-${environmentName}'
  params: {
    location: location
    name: 'ai-${namePrefix}'
    logAnalyticsWorkspaceId: logAnalytics.outputs.workspaceId
  }
}

module keyVault 'modules/keyVault.bicep' = {
  name: 'kv-${environmentName}'
  params: {
    location: location
    name: take('kv-${replace(namePrefix, '-', '')}', 24)
    managedIdentityPrincipalId: managedIdentity.outputs.principalId
  }
}

module appService 'modules/appService.bicep' = {
  name: 'app-${environmentName}'
  params: {
    location: location
    planName: 'asp-${namePrefix}'
    webAppName: 'app-${namePrefix}'
    planSkuName: appServicePlanSkuName
    planSkuTier: appServicePlanSkuTier
    userAssignedIdentityId: managedIdentity.outputs.identityId
    appInsightsConnectionString: appInsights.outputs.appInsightsConnectionString
    keyVaultUri: keyVault.outputs.keyVaultUri
    containerImage: containerImage
  }
}

output environmentName string = environmentName
output webAppName string = appService.outputs.webAppName
output webAppUrl string = 'https://${appService.outputs.defaultHostName}'
output keyVaultName string = keyVault.outputs.keyVaultName
output managedIdentityClientId string = managedIdentity.outputs.clientId
output appInsightsConnectionString string = appInsights.outputs.appInsightsConnectionString
