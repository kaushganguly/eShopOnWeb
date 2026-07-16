# Configuration & Externalized Settings Inventory

eShopOnWeb uses ASP.NET Core's layered configuration system with 3 environments (Development, Docker, Production), Azure Key Vault integration for production secrets, and Docker Compose for local containerized development across two independently deployable services.

## Configuration Sources

| Source | Type | Path / Location | Notes |
|--------|------|----------------|-------|
| `appsettings.json` | JSON file | `src/Web/appsettings.json`, `src/PublicApi/appsettings.json` | Base defaults — loaded in all environments |
| `appsettings.Development.json` | JSON file | `src/Web/appsettings.Development.json`, `src/PublicApi/appsettings.Development.json` | Development overrides — Debug log level |
| `appsettings.Docker.json` | JSON file | `src/Web/appsettings.Docker.json`, `src/PublicApi/appsettings.Docker.json` | Docker Compose overrides — SQL Server in container, HTTP URLs |
| Environment variables | Env vars | `ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS` | Set by Docker Compose (`docker-compose.override.yml`) |
| `launchSettings.json` | Dev-only JSON | `src/Web/Properties/launchSettings.json`, `src/PublicApi/Properties/launchSettings.json` | IDE profiles — NOT loaded at runtime in production |
| Azure Key Vault | Secret store | URI from `AZURE_KEY_VAULT_ENDPOINT` env var | Production only; loaded via `Azure.Extensions.AspNetCore.Configuration.Secrets` + `Azure.Identity` |
| User Secrets | Secret store | `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json` | Developer-local only; mounted into Docker via volume |
| `docker-compose.yml` | Container config | `docker-compose.yml` + `docker-compose.override.yml` | Defines SQL Server container, environment variable injection, port mapping |
| `azure.yaml` | IaC | `azure.yaml` | Azure Developer CLI (`azd`) deployment descriptor — maps `web` service to Azure App Service |

## Build Profiles

| Profile | Activation | Purpose | Key Properties |
|---------|-----------|---------|----------------|
| Debug | Default in VS / `dotnet build` | Development build — full symbols, no optimization | No conditional symbols defined; `BuildBundlerMinifier` excluded |
| Release | `-c Release` / `dotnet publish -c Release` | Production build — bundle + minify static assets | `BuildBundlerMinifier` included (`Condition="'$(Configuration)'=='Release'"`) |

No additional MSBuild property-based conditional compilation or multi-targeting is configured; both build configurations target `net8.0`.

## Runtime Profiles

| Profile | Activation Method | Config Files Loaded | Key Overrides |
|---------|------------------|--------------------|---------------|
| `Development` | `ASPNETCORE_ENVIRONMENT=Development` (default in `launchSettings.json`) | `appsettings.json` + `appsettings.Development.json` | Log level Default=Debug, System/Microsoft=Information; `apiBase` → localhost:5099 |
| `Docker` | `ASPNETCORE_ENVIRONMENT=Docker` (set in `docker-compose.override.yml`) | `appsettings.json` + `appsettings.Docker.json` | SQL Server connection strings → containerized SQL Server (`sqlserver,1433`); `apiBase` → http://localhost:5200/api/; HTTP only (no TLS inside container) |
| `Production` | `ASPNETCORE_ENVIRONMENT=Production` | `appsettings.json` + Azure Key Vault | Connection strings resolved from Azure Key Vault via indirection keys (`AZURE_SQL_CATALOG_CONNECTION_STRING_KEY`, `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY`); EF Core SQL Server retry enabled |
| `UseOnlyInMemoryDatabase` | `UseOnlyInMemoryDatabase=true` in any config source | Any environment | Replaces both SQL Server DbContexts with EF Core InMemory provider; skips migrations |

## Properties Inventory

### Web (MVC Storefront)

