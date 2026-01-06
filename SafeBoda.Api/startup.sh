#!/bin/bash

# Ensure data directory exists
mkdir -p /app/data

# Set proper permissions
chmod 755 /app/data

# Start the application
exec dotnet SafeBoda.Api.dll