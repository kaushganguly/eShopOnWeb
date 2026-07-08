# Progress Details: 04-web-storefront

## Summary
Upgraded `src/Web`, `tests/UnitTests`, and `tests/IntegrationTests` from net8.0 to net10.0. Fixed a CS0433 ambiguity in PublicApiIntegrationTests caused by both Web and PublicApi defining a top-level `Program` class. Fixed xUnit2013 analyzer warnings in test files. All projects build cleanly and all tests pass on net10.0.

## Files Changed

### Project Files
| File | Change |
|---|---|
| `src/Web/Web.csproj` | Added `<TargetFramework>net10.0</TargetFramework>` override; added `VersionOverride="10.0.9"` for Microsoft.AspNetCore.* and Microsoft.EntityFrameworkCore.* packages; added `VersionOverride="10.0.2"` for Microsoft.VisualStudio.Web.CodeGeneration.Design; added `VersionOverride="10.0.9"` for Microsoft.AspNetCore.Components.WebAssembly.Server; added comments for deferred deprecated packages |
| `tests/UnitTests/UnitTests.csproj` | Added `<TargetFramework>net10.0</TargetFramework>` override; added comments for deferred deprecated xunit packages |
| `tests/IntegrationTests/IntegrationTests.csproj` | Added `<TargetFramework>net10.0</TargetFramework>` override; added `VersionOverride="10.0.9"` for Microsoft.EntityFrameworkCore.InMemory; added comment for deferred deprecated xunit package |

### Source Code Fixes

#### Api.0002 — Source Incompatible: TimeSpan.FromMinutes overload resolution
`src/Web/Configuration/ConfigureCookieSettings.cs` (line 26)
- **Before**: `TimeSpan.FromMinutes(ValidityMinutesPeriod)` where `ValidityMinutesPeriod` is `const int 60`
- **After**: `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)`
- **Reason**: .NET 10 added `TimeSpan.FromMinutes(long)` overload; an `int` argument now resolves to `long` instead of `double`. Explicit cast to `(double)` preserves original behavior and ensures deterministic overload resolution.

#### CS0433 — Program class ambiguity fix
`src/PublicApi/TestAnchor.cs` (new file)
- Added `public sealed class TestAnchor` in `Microsoft.eShopWeb.PublicApi` namespace
- Provides a named anchor type for `WebApplicationFactory<T>` that avoids conflict with Web's `Program` class

`tests/PublicApiIntegrationTests/ProgramTest.cs`
- Changed `WebApplicationFactory<Program>` → `WebApplicationFactory<TestAnchor>`
- Added `using Microsoft.eShopWeb.PublicApi;`
- **Reason**: Both Web and PublicApi define a `Program` class in the global namespace (C# top-level statements). After Web moved to net10.0, both assemblies are now the same TFM and the conflict becomes a build error (CS0433). Using a named anchor type from PublicApi's namespace resolves the ambiguity cleanly.

#### xUnit2013 — Collection size assertions in UnitTests and IntegrationTests
`tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
- `Assert.Equal(1, result.OrderItems.Count)` → `Assert.Single(result.OrderItems)` (line 22)
- `Assert.Equal(1, result[0].OrderItems.Count)` → `Assert.Single(result[0].OrderItems)` (line 35)

`tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (line 19)

`tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
- `Assert.Equal(0, basket.Items.Count)` → `Assert.Empty(basket.Items)` (line 38)

## Assessment Issues Addressed

| Rule | Status | Notes |
|---|---|---|
| Project.0002 | ✅ Fixed | TFM upgraded to net10.0 for Web, UnitTests, IntegrationTests |
| NuGet.0002 (Web) | ✅ Fixed | VersionOverride added for all AspNetCore/EF Core packages |
| NuGet.0002 (IntegrationTests) | ✅ Fixed | VersionOverride added for EFCore.InMemory |
| NuGet.0004 (Azure.Identity security) | ✅ Confirmed | Already at 1.21.0 from task 01 |
| NuGet.0005 (deprecated packages) | ⏭️ Deferred | AutoMapper.Extensions, System.IdentityModel.Tokens.Jwt, xunit packages — deferred per upgrade strategy |
| Api.0001 (configuration APIs) | ✅ No code change needed | DynamicallyAccessedMembers annotation removal does not cause errors in standard (non-AOT) builds |
| Api.0002 (TimeSpan.FromMinutes) | ✅ Fixed | Cast to (double) for deterministic overload resolution |
| Api.0003 (behavioral changes) | ✅ Informational | HttpContent streaming, URI limits, UseExceptionHandler, AddConsole — no code changes required for server-side ASP.NET Core |

## Build Results
- **Errors**: 0
- **Warnings (non-security)**: 0 in modified projects (xUnit2013 warnings fixed)
- **Warnings (NU190x security)**: Present — all are from deferred packages (AutoMapper 12.0.1, NuGet.Packaging/Protocol via VisualStudio.Web.CodeGeneration.Design) deferred per upgrade strategy

## Test Results
| Suite | Tests | Passed | Failed |
|---|---|---|---|
| UnitTests (net10.0) | 44 | 44 | 0 |
| IntegrationTests (net10.0) | 3 | 3 | 0 |
| FunctionalTests (net10.0) | 12 | 12 | 0 |

## Infrastructure Packages — Azure.Identity Note
The NU1902 warning about `Azure.Identity 1.10.3` from Infrastructure comes from a transitive dependency in Infrastructure's net8.0 build slice (not Web's direct reference). Web references Azure.Identity 1.21.0 (the fixed version set globally in task 01). This transitive issue in Infrastructure will be resolved when task 05 removes the net8.0 multi-targeting from shared projects.
