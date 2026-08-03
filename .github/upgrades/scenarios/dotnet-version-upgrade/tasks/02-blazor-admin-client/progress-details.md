# Task 02-blazor-admin-client: Progress Details

## Summary

Successfully upgraded BlazorAdmin and BlazorShared to net10.0. Because TFM and package versions are managed centrally in `Directory.Packages.props`, this task also performed the central changes that affect all projects. BlazorAdmin and BlazorShared build cleanly with **0 warnings and 0 errors**.

## Central Changes (Directory.Packages.props)

### Target Framework
| Property | Before | After |
|---|---|---|
| `TargetFramework` | net8.0 | net10.0 |

### Package Version Variables
| Variable | Before | After | Affects |
|---|---|---|---|
| `AspNetVersion` | 8.0.2 | 10.0.10 | All Microsoft.AspNetCore.* packages |
| `SystemExtensionVersion` | 8.0.0 | 10.0.10 | Microsoft.Extensions.Logging.Configuration |
| `EntityFramworkCoreVersion` | 8.0.2 | 10.0.10 | All Microsoft.EntityFrameworkCore.* packages |
| `VSCodeGeneratorVersion` | 8.0.0 | 10.0.2 | Microsoft.VisualStudio.Web.CodeGeneration.Design |

### Explicit Package Version Updates
| Package | Before | After | Reason |
|---|---|---|---|
| `System.Text.Json` | 8.0.3 | 10.0.10 | Recommended upgrade |
| `Azure.Identity` | 1.10.4 | 1.21.0 | Security vulnerability fix + transitive conflict: SqlClient 6.1.1 requires >= 1.14.2 |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | 8.19.2 | Transitive conflict: JwtBearer 10.0.10 requires >= 8.19.2 |

### Package Entries Removed
| Package | Reason |
|---|---|
| `System.Security.Claims 4.3.0` | Functionality included in .NET 10 framework; assessment flagged as "package functionality is included with framework reference" |
| `System.Net.Http.Json` | Functionality included in .NET 10 framework (NU1510 warning confirmed it won't be pruned) |

## Project File Changes

### src/ApplicationCore/ApplicationCore.csproj
- Removed `<PackageReference Include="System.Security.Claims" />` — now part of .NET 10 framework

### src/PublicApi/PublicApi.csproj
- Removed `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` — incompatible with net10.0 (assessment flagged ⚠️NuGet package is incompatible)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `<PackageReference Include="System.Net.Http.Json" />` — now part of .NET 10 framework (eliminated NU1510 warning)
- All package versions updated implicitly via `Directory.Packages.props` central version bumps

### src/BlazorShared/BlazorShared.csproj
- No direct changes needed — inherits TFM from central props; all packages (`BlazorInputFile`, `FluentValidation`) remain compatible

## Build Results

### BlazorAdmin & BlazorShared (Task Scope)
```
BlazorShared → bin/Debug/net10.0/BlazorShared.dll
BlazorAdmin  → bin/Debug/net10.0/BlazorAdmin.dll
BlazorAdmin (Blazor output) → bin/Debug/net10.0/wwwroot

Build succeeded. 0 Warning(s). 0 Error(s).
```
✅ **Done-When criteria met**: BlazorShared and BlazorAdmin both target net10.0 and build cleanly.

### Full Solution Build — Remaining Issues in Other Projects

These issues are **expected** and will be fixed in Tasks 03 and 04:

#### Errors (1)
| Project | Error | Task to Fix |
|---|---|---|
| `PublicApiIntegrationTests` | `CS0433: The type 'Program' exists in both 'PublicApi' and 'Web'` — test project references both app entry points which both define internal `Program` class via top-level statements | Task 04 |

#### Warnings in ApplicationCore (Task 03)
| Warning | Location | Description |
|---|---|---|
| `NU1510` | `ApplicationCore.csproj` | `System.Text.Json` is now in-framework; explicit PackageReference should be removed |
| `SYSLIB0051` | `EmptyBasketOnCheckoutException.cs:12` | `Exception(SerializationInfo, StreamingContext)` serialization constructor is obsolete |

#### Warnings in PublicApi / Web (Task 03)
| Warning | Description |
|---|---|
| `NU1903` | `AutoMapper 12.0.1` has known high-severity vulnerability — package needs upgrade or replacement |
| `NU1901` | `NuGet.Packaging 6.12.1`, `NuGet.Protocol 6.12.1` — low severity, transitive from .NET SDK tooling |

#### Warnings in Test Projects (Task 04)
| Warning | Location | Description |
|---|---|---|
| `xUnit2013` | `UnitTests/CustomerOrdersWithItemsSpecification.cs:22,35` | Use `Assert.Single` instead of `Assert.Equal(1, ...)` |
| `xUnit2013` | `UnitTests/BasketRemoveEmptyItems.cs:19` | Use `Assert.Empty` instead of `Assert.Equal(0, ...)` |
| `xUnit2013` | `IntegrationTests/SetQuantities.cs:38` | Use `Assert.Empty` instead of `Assert.Equal(0, ...)` |
| `NU1903/NU1901` | All test projects | AutoMapper / NuGet.Packaging vulnerability (transitive) |

## Assessment API Incidents — BlazorAdmin Disposition

The assessment flagged 3 API incidents in BlazorAdmin (Program.cs, HttpService.cs):

| API | Category | File | Action Taken |
|---|---|---|---|
| `System.Net.Http.HttpContent` | Behavioral Change | `HttpService.cs` | No code change needed — `ReadAsStringAsync()` still works; runtime behavior difference is acceptable |
| `System.Uri` | Behavioral Change | `Program.cs` | No code change needed — `new Uri(baseAddress)` still works; behavioral change is a tightened validity check |
| `OptionsConfigurationServiceCollectionExtensions.Configure<T>(IConfiguration)` | Binary Incompatible | `Program.cs` | No code change needed — binary incompatible means recompile against new assemblies is sufficient; method API is unchanged at source level |

All three were resolved by recompiling against the new net10.0 packages — no source edits to BlazorAdmin or BlazorShared were required.

## Files Modified
- `Directory.Packages.props` — central TFM + package version upgrades, removed incompatible/in-framework packages
- `src/ApplicationCore/ApplicationCore.csproj` — removed `System.Security.Claims` reference
- `src/PublicApi/PublicApi.csproj` — removed incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`
- `src/BlazorAdmin/BlazorAdmin.csproj` — removed `System.Net.Http.Json` (now in-framework)
