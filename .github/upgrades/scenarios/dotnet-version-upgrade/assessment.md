# Assessment — Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0

> Note: The dedicated assessment tool (`generate_dotnet_upgrade_assessment`) was unavailable in this
> session ("Orchestrator channel was not available" — repeated across 7 retries over ~3 minutes).
> This assessment was produced manually via direct project analysis and live NuGet registry queries,
> satisfying the same goal (dependency graph, package analysis, risks) that the tool would normally provide.

## Solution Overview

- Solution: `eShopOnWeb.sln` (14 projects; there is also an `Everything.sln` with 13 projects — same
  core projects, not targeted separately since all core/test projects are shared)
- Central Package Management: **enabled** (`Directory.Packages.props`, `ManagePackageVersionsCentrally=true`)
- Target Framework: centrally declared once in `Directory.Packages.props` (`<TargetFramework>net8.0</TargetFramework>`) — no project overrides it individually, so a single edit retargets the whole solution.
- `global.json` pins SDK `8.0.x` with `rollForward: latestFeature` — must be updated to `10.0.x` or removed/relaxed, otherwise the installed .NET 10 SDK will be rejected.

### Project Dependency Graph (topological order)

```
BlazorShared ─▶ ApplicationCore ─▶ Infrastructure ─▶ BlazorAdmin ─▶ Web ─▶ PublicApi
                                                                      │
      tests/UnitTests ──────────────────────────────────────────────┘ (+ ApplicationCore, Web)
      tests/PublicApiIntegrationTests ─▶ PublicApi, Web
      tests/FunctionalTests ─▶ ApplicationCore, PublicApi, Web
      tests/IntegrationTests ─▶ Infrastructure, UnitTests
```

All 10 projects (6 src + 4 test) are in scope: `ApplicationCore`, `Infrastructure`, `Web`, `PublicApi`,
`BlazorAdmin`, `BlazorShared`, `UnitTests`, `IntegrationTests`, `FunctionalTests`, `PublicApiIntegrationTests`.

## Package Analysis (current → proposed, net10-compatible)

Versions below were confirmed against the live NuGet v3 flat-container index (stable releases only).

| Package | Current | Proposed | Notes |
|---|---|---|---|
| Microsoft.AspNetCore.Components.Authorization | 8.0.2 (`$(AspNetVersion)`) | **10.0.12** | Blazor/ASP.NET Core family, bump `AspNetVersion` |
| Microsoft.AspNetCore.Components.WebAssembly | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Components.WebAssembly.Authentication | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Components.WebAssembly.Server | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Identity.UI | 8.0.2 | **10.0.12** | |
| Microsoft.Extensions.Identity.Core | 8.0.2 | **10.0.12** | |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | **10.0.12** | test package |
| Microsoft.Extensions.Logging.Configuration | 8.0.0 (`$(SystemExtensionVersion)`) | **10.0.12** | |
| System.Net.Http.Json | 8.0.0 | **10.0.12** | |
| System.Text.Json | 8.0.3 | **10.0.12** | security baseline also addressed by this bump |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.2 (`$(EntityFramworkCoreVersion)`) | **10.0.12** | |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.2 | **10.0.12** | |
| Microsoft.EntityFrameworkCore.Tools | 8.0.2 | **10.0.12** | |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 (`$(VSCodeGeneratorVersion)`) | **10.0.2** | separate release line from ASP.NET Core |
| Azure.Identity | 1.10.4 | **1.21.0** | Azure SDK |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | **1.5.2** | Azure SDK |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | **1.23.0** | |
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 | already latest |
| Ardalis.GuardClauses | 4.0.1 | **5.0.0** | |
| Ardalis.Result | 7.0.0 | **10.1.0** | |
| Ardalis.Specification | 7.0.0 | **9.3.1** | required for EF Core 10 compatibility |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | **9.3.1** | required for EF Core 10 compatibility |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 | already latest |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 | already latest (no net10-specific need) |
| FluentValidation | 11.9.0 | **12.1.1** | |
| MediatR | 12.0.1 | **12.4.1** | capped at the last MIT-licensed 12.x release — v13+ requires a commercial license (LuckyPennySoftware) for non-OSS use; staying on 12.x avoids introducing a new licensing obligation as a side effect of this TFM upgrade |
| Swashbuckle.AspNetCore(.SwaggerUI/.Annotations) | 6.5.0 | **10.2.3** | |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 | already latest; targets net6.0 (forward-compatible, no net10-specific asset) |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | **8.23.0** | |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | **3.0.114** | |
| BuildBundlerMinifier | 3.2.449 | 3.2.449 | already latest |
| NSubstitute | 5.1.0 | **6.2.0** | test package |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 | already latest |
| Microsoft.NET.Test.Sdk | 17.9.0 | **18.10.1** | test package |
| xunit | 2.7.0 | **2.9.3** | test package |
| xunit.runner.visualstudio | 2.5.6 | **3.1.5** | 4.0.0 exists but is very new/unproven; pin to the latest 3.x for safety |
| xunit.runner.console | 2.7.0 | **2.9.3** | test package |
| MSTest.TestAdapter | 3.2.2 | **4.4.1** | test package |
| MSTest.TestFramework | 3.2.2 | **4.4.1** | test package |
| coverlet.collector | 6.0.2 | **10.1.0** | test package |

