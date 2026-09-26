@description('Azure region for resources.')
param location string

@description('Log Analytics workspace name.')
param name string

@description('Retention in days.')
param retentionInDays int = 30

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: name
  location: location
  properties: {
    retentionInDays: retentionInDays
    sku: {
      name: 'PerGB2018'
    }
  }
}

output workspaceId string = logAnalytics.id
output workspaceName string = logAnalytics.name
