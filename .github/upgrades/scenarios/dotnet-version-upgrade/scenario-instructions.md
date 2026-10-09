# Scenario Instructions — .NET Version Upgrade

## Goal
Upgrade eShopOnWeb solution (ApplicationCore, Infrastructure, BlazorShared, BlazorAdmin, Web, PublicApi,
and all test projects) from .NET 8.0 to .NET 10.0, including TFMs, centrally managed NuGet package
versions (ASP.NET Core, Entity Framework Core, and related dependencies), and resolving resulting
API/behavioral compatibility issues.

## Parameters
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- Target Framework: net10.0 (LTS)
- Scope: solution-wide (all projects)

## Preferences

### Flow Mode
**Automatic** — do not pause for user input/review at any stage. Only pause if genuinely blocked
(missing info, ambiguous decision with major consequence, unresolved build/test error after
reasonable attempts).

### Source Control
- Git repo detected, working tree was clean.
- Created working branch `upgrade/net10` off `main` before initializing scenario.
- Commit strategy: **Single Commit at End** (recommended default for All-at-Once strategy —
  updated from initial "commit at logical checkpoints" placeholder once strategy was selected).

### Success Criteria (user-specified)
- passBuild = true
- generateNewUnitTests = false
- passUnitTests = true
- securityComplianceCheck = true

## Decisions
- Task requirements explicitly named target framework (net10.0) and scope, so pre-initialization
  confirmation prompt was skipped per Automatic mode rules (all required parameters were already known).
- Upgrade complexity classification: **Simple** (all 10 projects already on net8.0/SDK-style,
  CPM already in place, no .NET Framework projects, no unsupported-API signals surfaced).
  Only the Upgrade Strategy option was evaluated; Test Coverage was not recommended and
  `generateNewUnitTests=false` was explicitly requested by the user, so it was not added.
- Upgrade Strategy selected: **All-at-Once** — 10 projects (≤15), purely mechanical TFM bump
  net8.0→net10.0, no CI-green constraint stated, no multi-team concurrency signal. Per Automatic
  mode this default was accepted without pausing for confirmation (surfaced here instead).

## Upgrade Options
- Strategy: All-at-Once
- Test Coverage: Skip (user explicitly set generateNewUnitTests=false)

## Strategy
**Selected**: All-at-Once
**Rationale**: 10 projects (≤15), all already on net8.0/SDK-style with CPM, mechanical TFM
bump to net10.0, no CI-green constraint, no multi-team concurrency signal.

### Execution Constraints
- Single atomic upgrade — all projects updated together (TFM + package bumps) in one pass
- Update project files and Directory.Packages.props first, then restore, then build and fix
  all compilation errors in one bounded pass (not an iterative retry loop)
- Testing comes AFTER the atomic upgrade build succeeds
- Validate full solution build (0 errors) before running tests
- Commit once at the end (Single Commit at End)
