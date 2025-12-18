# Fix "Username and password required" Error

## The Problem
Your workflow is failing with: **"Username and password required"**

This means the GitHub secrets `DOCKER_USERNAME` and `DOCKER_PASSWORD` are missing.

## Quick Fix (5 minutes)

### Step 1: Get Your Docker Hub Username
1. Go to https://hub.docker.com
2. Sign in (or create an account if you don't have one)
3. Your username is shown in the top-right corner
4. **Write it down** (e.g., `nadine16uwitonze`)

### Step 2: Get Your Docker Hub Password/Token

**Option A: Use Your Password (Simple)**
- Use your Docker Hub account password

**Option B: Create Access Token (Recommended - More Secure)**
1. Go to Docker Hub → Click your profile → **Account Settings**
2. Click **Security** in the left menu
3. Click **New Access Token**
4. Token description: `github-actions`
5. Permissions: **Read & Write**
6. Click **Generate**
7. **COPY THE TOKEN IMMEDIATELY** (you won't see it again!)
8. This token is your `DOCKER_PASSWORD`

### Step 3: Add Secrets to GitHub

1. Go to your GitHub repository
2. Click **Settings** (top menu)
3. Click **Secrets and variables** → **Actions** (left sidebar)
4. Click **New repository secret**

**Add First Secret:**
- **Name:** `DOCKER_USERNAME`
- **Secret:** Your Docker Hub username (e.g., `nadine16uwitonze`)
- Click **Add secret**

**Add Second Secret:**
- Click **New repository secret** again
- **Name:** `DOCKER_PASSWORD`
- **Secret:** Your Docker Hub password OR the access token you created
- Click **Add secret**

### Step 4: Verify Secrets Are Added

You should now see in the secrets list:
- ✅ `DOCKER_USERNAME`
- ✅ `DOCKER_PASSWORD`

### Step 5: Test the Fix

1. Make a small change to trigger the workflow:
   ```bash
   echo "# Fixed Docker secrets" >> README.md
   git add README.md
   git commit -m "Test workflow after adding Docker secrets"
   git push origin main
   ```

2. Go to GitHub → **Actions** tab
3. Watch the workflow run
4. The `build-and-push-docker` job should now succeed! ✅

## Visual Guide

```
GitHub Repository
  └── Settings
      └── Secrets and variables
          └── Actions
              ├── New repository secret
              │   ├── Name: DOCKER_USERNAME
              │   └── Secret: your-dockerhub-username
              └── New repository secret
                  ├── Name: DOCKER_PASSWORD
                  └── Secret: your-password-or-token
```

## Troubleshooting

### "Still getting the same error"
- Make sure secret names are EXACTLY: `DOCKER_USERNAME` and `DOCKER_PASSWORD` (case-sensitive)
- Make sure you clicked "Add secret" after entering each one
- Wait a few seconds after adding, then trigger a new workflow run

### "Docker Hub login fails"
- Test locally: `docker login -u YOUR_USERNAME`
- If it fails, your credentials are wrong - reset your Docker Hub password or create a new token

### "Can't find Docker Hub account"
- Sign up at https://hub.docker.com (it's free)
- Verify your email
- Then add the secrets

## What Happens Next

Once the secrets are added:
1. ✅ `build-and-test` job - Already passing
2. ✅ `build-and-push-docker` job - Will now succeed (pushes image to Docker Hub)
3. ✅ `deploy-to-azure` job - Will run and deploy to Azure App Service

Your API will be live at: `https://safeboda-api.azurewebsites.net` (or your App Service URL)

