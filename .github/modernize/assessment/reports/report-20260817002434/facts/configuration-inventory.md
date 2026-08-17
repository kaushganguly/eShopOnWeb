# Configuration & Externalized Settings Inventory

eShopOnWeb uses appsettings files, launch profiles, Docker Compose overrides, user secrets, environment variables, and Azure Key Vault integration for configuration. Sensitive values are present as local-development or Docker sample values and should be treated as non-production placeholders.

## Configuration Sources

| Source | Type | Path/Location | Notes |
|---|---|---|---|
| Web appsettings | JSON | `src/Web/appsettings.json` | Base URLs, connection strings, catalog base URL, logging |
| Web environment appsettings | JSON | `src/Web/appsettings.Development.json`, `src/Web/appsettings.Docker.json` | Environment-specific overrides |
| PublicApi appsettings | JSON | `src/PublicApi/appsettings.json` | Base URLs, connection strings, catalog base URL, logging |
| PublicApi environment appsettings | JSON | `src/PublicApi/appsettings.Development.json`, `src/PublicApi/appsettings.Docker.json` | Environment-specific overrides |
| BlazorAdmin appsettings | JSON | `src/BlazorAdmin/wwwroot/appsettings*.json` | Client base URL settings |
| Launch settings | JSON | `src/Web/Properties/launchSettings.json`, `src/PublicApi/Properties/launchSettings.json`, `src/BlazorAdmin/Properties/launchSettings.json` | Local profile URLs and environment |
| Docker Compose | YAML | `docker-compose.yml`, `docker-compose.override.yml` | Container images, ports, environment variables, SQL Edge service |
| User secrets | .NET user secrets | Web and PublicApi UserSecretsId values in csproj | Local secret storage for development |
| Azure Key Vault | External secret store | Configured by `AZURE_KEY_VAULT_ENDPOINT` | Production Web startup adds Key Vault configuration |
| Environment variables | Runtime provider | Added in Web and PublicApi Program.cs | Overrides appsettings at runtime |

## Build Profiles

| Profile | Activation | Purpose | Key Dependencies/Plugins |
|---|---|---|---|
| Debug | MSBuild default or `-c Debug` | Local development build | Standard SDK build |
| Release | `-c Release` | Optimized production build | BuildBundlerMinifier is conditionally included in Web |
| Docker | Docker Compose build | Build Web and PublicApi container images | Dockerfiles and Microsoft.VisualStudio.Azure.Containers.Tools.Targets |
| Central package management | Always | Consistent NuGet versions | Directory.Packages.props with ManagePackageVersionsCentrally |

## Runtime Profiles

| Profile | Activation Method | Config Files | Key Overrides |
|---|---|---|---|
| Development | ASPNETCORE_ENVIRONMENT=Development | appsettings.json plus appsettings.Development.json | Developer exception pages, migrations endpoint, local URLs |
| Docker | ASPNETCORE_ENVIRONMENT=Docker | appsettings.json plus appsettings.Docker.json and docker-compose.override.yml | Container URL binding to port 8080 and Docker service URLs |
| Production or non-development | Any non Development and non Docker environment | appsettings.json plus environment variables and Key Vault | Azure Key Vault configuration and Azure SQL connection string keys |
| Test | PublicApi test host loads appsettings.test.json | tests/PublicApiIntegrationTests/appsettings.test.json | Test-specific configuration for API integration tests |

## Properties Inventory

### Web and PublicApi

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| baseUrls:apiBase | local HTTPS API URL | base, Docker, Development | appsettings files |
| baseUrls:webBase | local HTTPS Web URL | base, Docker, Development | appsettings files |
| ConnectionStrings:CatalogConnection | LocalDB catalog connection | base, Docker, Test | appsettings files or Key Vault indirection |
| ConnectionStrings:IdentityConnection | LocalDB identity connection | base, Docker, Test | appsettings files or Key Vault indirection |
| CatalogBaseUrl | empty | base and overrides | appsettings files |
| Logging:LogLevel:Default | Warning | base and overrides | appsettings files |
| UseOnlyInMemoryDatabase | absent by default | optional and tests | configuration provider |
| AZURE_KEY_VAULT_ENDPOINT | no default | production | environment variable |
| AZURE_SQL_CATALOG_CONNECTION_STRING_KEY | no default | production | environment variable |
| AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY | no default | production | environment variable |
| ASPNETCORE_URLS | http://+:8080 in Docker | Docker | docker-compose.override.yml |
| ASPNETCORE_ENVIRONMENT | Docker in compose | Docker, Development | launchSettings or docker compose |

