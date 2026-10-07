# Scenario Instructions — dotnet-version-upgrade

## Goal
Upgrade the eShopOnWeb solution (ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin, BlazorShared,
and all test projects) from .NET 8.0 to .NET 10.0, including the centrally managed TargetFramework
(Directory.Packages.props / Directory.Build.props) and all NuGet package versions (ASP.NET Core, EF Core,
Azure SDK, Blazor, test packages) to versions compatible with .NET 10. Resolve any resulting API/build
compatibility issues across all projects.

## Parameters
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- Target Framework: net10.0 (LTS)
- TaskId (external tracking): 001-upgrade-dotnet-to-net10
- Success Criteria: passBuild=true, generateNewUnitTests=false, passUnitTests=true, securityComplianceCheck=true

## Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for user review/approval at any stage. Only pause if genuinely blocked.
- generateNewUnitTests = false — do not add new unit tests; only keep existing tests passing.

### Source Control
- Working branch: `upgrade/net10-001-upgrade-dotnet-to-net10` (created from `main`, which was clean)

## Upgrade Options

- **Upgrade Strategy**: All-At-Once — selected: 10 projects (6 src + 4 test), all already SDK-style
  and on `net8.0`, Central Package Management in use, dependency graph only 3 tiers deep, no .NET
  Framework projects. Rationale: well within the All-At-Once "ideal" criteria (≤15 projects, ≤3 tiers,
  ≤2 high-risk package migrations — only `Ardalis.Specification*` + `BlazorInputFile` removal qualify
  as notable).
- **Test Coverage option**: not applicable — `generateNewUnitTests=false` per task success criteria;
  existing tests must keep passing but no new tests are generated.
- Complexity classification: **Simple** (all SDK-style, all net8.0+, no other complex triggers).

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all on .NET 8.0 today, all SDK-style, CPM already centralizes versions, dependency graph only 3 tiers deep, no .NET Framework projects.

### Execution Constraints
- Single atomic upgrade — all projects updated together (one edit to `Directory.Packages.props` + `global.json` retargets everything)
- Validate with a full solution build after the upgrade — one bounded fix pass, not iterative back-and-forth
- Testing happens after the atomic upgrade completes successfully, as its own final task
- Commit Strategy: **Single Commit at End** (recommended default for All-at-Once)

## Known Tool Limitation (this session)

`generate_dotnet_upgrade_assessment` (dedicated DotnetVersionAssessor) returned
"Orchestrator channel was not available within 5000 ms. Extender is not connected to an orchestrator
peer." on every retry (7 attempts, ~3 minutes of backoff). Assessment was produced manually instead
(project file inspection + live NuGet registry queries + `get_supported_package_version` spot checks).
If `generate_dotnet_upgrade_assessment` or other orchestrator-backed tools are needed later and still
fail the same way, treat it as an environment limitation, not a retry-forever condition — fall back to
manual equivalents (as done here) rather than blocking the upgrade.

## Decisions

- **MediatR capped at 12.4.1** (not bumped to latest) — v13+ switched to a commercial
  license (LuckyPennySoftware); 12.4.1 is the last MIT-licensed release. Deliberate
  choice to avoid introducing a licensing change as an upgrade side effect.
- **AutoMapper bumped to 16.2.0** (latest, not capped) — AutoMapper 15.0.0+ also
  switched to a non-MIT license, but the known DoS vulnerability
  (GHSA-rvv3-g6hj-g44x, unbounded recursive mapping) is unpatched in every
  MIT-licensed release (`< 15.1.1`). Since `securityComplianceCheck=true` is an
  explicit success criterion, security was prioritized over licensing consistency —
  an intentional deviation from the MediatR decision, flagged prominently in the
  final report. `AutoMapper.Extensions.Microsoft.DependencyInjection` (hard-pinned to
  AutoMapper `[12.0.1]`) was replaced with a direct `AutoMapper` package reference
  since DI registration is now built into the core package (14.0+).
- **BlazorInputFile removed** (0.2.0, abandoned, no net10 asset) — confirmed genuinely
  dead code (no `.razor` file used `InputFile`/`IFileListEntry`); safe removal, not a
  replacement scenario.
- **NuGet.Packaging/NuGet.Protocol pinned to 6.12.5** via per-project `PackageReference`
  (not `GlobalPackageReference`, which broke the non-SDK `docker-compose.dcproj`) to
  remediate a transitive low-severity vulnerability from
  `Microsoft.VisualStudio.Web.CodeGeneration.Design`.
- **`PublicApiIntegrationTests` no longer references `Web.csproj`** — the reference was
  only needed because one test incorrectly deserialized into an unrelated `Web` MVC
  view model; switched to the correct `PublicApi` response model instead. This also
  fixed a net10 runtime regression where `Web` → `BlazorAdmin` → `Microsoft.JSInterop.WebAssembly`
  crashed with a `TypeLoadException` when reflected over by ASP.NET Core's
  `ApplicationPartManager` in the test host process (confirmed via `git stash` A/B test
  that this passed unmodified on net8.0, so it's a genuine upgrade regression, not
  pre-existing).
- Implicit top-level `Program` class visibility changed from `internal` (net8 SDK) to
  `public` (net10 SDK) by default — confirmed via isolated repro. Noted here in case it
  affects future work in this repo (e.g. other `WebApplicationFactory<Program>` usages).
