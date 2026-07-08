# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0.
**Scope**: 10 SDK-style projects with centralized package management in Directory.Packages.props, 3 ASP.NET Core entry points, and a 0→5 dependency chain spanning shared libraries and tests.

### Selected Strategy
**Top-Down (Application-First)** — Applications upgraded first, with temporary dual-targeting introduced only where a net10.0 application must coexist with a remaining net8.0 consumer.
**Rationale**: The solution is already on modern .NET, so the primary risk is not project conversion but controlled movement through 21 mandatory incidents, binary incompatibilities in PublicApi and Web, source incompatibilities in ApplicationCore and Web, and package churn concentrated in the web-facing projects. Upgrading the applications incrementally keeps the repository buildable while shared projects absorb only the minimum compatibility work needed for each step.

**Applications (priority order)**: BlazorAdmin → PublicApi → Web.
**Libraries needing temporary Phase 1 coexistence support**: BlazorShared, ApplicationCore, Infrastructure, and BlazorAdmin while Web still references it from net8.0.
**Application dependency map**: BlazorAdmin depends on BlazorShared; PublicApi depends on ApplicationCore and Infrastructure; Web depends on ApplicationCore, BlazorAdmin, BlazorShared, and Infrastructure.
**Phase 2 trigger**: Start consolidation only after BlazorAdmin, PublicApi, and Web all build on net10.0 and their dependent tests are green.

## Tasks

### 01-toolchain-baseline: Align SDK and centralized package management for net10.0

This task establishes the shared baseline before any application is moved. Scope includes global.json and Directory.Packages.props, which currently control the SDK expectation, the repo-wide target framework value, and most package versions for all 10 projects. The assessment shows 19 upgrade-recommended packages, 5 incompatible packages, and 21 mandatory incidents; the current baseline test run also surfaces Azure.Identity and System.Text.Json advisories, so this task needs to separate what can move centrally now from what will remain temporarily pinned under the confirmed defer-resolution option.

Research should start with the version properties that fan out across the entire solution—AspNetVersion, EntityFramworkCoreVersion, SystemExtensionVersion, VSCodeGeneratorVersion, and the packages flagged as deprecated or incompatible. Because unsupported packages are being deferred rather than resolved up front, this task should also identify the exact packages that may stay in place during Phase 1 and the validation threshold that would force them back into scope.

**Done when**: net10.0 SDK availability and global.json compatibility are verified, the centralized framework/package version files are prepared for incremental net10.0 adoption, and a concrete deferred-package watchlist exists for incompatible packages that are not being replaced immediately.

---

### 02-blazor-admin-surface: Upgrade BlazorAdmin and BlazorShared for the first net10.0 entry point

This task upgrades the simplest application path first: BlazorAdmin with its BlazorShared dependency. BlazorShared has only the framework-target change, while BlazorAdmin carries 7 package issues and 3 API incidents, all in a relatively shallow dependency slice. Because Web still references both projects from net8.0, this task must preserve cross-project compatibility while BlazorAdmin moves ahead as the pilot application for the top-down rollout.

Research should focus on the WebAssembly package set, System.Net.Http.Json, logging/configuration version alignment, and whether BlazorAdmin itself needs temporary dual-targeting while Web remains on net8.0. Validation for this task is not limited to the upgraded app; it also needs a mixed-target check proving that the unchanged Web project can still consume the shared Blazor components until its own task begins.

**Done when**: BlazorAdmin builds and runs on net10.0, BlazorShared provides the compatibility required for the mixed-target phase, and the remaining net8.0 Web path still compiles against the upgraded Blazor projects.

---

### 03-public-api-surface: Upgrade PublicApi with ApplicationCore and Infrastructure compatibility work

This task upgrades PublicApi and prepares the core libraries it consumes. PublicApi has 11 package issues, 4 binary-incompatible API findings, and one additional behavioral change; ApplicationCore adds source incompatibility around obsolete serialization constructors, and Infrastructure adds 4 package issues around EF Core and Identity dependencies. Together they form the second application slice and introduce the first meaningful code-fix work required by the inline API-handling option.

Research should start with configuration binder and options registration changes, EF Core and Identity package alignment, the incompatible Microsoft.VisualStudio.Azure.Containers.Tools.Targets entry, and the serialization constructor cleanup in ApplicationCore. Any FunctionalTests or PublicApiIntegrationTests changes caused by this application move belong here, but the executor should preserve mixed-target compatibility for the still-pending Web upgrade rather than forcing final cleanup early.

**Done when**: PublicApi, ApplicationCore, and Infrastructure build on the target framework with required compatibility adjustments in place, PublicApi-specific test coverage is updated for the mixed-target phase, and no blocking binary/source incompatibility remains in this slice.

---

### 04-web-storefront: Upgrade Web and its dependent test surface to complete the application cutover

This task upgrades the highest-risk application last, after the shared slices beneath it are already understood. Web has the heaviest assessment load in the solution: 13 package issues, 15 API issues, binary and source incompatibilities, and the Azure.Identity security finding. It also depends on every shared project already touched in earlier tasks, so this is the point where the temporary coexistence model is retired in practice and the remaining application/test surface converges on net10.0.

Research should focus on configuration binding changes, exception-handler and URI/HttpContent behavioral shifts, Azure.Identity remediation, JWT/Identity alignment, and the packages marked deprecated or incompatible in the web stack. UnitTests, FunctionalTests, and PublicApiIntegrationTests all touch Web directly and should be updated as part of this task, with special attention to behavioral regressions that may only appear once the full application path is on the new runtime.

**Done when**: Web builds and runs on net10.0, Azure.Identity is no longer left on the vulnerable version, and the Web-dependent test projects are updated and passing on the new framework.

---

### 05-shared-target-consolidation: Remove temporary coexistence targets and clean central version drift

This task converts the temporary compatibility work from Phase 1 into the final steady state. Scope includes every project that carried dual-targeting, conditional package references, or temporary compatibility code so that BlazorShared, ApplicationCore, Infrastructure, and any still-multi-targeted BlazorAdmin artifacts return to a single net10.0 posture. This is also the point to normalize any central version overrides that were held back only to support mixed net8.0/net10.0 operation.

Research should start with TargetFrameworks-to-TargetFramework cleanup, conditional ItemGroup or PropertyGroup entries added during the application tasks, and remaining test-only projects such as IntegrationTests that may still need final alignment once the old target is removed. The defer-resolution option still applies here, so unresolved package replacements should be reviewed, kept only if harmless, and clearly separated from migration leftovers that no longer serve a purpose.

**Done when**: all temporary net8.0 coexistence support is removed, every project in the solution targets its intended final framework/package set, and no conditional compatibility code remains solely to support the phased upgrade.

---

### 06-solution-validation: Validate the complete net10.0 solution and record deferred follow-ups

This closing task verifies the end state of the migration rather than a single project slice. It should cover the full solution build and test matrix, including the main eShopOnWeb.sln path and any secondary solution entry points that participate in normal development, with attention to the behavioral-change-heavy areas identified by assessment. The goal is to prove that the phased plan did not leave hidden regressions after the final consolidation.

Research should start with the 49 behavioral changes called out by assessment, the five incompatible packages that were allowed to stay deferred, and any warnings that persist after the target framework and package updates are complete. This task should also capture the small set of non-blocking post-upgrade recommendations that remain so execution can finish cleanly without losing deferred context.

**Done when**: the full solution builds on net10.0, the existing automated test suites pass, and any intentionally deferred incompatible package or follow-up item is documented with enough detail to track separately from the completed framework upgrade.
