# GitHub Actions & Azure Deployment Setup Guide

## Prerequisites

### 1. GitHub Repository Setup
- Push your code to GitHub on the `main` branch
- Your repository should be connected to this workspace

### 2. Azure Account Setup
- Create a free Azure account at https://azure.microsoft.com/free/
- Create a resource group for SafeBoda (e.g., `safeboda-rg`)

### 3. Azure App Service Creation
1. In Azure Portal, create a new **App Service**
2. Configure:
   - **Name**: `safeboda-api` (or your preferred name)
   - **Publish**: Docker Container
   - **OS**: Linux
   - **Region**: Select closest to your users
   - **App Service Plan**: Create new (B1 or B2 for testing)

### 4. Container Registry (GitHub Container Registry - GHCR)
- GitHub Container Registry is automatically available with your GitHub account
- No additional setup needed - the workflow uses `secrets.GITHUB_TOKEN`

## Required GitHub Secrets

Add these secrets to your GitHub repository settings (Settings → Secrets and variables → Actions):

```
AZURE_CREDENTIALS          # Azure service principal credentials
AZURE_WEBAPP_NAME          # Your App Service name (e.g., safeboda-api)
AZURE_RESOURCE_GROUP       # Your resource group name (e.g., safeboda-rg)
```

### How to Generate AZURE_CREDENTIALS

1. Open Azure CLI or Azure Cloud Shell
2. Run this command:
```bash
az ad sp create-for-rbac --name "github-actions" --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group-name}
```

3. Copy the entire JSON output and paste it as the `AZURE_CREDENTIALS` secret value

It should look like:
```json
{
  "clientId": "xxxx",
  "clientSecret": "xxxx",
  "subscriptionId": "xxxx",
  "tenantId": "xxxx"
}
```

## Workflow Process

The CI/CD pipeline (`.github/workflows/main.yml`) performs these steps:

### Step 1: Build & Test
- Checks out your code
- Sets up .NET 8.0
- Restores NuGet packages
- Builds the SafeBoda.Api project in Release mode
- Runs unit tests

### Step 2: Build & Push Docker Image
- Logs into GitHub Container Registry (GHCR)
- Builds Docker image from `SafeBoda.Api/Dockerfile`
- Tags image with:
  - `latest` tag
  - Git commit SHA for version tracking
- Pushes image to GHCR

### Step 3: Deploy to Azure
- Logs into Azure using service principal credentials
- Updates App Service container configuration
- Azure pulls and runs the new Docker image

## Dockerfile Requirements

Ensure your `SafeBoda.Api/Dockerfile` exists and is properly configured:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SafeBoda.Api/SafeBoda.Api.csproj", "SafeBoda.Api/"]
COPY ["SafeBoda.Core/SafeBoda.Core.csproj", "SafeBoda.Core/"]
COPY ["SafeBoda.Application/SafeBoda.Application.csproj", "SafeBoda.Application/"]
COPY ["SafeBoda.Infrastructure/SafeBoda.Infrastructure.csproj", "SafeBoda.Infrastructure/"]
RUN dotnet restore "SafeBoda.Api/SafeBoda.Api.csproj"
COPY . .
RUN dotnet build "SafeBoda.Api/SafeBoda.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SafeBoda.Api/SafeBoda.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "SafeBoda.Api.dll"]
```

## Deployment Steps

### 1. Make Changes to Your Code
```bash
# Edit your code locally
git add .
git commit -m "Feature: Add new functionality"
```

### 2. Push to Main Branch
```bash
git push origin main
```

### 3. Monitor Workflow
- Go to your GitHub repository
- Click **Actions** tab
- Watch the workflow run
- Check logs for any errors

### 4. Verify Deployment
- Go to Azure Portal
- Navigate to your App Service
- Click **Overview** → **URL** to view your deployed application

## Troubleshooting

### Workflow Fails at Docker Build
- Check `SafeBoda.Api/Dockerfile` exists and is valid
- Ensure all project references in Dockerfile match your solution structure
- Run locally: `docker build -f SafeBoda.Api/Dockerfile .`

### Deployment to Azure Fails
- Verify `AZURE_CREDENTIALS` secret is valid and properly formatted JSON
- Check `AZURE_WEBAPP_NAME` and `AZURE_RESOURCE_GROUP` are correct
- Ensure App Service has Docker container configuration enabled

### Docker Image Not Found
- Verify GitHub Container Registry login is working
- Check image tag matches what App Service is pulling
- Ensure token has correct permissions

## Local Testing

### Test Docker Build Locally
```bash
docker build -f SafeBoda.Api/Dockerfile -t safeboda-api:test .
docker run -p 8080:80 safeboda-api:test
```

### Test Locally with Docker Compose
Create `docker-compose.yml`:
```yaml
version: '3.8'
services:
  api:
    build:
      context: .
      dockerfile: SafeBoda.Api/Dockerfile
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

Then run:
```bash
docker-compose up
```

## Next Steps

1. ✅ Ensure `.github/workflows/main.yml` is in your repository
2. ✅ Add required secrets to GitHub repository
3. ✅ Create/verify Azure resources
4. ✅ Push code to main branch to trigger workflow
5. ✅ Monitor GitHub Actions for successful deployment
6. ✅ Verify application is running on Azure App Service URL

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure App Service Documentation](https://learn.microsoft.com/en-us/azure/app-service/)
- [Docker for .NET Core](https://docs.microsoft.com/en-us/virtualization/windowscontainers/quick-start/quick-start-windows-10-linux-containers)
- [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)