### BlazorAdmin

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| baseUrls:apiBase | PublicApi base URL | base, Development, Docker | wwwroot appsettings files |
| baseUrls:webBase | Web base URL | base, Development, Docker | wwwroot appsettings files |

## Startup Parameters & Resource Requirements

| Service | JVM/Runtime Options | Memory | Instance Count |
|---|---|---|---|
| eshopwebmvc | ASPNETCORE_ENVIRONMENT=Docker, ASPNETCORE_URLS=http://+:8080 | Not specified | 1 in docker-compose |
| eshoppublicapi | ASPNETCORE_ENVIRONMENT=Docker, ASPNETCORE_URLS=http://+:8080 | Not specified | 1 in docker-compose |
| sqlserver | ACCEPT_EULA=Y and masked SA password | Not specified | 1 in docker-compose |
| Local launch profiles | ASPNETCORE_ENVIRONMENT=Development | Not specified | Developer controlled |

## Startup Dependency Chain

1. `sqlserver` starts as the database container.
2. `eshopwebmvc` waits for `sqlserver` through Docker Compose `depends_on`, without an explicit health check.
3. `eshoppublicapi` waits for `sqlserver` through Docker Compose `depends_on`, without an explicit health check.
4. Web startup seeds CatalogContext and AppIdentityDbContext before serving traffic.
5. PublicApi startup seeds CatalogContext and AppIdentityDbContext before serving traffic.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage (masked) |
|---|---|---|
| ConnectionStrings:CatalogConnection | Database connection string | appsettings or Key Vault reference, value masked in report |
| ConnectionStrings:IdentityConnection | Database connection string | appsettings or Key Vault reference, value masked in report |
| SA_PASSWORD | SQL Edge administrator password | docker-compose sample value masked |
| AZURE_KEY_VAULT_ENDPOINT | Secret store endpoint | environment variable |
| AZURE_SQL_CATALOG_CONNECTION_STRING_KEY | Key Vault secret name | environment variable |
| AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY | Key Vault secret name | environment variable |
| UserSecretsId | Local development secret scope | project files reference user secret store |
| AuthorizationConstants.JWT_SECRET_KEY | JWT signing secret constant | source constant, value not reproduced |

### Secrets Provisioning Workflow

Development reads appsettings and optional .NET user secrets. Docker Compose injects container environment variables and mounts user secret and HTTPS certificate folders. Production Web startup authenticates with AzureDeveloperCliCredential or DefaultAzureCredential, adds Azure Key Vault as a configuration provider, then resolves configured secret names for catalog and identity SQL connection strings before registering DbContexts.

## Feature Flags

| Flag Name | Default | Controlled By |
|---|---|---|
| UseOnlyInMemoryDatabase | false or absent | Configuration key read by Infrastructure.Dependencies |
| EnvironmentName Docker | false unless ASPNETCORE_ENVIRONMENT is Docker | ASPNETCORE_ENVIRONMENT |
| CatalogBaseUrl path base | empty | CatalogBaseUrl configuration |
| Swagger UI | enabled in PublicApi | Always mapped in PublicApi Program.cs |

## Framework & Runtime Versions

| Component | Version | Source |
|---|---:|---|
| Target framework | net8.0 | Directory.Packages.props |
| .NET SDK | 8.0.x rollForward latestFeature | global.json |
| ASP.NET Core package version | 8.0.2 | Directory.Packages.props |
| EF Core package version | 8.0.2 | Directory.Packages.props |
| System.Text.Json | 8.0.3 | Directory.Packages.props |
| Swashbuckle | 6.5.0 | Directory.Packages.props |
| Docker SQL image | mcr.microsoft.com/azure-sql-edge | docker-compose.yml |
| Web Docker base | ASP.NET runtime image from Dockerfile | src/Web/Dockerfile |
| PublicApi Docker base | ASP.NET runtime image from Dockerfile | src/PublicApi/Dockerfile |
