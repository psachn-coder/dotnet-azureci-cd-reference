using '../main.bicep'

param environmentName = 'dev'
param applicationName = 'orders'
param appServicePlanSkuName = 'B1'
param appServicePlanSkuTier = 'Basic'
param containerImage = ''
