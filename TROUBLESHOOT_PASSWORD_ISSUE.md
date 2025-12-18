# Troubleshooting: Password Not Working

If your correct password still doesn't work, let's check a few things:

## Step 1: Verify Password Works Locally

Test if you can push an image locally:

```bash
# Make sure you're logged in
docker login -u nadine16uwitonze

# Try to tag and push a test image
docker tag hello-world nadine16uwitonze/test-image:latest
docker push nadine16uwitonze/test-image:latest
```

**If this fails locally**, the issue is with your Docker Hub account, not GitHub.

**If this works locally**, the issue is with how GitHub is using the credentials.

## Step 2: Check Docker Hub Account Status

1. Go to https://hub.docker.com
2. Check if your account is:
   - ✅ **Verified** (check your email)
   - ✅ **Active** (not suspended)
   - ✅ **Free tier** allows pushing (should work)

## Step 3: Verify Secret Values Exactly

1. Go to GitHub → Settings → Secrets → Actions
2. **DOCKER_USERNAME:**
   - Should be exactly: `nadine16uwitonze`
   - No spaces before or after
   - All lowercase
   - Click edit to verify

3. **DOCKER_PASSWORD:**
   - Should be your exact password
   - No extra spaces
   - No line breaks
   - Click edit to verify (you can't see the value, but make sure you entered it correctly)

## Step 4: Check for 2FA or Account Restrictions

1. Go to Docker Hub → Account Settings
2. Check if **Two-Factor Authentication (2FA)** is enabled
   - If 2FA is enabled, you **cannot use password** - you MUST use an access token
   - This is likely the issue!

## Step 5: Create Access Token (Required if 2FA is Enabled)

If 2FA is enabled, you MUST use an access token:

1. Go to: https://hub.docker.com/settings/security
   - Or try: https://hub.docker.com/settings/access-tokens
   - Or: Click your profile → Look for "Access Tokens" or "API Tokens"

2. If you still can't find it, try:
   - Go to: https://hub.docker.com/settings/general
   - Look for a "Security" or "Tokens" tab/link

3. Create token with **Read & Write** permissions

4. Update `DOCKER_PASSWORD` secret with the token

## Step 6: Check the Exact Error Message

Go to GitHub Actions and check the **exact error message**:
1. Click on the failed workflow
2. Click on "build-and-push-docker"
3. Click on "Build and push Docker image"
4. Look for the exact error

Common errors:
- "authentication required" → Wrong password
- "insufficient_scope" → Need token with write permissions (2FA enabled)
- "rate limit" → Wait and try again
- "repository does not exist" → This is OK, Docker Hub will create it

## Step 7: Try Creating Repository Manually

Sometimes Docker Hub requires the repository to exist first:

1. Go to https://hub.docker.com
2. Click "Create Repository" or "Repositories" → "Create"
3. Name: `safeboda-api`
4. Visibility: Public (or Private if you have a paid account)
5. Click "Create"

Then try the workflow again.

## Most Likely Issue: 2FA Enabled

If your account has **Two-Factor Authentication (2FA)** enabled:
- Passwords **will NOT work** for API access
- You **MUST use an access token**
- This is a Docker Hub security requirement

## Quick Test: Check if 2FA is Enabled

1. Go to Docker Hub → Account Settings
2. Look for "Two-Factor Authentication" or "2FA"
3. If it says "Enabled", that's your problem - you need a token

## Solution if 2FA is Enabled

You need to find where to create access tokens. Try these URLs:
- https://hub.docker.com/settings/security
- https://hub.docker.com/settings/access-tokens
- https://hub.docker.com/settings/api-tokens

Or:
1. Go to Docker Hub
2. Click your profile → Account Settings
3. Look through all the tabs/sections for "Tokens" or "Access Tokens"
4. Create a token with **Read & Write** permissions
5. Use that token as `DOCKER_PASSWORD`

