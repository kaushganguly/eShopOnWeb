# .NET Upgrade Plan: eShopOnWeb net8.0 → net10.0

## Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.  
**Rationale**: 10 projects, all on net8.0, CPM already in place. Single-pass upgrade is efficient and straightforward.

## Solution
eShopOnWeb.sln — 10 projects across 5 dependency tiers.

## Projects
**Libraries (Levels 0-2)**
- `src/BlazorShared/BlazorShared.csproj`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`
- `src/Infrastructure/Infrastructure.csproj`

**Applications (Level 3)**
- `src/PublicApi/PublicApi.csproj`
- `src/Web/Web.csproj`

**Test Projects (Levels 4-5)**
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`

---

## Tasks

### Task 01-prerequisites
**Update SDK and toolchain configuration for .NET 10**

Update `global.json` to target the .NET 10 SDK. Verify the SDK is installed.

**Scope**:
- `global.json`: update `sdk.version` to `10.0.x`

**Done when**: `global.json` references .NET 10 SDK and `dotnet --version` resolves to 10.x.

---

### Task 02-core-upgrade
**Upgrade all projects to net10.0 — TFM, packages, and code fixes**

Update the central package management file and fix all API breaking changes across the solution.

**Scope**:

**Directory.Packages.props changes**:
- Change `<TargetFramework>` from `net8.0` to `net10.0`
- Update `<AspNetVersion>` to `10.0.10`
- Update `<SystemExtensionVersion>` to `10.0.10`
- Update `<EntityFramworkCoreVersion>` to `10.0.10`
- Update `<VSCodeGeneratorVersion>` to `10.0.2`
- Update `Azure.Identity` to `1.21.0`
- Update `System.Text.Json` to `10.0.10`
- Update `System.IdentityModel.Tokens.Jwt` to `8.19.2`
- Remove `System.Security.Claims` (functionality included in framework reference)
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no supported version for net10.0)
- Remove `Microsoft.AspNetCore.Mvc` version entry (included in framework)
- Update `Microsoft.Extensions.Logging.Configuration` version property reference
- Update `System.Net.Http.Json` version property reference

**Individual project file changes**:
- Remove `<PackageReference Include="System.Security.Claims" />` from `ApplicationCore.csproj`
- Remove `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` from projects that reference it

**API breaking change fixes**:
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: Remove the `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor (removed in .NET 9+)
- `src/Web/Configuration/ConfigureWebServices.cs`: Fix `services.Configure<CatalogSettings>(configuration)` — bind to specific section
- `src/Web/Configuration/ConfigureCoreServices.cs`: Fix `configuration.Get<CatalogSettings>()` — use section-based binding
- `src/PublicApi/Program.cs`: Fix `builder.Services.Configure<CatalogSettings>(builder.Configuration)` and `builder.Configuration.Get<CatalogSettings>()` — use section-based binding

**Done when**: All packages resolve, all projects build with 0 errors, 0 warnings.

---

### Task 03-validation
**Full solution build and test suite**

Run the complete build and all unit tests to confirm the upgrade is successful.

**Scope**:
- `dotnet build eShopOnWeb.sln`
- `dotnet test eShopOnWeb.sln`

**Done when**: Solution builds with 0 errors, all tests pass.
