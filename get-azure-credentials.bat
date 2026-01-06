@echo off
echo Getting Azure credentials for GitHub Actions...
echo.
echo Make sure you're logged in to Azure CLI first by running: az login
echo.
pause

REM Replace these values with your actual resource information
set SUBSCRIPTION_ID=your-subscription-id
set RESOURCE_GROUP=safeboda-rg
set APP_NAME=github-actions-safeboda

echo Creating service principal...
az ad sp create-for-rbac --name "%APP_NAME%" --role contributor --scopes /subscriptions/%SUBSCRIPTION_ID%/resourceGroups/%RESOURCE_GROUP% --sdk-auth

echo.
echo Copy the JSON output above and add it as AZURE_CREDENTIALS secret in GitHub
pause