# Workflow Consolidation Guide

## Problem
You have **3 workflow files** that all trigger on push to `main`, causing multiple workflows to run simultaneously:
- `.github/workflows/main.yml` (Azure Container Registry)
- `.github/workflows/main-dockerhub.yml` (Docker Hub) ✅ **KEEP THIS ONE**
- `.github/workflows/ci-cd.yml` (Azure Container Registry - old version)

## Solution
I've disabled the automatic triggers for `main.yml` and `ci-cd.yml`. They can still be run manually if needed, but won't trigger on every push.

**Recommended:** Use `main-dockerhub.yml` (Docker Hub version) as it's simpler and doesn't require Azure Container Registry setup.

## What I Changed

1. **Disabled `main.yml`** - Commented out the `push` trigger
2. **Disabled `ci-cd.yml`** - Commented out the `push` trigger
3. **Kept `main-dockerhub.yml` active** - This will continue to run on pushes

## Next Steps

1. **Wait for current workflows to finish** - Let the in-progress ones complete
2. **Check the results** - See which one succeeds or what errors occur
3. **If you prefer Azure Container Registry instead:**
   - Delete or disable `main-dockerhub.yml`
   - Re-enable `main.yml` by uncommenting the push trigger
   - Make sure you have these secrets:
     - `AZURE_REGISTRY_LOGIN_SERVER`
     - `AZURE_REGISTRY_USERNAME`
     - `AZURE_REGISTRY_PASSWORD`

## To Delete Unused Workflows (Optional)

If you're sure you only want one workflow, you can delete the others:

```bash
# Delete the ones you don't need
git rm .github/workflows/main.yml
git rm .github/workflows/ci-cd.yml
git commit -m "Remove duplicate workflows"
git push
```

**OR** keep them disabled (as I did) so you can reference them later if needed.

## Current Status

- ✅ `main-dockerhub.yml` - **ACTIVE** (runs on push to main)
- ⏸️ `main.yml` - **DISABLED** (manual trigger only)
- ⏸️ `ci-cd.yml` - **DISABLED** (manual trigger only)

## Required Secrets for main-dockerhub.yml

Make sure you have these secrets configured:
- `DOCKER_USERNAME` - Your Docker Hub username
- `DOCKER_PASSWORD` - Your Docker Hub password/token
- `AZURE_CREDENTIALS` - Azure service principal JSON
- `AZURE_WEBAPP_NAME` - Your App Service name (or set in workflow env)
- `AZURE_RESOURCE_GROUP` - Your resource group name (or set in workflow env)

