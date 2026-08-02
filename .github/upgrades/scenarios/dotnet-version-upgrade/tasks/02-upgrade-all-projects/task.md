# 02-upgrade-all-projects: 02-upgrade-all-projects

Execute task 02-upgrade-all-projects.

## Research Findings

### Target Framework
- All 10 projects share `TargetFramework` defined centrally in `Directory.Packages.props` via property `<TargetFramework>net8.0</TargetFramework>` (no per-project overrides needed).
- Changed to `net10.0`.

### Package Version Variables
| Variable | Old Value | New Value | Packages Affected |
|---|---|---|---|
| `AspNetVersion` | 8.0.2 | 10.0.0 | All `Microsoft.AspNetCore.*` and identity packages |
| `SystemExtensionVersion` | 8.0.0 | 10.0.0 | `System.Net.Http.Json`, `Microsoft.Extensions.Logging.Configuration` |
| `EntityFramworkCoreVersion` | 8.0.2 | 10.0.0 | All `Microsoft.EntityFrameworkCore.*` packages |
| `VSCodeGeneratorVersion` | 8.0.0 | 10.0.2 | `Microsoft.VisualStudio.Web.CodeGeneration.Design` |

### Removed Packages
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 — incompatible with .NET 10 (removed from Directory.Packages.props and PublicApi.csproj)
- `System.Security.Claims` 4.3.0 — now included in framework reference (removed from Directory.Packages.props and ApplicationCore.csproj)
- `Microsoft.AspNetCore.Mvc` 2.2.0 — pinned to incompatible version, not referenced by any project (removed from Directory.Packages.props)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 — deprecated; replaced with `AutoMapper` 14.0.0 directly (AddAutoMapper is in AutoMapper core 12+)
- `System.Text.Json` — explicit reference removed from ApplicationCore.csproj (now in .NET 10 framework)
- `System.Net.Http.Json` — explicit reference removed from BlazorAdmin.csproj (now in .NET 10 framework)
- `AutoMapper` from Web.csproj — Web project doesn't use AutoMapper at all; was unnecessary

### Security Updates
- `Azure.Identity`: 1.10.4 → 1.21.0 (security vulnerability fix)

### Deprecated Package Updates
- `xunit`: 2.7.0 → 2.9.3
- `xunit.runner.visualstudio`: 2.5.6 → 2.8.2
- `xunit.runner.console`: 2.7.0 → 2.9.3
- `MSTest.TestAdapter`: 3.2.2 → 3.8.3
- `MSTest.TestFramework`: 3.2.2 → 3.8.3
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.5.0
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 → replaced with `AutoMapper` 14.0.0

### NuGet Vulnerability Suppressions
- `GHSA-rvv3-g6hj-g44x` (AutoMapper HIGH) — affects all AutoMapper versions, no patched version exists; suppressed via `NuGetAuditSuppress`
- `GHSA-g4vj-cjjj-v7hg` (NuGet.Packaging/NuGet.Protocol LOW) — transitive from `Microsoft.VisualStudio.Web.CodeGeneration.Design` tooling package; cannot be upgraded without changing the tooling version; suppressed via `NuGetAuditSuppress`

### API Breaking Changes Fixed
1. **Source Incompatible (Api.0002)** — `Exception(SerializationInfo, StreamingContext)` constructor:
   - File: `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
   - Fix: Removed the protected serialization constructor (base `Exception` no longer has this constructor in .NET 10)

2. **Source Incompatible (Api.0002)** — `TimeSpan.FromMinutes(double)` ambiguity:
   - File: `src/Web/Configuration/ConfigureCookieSettings.cs`
   - Fix: Changed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` to `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)` to resolve overload ambiguity

3. **Compilation Error Fix** — Ambiguous `Program` class:
   - File: `tests/PublicApiIntegrationTests/ProgramTest.cs`
   - Fix: Changed `WebApplicationFactory<Program>` to `WebApplicationFactory<AuthenticateEndpoint>` to resolve CS0433 ambiguity (both PublicApi and Web define `partial class Program` via top-level statements in .NET 10)

### xUnit Analyzer Warnings Fixed (xUnit2013)
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`: `Assert.Equal(1, ...)` → `Assert.Single(...)`
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`: `Assert.Equal(0, ...)` → `Assert.Empty(...)`
- `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`: `Assert.Equal(0, ...)` → `Assert.Empty(...)`

### Final Build Status
- **Errors**: 0
- **Warnings**: 0
- **All 10 projects**: Successfully compiled targeting net10.0
