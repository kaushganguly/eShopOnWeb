# Scenario Instructions — dotnet-version-upgrade

## Scope

Upgrade the eShopOnWeb solution (`eShopOnWeb.sln`) from .NET 8.0 to .NET 10.0.

Projects in scope:
- Web
- PublicApi
- ApplicationCore
- Infrastructure
- BlazorAdmin
- BlazorShared
- UnitTests
- IntegrationTests
- FunctionalTests
- PublicApiIntegrationTests

## Target Framework

- Target: `net10.0` (LTS, support ends 2028-11-14)

## User Preferences

### Flow Mode

- **Automatic** — do not pause for user input at any workflow stage. Only stop if genuinely blocked
  (missing information, unresolvable ambiguity, or unrecoverable errors). Surface assessment, plan,
  and progress inline without waiting for approval.

### Execution Style

- Fully autonomous, end-to-end execution requested by user.
- generateNewUnitTests = false — do not author new unit tests, only fix/adapt existing ones as needed for the upgrade.
- Success criteria: solution builds (passBuild=true), existing unit tests pass (passUnitTests=true), security/vulnerability compliance check passes (securityComplianceCheck=true).

## Upgrade Options

| Option | Selected | Rationale |
|--------|----------|-----------|
| Strategy | All-at-Once | 10 projects, all already net8.0 (modern-to-modern), low difficulty across the board, no CI-green constraint, no urgency signal — small scope default per strategy option default logic |
| Test Coverage | Skip | User explicitly set generateNewUnitTests=false; existing tests will be fixed/adapted as needed, no new tests authored |

(Resolved automatically per Automatic flow mode — user requested no pauses and provided generateNewUnitTests=false explicitly.)

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects, all already net8.0 (modern-to-modern), low difficulty across the board per assessment, ≤3-tier dependency depth, no CI-green constraint, no urgency signal.

### Execution Constraints
- Single atomic upgrade — all 10 projects updated together (TFM bump + CPM package version bumps + code fixes)
- One bounded build-and-fix pass per task group — not an iterative retry loop
- Testing happens after the atomic upgrade of a group completes (validate build first, then tests in 04/05)
- Commit strategy: Single Commit at End (per All-at-Once default) — see Source Control below
- No SDK-style conversion needed (all projects already SDK-style); no multi-targeting needed (modern-to-modern)

## Source Control

- Git repo. Working branch: `upgrade/dotnet10` (created from clean `main`).
- Commit strategy: Single Commit at End (default for All-at-Once strategy) — no push unless requested.
