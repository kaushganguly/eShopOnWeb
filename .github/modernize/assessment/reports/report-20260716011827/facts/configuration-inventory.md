# Configuration & Externalized Settings Inventory

The solution uses a small but varied configuration surface: appsettings files for each executable, launch profiles for local development, docker-compose settings for container runs, and optional Azure Key Vault integration for production secrets.

## Configuration Sources

| Source | Type | Path/Location | Notes |
| --- | --- | --- | --- |
| Web base settings | JSON | `src/Web/appsettings.json` | Defines base URLs, SQL connection strings, logging, and `CatalogBaseUrl` |
| Web development settings | JSON | `src/Web/appsettings.Development.json` | Overrides base URLs and logging for local development |
| Web docker settings | JSON | `src/Web/appsettings.Docker.json` | Points connection strings to the SQL Edge container and docker host URLs |
| PublicApi base settings | JSON | `src/PublicApi/appsettings.json` | Defines base URLs, SQL connection strings, and logging |
| PublicApi development settings | JSON | `src/PublicApi/appsettings.Development.json` | Development-specific base URLs and logging |
| PublicApi docker settings | JSON | `src/PublicApi/appsettings.Docker.json` | Docker-specific SQL and base URL settings |
| BlazorAdmin client settings | JSON | `src/BlazorAdmin/wwwroot/appsettings*.json` | Supplies base URLs and logging for the browser client |
| Launch profiles | JSON | `src/*/Properties/launchSettings.json` | Local URLs, browser launch behavior, and `ASPNETCORE_ENVIRONMENT` values |
| Docker composition | YAML | `docker-compose.yml`, `docker-compose.override.yml` | Declares `eshopwebmvc`, `eshoppublicapi`, and `sqlserver` services plus environment overrides |
| Azure Key Vault integration | Runtime config source | `src/Web/Program.cs` | Production web host loads secrets from Key Vault using `AZURE_KEY_VAULT_ENDPOINT` and secret-key indirection |
| Environment variables | Process environment | `AddEnvironmentVariables()` in `Web` and `PublicApi` | Allows last-mile overrides for app settings and connection metadata |

## Build Profiles

| Profile | Activation | Purpose | Key Dependencies/Plugins |
| --- | --- | --- | --- |
| `Debug` | Default local build configuration | Developer build with diagnostics and no bundling conditionals | Standard .NET SDK toolchain |
| `Release` | Explicit build configuration | Production-style build; enables conditional `BuildBundlerMinifier` reference in `Web` | `BuildBundlerMinifier` |
| Docker project profile | Visual Studio / docker-compose workflow | Builds container images for `Web` and `PublicApi` | `docker-compose.dcproj`, Azure container tooling package |
| Web scaffolding profile | Manual developer use | Supports code generation and library manager tasks | `Microsoft.VisualStudio.Web.CodeGeneration.Design`, `Microsoft.Web.LibraryManager.Build` |

## Runtime Profiles

| Profile | Activation Method | Config Files | Key Overrides |
| --- | --- | --- | --- |
| Development | `ASPNETCORE_ENVIRONMENT=Development` via launch settings | `appsettings.json` + `appsettings.Development.json` | Localhost URLs, debug/information logging |
| Production | `ASPNETCORE_ENVIRONMENT=Production` | `appsettings.json` plus environment variables and Key Vault for `Web` | Web host switches to Azure Key Vault-backed connection string resolution and HSTS |
| Docker | Docker profile / `EnvironmentName == "Docker"` | `appsettings.Docker.json` | SQL Edge connection strings and host-docker base URLs |
| InMemory test mode | `UseOnlyInMemoryDatabase=true` | Environment/config override only | Replaces SQL providers with EF Core InMemory stores |

## Properties Inventory

### Web

| Property Key | Default | Profiles | Source |
| --- | --- | --- | --- |
| `baseUrls:apiBase` | `https://localhost:5099/api/` | Development, Docker | `appsettings*.json` |
| `baseUrls:webBase` | `https://localhost:44315/` | Development, Docker | `appsettings*.json` |
| `ConnectionStrings:CatalogConnection` | LocalDB SQL connection | Base, Docker override | `appsettings.json`, `appsettings.Docker.json` |
| `ConnectionStrings:IdentityConnection` | LocalDB SQL connection | Base, Docker override | `appsettings.json`, `appsettings.Docker.json` |
| `CatalogBaseUrl` | empty string | Base | `appsettings.json` |
| `Logging:LogLevel:*` | Warning / Debug / Information by environment | Base, Development, Docker | `appsettings*.json` |
| `AZURE_KEY_VAULT_ENDPOINT` | none | Production via environment | Runtime environment variable |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | none | Production via environment | Runtime environment variable |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | none | Production via environment | Runtime environment variable |
| `UseOnlyInMemoryDatabase` | false when absent | Tests / override scenarios | Arbitrary config or environment variable |

### PublicApi

