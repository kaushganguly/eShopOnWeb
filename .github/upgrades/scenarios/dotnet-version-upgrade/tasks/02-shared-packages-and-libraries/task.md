# 02-shared-packages-and-libraries: 02-shared-packages-and-libraries

Execute task 02-shared-packages-and-libraries.

## Research Findings

### TargetFramework architecture
- Repo does **not** declare `<TargetFramework>` in any individual `.csproj`, and has no `Directory.Build.props`.
- `Directory.Packages.props` (repo root) declares `<TargetFramework>net8.0</TargetFramework>` as a plain MSBuild property, implicitly imported into every project via NuGet CPM (`ManagePackageVersionsCentrally=true`). All 10 projects inherit the TFM from this single property.
- Confirmed via `dotnet build src/ApplicationCore/ApplicationCore.csproj` succeeding pre-upgrade despite no explicit TFM in the csproj.
- **Action**: bumping `<TargetFramework>` to `net10.0` in `Directory.Packages.props` upgrades all 10 projects at once (consistent with All-At-Once strategy).

### Shared version properties
`Directory.Packages.props` defines 4 version-variable MSBuild properties referenced via `$(...)` by ~14 `PackageVersion` entries:
- `AspNetVersion` (8.0.2 → **10.0.12**) — covers all `Microsoft.AspNetCore.*`, `Microsoft.Extensions.Identity.Core`, `Microsoft.AspNetCore.Mvc`, `Microsoft.AspNetCore.Mvc.Testing`
- `SystemExtensionVersion` (8.0.0 → **10.0.12**) — covers `Microsoft.Extensions.Logging.Configuration`, `System.Net.Http.Json`
- `EntityFramworkCoreVersion` (8.0.2 → **10.0.12**) — covers `Microsoft.EntityFrameworkCore.InMemory/SqlServer/Tools`
- `VSCodeGeneratorVersion` (8.0.0 → **10.0.2**) — covers `Microsoft.VisualStudio.Web.CodeGeneration.Design`

All values verified individually via `get_supported_package_version(targetFrameworks="net10.0")`, re-queried per-package after discovering that large (~40 item) parallel tool-call batches could not be reliably re-associated with their package names during context compaction — smaller batches (≤4) and single calls were cross-verified as reliable.

### Individually-versioned package updates (verified via get_supported_package_version)
| Package | Old | New | Notes |
|---|---|---|---|
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 | unchanged, already latest compatible |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 | |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 | |
| Ardalis.Result | 7.0.0 | 10.1.0 | |
| Ardalis.Specification | 7.0.0 | 9.3.1 | |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 | unchanged |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 | |
| Azure.Identity | 1.10.4 | 1.21.0 | fixes assessment-flagged vulnerability |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 | unchanged |
| BlazorInputFile | 0.2.0 | 0.2.0 | unchanged; abandoned package, netstandard-based, expected to still work |
| Blazored.LocalStorage | 4.5.0 | 4.5.0 | unchanged; tool reported "no supported version" (no net10 compat metadata) but 4.5.0 is already the latest published version per nuget.org — kept as-is, netstandard-based so should still work at runtime |
| BuildBundlerMinifier | 3.2.449 | 3.2.449 | unchanged |
| FluentValidation | 11.9.0 | 12.1.1 | |
| MediatR | 12.0.1 | 14.2.0 | |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 | |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.22.1 | assessment flagged as NuGet.0001 incompatible; tool returned "no supported version" but nuget.org confirms 1.22.1 is latest stable — this is VS/Docker design-time tooling only (referenced only by PublicApi.csproj), bumped to latest stable |
| NSubstitute | 5.1.0 | 6.2.0 | |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 | unchanged |
| System.Security.Claims | 4.3.0 | 4.3.0 | unchanged |
| System.Text.Json | 8.0.3 | 10.0.12 | fixes assessment-flagged vulnerability |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.23.0 | |
| Swashbuckle.AspNetCore / SwaggerUI / Annotations | 6.5.0 | 10.2.3 | |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.10.1 | |
| xunit | 2.7.0 | 2.9.3 | stayed on v2 line (v3 would be a much larger breaking change, out of scope) |
| xunit.runner.visualstudio | 2.5.6 | 4.0.0 | verified via nuget.org package description: "It can run tests from xUnit.net 1.9.2 and later" — confirmed compatible with our xunit 2.9.3 |
| xunit.runner.console | 2.7.0 | 2.9.3 | |
| MSTest.TestAdapter / TestFramework | 3.2.2 | 4.4.1 | |
| coverlet.collector | 6.0.2 | 10.1.0 | |
| Microsoft.AspNetCore.Mvc | 2.2.0 (hardcoded, unused — no project references it directly) | switched to `$(AspNetVersion)` | cleanup only, no behavior change since unused |

### Known risk areas for this task's projects (ApplicationCore, BlazorShared, Infrastructure)
- `EmptyBasketOnCheckoutException.cs` (ApplicationCore) uses the obsolete `SerializationInfo`/`StreamingContext` binary-serialization constructor → SYSLIB0051 warning. Must be fixed (removed), not suppressed, per workflow rule on warnings.
- Watch for `ConfigurationBinder.Get<T>()` / `Configure<T>()` binary-incompatible overload changes and other Extensions.Configuration API shifts flagged in the assessment for Infrastructure.

## Actions Taken
1. Edited `Directory.Packages.props`: bumped `TargetFramework` to `net10.0`, bumped the 4 shared version properties, and bumped/kept individual package versions per the table above.
2. (next) Fix SYSLIB0051 in ApplicationCore, restore + build ApplicationCore/BlazorShared/Infrastructure, fix all compile errors and warnings.