| Property Key | Default Value | Profile Override | Source |
|-------------|-------------|------------------|--------|
| `ConnectionStrings:CatalogConnection` | SQL Server LocalDB (`eShopOnWeb.CatalogDb`) | Docker: SQL Server container; Production: Key Vault | `appsettings.json` / Azure KV |
| `ConnectionStrings:IdentityConnection` | SQL Server LocalDB (`eShopOnWeb.Identity`) | Docker: SQL Server container; Production: Key Vault | `appsettings.json` / Azure KV |
| `baseUrls:apiBase` | `https://localhost:5099/api/` | Docker: `http://localhost:5200/api/` | `appsettings.json` / `appsettings.Docker.json` |
| `baseUrls:webBase` | `https://localhost:44315/` | Docker: `http://host.docker.internal:5106/` | `appsettings.json` / `appsettings.Docker.json` |
| `CatalogBaseUrl` | `""` (empty) | None | `appsettings.json` — controls product image URL prefix |
| `UseOnlyInMemoryDatabase` | `null` (unset) | Set to `true` in test/CI environments | Environment variable or `appsettings.json` |
| `AZURE_KEY_VAULT_ENDPOINT` | Not set | Production only | Environment variable → Key Vault URI |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | Not set | Production only | Environment variable → Key Vault secret name |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | Not set | Production only | Environment variable → Key Vault secret name |
| `Logging:LogLevel:Default` | `Warning` | Development: `Debug`; Docker: `Debug` | `appsettings.json` |
| `Logging:LogLevel:Microsoft` | `Warning` | Development/Docker: `Information` | `appsettings.json` |
| `Cookie:ExpireTimeSpan` | 60 minutes | Hardcoded constant (`ConfigureCookieSettings.ValidityMinutesPeriod`) | `ConfigureCookieSettings.cs` |
| `Cookie:Name` | `EshopIdentifier` | Hardcoded constant | `ConfigureCookieSettings.cs` |

### PublicApi

| Property Key | Default Value | Profile Override | Source |
|-------------|-------------|------------------|--------|
| `ConnectionStrings:CatalogConnection` | SQL Server LocalDB (`eShopOnWeb.CatalogDb`) | Docker: SQL Server container | `appsettings.json` / `appsettings.Docker.json` |
| `ConnectionStrings:IdentityConnection` | SQL Server LocalDB (`eShopOnWeb.Identity`) | Docker: SQL Server container | `appsettings.json` / `appsettings.Docker.json` |
| `baseUrls:apiBase` | `https://localhost:5099/api/` | Docker: `http://localhost:5200/api/` | `appsettings.json` |
| `baseUrls:webBase` | `https://localhost:5001/` | Docker: `http://host.docker.internal:5106/` | `appsettings.json` |
| `CatalogBaseUrl` | `""` (empty) | None | `appsettings.json` |
| `UseOnlyInMemoryDatabase` | `null` (unset) | Test/CI override | Environment variable or `appsettings.json` |
| `Logging:LogLevel:Default` | `Warning` | Development: `Information` | `appsettings.json` |

## Startup Parameters & Resource Requirements

| Service | Runtime Options | Env Var Overrides | Container Ports | Memory/CPU Limits |
|---------|----------------|------------------|-----------------|-------------------|
| Web (`eshopwebmvc`) | `dotnet Web.dll` | `ASPNETCORE_ENVIRONMENT=Docker`, `ASPNETCORE_URLS=http://+:8080` | Host:5106 → Container:8080 | Not specified (no `mem_limit` / K8s resources in docker-compose) |
| PublicApi (`eshoppublicapi`) | `dotnet PublicApi.dll` | `ASPNETCORE_ENVIRONMENT=Docker`, `ASPNETCORE_URLS=http://+:8080` | Host:5200 → Container:8080 | Not specified |
| SQL Server (`sqlserver`) | `mcr.microsoft.com/azure-sql-edge` | `SA_PASSWORD=[MASKED]`, `ACCEPT_EULA=Y` | Host:1433 → Container:1433 | Not specified |

No JVM heap settings apply (this is a .NET application). No Kubernetes manifests with resource constraints were found.

## Startup Dependency Chain

```
sqlserver (SQL Server / Azure SQL Edge)
  └── eshopwebmvc (Web MVC)     — docker-compose depends_on: sqlserver
  └── eshoppublicapi (PublicApi) — docker-compose depends_on: sqlserver
```

