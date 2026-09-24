# Scenario Instructions: dotnet-version-upgrade

## Goal
Upgrade eShopOnWeb (Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, and
associated test projects: UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests)
from .NET 8.0 to .NET 10.0.

## Parameters
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- Target Framework: net10.0 (LTS)
- Task ID (external tracking): 001-upgrade-dotnet-to-net10
- Work folder (external tracking): .github/modernize/upgrade-to-lts-20260924072938

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for user confirmation at any stage. Only stop if genuinely
  blocked (missing info, ambiguous decision with major consequences, unresolved build/test failure
  after reasonable attempts).
- Success criteria required: passBuild=true, passUnitTests=true, generateNewUnitTests=false (do not
  author new unit tests, only fix/update existing ones as needed), securityComplianceCheck=true
  (no known vulnerable package versions after upgrade).

### Source Control
- Working branch: upgrade/net10-001-upgrade-dotnet-to-net10 (already created and checked out before
  scenario initialization).
- Repo was clean (no pending changes) prior to starting.

## Upgrade Options

| Option | Selected | Rationale |
|--------|----------|-----------|
| Upgrade Strategy | All-at-Once | 10 projects, all already net8.0 (modern .NET, not Framework), low migration risk per assessment (all 🟢 Low difficulty), ≤3-tier depth — small scope, mechanical TFM bump matches All-at-Once default logic. Flow mode is Automatic, so this default was accepted without pausing. |

Test Coverage option not applicable — assessment did not surface a Test Coverage recommendation.
All other options (Project Approach, Package Management, Unsupported Packages, etc.) not applicable
— no .NET Framework projects, solution already uses CPM (Directory.Packages.props), no incompatible
(unsupportable) packages reported, only ordinary NuGet upgrade/vulnerability findings.

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all already on net8.0 (modern .NET), all 🟢 Low difficulty per assessment, ≤3-tier dependency depth, no CI-green constraint — small scope, mechanical TFM bump.

### Execution Constraints
- Single atomic upgrade — all 10 projects updated together (TFM bumps + package bumps + code fixes)
- No tier ordering / no phasing — but plan.md splits tasks by concern (libraries, apps, tests, validation) for manageability
- Update all project files first, then all package references, then restore, then build-and-fix in one bounded pass (not build→fix→rebuild ambiguity)
- Testing happens after the atomic code upgrade completes successfully (task 04/05)
- Validate full solution build after upgrade; fix all warnings in touched projects (not just errors)

### Commit Strategy
**Selected**: Single Commit at End (matches All-at-Once default) — recommend one commit once the full upgrade + validation succeeds, since this is a small, low-risk atomic upgrade. Will still commit incrementally if needed if scope grows unexpectedly.

## Decisions
- Target framework net10.0 chosen directly per explicit user task requirements (no need to prompt
  user to pick between net9.0/net10.0/net11.0).
- Centrally managed package versions via Directory.Packages.props — update central versions to
  .NET 10 compatible versions.
- global.json SDK version must be updated to a .NET 10 SDK version.
