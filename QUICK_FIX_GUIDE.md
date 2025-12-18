# Quick Fix for Failed GitHub Actions Workflow

Your workflow "Containerizer and Deploy Api" is failing. Here's how to fix it quickly:

## Most Likely Issues

### 1. Missing GitHub Secrets (90% of failures)

Your workflow needs these secrets in GitHub. Add them now:

1. **Go to your GitHub repository**
2. **Click Settings** → **Secrets and variables** → **Actions**
3. **Click "New repository secret"** and add these:

#### Required Secrets:

| Secret Name | What It Is | How to Get It |
|------------|------------|---------------|
| `DOCKER_USERNAME` | Your Docker Hub username | Your Docker Hub account username |
| `DOCKER_PASSWORD` | Your Docker Hub password/token | Docker Hub password OR access token (recommended) |
| `AZURE_CREDENTIALS` | Azure service principal JSON | See instructions below |

### 2. Get Docker Hub Credentials

**Option A: Use Password (Simple)**
- Go to https://hub.docker.com
- Sign in or create account
- Use your username and password

**Option B: Use Access Token (Recommended)**
- Go to Docker Hub → Account Settings → Security
- Click "New Access Token"
- Name: `github-actions`
- Permissions: Read & Write
- Copy the token and use it as `DOCKER_PASSWORD`

### 3. Get Azure Credentials

Run this command in Azure Cloud Shell or with Azure CLI:

```bash
az login
az ad sp create-for-rbac --name "github-actions-safeboda" \
  --role contributor \
  --scopes /subscriptions/{YOUR-SUBSCRIPTION-ID}/resourceGroups/safeboda-rg \
  --sdk-auth
```

**Replace `{YOUR-SUBSCRIPTION-ID}` with your actual Azure subscription ID.**

This will output JSON like:
```json
{
  "clientId": "...",
  "clientSecret": "...",
  "subscriptionId": "...",
  "tenantId": "..."
}
```

**Copy the ENTIRE JSON output** and paste it as the `AZURE_CREDENTIALS` secret value.

### 4. Verify Workflow Configuration

Make sure these match your Azure resources:

1. Open `.github/workflows/main-dockerhub.yml`
2. Check these lines (around line 13-14):
   ```yaml
   AZURE_WEBAPP_NAME: safeboda-api    # Must match your App Service name
   AZURE_RESOURCE_GROUP: safeboda-rg  # Must match your resource group
   ```
3. Update if they don't match your actual Azure resources

## Step-by-Step Fix

1. ✅ **Add `DOCKER_USERNAME` secret** (your Docker Hub username)
2. ✅ **Add `DOCKER_PASSWORD` secret** (your Docker Hub password or token)
3. ✅ **Add `AZURE_CREDENTIALS` secret** (the JSON from Azure CLI)
4. ✅ **Verify workflow file** has correct Azure resource names
5. ✅ **Commit and push** a small change to trigger the workflow again

## Test the Fix

After adding all secrets:

1. Make a small change (e.g., add a comment to `Program.cs`)
2. Commit and push:
   ```bash
   git add .
   git commit -m "Fix workflow secrets"
   git push origin main
   ```
3. Go to GitHub → Actions tab
4. Watch the workflow run - it should succeed now!

## Still Failing?

Check the specific error in the workflow logs:
1. Go to Actions tab
2. Click on the failed workflow run
3. Click on the failed job
4. Look for the red error message
5. Common errors:
   - "Username or password is incorrect" → Check Docker Hub credentials
   - "Invalid service principal" → Check Azure credentials JSON format
   - "Resource group not found" → Update `AZURE_RESOURCE_GROUP` in workflow file
   - "App not found" → Update `AZURE_WEBAPP_NAME` in workflow file

## Need Help?

See `WORKFLOW_TROUBLESHOOTING.md` for detailed troubleshooting steps.

