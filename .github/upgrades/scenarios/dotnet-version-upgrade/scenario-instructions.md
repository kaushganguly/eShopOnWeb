# .NET Version Upgrade — Scenario Instructions

## Scope

Upgrade eShopOnWeb solution (Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin,
BlazorShared, and associated test projects) from .NET 8.0 to .NET 10.0.

- Solution file: `eShopOnWeb.sln` (also `Everything.sln`)
- global.json SDK version update to 10.0.x
- Centrally managed TargetFramework update in `Directory.Packages.props`
- NuGet package version updates for ASP.NET Core / EF Core / Blazor / Azure SDK / test dependencies
- Resolve any API compatibility issues introduced by the version change

## Target Framework

- Target: **net10.0** (LTS, support ends 2028-11-14)

## User Preferences

### Execution Style

- **Flow Mode: Automatic** — do not pause for user input/review at any stage. Only pause if
  genuinely blocked (missing info, ambiguous decision with major consequences, unresolved error).
- Success criteria: build must pass, unit tests must pass, no new unit tests generated,
  security/compliance check required.

### Technical Preferences

- Target framework: net10.0 for all projects.

## Source Control

- Git repo: yes
- Working branch: `upgrade/dotnet10-net10-20261005` (created from clean `main`)
- Commit strategy: commit per task (or at logical checkpoints) on the working branch.

## Upgrade Options

**Strategy**
- **Upgrade Strategy**: All-at-Once — 10 SDK-style projects, all already on net8.0 (modern-to-modern),
  no .NET Framework, no CI-green constraint, single-session autonomous run; small scope with
  centrally-managed packages makes an atomic TFM bump the lowest-overhead approach.
  - Alternative — Top-Down: would stage entry-point apps first and multi-target libraries;
    unnecessary overhead for a 10-project solution with centralized package management.

(No other options applicable — Test Coverage not recommended by assessment;
all other options only trigger on .NET Framework / legacy-project signals, none present here.)

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 SDK-style projects, all currently net8.0 (modern-to-modern), centrally managed
packages (single property bump covers most dependents), ≤2 high-risk package migrations
(MediatR license-line jump, BlazorInputFile staleness), no CI-green constraint, single
autonomous session.

### Execution Constraints
- Single atomic upgrade — update global.json, Directory.Packages.props TFM + all package
  versions, and all project files together; validate full solution build after the upgrade.
- One bounded build-and-fix pass (not an open-ended retry loop) to resolve compilation errors
  across all 10 projects.
- Testing happens after the atomic upgrade completes successfully (not interleaved per project).
- Commit strategy: **Single Commit at End** (All-at-Once default) — one commit once build + tests
  pass.

## Decisions

- Flow mode: Automatic — surfaced the above options summary without pausing, per explicit user
  instruction to run fully autonomously with no confirmation pauses.
- Commit strategy updated to **Single Commit at End** (was: per-task) — better fit for
  All-at-Once strategy.
