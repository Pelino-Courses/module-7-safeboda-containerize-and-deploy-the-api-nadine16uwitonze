# SafeBoda API Deployment Checklist

Use this checklist to track your progress through the deployment tasks.

## ✅ Task 1: Install Docker Desktop
- [ ] Docker Desktop downloaded and installed
- [ ] Docker Desktop is running
- [ ] Verified with `docker --version` command
- [ ] Successfully built and ran the SafeBoda API container locally

## ✅ Task 2: Add a Dockerfile
- [x] Dockerfile created in `SafeBoda.Api/Dockerfile`
- [x] Multi-stage build implemented
- [x] Dockerfile tested and working

## ✅ Task 3: Add a .dockerignore File
- [x] `.dockerignore` file created
- [x] Unnecessary files excluded (bin/, obj/, etc.)

## ✅ Task 4: Build and Run the Docker Image
- [x] Docker image built successfully: `docker build -t safeboda-api -f SafeBoda.Api/Dockerfile .`
- [x] Container runs successfully: `docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name safeboda-api-container safeboda-api`
- [x] API accessible at `http://localhost:8080/swagger`
- [x] Health endpoint works: `http://localhost:8080/api/health`

## ✅ Task 5: Cloud Provider Setup
- [ ] Azure account created (https://azure.microsoft.com/free/)
- [ ] Azure Portal explored
- [ ] Familiar with Azure App Service concepts

## ✅ Task 6: Create an App Service
- [ ] Resource Group created: `safeboda-rg`
- [ ] Azure Container Registry (ACR) created: `safebodaregistry`
- [ ] ACR Admin user enabled
- [ ] ACR credentials saved (Login server, Username, Password)
- [ ] App Service Plan created: `safeboda-plan`
- [ ] Web App (App Service) created: `safeboda-api`
- [ ] App Service configured to use Docker Container
- [ ] Application settings configured (`WEBSITES_PORT`, `ASPNETCORE_ENVIRONMENT`)

## ✅ Task 7: Configure GitHub Actions Workflow
- [x] `.github/workflows/main.yml` file exists
- [ ] GitHub repository secrets configured:
  - [ ] `AZURE_CREDENTIALS` (service principal JSON)
  - [ ] `AZURE_REGISTRY_LOGIN_SERVER` (ACR login server)
  - [ ] `AZURE_REGISTRY_USERNAME` (ACR username)
  - [ ] `AZURE_REGISTRY_PASSWORD` (ACR password)
- [ ] Workflow environment variables updated in `main.yml`:
  - [ ] `AZURE_WEBAPP_NAME` matches your App Service name
  - [ ] `AZURE_RESOURCE_GROUP` matches your resource group name

## ✅ Task 8: Push Changes and Monitor Workflow
- [ ] Made a small change to code (e.g., updated a comment)
- [ ] Changes committed: `git commit -m "Test CI/CD pipeline"`
- [ ] Changes pushed to main branch: `git push origin main`
- [ ] GitHub Actions workflow triggered
- [ ] Workflow completed successfully:
  - [ ] Build step passed
  - [ ] Test step passed (or skipped)
  - [ ] Docker image built and pushed
  - [ ] Deployment to Azure succeeded
- [ ] API accessible at: `https://your-app-name.azurewebsites.net/api/health`
- [ ] Swagger UI accessible at: `https://your-app-name.azurewebsites.net/swagger`

## 🎉 Success Criteria
- [ ] API is running in Docker locally
- [ ] API is deployed to Azure App Service
- [ ] CI/CD pipeline works automatically on push to main
- [ ] API is accessible via public URL
- [ ] All endpoints are working correctly

## 📝 Notes
- Your App Service URL: `https://________________.azurewebsites.net`
- Your ACR Login Server: `________________.azurecr.io`
- Your Resource Group: `________________`

## 🔧 Troubleshooting
If something fails, check:
1. GitHub Actions logs (Repository → Actions tab)
2. Azure App Service logs (Portal → App Service → Log stream)
3. Container Registry (Portal → Container Registry → Repositories)
4. Application settings in App Service (Configuration → Application settings)