### Packages Needing Special Handling

| Package | Issue | Resolution |
|---|---|---|
| **BlazorInputFile** 0.2.0 | Abandoned (last publish is a preview-era 0.2.0, no net10 asset). Only referenced via `@using BlazorInputFile` in `BlazorAdmin/_Imports.razor` and a single dead helper method `CatalogItem.DataToBase64(IFileListEntry)` in `BlazorShared/Models/CatalogItem.cs` — **no `.razor` markup anywhere in the solution actually uses an `<InputFile>`/`IFileListEntry` component**, confirmed via full-text search. | Remove the package reference and the dead code (unused `using` + unused static method) rather than attempt a same-shape replacement for code nothing calls. |
| **Microsoft.AspNetCore.Mvc** 2.2.0 | Orphaned `PackageVersion` entry in `Directory.Packages.props` — not referenced by any project's `PackageReference`. Latest available is still the legacy ASP.NET Core 2.x line (2.3.13), unrelated to the net10 shared framework. | Remove the unused `PackageVersion` entry as cleanup; it has no effect on the build either way. |
| **System.Security.Claims** 4.3.0 | Legacy compat-shim package (facade over functionality built into the BCL since .NET Core 2.x+). | Keep for now unless build proves it unnecessary; removing is optional cleanup, not required for the TFM bump. |

## Security Vulnerabilities

No CVEs were found pinned to the currently-referenced versions during this analysis (no automated
vulnerability scan tool was reachable this session). However, several packages carry known-fixed
issues in old release lines bundled into this upgrade regardless:
- `System.Text.Json` 8.0.3 → 10.0.12 (multiple System.Text.Json advisories were fixed in 8.0.4/8.0.5+; moving to the 10.x line supersedes all of them)
- `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.23.0 (JWT validation hardening across the 8.x line)

These are addressed as part of the package bump either way.

## Risks / Breaking Changes to Watch

1. **Blazor WebAssembly hosting model** — ASP.NET Core 8→10 continued consolidating Blazor WASM hosting; `Microsoft.AspNetCore.Components.WebAssembly.Server`/`DevServer` APIs used in `Web/Program.cs` need a build-time check.
2. **EF Core 8 → 10** — `Ardalis.Specification[.EntityFrameworkCore]` must move in lockstep to 9.3.1 (built against EF Core 9/10) or specification evaluation will fail to resolve at runtime/build.
3. **MediatR licensing** — intentionally capped at 12.4.1 (see table) to avoid silently pulling in a commercially-licensed major version as a side effect of the TFM bump.
4. **C# language version** — `LangVersion` is set to `latest` in `Web.csproj`; other projects don't pin a `LangVersion`, so the default will float up to C# 14 (paired with net10) automatically. No explicit action required but worth a quick review for new analyzer warnings.
5. **`global.json` SDK pin** — must move from `8.0.x` to `10.0.x` or the solution won't build with the installed SDKs (`10.0.112` / `10.0.204` / `10.0.303` / `10.0.401` are available; 8.x SDKs are also still present but shouldn't be used for this upgrade).
6. **Test framework major bumps** (`Microsoft.NET.Test.Sdk` 17→18, `MSTest` 3→4, `coverlet.collector` 6→10) — low historical risk for this repo's simple test usage, but full test run after upgrade is required per success criteria.

## Checklist

- [x] Analysis complete (manual, tool-assisted via `get_supported_package_version` + live NuGet queries)
- [x] assessment.md created
- [x] Security-relevant package bumps documented (see above) — included in upgrade by default
- [x] Assessment summary to be surfaced to user in Automatic mode (no pause)