- **Wait mechanism**: Docker Compose `depends_on` ensures the `sqlserver` container starts before the application containers. However, only container *start* is checked — no health probe is defined for SQL Server readiness. Applications handle connection failures at startup via `CatalogContextSeed` retry logic (up to 10 attempts with exponential back-off).
- **Health checks** (Web only): `/health` (aggregate), `/home_page_health_check`, `/api_health_check` — exposed as ASP.NET Core health check middleware endpoints. No Kubernetes readiness/liveness probes are configured.
- **Azure production**: Services are deployed via `azd` to Azure App Service; the SQL Server dependency is replaced by Azure SQL. No orchestration startup ordering is defined for the Azure target.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage |
|-----------------|------|---------|
| `ConnectionStrings:CatalogConnection` (SQL Server) | Database connection string | `appsettings.Docker.json` (hardcoded — see risk note); Azure Key Vault (production) |
| `ConnectionStrings:IdentityConnection` (SQL Server) | Database connection string | `appsettings.Docker.json` (hardcoded — see risk note); Azure Key Vault (production) |
| `SA_PASSWORD` | SQL Server SA password | `docker-compose.yml` environment variable — `[MASKED, default placeholder]` |
| `AZURE_KEY_VAULT_ENDPOINT` | Azure Key Vault URI | Environment variable (production) |
| `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` | Key Vault secret name pointer | Environment variable (production) |
| `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` | Key Vault secret name pointer | Environment variable (production) |

**Risk**: `appsettings.Docker.json` contains a hardcoded SQL Server SA password (`@someThingComplicated1234`) committed to source control. This is a sample-app placeholder but would be a critical secret exposure in a production repository.

### Secrets Provisioning Workflow

**Development / Local**:
1. Developers use .NET User Secrets (`~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`) for local connection string overrides (not committed).
2. Docker Compose mounts `~/.microsoft/usersecrets` into containers so User Secrets are available inside the container.

**Production (Azure)**:
1. Azure Developer CLI (`azd`) provisions the Azure App Service and sets the `AZURE_KEY_VAULT_ENDPOINT` environment variable on the App Service.
2. The App Service uses a **ChainedTokenCredential** (`AzureDeveloperCliCredential` → `DefaultAzureCredential`) to authenticate to Azure Key Vault — typically via a System-Assigned Managed Identity.
3. At startup, `AddAzureKeyVault()` loads all Key Vault secrets into the configuration layer.
4. `AZURE_SQL_CATALOG_CONNECTION_STRING_KEY` and `AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY` are indirection keys: their *values* are the names of the Azure SQL connection string secrets stored in Key Vault. The application performs a second lookup: `builder.Configuration[builder.Configuration["AZURE_SQL_CATALOG_CONNECTION_STRING_KEY"]]`.
5. EF Core receives the resolved connection string with SQL Server retry-on-failure enabled.

## Feature Flags

| Flag / Toggle | Type | Default | Controlled By |
|--------------|------|---------|---------------|
| `UseOnlyInMemoryDatabase` | Boolean config key | `null` (disabled) | `appsettings.json`, environment variable, or User Secrets — any config source |

No formal feature flag framework (LaunchDarkly, Microsoft.FeatureManagement, Unleash) is used. The `UseOnlyInMemoryDatabase` toggle is the only conditional behavior gate in the codebase.

## Framework & Runtime Versions

| Component | Version | Source |
|-----------|---------|--------|
| Target Framework | net8.0 | `Directory.Packages.props` (`TargetFramework`) |
| .NET SDK (required) | 8.0.x (latestFeature rollForward) | `global.json` |
| ASP.NET Core | 8.0.2 | `Directory.Packages.props` (`AspNetVersion`) |
| Entity Framework Core | 8.0.2 | `Directory.Packages.props` (`EntityFramworkCoreVersion`) |
| Ardalis.Specification | 7.0.0 | `Directory.Packages.props` |
| MediatR | 12.0.1 | `Directory.Packages.props` |
| AutoMapper | 12.0.1 | `Directory.Packages.props` |
| Azure.Identity | 1.10.4 | `Directory.Packages.props` |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | `Directory.Packages.props` |
| Swashbuckle.AspNetCore | 6.5.0 | `Directory.Packages.props` |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | `Directory.Packages.props` |
| Docker base image (Web) | `mcr.microsoft.com/dotnet/aspnet:8.0` | `src/Web/Dockerfile` |
| Docker base image (PublicApi) | `mcr.microsoft.com/dotnet/aspnet:8.0` | `src/PublicApi/Dockerfile` |
| Docker build image | `mcr.microsoft.com/dotnet/sdk:8.0` | Both Dockerfiles |
| SQL Server container image | `mcr.microsoft.com/azure-sql-edge` (latest) | `docker-compose.yml` |
| Azure target deployment | Azure App Service | `azure.yaml` |