| Property Key | Default | Profiles | Source |
| --- | --- | --- | --- |
| `baseUrls:apiBase` | `https://localhost:5099/api/` | Development, Docker | `appsettings*.json` |
| `baseUrls:webBase` | `https://localhost:5001/` | Development, Docker | `appsettings*.json` |
| `ConnectionStrings:CatalogConnection` | LocalDB SQL connection | Base, Docker override | `appsettings.json`, `appsettings.Docker.json` |
| `ConnectionStrings:IdentityConnection` | LocalDB SQL connection | Base, Docker override | `appsettings.json`, `appsettings.Docker.json` |
| `Logging:LogLevel:*` | Warning / Information by environment | Base, Development, Docker | `appsettings*.json` |

### BlazorAdmin

| Property Key | Default | Profiles | Source |
| --- | --- | --- | --- |
| `baseUrls:apiBase` | `https://localhost:5099/api/` | Development, Docker | `wwwroot/appsettings*.json` |
| `baseUrls:webBase` | `https://localhost:44315/` | Development, Docker | `wwwroot/appsettings*.json` |
| `Logging:LogLevel:*` | Information / Warning | Base, Development, Docker | `wwwroot/appsettings*.json` |

## Startup Parameters & Resource Requirements

| Service | JVM/Runtime Options | Memory | Instance Count |
| --- | --- | --- | --- |
| `Web` | No custom runtime flags in repo; launch settings set `ASPNETCORE_ENVIRONMENT` | Not specified | 1 local process / 1 container |
| `PublicApi` | No custom runtime flags in repo; launch settings set `ASPNETCORE_ENVIRONMENT` and optional `ASPNETCORE_URLS` in WSL | Not specified | 1 local process / 1 container |
| `BlazorAdmin` | No custom runtime flags in repo | Not specified | 1 local process |
| `sqlserver` | Environment-driven container startup | Not specified in compose file | 1 container |

## Startup Dependency Chain

1. `sqlserver` must be available before `eshopwebmvc` and `eshoppublicapi` in Docker; both services declare `depends_on` for the database container.
2. `Web` and `PublicApi` create scopes on startup, run EF migrations where applicable, and seed catalog plus identity data before serving requests.
3. In production `Web` loads Azure Key Vault configuration before registering SQL Server DbContexts, so Key Vault availability affects startup.
4. Health checks (`/health`, `/home_page_health_check`, `/api_health_check`) are exposed after middleware setup and can be used as readiness indicators.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage (masked) |
| --- | --- | --- |
| `ConnectionStrings:CatalogConnection` | Database credential | LocalDB in development; Docker variant includes SQL credentials but values are masked in this inventory |
| `ConnectionStrings:IdentityConnection` | Database credential | LocalDB in development; Docker variant includes SQL credentials but values are masked in this inventory |
| `AZURE_KEY_VAULT_ENDPOINT` | Secret store endpoint | Environment variable |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | Secret name indirection | Environment variable |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | Secret name indirection | Environment variable |
| Seeded user password constant | Application credential seed | Source constant exists, but value is intentionally not reproduced here |
| Docker SQL SA password | Container credential | `docker-compose.yml` environment section, shown as `[MASKED]` |

### Secrets Provisioning Workflow

In local development the applications read connection strings directly from `appsettings.json` or Docker-specific overrides. In production the web host uses `AzureDeveloperCliCredential` / `DefaultAzureCredential` to access Azure Key Vault, reads the vault endpoint from an environment variable, then uses two additional environment-provided key names to look up the actual catalog and identity connection strings. Docker uses environment entries for the SQL container, but there is no external secret manager or sealed-secret workflow defined in this repository.

## Feature Flags

| Flag Name | Default | Controlled By |
| --- | --- | --- |
| `UseOnlyInMemoryDatabase` | `false` when absent | Configuration / environment variable |
| `ASPNETCORE_ENVIRONMENT` | `Development` in launch profiles | Launch settings, Docker, or deployment environment |
| `CatalogBaseUrl` path-base behavior | empty string (disabled) | `appsettings.json` or override |

## Framework & Runtime Versions

| Component | Version | Source |
| --- | --- | --- |
| .NET target framework | `net8.0` | `Directory.Packages.props` and project files |
| ASP.NET Core shared packages | `8.0.2` | `Directory.Packages.props` |
| EF Core | `8.0.2` | `Directory.Packages.props` |
| System.Text.Json | `8.0.3` | `Directory.Packages.props` |
| Azure.Identity | `1.10.4` | `Directory.Packages.props` |
| Swashbuckle | `6.5.0` | `Directory.Packages.props` |
| MediatR | `12.0.1` | `Directory.Packages.props` |
| FluentValidation | `11.9.0` | `Directory.Packages.props` |
| Docker base image for `Web` build | `mcr.microsoft.com/dotnet/sdk:8.0` | `src/Web/Dockerfile` |
| Docker base image for `Web` runtime | `mcr.microsoft.com/dotnet/aspnet:8.0` | `src/Web/Dockerfile` |
| Docker database image | `mcr.microsoft.com/azure-sql-edge` | `docker-compose.yml` |
