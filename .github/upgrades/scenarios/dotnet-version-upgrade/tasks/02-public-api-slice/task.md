# 02-public-api-slice: PublicApi Application Slice — net10.0 Upgrade

## Objective

Upgrade the `PublicApi` application slice to `net10.0`, including its direct dependencies
(`ApplicationCore`, `Infrastructure`) and the integration tests that cover it
(`PublicApiIntegrationTests`). This task absorbs the first round of `TargetFramework` and package
changes introduced in task 01 (central package management via `Directory.Packages.props`).

## Scope

**Projects modified:**

| Project | Path |
|---|---|
| ApplicationCore | `src/ApplicationCore/ApplicationCore.csproj` |
| ApplicationCore exception | `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` |
| PublicApiIntegrationTests (test anchor) | `tests/PublicApiIntegrationTests/ProgramTest.cs` |

**Projects verified green (no changes needed):**

| Project | Path | Notes |
|---|---|---|
| Infrastructure | `src/Infrastructure/Infrastructure.csproj` | No compilation changes required; all package versions resolved from central management |
| PublicApi | `src/PublicApi/PublicApi.csproj` | No source or project-file changes required |

## Research Findings

### TargetFramework
All in-scope projects inherit `net10.0` from `Directory.Packages.props`
(`<TargetFramework>net10.0</TargetFramework>` under the `ManagePackageVersionsCentrally=true`
property group). No per-project `TargetFramework` entries are needed.

### Warnings fixed

| Warning | Source file | Fix applied |
|---|---|---|
| `NU1510` — `System.Security.Claims` unnecessary | `ApplicationCore.csproj` | Removed; inbox on net10.0 |
| `NU1510` — `System.Text.Json` unnecessary | `ApplicationCore.csproj` | Removed; inbox on net10.0 |
| `SYSLIB0051` — obsolete `Exception(SerializationInfo, StreamingContext)` | `EmptyBasketOnCheckoutException.cs` | Removed serialization constructor |
| `CS0433` — ambiguous `Program` type (PublicApi vs Web) | `ProgramTest.cs` | Changed `WebApplicationFactory<Program>` → `WebApplicationFactory<MappingProfile>` |

### Deferred items (non-blocking, net10.0 build is green)

| Warning | Package | Reason for deferral |
|---|---|---|
| `NU1903` HIGH — AutoMapper 12.0.1 vulnerability (GHSA-rvv3-g6hj-g44x) | `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 | Fixing requires migrating to AutoMapper 13+ (built-in DI, breaking API changes); explicitly deferred in task spec |
| `NU1901` LOW — NuGet.Packaging 6.12.1 vulnerability (GHSA-g4vj-cjjj-v7hg) | Transitive from code-gen/EF tooling | Tooling-only transitive; low severity; addressed in tooling update task |
| `NU1901` LOW — NuGet.Protocol 6.12.1 vulnerability (GHSA-g4vj-cjjj-v7hg) | Transitive from code-gen/EF tooling | Same as above |
| `NU1510` — `System.Net.Http.Json` in BlazorAdmin | `BlazorAdmin.csproj` | Out of scope for this task (Blazor slice handled in a later task) |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 | `PublicApi.csproj` | Assessment notes it as package-incompatible; does not block build currently; keep as-is until a compatible version ships |

### Binary/source compatibility
The assessment listed `ConfigurationBinder.Get<T>`, `OptionsConfigurationServiceCollectionExtensions.Configure<T>`, and
`ConfigurationBinder.GetValue` as potentially binary-incompatible with net10.0. After building, **none of these required
source-level changes** — they compiled cleanly after the framework upgrade.

## Done When

- [x] `ApplicationCore` builds warning-free for actionable warnings (net10.0) — `NU1510` and `SYSLIB0051` resolved
- [x] `Infrastructure` builds clean on net10.0
- [x] `PublicApi` builds on net10.0 (0 errors)
- [x] `PublicApiIntegrationTests` builds and all 15 tests pass on net10.0
- [x] Deferred follow-up items documented above
