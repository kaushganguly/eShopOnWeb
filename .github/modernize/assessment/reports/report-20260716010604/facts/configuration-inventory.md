# Configuration & Externalized Settings Inventory

Configuration is layered through appsettings files, launch profiles, docker compose overrides, environment variables, and optional Azure Key Vault integration for production secrets.

## Configuration Sources

| Source | Type | Path/Location | Notes |
|---|---|---|---|
| Web appsettings | JSON | `src/Web/appsettings*.json` | Base + Development + Docker variants |
| PublicApi appsettings | JSON | `src/PublicApi/appsettings*.json` | Base + Development + Docker variants |
| BlazorAdmin settings | JSON | `src/BlazorAdmin/wwwroot/appsettings*.json` | Client-facing base URLs by environment |
| Launch profiles | JSON | `src/*/Properties/launchSettings.json` | Dev/prod launch URLs and ASPNETCORE_ENVIRONMENT |
| Docker compose | YAML | `docker-compose.yml`, `docker-compose.override.yml` | Service composition, env vars, mounted secrets paths |
| Environment variables | Process env | runtime | `ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS`, KeyVault key aliases |
| Azure Key Vault | External secret store | configured in Web Program | Used in non-development path with chained credential |

## Build Profiles

| Profile | Activation | Purpose | Key Dependencies/Plugins |
|---|---|---|---|
| Debug | Default local build config | Developer iteration | .NET SDK build defaults |
| Release | `-c Release` (Dockerfile publish/build) | Optimized deployment artifacts | .NET SDK publish pipeline |
| Docker image build | `docker compose build` | Containerized deployment packaging | `mcr.microsoft.com/dotnet/sdk:8.0` and `aspnet:8.0` images |

## Runtime Profiles

| Profile | Activation Method | Config Files | Key Overrides |
|---|---|---|---|
| Development | launchSettings / local run | `appsettings.json` + `appsettings.Development.json` | verbose logging, localhost base URLs |
| Docker | docker-compose env vars | `appsettings.Docker.json` + compose overrides | service URLs, SQL container connection strings |
| Production | host env + non-dev branch in Program.cs | `appsettings.json` + env/KeyVault | Key Vault sourced connection-string keys |

## Properties Inventory

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| `baseUrls:apiBase` | localhost API URL | all | appsettings variants |
| `baseUrls:webBase` | localhost web URL | all | appsettings variants |
| `ConnectionStrings:CatalogConnection` | LocalDB/SQL container string | base + Docker | appsettings + appsettings.Docker |
| `ConnectionStrings:IdentityConnection` | LocalDB/SQL container string | base + Docker | appsettings + appsettings.Docker |
| `CatalogBaseUrl` | empty string | base | appsettings |
| `Logging:LogLevel:*` | Warning/Information/Debug by env | all | appsettings variants |
| `UseOnlyInMemoryDatabase` | false when absent | conditional | IConfiguration runtime toggle |
| `AZURE_KEY_VAULT_ENDPOINT` | unset by default | production path | environment variable |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | unset by default | production path | environment variable |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | unset by default | production path | environment variable |

## Startup Parameters & Resource Requirements

| Service | JVM/Runtime Options | Memory | Instance Count |
|---|---|---|---|
| Web | `ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS` | Not explicitly set in repo | 1 per compose service definition |
| PublicApi | `ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS` | Not explicitly set in repo | 1 per compose service definition |
| SQL Server | `SA_PASSWORD`, `ACCEPT_EULA` | Not explicitly set in repo | 1 per compose service definition |

## Startup Dependency Chain

1. `sqlserver` starts first in docker-compose.
2. `eshopwebmvc` waits on `sqlserver` via `depends_on` before startup.
3. `eshoppublicapi` waits on `sqlserver` via `depends_on` before startup.
4. Web/PublicApi startup then seeds catalog and identity databases during app initialization.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage (masked) |
|---|---|---|
| `SA_PASSWORD` | SQL admin password | docker-compose env variable (`[MASKED]`) |
| `ConnectionStrings:*` password segments | DB credential | appsettings.Docker.json (`[MASKED]`) |
| `AZURE_KEY_VAULT_ENDPOINT` | Key Vault endpoint | environment variable reference |
| `AZURE_SQL_*_CONNECTION_STRING_KEY` | indirection key names | environment variable reference |

### Secrets Provisioning Workflow

In development/docker, secrets are supplied through local appsettings files and compose environment variables. In production path, Web retrieves secrets by using Azure credentials to access Key Vault, resolves configured connection string key names, and binds those values into DbContext setup during startup.

## Feature Flags

| Flag Name | Default | Controlled By |
|---|---|---|
| `UseOnlyInMemoryDatabase` | false/absent | runtime configuration value |
| Environment-specific behavior (`Development`, `Docker`, `Production`) | Development in launch profiles | `ASPNETCORE_ENVIRONMENT` |

## Framework & Runtime Versions

| Component | Version | Source |
|---|---|---|
| .NET target framework | net8.0 | `Directory.Packages.props` |
| ASP.NET Core packages | 8.0.2 | `Directory.Packages.props` (`AspNetVersion`) |
| EF Core packages | 8.0.2 | `Directory.Packages.props` (`EntityFramworkCoreVersion`) |
| System extensions | 8.0.0 | `Directory.Packages.props` |
| Swagger/Swashbuckle | 6.5.0 | `Directory.Packages.props` |
| Docker build/runtime images | .NET SDK 8.0 / ASP.NET 8.0 | `src/Web/Dockerfile`, `src/PublicApi/Dockerfile` |
