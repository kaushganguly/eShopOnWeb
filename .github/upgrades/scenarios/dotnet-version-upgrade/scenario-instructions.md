# Scenario Instructions: .NET Version Upgrade

## Scope

- Solution: `eShopOnWeb.sln` (identical project set to `Everything.sln`)
- Projects: ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure, PublicApi, Web
- Test projects: FunctionalTests, IntegrationTests, PublicApiIntegrationTests, UnitTests
- Source target framework: net8.0
- **Target framework: net10.0** (confirmed — user-provided, LTS, support ends 2028-11-14)

## Preferences

### Flow Mode
**Automatic** — do not pause for user confirmation at any stage. Surface assessment/plan/progress
inline and keep moving unless genuinely blocked. Confirmed by explicit user instruction: "Run in
Automatic flow mode — do NOT pause for user input at any point."

### Technical Preferences
- Target framework: net10.0 (explicitly requested by user)
- Package versions centrally managed via `Directory.Packages.props` — update TargetFramework and
  package versions there for net10 compatibility.
- generateNewUnitTests = false — do not author new unit tests, only fix/adapt existing ones as
  needed for compilation/breaking changes.

### Execution Style
- Fully autonomous, end-to-end execution.
- Success criteria: passBuild=true, passUnitTests=true, securityComplianceCheck=true.

## Upgrade Options

| Option | Selected | Rationale |
|---|---|---|
| Upgrade Strategy | All-At-Once | 10 projects, all already on net8.0 (modern), all SDK-style, low complexity (🟢) per assessment, shallow dependency graph, no .NET Framework projects, no OData/Newtonsoft/LINQ-to-SQL high-risk migrations. Fits All-At-Once criteria exactly. |
| Test Coverage | Skip | User explicitly set `generateNewUnitTests=false` in task requirements — do not author new tests. |

*(Auto-confirmed under Automatic flow mode per explicit user instruction to never pause for confirmation.)*

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all on .NET 8.0, all SDK-style, low per-project difficulty (🟢 Low across the board), shallow dependency graph. Assessment shows only 2 binary-incompatible + a handful of source/behavioral API issues — well within All-At-Once tolerance.

### Execution Constraints
- Single atomic upgrade — all projects (src + tests) updated together in one pass
- Update TargetFramework centrally in `Directory.Packages.props` / project files, then bump all package versions
- Restore, then build the full solution and fix all compilation errors in one bounded pass (not an iterative retry loop)
- Testing happens AFTER the atomic upgrade build succeeds
- Commit strategy: Single Commit at End (matches All-At-Once default)

## Source Control

- Git repo: yes
- Working branch: `upgrade/net10` (created from `main`, which was clean at start)
- Commit strategy: Single Commit at End (matches All-At-Once strategy default; no push unless requested)
