# 05-final-validation: Full solution build and test validation

## Objective

Perform a clean, from-scratch restore/build/test pass of the entire `eShopOnWeb.sln` (all 10
projects, both `Debug` and `Release` configurations), confirm zero errors/warnings, confirm the
full test suite passes, re-confirm no vulnerable packages remain, and document any deferred/
out-of-scope items surfaced by the assessment.

## Validation performed

1. Deleted all `bin`/`obj` folders solution-wide to eliminate any incremental-build masking.
2. `dotnet restore eShopOnWeb.sln` — clean restore, all 11 projects (10 app/test projects) restored
   successfully against `net10.0` with no restore warnings/errors.
3. `dotnet build eShopOnWeb.sln --configuration Release` — **Build succeeded, 0 Warning(s),
   0 Error(s)**. (Also previously re-verified in `Debug`, the default configuration used throughout
   tasks 01-04, with the same 0/0 result.)
4. `dotnet test eShopOnWeb.sln --configuration Release --no-build`:
   - `UnitTests.dll` — 44/44 passed
   - `IntegrationTests.dll` — 3/3 passed
   - `FunctionalTests.dll` — 12/12 passed
   - `PublicApiIntegrationTests.dll` — 15/15 passed
   - **Total: 74/74 passed, 0 failed, 0 skipped** (matches the CI workflow's own
     `dotnet test`/Release convention in `.github/workflows/dotnetcore.yml`).
5. `dotnet list <project> package --vulnerable --include-transitive` run individually against all
   10 projects (the solution-level command silently omits some projects on this SDK) —
   **no vulnerable packages found in any project**. This includes the originally-tracked
   `Azure.Identity 1.10.4` moderate CVE, resolved in task 03 by bumping to `1.21.0`, and the
   `NuGet.Packaging`/`NuGet.Protocol` GHSA-g4vj-cjjj-v7hg advisory, resolved in task 03 via a
   central `PackageVersion` override to `6.12.5`.
6. `dotnet list eShopOnWeb.sln package --deprecated` — one deprecation family remains, tracked as a
   deliberate deferral (see below).

## Deferred / out-of-scope items (documented per plan.md's "Done when" criteria)

- **`xunit` v2 → `xunit.v3` migration** (`UnitTests`, `IntegrationTests`, `FunctionalTests`):
  `dotnet list package --deprecated` flags `xunit 2.9.3` / `xunit.runner.console 2.9.3` as
  "Legacy" with `xunit.v3` as the suggested alternative. The original assessment (task 04 research)
  itself recommended staying on the v2 line for this upgrade — migrating to xunit v3 is a
  separate, higher-risk breaking change (new assembly names, `[Fact]`/`[Theory]` behavior changes,
  different test-runner model) that is out of scope for a pure .NET 8 → .NET 10 framework-version
  upgrade. All xunit-based tests (59 of 74) pass on the current `2.9.3` version. Recommendation:
  track `xunit.v3` migration as a separate, dedicated modernization task if desired.
- **`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` / `Microsoft.Web.LibraryManager.Build`**
  (bumped in task 03 to the latest versions the assessment tool could resolve, `1.22.1` /
  `3.0.114` respectively) — no further action needed, but noting these are IDE/tooling-only
  packages with no runtime impact, included here for completeness of the version-bump record.
- **`BlazorInputFile` removal** (task 04): this was a genuine code change (dead-code + abandoned
  package removal), not merely a version bump — called out again here since it changed public
  surface area of `BlazorShared.Models.CatalogItem` (removed the unused `DataToBase64` method).
  Confirmed zero callers solution-wide before removal.

## Success criteria check (against original request)

| Criterion | Result |
|---|---|
| `passBuild=true` | ✅ `dotnet build eShopOnWeb.sln` (Debug and Release) — 0 errors, 0 warnings |
| `generateNewUnitTests=false` | ✅ No new test files authored; only existing tests fixed for analyzer warnings (xUnit2013/MSTEST0044) — assertions rewritten to equivalent, more-idiomatic forms, no new test methods added |
| `passUnitTests=true` | ✅ 74/74 existing tests pass across all 4 test projects |
| `securityComplianceCheck=true` | ✅ No vulnerable packages in any of the 10 projects; both tracked CVEs (Azure.Identity, NuGet.Packaging/NuGet.Protocol) resolved |
