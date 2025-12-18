# Azure Deployment Setup Guide for SafeBoda API

This guide will walk you through setting up Azure App Service and configuring GitHub Actions to automatically deploy your SafeBoda API.

## Prerequisites

- ✅ Azure free account (sign up at https://azure.microsoft.com/free/)
- ✅ GitHub repository with your SafeBoda API code
- ✅ Docker Desktop installed and working locally

## Task 5: Azure Account Setup

### Step 1: Create Azure Account
1. Go to https://azure.microsoft.com/free/
2. Click "Start free" and sign up with your Microsoft account
3. Complete the verification process
4. You'll get $200 in free credits for 30 days, plus free services for 12 months

### Step 2: Explore Azure App Service
- Azure App Service is a platform-as-a-service (PaaS) that hosts web apps and APIs
- It supports multiple deployment options including Docker containers
- Visit https://portal.azure.com to explore the Azure Portal

## Task 6: Create Azure App Service

### Option A: Using Azure Portal (Recommended for Beginners)

1. **Log in to Azure Portal**
   - Go to https://portal.azure.com
   - Sign in with your Azure account

2. **Create a Resource Group**
   - Click "Create a resource" → Search for "Resource group"
   - Click "Create"
   - Name: `safeboda-rg` (or your preferred name)
   - Region: Choose closest to you (e.g., "East US")
   - Click "Review + create" → "Create"

3. **Create Azure Container Registry (ACR)**
   - Click "Create a resource" → Search for "Container Registry"
   - Click "Create"
   - **Resource group**: Select `safeboda-rg`
   - **Registry name**: `safebodaregistry` (must be globally unique, lowercase, alphanumeric)
   - **Location**: Same as resource group
   - **SKU**: Basic (cheapest option)
   - Click "Review + create" → "Create"
   - Wait for deployment to complete
   - **Note down**: Registry name (e.g., `safebodaregistry.azurecr.io`)

4. **Enable Admin User for ACR**
   - Go to your Container Registry in Azure Portal
   - Click "Access keys" in the left menu
   - Toggle "Admin user" to **Enabled**
   - **Copy and save**:
     - Login server (e.g., `safebodaregistry.azurecr.io`)
     - Username
     - Password (either password)

5. **Create App Service Plan**
   - Click "Create a resource" → Search for "App Service Plan"
   - Click "Create"
   - **Resource group**: `safeboda-rg`
   - **Name**: `safeboda-plan`
   - **Operating System**: Linux
   - **Region**: Same as resource group
   - **Pricing tier**: Free F1 (for testing) or Basic B1 (for production)
   - Click "Review + create" → "Create"

6. **Create Web App (App Service)**
   - Click "Create a resource" → Search for "Web App"
   - Click "Create"
   - **Resource group**: `safeboda-rg`
   - **Name**: `safeboda-api` (must be globally unique)
   - **Publish**: Docker Container
   - **Operating System**: Linux
   - **Region**: Same as resource group
   - **App Service Plan**: Select `safeboda-plan`
   - Click "Review + create" → "Create"
   - Wait for deployment to complete

7. **Configure App Service for Docker Container**
   - Go to your App Service (`safeboda-api`) in Azure Portal
   - Click "Deployment Center" in the left menu
   - **Source**: Azure Container Registry
   - **Registry**: Select your registry (`safebodaregistry`)
   - **Image**: `safeboda-api`
   - **Tag**: `latest`
   - Click "Save"
   
   - Go to "Configuration" → "Application settings"
   - Add these settings:
     - `WEBSITES_PORT`: `8080` (the port your app listens on)
     - `ASPNETCORE_ENVIRONMENT`: `Production` (or `Development` for testing)
   - Click "Save"
   
   - Go to "Overview" → Click "Restart" to apply changes

### Option B: Using Azure CLI (Advanced)

If you have Azure CLI installed, you can run these commands:

```bash
# Login to Azure
az login

# Create resource group
az group create --name safeboda-rg --location eastus

# Create Container Registry
az acr create --resource-group safeboda-rg --name safebodaregistry --sku Basic

# Enable admin user
az acr update -n safebodaregistry --admin-enabled true

# Create App Service Plan
az appservice plan create --name safeboda-plan --resource-group safeboda-rg --sku FREE --is-linux

# Create Web App
az webapp create --resource-group safeboda-rg --plan safeboda-plan --name safeboda-api --deployment-container-image-name nginx
```

## Task 7: Configure GitHub Secrets

You need to add secrets to your GitHub repository so the workflow can authenticate with Azure.

### Step 1: Get Azure Credentials

1. **Get ACR Credentials** (from Task 6, Step 4):
   - Login server: `safebodaregistry.azurecr.io`
   - Username: (from Access keys)
   - Password: (from Access keys)

2. **Get Azure Service Principal** (for App Service deployment):
   
   **Option A: Using Azure Portal**
   - Go to Azure Portal → "Azure Active Directory"
   - Click "App registrations" → "New registration"
   - Name: `github-actions-safeboda`
   - Click "Register"
   - **Note down**: Application (client) ID
   - Click "Certificates & secrets" → "New client secret"
   - Description: `GitHub Actions`
   - Expires: 24 months
   - Click "Add"
   - **Copy and save immediately**: The secret value (you won't see it again!)

   - Go to your Resource Group (`safeboda-rg`)
   - Click "Access control (IAM)" → "Add" → "Add role assignment"
   - Role: "Contributor"
   - Assign access to: "User, group, or service principal"
   - Select: `github-actions-safeboda`
   - Click "Review + assign"

   **Option B: Using Azure CLI** (easier)
   ```bash
   # Create service principal
   az ad sp create-for-rbac --name "github-actions-safeboda" \
     --role contributor \
     --scopes /subscriptions/{subscription-id}/resourceGroups/safeboda-rg \
     --sdk-auth
   ```
   This outputs JSON with all credentials needed.

### Step 2: Add Secrets to GitHub

1. Go to your GitHub repository
2. Click "Settings" → "Secrets and variables" → "Actions"
3. Click "New repository secret" and add these secrets:

   | Secret Name | Value | Description |
   |------------|-------|-------------|
   | `AZURE_CREDENTIALS` | JSON from service principal | Full JSON output from `az ad sp create-for-rbac` |
   | `AZURE_REGISTRY_LOGIN_SERVER` | `safebodaregistry.azurecr.io` | Your ACR login server |
   | `AZURE_REGISTRY_USERNAME` | ACR username | From ACR Access keys |
   | `AZURE_REGISTRY_PASSWORD` | ACR password | From ACR Access keys |

   **For AZURE_CREDENTIALS**, if using Azure CLI, paste the entire JSON output. If using Portal, create JSON:
   ```json
   {
     "clientId": "your-client-id",
     "clientSecret": "your-client-secret",
     "subscriptionId": "your-subscription-id",
     "tenantId": "your-tenant-id"
   }
   ```

## Task 8: Update Workflow Configuration

The workflow file (`.github/workflows/main.yml`) is already configured, but you may need to update these environment variables:

1. Open `.github/workflows/main.yml`
2. Update these values in the `env` section:
   ```yaml
   env:
     AZURE_WEBAPP_NAME: safeboda-api    # Your App Service name
     AZURE_RESOURCE_GROUP: safeboda-rg  # Your resource group name
     DOCKER_IMAGE_NAME: safeboda-api
   ```

## Task 9: Test the Deployment

1. **Make a small change** to your code (e.g., update a comment in `Program.cs`)
2. **Commit and push** to the `main` branch:
   ```bash
   git add .
   git commit -m "Test CI/CD pipeline"
   git push origin main
   ```
3. **Monitor the workflow**:
   - Go to your GitHub repository
   - Click the "Actions" tab
   - You should see "CI/CD Pipeline" running
   - Click on it to see detailed logs
   - Wait for all steps to complete (Build → Test → Build Docker → Deploy)

4. **Verify deployment**:
   - Once deployment completes, go to Azure Portal
   - Navigate to your App Service (`safeboda-api`)
   - Click "Overview" → Copy the "Default domain" URL
   - Open in browser: `https://your-app-name.azurewebsites.net/api/health`
   - You should see: `{"status":"healthy"}`

## Troubleshooting

### Workflow fails at "Log in to Azure Container Registry"
- Verify `AZURE_REGISTRY_LOGIN_SERVER`, `AZURE_REGISTRY_USERNAME`, and `AZURE_REGISTRY_PASSWORD` secrets are correct
- Ensure Admin user is enabled in ACR

### Workflow fails at "Azure Login"
- Verify `AZURE_CREDENTIALS` secret contains valid JSON
- Ensure service principal has Contributor role on the resource group

### App Service shows "Application Error"
- Check App Service logs: Portal → App Service → "Log stream"
- Verify the Docker image was pushed successfully to ACR
- Check that the container is configured to listen on port 80 or 8080

### Container fails to start
- Check App Service → "Configuration" → "Application settings"
- Ensure `ASPNETCORE_ENVIRONMENT` is set if needed
- Verify port mapping in Dockerfile matches App Service expectations

## Next Steps

- Set up custom domain (optional)
- Configure SSL/TLS certificates
- Set up staging environments
- Add environment-specific configuration
- Monitor application performance with Application Insights

## Alternative: Using Docker Hub (Simpler)

If you prefer not to use Azure Container Registry, you can use Docker Hub instead:

1. Create a Docker Hub account at https://hub.docker.com
2. Update the workflow to push to Docker Hub instead of ACR
3. Configure App Service to pull from Docker Hub

See `.github/workflows/main-dockerhub.yml` for an example (if created).

## Resources

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Deploy to App Service using GitHub Actions](https://docs.microsoft.com/azure/app-service/deploy-github-actions)
- [Azure Container Registry Documentation](https://docs.microsoft.com/azure/container-registry/)

