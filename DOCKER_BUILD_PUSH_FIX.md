# Fix "Build and Push Docker" Failure

## Common Causes and Solutions

### 🔴 Issue 1: Missing Docker Hub Secrets (Most Common - 80% of failures)

**Error Messages:**
- `Error: Username or password is incorrect`
- `Error: unauthorized: authentication required`
- `Error: denied: requested access to the resource is denied`

**Solution:**
1. Go to GitHub → Your Repository → **Settings** → **Secrets and variables** → **Actions**
2. Check if these secrets exist:
   - `DOCKER_USERNAME` ❌ Missing?
   - `DOCKER_PASSWORD` ❌ Missing?

3. **If missing, add them:**

   **For DOCKER_USERNAME:**
   - Value: Your Docker Hub username (e.g., `nadine16uwitonze`)
   - Get it: https://hub.docker.com → Sign in → Your username is in the top right

   **For DOCKER_PASSWORD:**
   - **Option A (Simple):** Use your Docker Hub account password
   - **Option B (Recommended):** Create an access token:
     1. Go to https://hub.docker.com → Account Settings → Security
     2. Click "New Access Token"
     3. Name: `github-actions`
     4. Permissions: **Read & Write**
     5. Click "Generate"
     6. **Copy the token immediately** (you won't see it again!)
     7. Use this token as `DOCKER_PASSWORD`

### 🔴 Issue 2: Invalid Docker Hub Credentials

**Error Message:**
```
Error: Username or password is incorrect
```

**Solution:**
1. **Test your credentials locally:**
   ```bash
   docker login -u YOUR_USERNAME
   # Enter your password when prompted
   ```
   If this fails, your credentials are wrong.

2. **If using password:**
   - Make sure you're using your Docker Hub account password (not GitHub password)
   - If you forgot it, reset it at https://hub.docker.com

3. **If using access token:**
   - Make sure you copied the entire token (they're long)
   - Create a new token if the old one expired or was lost

### 🔴 Issue 3: Docker Hub Account Doesn't Exist

**Error Message:**
```
Error: unauthorized: authentication required
```

**Solution:**
1. Go to https://hub.docker.com
2. Click "Sign Up" if you don't have an account
3. Complete the registration
4. Verify your email
5. Add your username and password to GitHub secrets

### 🔴 Issue 4: Dockerfile Path or Build Context Issue

**Error Messages:**
```
Error: failed to solve: failed to compute cache key
Error: failed to read dockerfile
```

**Solution:**
1. Verify the Dockerfile exists at: `SafeBoda.Api/Dockerfile`
2. Check the workflow file (`.github/workflows/main-dockerhub.yml`):
   ```yaml
   context: .                    # Should be repository root
   file: ./SafeBoda.Api/Dockerfile  # Should point to Dockerfile
   ```
3. Make sure the Dockerfile is committed to the repository:
   ```bash
   git add SafeBoda.Api/Dockerfile
   git commit -m "Add Dockerfile"
   git push
   ```

### 🔴 Issue 5: Image Name Format Issue

**Error Message:**
```
Error: invalid reference format
Error: denied: requested access to the resource is denied
```

**Solution:**
The image tag format must be: `username/imagename:tag`

1. Check your `DOCKER_USERNAME` secret:
   - Must be lowercase
   - No special characters (except hyphens/underscores)
   - Example: `nadine16uwitonze` ✅
   - Example: `Nadine16Uwitonze` ❌ (wrong case)

2. The workflow creates tags like:
   - `your-username/safeboda-api:latest`
   - `your-username/safeboda-api:abc123` (commit SHA)

3. Make sure your Docker Hub username matches exactly (case-sensitive)

### 🔴 Issue 6: Docker Hub Rate Limiting

**Error Message:**
```
Error: toomanyrequests: You have reached your pull rate limit
```

**Solution:**
- Docker Hub has rate limits for free accounts
- Wait a few minutes and try again
- Or upgrade to a paid Docker Hub plan

## Step-by-Step Fix

### Step 1: Verify Secrets Exist
1. Go to: `https://github.com/YOUR_USERNAME/YOUR_REPO/settings/secrets/actions`
2. Check for:
   - ✅ `DOCKER_USERNAME`
   - ✅ `DOCKER_PASSWORD`
3. If missing, add them (see Issue 1 above)

### Step 2: Test Docker Hub Login Locally
```bash
docker login -u YOUR_DOCKER_USERNAME
# Enter password when prompted
```
If this works, your credentials are correct.

### Step 3: Verify Workflow Configuration
Open `.github/workflows/main-dockerhub.yml` and check:
- Line 61: `username: ${{ secrets.DOCKER_USERNAME }}` ✅
- Line 62: `password: ${{ secrets.DOCKER_PASSWORD }}` ✅
- Line 71-72: Uses `${{ secrets.DOCKER_USERNAME }}` ✅

### Step 4: Check the Actual Error
1. Go to GitHub → Actions tab
2. Click on the failed workflow run
3. Click on "build-and-push-docker" job
4. Click on "Build and push Docker image" step
5. Look for the red error message
6. Match it to one of the issues above

### Step 5: Retry After Fixing
1. Make a small change to trigger the workflow:
   ```bash
   echo "# Test" >> README.md
   git add README.md
   git commit -m "Test Docker build after fixing secrets"
   git push origin main
   ```
2. Watch the workflow in GitHub Actions tab
3. It should succeed now!

## Quick Checklist

- [ ] Docker Hub account exists and is verified
- [ ] `DOCKER_USERNAME` secret is added (lowercase, matches Docker Hub)
- [ ] `DOCKER_PASSWORD` secret is added (password or access token)
- [ ] Can login to Docker Hub locally: `docker login -u YOUR_USERNAME`
- [ ] Dockerfile exists at `SafeBoda.Api/Dockerfile`
- [ ] Dockerfile is committed to the repository
- [ ] Workflow file uses `${{ secrets.DOCKER_USERNAME }}` (not hardcoded)

## Still Failing?

1. **Check the exact error message** in the workflow logs
2. **Share the error** and I can help diagnose further
3. **Common additional issues:**
   - Network timeout (retry the workflow)
   - Docker Hub maintenance (wait and retry)
   - Repository permissions (make sure repo is public or you have access)

## Alternative: Skip Docker Hub (Use Azure Container Registry)

If Docker Hub continues to cause issues, you can switch to Azure Container Registry:
1. Use `.github/workflows/main.yml` instead
2. Set up Azure Container Registry (see `AZURE_DEPLOYMENT_SETUP.md`)
3. Add ACR secrets instead of Docker Hub secrets

