# 02-upgrade-projects: Upgrade All Projects to net10.0

## Objective
Update all 10 projects in the solution from net8.0 to net10.0 via centralized package management in `Directory.Packages.props`.

## Scope
- `Directory.Packages.props` — central version management (framework + all packages)
- `src/ApplicationCore/ApplicationCore.csproj` — remove framework-included packages
- `src/BlazorAdmin/BlazorAdmin.csproj` — remove framework-included packages
- `tests/FunctionalTests/FunctionalTests.csproj` — remove legacy DotNetCliToolReference
- `tests/PublicApiIntegrationTests/ProgramTest.cs` — fix ambiguous Program type
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — remove obsolete serialization ctor
- `tests/UnitTests/...` — fix xUnit2013 warnings
- `tests/IntegrationTests/...` — fix xUnit2013 warnings

## Research Findings

### Package Updates
| Package | Old | New | Reason |
|---------|-----|-----|--------|
| TargetFramework | net8.0 | net10.0 | Target upgrade |
| AspNetVersion | 8.0.2 | 10.0.10 | Framework version |
| EntityFramworkCoreVersion | 8.0.2 | 10.0.10 | Framework version |
| SystemExtensionVersion | 8.0.0 | 10.0.10 | Framework version |
| VSCodeGeneratorVersion | 8.0.0 | 10.0.2 | Framework version |
| System.Text.Json | 8.0.3 | 10.0.10 | Framework version |
| Azure.Identity | 1.10.4 | 1.21.0 | Security fix |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.21.0 | Required by JwtBearer 10.0.10 |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.23.0 | Compatible version found |
| NuGet.Packaging | (transitive) | 6.13.1 | Fix NU1901 low-severity vulnerability |
| NuGet.Protocol | (transitive) | 6.13.1 | Fix NU1901 low-severity vulnerability |

### Packages Removed
- `System.Security.Claims` — included in net10.0 framework
- `System.Text.Json` from ApplicationCore — included in net10.0 framework
- `System.Net.Http.Json` from BlazorAdmin — included in net10.0 framework

### Key Fixes Required
1. **NU1605 downgrade** — System.IdentityModel.Tokens.Jwt 7.3.1 was lower than what JwtBearer 10.0.10 needs (8.x). Fixed by updating to 8.21.0.
2. **DotNetCliToolReference** in FunctionalTests — legacy tool reference (`dotnet-xunit`) not supported in modern .NET. Removed.
3. **CS0433 ambiguous Program** — Both PublicApi and Web expose a `Program` class. PublicApiIntegrationTests.ProgramTest used `WebApplicationFactory<Program>`, causing ambiguity. Fixed by using `WebApplicationFactory<AuthenticateEndpoint>` (pattern from FunctionalTests).
4. **SYSLIB0051** — `EmptyBasketOnCheckoutException` had obsolete serialization constructor. Removed.
5. **xUnit2013** — Multiple test files used `Assert.Equal(0, ...)` / `Assert.Equal(1, ...)` for collection sizes. Fixed to use `Assert.Empty()` / `Assert.Single()`.
6. **CentralPackageTransitivePinningEnabled** — Enabled to allow pinning NuGet.Packaging/NuGet.Protocol transitive dependencies to fixed versions.

### Known Remaining Limitation
**AutoMapper NU1903** (high-severity vulnerability):
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 pins AutoMapper to EXACTLY `[12.0.1]`
- No newer version of the extension package exists that uses a patched AutoMapper
- AutoMapper 13.0.0+ has breaking changes incompatible with the extension package
- 6 NU1903 warnings remain (one per project with AutoMapper in dependency graph)
- Not exploitable in this codebase: all mappings are developer-defined, no user-controlled mapping expressions
