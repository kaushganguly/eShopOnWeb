# Configuration & Externalized Settings Inventory

The project uses layered .NET configuration via `appsettings*.json`, environment variables, and Docker settings, with optional Azure Key Vault integration in non-development Web environments. Configuration is split across Web, PublicApi, Blazor static config, and test settings.

## Configuration Sources

| Source | Type | Path/Location | Notes |
|---|---|---|---|
| Web base settings | JSON config | `src/Web/appsettings.json` | Base URLs, connection strings, logging |
| Web environment settings | JSON config | `src/Web/appsettings.Development.json`, `src/Web/appsettings.Docker.json` | Environment overrides |
| PublicApi base settings | JSON config | `src/PublicApi/appsettings.json` | Base URLs, connection strings, logging |
| PublicApi environment settings | JSON config | `src/PublicApi/appsettings.Development.json`, `src/PublicApi/appsettings.Docker.json` | Environment overrides |
| BlazorAdmin settings | JSON static config | `src/BlazorAdmin/wwwroot/appsettings*.json` | Client-side app settings |
| Test settings | JSON config | `tests/PublicApiIntegrationTests/appsettings.test.json` | Test-specific overrides |
| Docker Compose | Compose YAML env vars | `docker-compose.yml`, `docker-compose.override.yml` | Container ports, env vars, SQL credentials |
| Environment variables | Runtime source | `AddEnvironmentVariables()` in Web/PublicApi | Overrides JSON values at runtime |
| Azure Key Vault | Secret store | Configured in `src/Web/Program.cs` | Enabled for non-development Web startup |

## Build Profiles

| Profile | Activation | Purpose | Key Dependencies/Plugins |
|---|---|---|---|
| Debug | Default local build | Developer diagnostics and local debugging | Standard package set |
| Release | `-c Release` | Production-oriented build output | `BuildBundlerMinifier` conditioned on Release |
| Docker container builds | Dockerfile + compose | Containerized Web/PublicApi deployment | ASP.NET runtime images, compose networking |

## Runtime Profiles

| Profile | Activation Method | Config Files | Key Overrides |
|---|---|---|---|
| Development | `ASPNETCORE_ENVIRONMENT=Development` | `appsettings.json` + `appsettings.Development.json` | Verbose logging, local URLs |
| Docker | `ASPNETCORE_ENVIRONMENT=Docker` | `appsettings.json` + `appsettings.Docker.json` | Container URL bindings and compose ports |
| Production-like | Non-development environment | `appsettings.json` + environment vars + Key Vault | Key Vault-backed SQL connection values |

## Properties Inventory

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| `baseUrls:apiBase` | Local HTTPS API URL | Base + environment override | Web/PublicApi appsettings |
| `baseUrls:webBase` | Local HTTPS Web URL | Base + environment override | Web/PublicApi appsettings |
| `ConnectionStrings:CatalogConnection` | localdb SQL string | Base (overridable by env/Key Vault) | Web/PublicApi appsettings |
| `ConnectionStrings:IdentityConnection` | localdb SQL string | Base (overridable by env/Key Vault) | Web/PublicApi appsettings |
| `CatalogBaseUrl` | Empty string | Base | Web/PublicApi appsettings |
| `Logging:LogLevel:*` | Warning/Information/Debug by file | Base + environment overrides | appsettings variants |
| `ASPNETCORE_ENVIRONMENT` | unset | Docker/deployment provided | compose/environment |
| `ASPNETCORE_URLS` | unset | Docker override | compose override |
| `AZURE_KEY_VAULT_ENDPOINT` | unset | Non-development Web path | environment variable |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | unset | Non-development Web path | environment variable |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | unset | Non-development Web path | environment variable |

## Startup Parameters & Resource Requirements

| Service | JVM/Runtime Options | Memory | Instance Count |
|---|---|---|---|
| Web | ASP.NET Core host options via environment variables (`ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS`) | Not explicitly specified | 1 per compose service |
| PublicApi | ASP.NET Core host options via environment variables (`ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS`) | Not explicitly specified | 1 per compose service |
| SQL Server container | SQL Edge env (`SA_PASSWORD`, `ACCEPT_EULA`) | Not explicitly specified | 1 per compose service |

## Startup Dependency Chain

1. `sqlserver` starts first in Docker Compose.
2. `eshopwebmvc` waits on `sqlserver` via `depends_on` and then performs DB seeding at startup.
3. `eshoppublicapi` waits on `sqlserver` via `depends_on` and then performs DB seeding at startup.
4. Health check endpoints (`/health`, `home_page_health_check`, `api_health_check`) indicate readiness after startup.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage (masked) |
|---|---|---|
| `ConnectionStrings:*` | Database credentials/connection data | appsettings or environment-provided override (`[MASKED]`) |
| `AZURE_KEY_VAULT_ENDPOINT` | Key Vault endpoint | Environment variable (`[MASKED]`) |
| `AZURE_SQL_*_CONNECTION_STRING_KEY` | Key Vault secret key names | Environment variable (`[MASKED]`) |
| `SA_PASSWORD` (compose) | SQL admin password | Docker Compose env var (`[MASKED]`) |

### Secrets Provisioning Workflow

Secrets are provided either directly through environment variables/compose settings (local and Docker scenarios) or through Azure Key Vault resolution in non-development Web startup. The application process identity reads secret references at startup, resolves SQL connection strings, and injects them into EF Core DbContext configuration before migrations/seeding and request handling begin.

## Feature Flags

| Flag Name | Default | Controlled By |
|---|---|---|
| Development middleware path | Enabled only in Development/Docker | `ASPNETCORE_ENVIRONMENT` |
| Production Key Vault SQL path | Disabled in Development/Docker | Runtime environment selection |

## Framework & Runtime Versions

| Component | Version | Source |
|---|---|---|
| .NET target framework | net8.0 | `Directory.Packages.props` |
| ASP.NET Core packages | 8.0.2 | Central package version (`AspNetVersion`) |
| EF Core packages | 8.0.2 | Central package version (`EntityFramworkCoreVersion`) |
| System extensions | 8.0.0 | Central package version (`SystemExtensionVersion`) |
| Swashbuckle.AspNetCore | 6.5.0 | `Directory.Packages.props` |
| Docker SQL image | `mcr.microsoft.com/azure-sql-edge` | `docker-compose.yml` |
