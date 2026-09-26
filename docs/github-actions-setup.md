# GitHub Actions CD (OIDC)

The `CD` workflow is **manual** (`workflow_dispatch`) and skips deployment when Azure secrets are missing, so public CI stays green without credentials.

## 1. Create an app registration (federated credential)

1. In Microsoft Entra ID, register an application (e.g. `github-orders-cd`).
2. Add a **Federated credential**:
   - Issuer: `https://token.actions.githubusercontent.com`
   - Subject: `repo:<ORG>/<REPO>:environment:dev` (repeat for `prod` or use a broader subject for testing)
   - Audience: `api://AzureADTokenExchange`
3. Assign **Contributor** (or scoped roles) on the target subscription/resource groups.

## 2. GitHub secrets (repository)

| Secret | Description |
|--------|-------------|
| `AZURE_CLIENT_ID` | App registration client ID |
| `AZURE_TENANT_ID` | Entra tenant ID |
| `AZURE_SUBSCRIPTION_ID` | Target subscription ID |

## 3. GitHub variables

| Variable | Example |
|----------|---------|
| `AZURE_LOCATION` | `westeurope` |
| `DEV_RESOURCE_GROUP` | `rg-orders-dev` |
| `PROD_RESOURCE_GROUP` | `rg-orders-prod` |
| `DEV_WEB_APP_NAME` | From Bicep output `webAppName` |
| `PROD_WEB_APP_NAME` | From Bicep output `webAppName` |

## 4. Environment protection

Create GitHub Environments `dev` and `prod`. Add required reviewers on **prod** to gate production deployments.
