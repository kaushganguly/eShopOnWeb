# Upgrade Plan

## Strategy Declaration
- **Selected Strategy**: Top-Down (Application-First)
- **Target Framework**: net10.0
- **Unsupported Packages**: Defer Resolution
- **Unsupported API Handling**: Fix Inline

This upgrade is a modern .NET 8 to .NET 10 move across 10 SDK-style projects. The selected Top-Down strategy prioritizes application entry points first because `src/Web`, `src/PublicApi`, and `src/BlazorAdmin` carry the highest runtime and behavioral risk, while shared libraries are consolidated afterward once application breakpoints are known.

Unsupported package replacements are deferred unless they block restore, build, or test execution. API incompatibilities are fixed inline within each task so every phase ends in a buildable, testable state before the next phase begins.

### 01-prerequisites: Verify SDK and update global.json

Validate that the .NET 10 SDK required by the scenario is available, and update `global.json` or related toolchain pins so local and CI builds resolve the intended SDK. Confirm the solution restores cleanly before any project file or package changes begin.

Capture any package feeds, workload prerequisites, or environment assumptions discovered during restore so later tasks are not blocked by tooling drift. This task establishes the baseline gate for the application-first flow and should produce a clean starting point for iterative upgrades.

**Done when**: .NET 10 SDK is installed or pinned appropriately, `global.json` is aligned to the target toolchain, and a solution restore completes without SDK-selection errors.

### 02-web-app: Upgrade src/Web application

Upgrade `src/Web` to `net10.0` and address its package and API surface first because it has the highest concentration of issues in the assessment, including binary incompatible APIs, source incompatibilities, behavioral changes, and a vulnerable `Azure.Identity` dependency. Apply package upgrades needed for ASP.NET Core, EF Core, identity, diagnostics, and web tooling, and fix incompatible configuration and runtime APIs inline.

Upgrade `tests/FunctionalTests` within the same task so the primary web application and its end-to-end coverage move together. Any temporary compatibility adjustments needed to keep shared dependencies stable during this application-first phase should be minimized and clearly isolated for later cleanup.

**Done when**: `src/Web` and `tests/FunctionalTests` target `net10.0`, restore and build successfully, vulnerable/directly blocking dependencies are updated, and the functional test suite for Web passes.

### 03-publicapi: Upgrade src/PublicApi application

Upgrade `src/PublicApi` to `net10.0` after the web application path is proven. This task should focus on the API project's binary incompatible and behavioral changes, package refreshes for ASP.NET Core and EF Core components, and review of incompatible or deprecated tooling packages that can be deferred if they do not block the build.

Upgrade `tests/PublicApiIntegrationTests` as part of the same slice so contract and integration behavior are validated immediately. Fix configuration binder and endpoint-related API changes inline, and document any unsupported package follow-up that is intentionally deferred under the selected strategy.

**Done when**: `src/PublicApi` and `tests/PublicApiIntegrationTests` target `net10.0`, build cleanly, integration tests pass, and any remaining deferred package actions are explicitly documented.

### 04-blazor: Upgrade BlazorAdmin and BlazorShared

Upgrade `src/BlazorAdmin` and its shared UI dependency `src/BlazorShared` together because their package graph and runtime behavior are tightly coupled. Refresh the WebAssembly and authentication packages, resolve behavioral changes surfaced by the assessment, and keep the shared component library aligned with the application so UI contracts do not drift.

Include any directly related UI validation or application build checks needed to prove the Blazor path is stable under .NET 10. This task should leave the Blazor application stack consistent and ready for shared backend library consolidation in the next phase.

**Done when**: `src/BlazorAdmin` and `src/BlazorShared` target `net10.0`, restore and build successfully, and Blazor-specific behavioral regressions identified during validation are resolved.

### 05-library-consolidation: Upgrade ApplicationCore and Infrastructure

Upgrade `src/ApplicationCore` and `src/Infrastructure` after the application layer is stable, then align `tests/IntegrationTests` and `tests/UnitTests` to the same target. This phase resolves remaining source incompatibilities, framework-inbox package cleanup, security-related package upgrades, and any temporary compatibility decisions introduced earlier to support the application-first sequence.

Because unsupported packages are being deferred, use this task to remove avoidable obsolete references, consolidate package versions, and document any residual exceptions that remain non-blocking. Ensure the domain and infrastructure layers are cleanly compiled against .NET 10 with their test coverage updated alongside them.

**Done when**: `src/ApplicationCore`, `src/Infrastructure`, `tests/IntegrationTests`, and `tests/UnitTests` target `net10.0`, build successfully, unit/integration tests pass, and any temporary compatibility shims or multi-targeting introduced earlier are removed.

### 06-final-validation: Full solution build and test validation

Run the complete validation pass across the upgraded solution once all project groups are on `net10.0`. This includes a full restore, full solution build, and execution of the entire automated test suite so package alignment, transitive dependency consistency, and runtime-sensitive API changes are verified together.

Use the final task to confirm there are no leftover application-first temporary changes, no unresolved SDK mismatches, and no hidden regressions from deferred package decisions. Any remaining warnings should be reviewed and either fixed in scope or explicitly tracked for post-upgrade follow-up if they are outside the confirmed strategy.

**Done when**: the full solution restores, builds, and all existing test projects pass on .NET 10, with no temporary upgrade scaffolding left behind.