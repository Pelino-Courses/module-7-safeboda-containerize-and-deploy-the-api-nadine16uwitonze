# Fix "push access denied - insufficient_scope" Error

## The Problem
The error "push access denied... insufficient_scope: authorization failed" means:
- ✅ Your Docker Hub login is working
- ❌ But your credentials don't have **write/push permissions**

## Solution: Create a New Access Token with Write Permissions

### Step 1: Create New Access Token
1. Go to https://hub.docker.com
2. Click your profile → **Account Settings**
3. Click **Security** in the left menu
4. Scroll to **Access Tokens**
5. Click **New Access Token**

### Step 2: Configure Token Permissions
- **Token description:** `github-actions-write`
- **Permissions:** Select **Read & Write** (NOT just "Read")
- Click **Generate**

### Step 3: Copy the Token
- **IMPORTANT:** Copy the token immediately (you won't see it again!)
- It will look like: `dckr_pat_xxxxxxxxxxxxxxxxxxxxx`

### Step 4: Update GitHub Secret
1. Go to GitHub → Settings → Secrets and variables → Actions
2. Find `DOCKER_PASSWORD`
3. Click the **pencil icon** (edit)
4. **Delete the old value**
5. **Paste the new access token** (the one with Read & Write permissions)
6. Click **Update secret**

### Step 5: Verify Username is Correct
While you're there, also check `DOCKER_USERNAME`:
1. Find `DOCKER_USERNAME`
2. Click the **pencil icon** (edit)
3. Make sure it's exactly: `nadine16uwitonze` (all lowercase, no spaces)
4. If wrong, update it and click "Update secret"

## Alternative: If Using Password Instead of Token

If you prefer to use your Docker Hub password instead of a token:

1. Make sure your Docker Hub account password is correct
2. Update `DOCKER_PASSWORD` secret with your actual password
3. Make sure the password matches what works with `docker login`

**However, using an access token is recommended** because:
- More secure
- Can be revoked without changing your password
- Better for automated workflows

## After Updating

1. **Wait 10 seconds** for secrets to update
2. **Trigger a new workflow run:**
   ```bash
   echo "# Test push permissions" >> README.md
   git add README.md
   git commit -m "Test workflow with write permissions"
   git push origin main
   ```
3. **Watch the workflow** - it should succeed now!

## Why This Happens

- **Read-only tokens** can pull images but can't push
- **Read & Write tokens** can both pull and push images
- The workflow needs **write permissions** to push the built image to Docker Hub

## Still Failing?

If it still fails after creating a token with Read & Write permissions:
1. Make sure you copied the entire token (they're long)
2. Make sure there are no extra spaces in the secret
3. Try creating a fresh token and updating the secret again

