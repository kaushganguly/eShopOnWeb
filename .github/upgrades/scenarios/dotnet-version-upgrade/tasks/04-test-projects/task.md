# 04-test-projects: Upgrade UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests to net10.0

## Scope
`tests/UnitTests`, `tests/IntegrationTests`, `tests/FunctionalTests`, `tests/PublicApiIntegrationTests`.
TargetFramework already net10.0 (central, task 01). This task resolves remaining test-package versions.

## Research findings (assessment via query_dotnet_assessment)
- **UnitTests**: 3 issues, all Optional/Mandatory (TFM change + 2 deprecated-package flags: xunit, xunit.runner.console). 0 API issues.
- **IntegrationTests**: 3 issues (TFM change, EF Core InMemory version bump — handled centrally, xunit deprecated flag). 0 API issues.
- **PublicApiIntegrationTests**: 11 issues (TFM change, Mvc.Testing version bump — handled centrally, MSTest.TestAdapter/TestFramework deprecated flags, 7 Api.0003 behavioral-change occurrences).
- **FunctionalTests**: 33 issues (TFM change, Mvc.Testing + EF InMemory version bumps — handled centrally, xunit deprecated flag, 29 Api.0003 behavioral-change occurrences — all `HttpContent.ReadAsStringAsync()`/`System.Uri.ToString()` in HTTP-integration test assertions, same pattern as task 03's behavioral-change findings: charset-detection/URI-parsing refinements that don't affect these simple string/Contains assertions).

## Package changes (central, Directory.Packages.props)
| Package | Old | New | Reason |
|---|---|---|---|
| `xunit` | 2.7.0 | 2.9.3 | Deprecated flag (NuGet.0005); latest resolvable 2.x version per `get_supported_package_version` (avoids the v2→v3 migration, which is a breaking API change out of scope for this upgrade) |
| `xunit.runner.console` | 2.7.0 | 2.9.3 | Same as above, keep in lockstep with `xunit` |
| `MSTest.TestAdapter` / `MSTest.TestFramework` | 3.2.2 | 3.11.1 | Deprecated flag (NuGet.0005); `get_supported_package_version` resolved 4.4.1 (major bump), but chose latest 3.x patch (3.11.1, confirmed to exist via NuGet flat-container listing) to avoid an unnecessary/riskier major-version jump — consistent with the conservative-upgrade approach used for AutoMapper/xunit in this scenario. |
| `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory` | 8.0.2 | 10.0.12 | Already resolved centrally via `$(AspNetVersion)`/`$(EntityFramworkCoreVersion)` (bumped tasks 01/02/03) — no project-specific edit needed. |
| `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `coverlet.collector`, `NSubstitute`, `NSubstitute.Analyzers.CSharp` | unchanged | unchanged | Assessment marks these ✅Compatible already on net10.0; no action needed. |

## API/behavioral issues
All flagged issues across the 4 test projects are Api.0003 (behavioral change only) for `HttpContent.ReadAsStringAsync()`
and `System.Uri` — consistent with task 03's findings. These are test-assertion call sites (string `.Contains()` checks,
`RequestUri.ToString()` comparisons) that are not sensitive to the specific charset/URI-parsing refinements introduced
in later .NET versions. No source changes required; verified by running the actual test suites (see progress-details.md).

## Validation plan
1. `dotnet restore` after central package bumps.
2. `dotnet build` each of the 4 test projects individually.
3. `dotnet test` each of the 4 test projects; confirm pass rate (task's `generateNewUnitTests=false`, `passUnitTests=true` — no new tests to be written, existing tests must pass).
