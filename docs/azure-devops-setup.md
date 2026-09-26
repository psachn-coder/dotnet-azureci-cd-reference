# Azure DevOps setup

This reference pipeline expects the following configuration in your Azure DevOps project.

## Service connection

1. Create an Azure Resource Manager service connection (recommended: workload identity federation).
2. Name it consistently with the pipeline variable `Azure.ServiceConnection` (placeholder: `sc-orders-azure`).
3. Grant the identity **Contributor** on the target subscription or resource groups (least privilege: scoped to `rg-orders-dev` and `rg-orders-prod`).

## Variable group: `orders-shared-variables`

| Variable | Example | Notes |
|----------|---------|-------|
| `Azure.ServiceConnection` | `sc-orders-azure` | Service connection name |
| `Dev.ResourceGroupName` | `rg-orders-dev` | Dev resource group |
| `Prod.ResourceGroupName` | `rg-orders-prod` | Prod resource group |
| `Dev.WebAppName` | `app-orders-dev-...` | From Bicep deployment output |
| `Prod.WebAppName` | `app-orders-prod-...` | From Bicep deployment output |
| `SonarCloud.ServiceConnection` | *(optional)* | Leave empty to skip SonarCloud |
| `SonarCloud.Organization` | *(optional)* | SonarCloud org key |
| `SonarCloud.ProjectKey` | *(optional)* | SonarCloud project key |

## Environments

Create **orders-dev** and **orders-prod** under Pipelines → Environments.

- **orders-dev**: optional automatic deployment after build on `main`.
- **orders-prod**: add **Approvals and checks** so production deploys require manual sign-off.

## SonarCloud (optional)

SonarCloud is free for public repositories. Add the service connection and variables above to enable the prepare/analyze/publish tasks in `.azuredevops/templates/build-test.yml`.
