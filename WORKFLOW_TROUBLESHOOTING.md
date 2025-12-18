# GitHub Actions Workflow Troubleshooting Guide

## Common Issues and Solutions

### Issue 1: Workflow Fails at "Log in to Docker Hub"

**Error Message:**
```
Error: Username or password is incorrect
```

**Solution:**
1. Go to your GitHub repository → Settings → Secrets and variables → Actions
2. Add these secrets if missing:
   - `DOCKER_USERNAME` - Your Docker Hub username
   - `DOCKER_PASSWORD` - Your Docker Hub password or access token

**To get Docker Hub credentials:**
- If you don't have a Docker Hub account: Sign up at https://hub.docker.com
- For password: Use your Docker Hub password OR create an access token:
  - Go to Docker Hub → Account Settings → Security → New Access Token
  - Create a token with read/write permissions
  - Use the token as `DOCKER_PASSWORD`

### Issue 2: Workflow Fails at "Azure Login"

**Error Message:**
```
Error: Invalid service principal credentials
```

**Solution:**
1. Verify `AZURE_CREDENTIALS` secret exists in GitHub
2. The secret should be a JSON object with this format:
   ```json
   {
     "clientId": "your-client-id",
     "clientSecret": "your-client-secret",
     "subscriptionId": "your-subscription-id",
     "tenantId": "your-tenant-id"
   }
   ```

**To create Azure credentials:**
```bash
az login
az ad sp create-for-rbac --name "github-actions-safeboda" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/safeboda-rg \
  --sdk-auth
```
Copy the entire JSON output and paste it as the `AZURE_CREDENTIALS` secret.

### Issue 3: Workflow Fails at "Build and push Docker image"

**Error Message:**
```
Error: failed to solve: failed to compute cache key
```

**Solution:**
- Check that the Dockerfile path is correct: `./SafeBoda.Api/Dockerfile`
- Verify the build context is set to `.` (repository root)
- Ensure all required project files are committed to the repository

### Issue 4: Workflow Fails at "Deploy to Azure App Service"

**Error Message:**
```
Error: Resource group 'safeboda-rg' could not be found
```

**Solution:**
1. Update the environment variables in the workflow file:
   ```yaml
   env:
     AZURE_WEBAPP_NAME: your-actual-app-name
     AZURE_RESOURCE_GROUP: your-actual-resource-group
   ```
2. Make sure these match exactly what you created in Azure Portal

### Issue 5: "DOCKER_USERNAME" Contains Placeholder Value

**Error Message:**
```
Error: invalid reference format
```

**Solution:**
The workflow file has a placeholder that needs to be replaced:

1. Open `.github/workflows/main-dockerhub.yml`
2. Find line 16:
   ```yaml
   DOCKER_USERNAME: your-dockerhub-username  # Replace with your Docker Hub username
   ```
3. Replace `your-dockerhub-username` with your actual Docker Hub username
4. OR use a GitHub secret instead (recommended):
   - Remove `DOCKER_USERNAME` from `env` section
   - Update line 72-73 to use `${{ secrets.DOCKER_USERNAME }}` instead of `${{ env.DOCKER_USERNAME }}`

### Issue 6: Workflow Not Running

**Possible Causes:**
1. Workflow file is not in `.github/workflows/` directory
2. File doesn't have `.yml` or `.yaml` extension
3. YAML syntax error (check for indentation issues)
4. Workflow is disabled in repository settings

**Solution:**
- Verify file location: `.github/workflows/main-dockerhub.yml`
- Check YAML syntax using an online validator
- Go to repository Settings → Actions → General → Ensure "Allow all actions" is enabled

## How to Check Workflow Logs

1. Go to your GitHub repository
2. Click the "Actions" tab
3. Click on the failed workflow run
4. Click on the failed job (e.g., "build-and-push-docker")
5. Click on the failed step to see detailed error messages
6. Look for red error messages that indicate what went wrong

## Quick Fix Checklist

- [ ] All required GitHub secrets are added:
  - [ ] `DOCKER_USERNAME`
  - [ ] `DOCKER_PASSWORD`
  - [ ] `AZURE_CREDENTIALS`
- [ ] `DOCKER_USERNAME` in workflow file is replaced with actual username OR using secret
- [ ] Azure resource names match in workflow:
  - [ ] `AZURE_WEBAPP_NAME`
  - [ ] `AZURE_RESOURCE_GROUP`
- [ ] Docker Hub account exists and credentials are correct
- [ ] Azure resources are created and accessible
- [ ] Workflow file has correct YAML syntax

## Testing the Workflow Step by Step

1. **Test Build Step:**
   - The workflow should pass "build-and-test" job
   - If it fails, check .NET version and project structure

2. **Test Docker Build:**
   - If "build-and-push-docker" fails, check Docker Hub credentials
   - Verify Dockerfile is correct

3. **Test Deployment:**
   - If "deploy-to-azure" fails, check Azure credentials
   - Verify App Service exists and is configured correctly

## Still Having Issues?

1. Check the specific error message in the workflow logs
2. Verify all secrets are set correctly
3. Ensure Azure resources are created
4. Make sure you're using the correct workflow file (check which one is actually running)

