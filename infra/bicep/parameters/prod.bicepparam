using '../main.bicep'

param environmentName = 'prod'
param applicationName = 'orders'
param appServicePlanSkuName = 'P1v3'
param appServicePlanSkuTier = 'PremiumV3'
param containerImage = ''
