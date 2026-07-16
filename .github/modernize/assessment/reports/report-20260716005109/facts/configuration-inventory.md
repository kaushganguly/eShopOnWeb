# Configuration & Externalized Settings Inventory

The repository uses layered configuration files for Web, PublicApi, and BlazorAdmin plus environment overrides for Development, Docker, and Production. Secrets are partly embedded for local/docker scenarios and partly externalized to Azure Key Vault in production-oriented startup logic.

## Configuration Sources

| Source | Type | Path/Location | Notes |
|---|---|---|---|
| `appsettings.json` | .NET JSON config | `src/Web`, `src/PublicApi`, `src/BlazorAdmin/wwwroot` | Base runtime configuration |
| `appsettings.Development.json` | Environment override | `src/Web`, `src/PublicApi`, `src/BlazorAdmin/wwwroot` | Development values and logging overrides |
| `appsettings.Docker.json` | Environment override | `src/Web`, `src/PublicApi`, `src/BlazorAdmin/wwwroot` | Container-specific base URLs and SQL connection strings |
| `launchSettings.json` | Local launch profile config | `src/Web/Properties`, `src/PublicApi/Properties`, `src/BlazorAdmin/Properties` | Local URLs and `ASPNETCORE_ENVIRONMENT` values |
| `docker-compose.yml` and override | Container orchestration config | repository root | Service wiring, SQL container environment variables |
| Environment variables | Runtime externalized config | loaded in `Program.cs` | Used for profile settings and Azure Key Vault selection |
| Azure Key Vault references | External secret store | `Web/Program.cs` via `AddAzureKeyVault` | Production connection string indirection via key names |

## Build Profiles

| Profile | Activation | Purpose | Key Dependencies/Plugins |
|---|---|---|---|
| Debug | Default local build | Developer diagnostics and fast iteration | Standard SDK tooling |
| Release | `-c Release` | Production build and bundling | `BuildBundlerMinifier` conditional package in Web |
| Docker publish flow | Docker build context | Container images for Web and PublicApi | `src/Web/Dockerfile`, `src/PublicApi/Dockerfile` |
| Central package management | Automatic via MSBuild | Shared dependency versions across solution | `Directory.Packages.props` |

## Runtime Profiles

| Profile | Activation Method | Config Files | Key Overrides |
|---|---|---|---|
| Development | `ASPNETCORE_ENVIRONMENT=Development` | `appsettings.json` + `appsettings.Development.json` | Debug logging and localhost base URLs |
| Docker | `ASPNETCORE_ENVIRONMENT=Docker` | `appsettings.json` + `appsettings.Docker.json` | SQL container host and docker-oriented URLs |
| Production | `ASPNETCORE_ENVIRONMENT=Production` | `appsettings.json` (+ environment variables) | Azure Key Vault-based connection lookup and HSTS path |

## Properties Inventory

### Web

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| `baseUrls.apiBase` | `https://localhost:5099/api/` | Development, base | appsettings files |
| `baseUrls.webBase` | `https://localhost:44315/` | Development, base | appsettings files |
| `ConnectionStrings.CatalogConnection` | LocalDB catalog DB | base; overridden in Docker/prod patterns | appsettings + env/KeyVault resolution |
| `ConnectionStrings.IdentityConnection` | LocalDB identity DB | base; overridden in Docker/prod patterns | appsettings + env/KeyVault resolution |
| `CatalogBaseUrl` | empty string | base | appsettings.json |
| `Logging.LogLevel.*` | Warning/Debug depending env | Development/Docker overrides | appsettings files |
| `AZURE_KEY_VAULT_ENDPOINT` | none | Production | environment variable consumed in Program.cs |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | none | Production | environment variable key indirection |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | none | Production | environment variable key indirection |

### PublicApi

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| `baseUrls.apiBase` | `https://localhost:5099/api/` | base/development/docker | appsettings files |
| `baseUrls.webBase` | `https://localhost:5001/` | base/development/docker | appsettings files |
| `ConnectionStrings.CatalogConnection` | LocalDB catalog DB | base; docker override | appsettings files |
| `ConnectionStrings.IdentityConnection` | LocalDB identity DB | base; docker override | appsettings files |
| `CatalogBaseUrl` | empty string | base | appsettings.json |
| `Logging.*` | warning/info by env | development override | appsettings files |

## Startup Parameters & Resource Requirements

| Service | JVM/Runtime Options | Memory | Instance Count |
|---|---|---|---|
| Web | .NET runtime; environment-specific middleware selection | Not explicitly declared in repo | 1 in docker-compose |
| PublicApi | .NET runtime; Swagger and auth middleware | Not explicitly declared in repo | 1 in docker-compose |
| SQL Server container | Azure SQL Edge image defaults | Container-managed (no explicit memory limit) | 1 in docker-compose |

## Startup Dependency Chain

1. `sqlserver` starts first as declared dependency target.
2. `eshopwebmvc` waits for `sqlserver` through `depends_on` and then seeds catalog/identity contexts on startup.
3. `eshoppublicapi` waits for `sqlserver` through `depends_on` and then seeds catalog/identity contexts on startup.
4. Health checks (`/health`, `home_page_health_check`, `api_health_check`) provide readiness signals for the Web process.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage (masked) |
|---|---|---|
| `ConnectionStrings.CatalogConnection` | DB connection string | appsettings values (contains credentials in Docker profile) |
| `ConnectionStrings.IdentityConnection` | DB connection string | appsettings values (contains credentials in Docker profile) |
| `SA_PASSWORD` | SQL Server admin password | docker-compose environment value `[MASKED]` |
| `AZURE_KEY_VAULT_ENDPOINT` | Key Vault URI | environment variable |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | Secret name indirection | environment variable |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | Secret name indirection | environment variable |

### Secrets Provisioning Workflow

Local and docker development load values directly from appsettings and compose environment variables. Production-oriented startup flow authenticates via Azure credentials, connects to Key Vault, resolves configured key names for SQL connection strings, and binds those resolved values into EF Core DbContext registrations for Web.

## Feature Flags

| Flag Name | Default | Controlled By |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Development in launch profiles | launchSettings / environment variable |
| `CatalogBaseUrl` | empty | appsettings value |
| Development middleware branch (`IsDevelopment` or `EnvironmentName == Docker`) | false in prod | runtime environment |

## Framework & Runtime Versions

| Component | Version | Source |
|---|---|---|
| .NET target framework | net8.0 | `Directory.Packages.props` |
| ASP.NET Core shared package version | 8.0.2 | `Directory.Packages.props` |
| EF Core package version | 8.0.2 | `Directory.Packages.props` |
| Azure.Identity | 1.10.4 | `Directory.Packages.props` |
| Swashbuckle.AspNetCore | 6.5.0 | `Directory.Packages.props` |
| Docker SQL image | `mcr.microsoft.com/azure-sql-edge` | `docker-compose.yml` |
