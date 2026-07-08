# 03-public-api-surface: Upgrade PublicApi with ApplicationCore and Infrastructure compatibility work

## Objective
Upgrade ApplicationCore, Infrastructure, and PublicApi to net10.0 (ApplicationCore and Infrastructure multi-target net8.0;net10.0 for Web compatibility), fix all source/binary incompatibilities, update packages, and ensure PublicApi-related tests pass.

## Projects in Scope
- `src/ApplicationCore/ApplicationCore.csproj` — ClassLibrary → multi-target net8.0;net10.0
- `src/Infrastructure/Infrastructure.csproj` — ClassLibrary → multi-target net8.0;net10.0
- `src/PublicApi/PublicApi.csproj` — AspNetCore → net10.0 exclusively
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — must move to net10.0 (references net10.0-only PublicApi)
- `tests/FunctionalTests/FunctionalTests.csproj` — must move to net10.0 (references net10.0-only PublicApi)

## Research Findings

### ApplicationCore Issues
1. **NuGet.0003** `System.Security.Claims 4.3.0` — Remove package, included with framework
2. **NuGet.0002** `System.Text.Json` — Already at 10.0.9 in Directory.Packages.props ✅
3. **Api.0002** `EmptyBasketOnCheckoutException.cs` — Remove obsolete `SerializationInfo`/`StreamingContext` constructor (SYSLIB0051)
4. **TFM**: Must multi-target net8.0;net10.0 since Web depends on ApplicationCore and stays net8.0

### Infrastructure Issues
1. **NuGet.0002** `Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.2` → 10.0.9 for net10.0 slice
2. **NuGet.0002** `Microsoft.EntityFrameworkCore.InMemory 8.0.2` → 10.0.9 for net10.0 slice
3. **NuGet.0002** `Microsoft.EntityFrameworkCore.SqlServer 8.0.2` → 10.0.9 for net10.0 slice
4. **NuGet.0005** `System.IdentityModel.Tokens.Jwt 7.3.1` — deprecated, DEFER per strategy
5. **TFM**: Must multi-target net8.0;net10.0 since Web depends on Infrastructure

### PublicApi Issues
1. **TFM**: Move to net10.0 exclusively (no consumers below it that require net8.0 other than tests)
2. **NuGet.0002** Multiple packages need 10.0.9 via VersionOverride: JwtBearer, Diagnostics.EF, Identity.EF, Identity.UI, EF packages
3. **NuGet.0002** `Microsoft.VisualStudio.Web.CodeGeneration.Design 8.0.0` → 10.0.2 via VersionOverride
4. **NuGet.0001** `Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6` — INCOMPATIBLE, remove
5. **NuGet.0005** `AutoMapper.Extensions.Microsoft.DependencyInjection` — deprecated, DEFER
6. **NuGet.0005** `System.IdentityModel.Tokens.Jwt` — deprecated, DEFER
7. **Api.0001** `ConfigurationBinder.Get<T>()` and `Configure<T>(IServiceCollection, IConfiguration)` — binary incompatible (method signature change to nullable return), existing code already handles nulls

### PublicApiIntegrationTests Issues
1. **TFM**: Move to net10.0 (references net10.0-only PublicApi; references Web at net8.0 which is fine from net10.0)
2. **NuGet.0002** `Microsoft.AspNetCore.Mvc.Testing 8.0.2` → 10.0.9 via VersionOverride

### FunctionalTests Issues
1. **TFM**: Move to net10.0 (references net10.0-only PublicApi)
2. `Microsoft.AspNetCore.Mvc.Testing` → 10.0.9 via VersionOverride
3. `Microsoft.EntityFrameworkCore.InMemory` → 10.0.9 via VersionOverride

## Package Version Strategy
- Central `AspNetVersion=8.0.2` and `EntityFramworkCoreVersion=8.0.2` stay unchanged in Directory.Packages.props
- Use `VersionOverride="10.0.9"` in net10.0 project files (consistent with BlazorAdmin pattern)
- Multi-targeted projects use `Condition="'$(TargetFramework)' == 'net10.0'"` ItemGroups

## Files to Change
1. `src/ApplicationCore/ApplicationCore.csproj` — add TargetFrameworks, remove System.Security.Claims
2. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — remove SYSLIB0051 constructor
3. `src/Infrastructure/Infrastructure.csproj` — add TargetFrameworks, conditional package groups
4. `src/PublicApi/PublicApi.csproj` — set net10.0, VersionOverride packages, remove incompatible
5. `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj` — set net10.0, update packages
6. `tests/FunctionalTests/FunctionalTests.csproj` — set net10.0, update packages
