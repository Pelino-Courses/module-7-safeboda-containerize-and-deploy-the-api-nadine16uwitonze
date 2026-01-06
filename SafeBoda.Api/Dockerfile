# Multi-stage Dockerfile for SafeBoda API (built for .NET 8)
# Build from repository root: docker build -f SafeBoda.Api/Dockerfile -t safeboda-api .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies (better layer caching)
COPY SafeBoda.Core/SafeBoda.Core.csproj SafeBoda.Core/
COPY SafeBoda.Application/SafeBoda.Application.csproj SafeBoda.Application/
COPY SafeBoda.Infrastructure/SafeBoda.Infrastructure.csproj SafeBoda.Infrastructure/
COPY SafeBoda.Api/SafeBoda.Api.csproj SafeBoda.Api/

RUN dotnet restore SafeBoda.Api/SafeBoda.Api.csproj

# Copy source code
COPY SafeBoda.Core/ SafeBoda.Core/
COPY SafeBoda.Application/ SafeBoda.Application/
COPY SafeBoda.Infrastructure/ SafeBoda.Infrastructure/
COPY SafeBoda.Api/ SafeBoda.Api/

# Build and publish
WORKDIR /src/SafeBoda.Api
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create data directory for SQLite and set permissions
RUN mkdir -p /app/data && chmod 755 /app/data

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

COPY --from=build /app/out .
COPY SafeBoda.Api/startup.sh /app/startup.sh

# Make startup script executable
RUN chmod +x /app/startup.sh

ENTRYPOINT ["/app/startup.sh"]
