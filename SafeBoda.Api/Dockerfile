FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# All paths in COPY are relative to the Docker build context.
# Build with context at the solution root, e.g.:
#   docker build -t safeboda-api -f SafeBoda.Api/Dockerfile .

WORKDIR /src

# Copy csproj files and restore dependencies first (better layer caching)
COPY SafeBoda.Core/SafeBoda.Core.csproj SafeBoda.Core/
COPY SafeBoda.Application/SafeBoda.Application.csproj SafeBoda.Application/
COPY SafeBoda.Infrastructure/SafeBoda.Infrastructure.csproj SafeBoda.Infrastructure/
COPY SafeBoda.Api/SafeBoda.Api.csproj SafeBoda.Api/

RUN dotnet restore SafeBoda.Api/SafeBoda.Api.csproj

# Copy the rest of the source code
COPY SafeBoda.Core/ SafeBoda.Core/
COPY SafeBoda.Application/ SafeBoda.Application/
COPY SafeBoda.Infrastructure/ SafeBoda.Infrastructure/
COPY SafeBoda.Api/ SafeBoda.Api/

# Publish the API project
WORKDIR /src/SafeBoda.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "SafeBoda.Api.dll"]
