# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the eShopOnWeb solution from net8.0 to net10.0.
**Scope**: 10 SDK-style projects using Central Package Management, with 3 ASP.NET Core applications and 7 shared or test libraries.

### Selected Strategy
**Top-Down (Application-First)** — Applications upgrade first, while shared-library changes stay incremental and are only expanded when the consuming application needs them.
**Rationale**: This is already a modern .NET solution, but the dependency graph is five levels deep and the highest-risk API and package work is concentrated in Web and PublicApi. An application-first plan keeps the most important entry points moving first, contains API fixes where they are exercised, and leaves shared-library cleanup until all applications are stable on net10.0.

Applications (priority order): Web, PublicApi, BlazorAdmin.

Shared libraries and dependent projects: BlazorShared, ApplicationCore, Infrastructure, FunctionalTests, PublicApiIntegrationTests, UnitTests, IntegrationTests.

Application dependency map: Web depends on ApplicationCore, BlazorAdmin, BlazorShared, and Infrastructure; PublicApi depends on ApplicationCore and Infrastructure; BlazorAdmin depends on BlazorShared.

Phase 2 trigger: start remaining library and dependent-test consolidation only after Web, PublicApi, and BlazorAdmin all build and run their impacted tests on net10.0.

## Tasks

### 01-prerequisites-and-central-package-baseline: Prepare SDK and central package versions

Update the shared upgrade baseline before touching individual projects. This task covers moving `global.json` to a .NET 10 SDK band and updating `Directory.Packages.props` so Central Package Management becomes the single source of truth for the net10.0 target framework and shared package versions.

Use this task to align the common ASP.NET Core, EF Core, and system package version properties with the target framework, apply the Azure.Identity security update, and remove the `System.Security.Claims` package because that functionality is now provided by the framework. Keep project-specific package removals or deferrals out of individual version edits unless a project reference must be removed or conditioned to keep later tasks buildable.

**Done when**: `global.json` points to a .NET 10 SDK band; `Directory.Packages.props` is updated for net10.0 and the shared package version properties required by the solution; `System.Security.Claims` is removed from central package management; the package upgrade and deferral baseline is ready for project tasks to consume.

---

### 02-upgrade-web-stack: Upgrade Web and its shared backend path

Upgrade Web first because it combines the broadest dependency chain with the highest-risk application findings, including the Azure.Identity security update, five binary-incompatible API issues, one source-incompatible API issue, and nine behavioral API changes. Resolve those API changes inline in the Web code path while aligning the shared projects that Web directly exercises: ApplicationCore, Infrastructure, BlazorShared, and any Web-facing portions of the BlazorAdmin integration surface.

Any test fallout caused by this work stays in the same task. Update UnitTests and FunctionalTests only where Web-driven code or behavior changes require them, so the first application upgrade proves the central package baseline and the main user-facing path together.

**Done when**: Web builds and runs on net10.0; the shared project changes needed by Web are in place; Web-specific API and package issues are resolved inline; the UnitTests and FunctionalTests affected by Web changes pass.

---

### 03-upgrade-publicapi-stack: Upgrade PublicApi and its integration coverage

Upgrade PublicApi after the shared backend path has already been exercised by the Web task. This task should remove or condition the incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` reference so it no longer blocks restore or build, then finish PublicApi’s API and package work, including its binary-incompatible changes, behavioral changes, and deprecated package cleanup.

Keep dependent test work attached to this application task. Any changes needed in PublicApiIntegrationTests or shared FunctionalTests because of PublicApi behavior changes should be completed here rather than split into separate testing tasks.

**Done when**: PublicApi builds and runs on net10.0; the incompatible container-tools package no longer blocks the project; PublicApi-specific API and package issues are fixed inline; the affected integration coverage passes.

---

### 04-upgrade-blazoradmin-stack: Upgrade BlazorAdmin and finalize client package alignment

Upgrade BlazorAdmin after the server applications because it has the simplest dependency chain and can consume the stabilized shared changes from earlier tasks. Align its WebAssembly, authentication, and logging-related package set to the central net10.0 baseline and fix its remaining project-level compatibility findings inside the same task.

This task is also the point where any BlazorShared adjustments that are only required by the admin client should be completed. Keep the work scoped to the client application and the shared UI contracts it actually consumes.

**Done when**: BlazorAdmin and the BlazorShared pieces it depends on build on net10.0; BlazorAdmin package alignment is complete under Central Package Management; any admin-specific API or package issues are fixed inline.

---

### 05-consolidate-remaining-libraries-and-dependent-tests: Finish shared-library cleanup after all apps move

Once all three applications are on net10.0, complete the remaining shared-library and dependent-test work that was intentionally deferred to preserve the application-first flow. This is the cleanup point for any library target framework updates that were postponed, any temporary compatibility conditions introduced during the application tasks, and any residual package removals or deprecation responses that still affect ApplicationCore, Infrastructure, or BlazorShared.

Handle late-arriving test work here only if it exists because of this consolidation step. IntegrationTests and any remaining dependent test adjustments belong in this task when they are driven by the shared-library cleanup rather than by a specific application upgrade.

**Done when**: No remaining shared library or dependent test project is left behind on net8.0; temporary compatibility conditions from earlier tasks are removed; IntegrationTests and any residual dependent test changes build and pass on net10.0; shared project references are clean for final validation.

---

### 06-final-validation: Validate the full solution and document deferred follow-up

Run the full repository validation pass after every project task is complete. This task verifies restore, build, and test coverage for the whole solution on .NET 10 and confirms that the application-level behavioral changes identified in the assessment were exercised through the existing test suites.

Use the validation pass to capture any intentionally deferred incompatible-package replacement work that remains after the upgrade, especially where a package was removed or conditioned only to keep the solution moving. The plan should end with a clean, buildable net10.0 solution and an explicit record of any remaining follow-up outside this execution.

**Done when**: Full solution restore, build, and tests succeed on .NET 10; no project or shared package property still points at the old net8.0 baseline; any remaining deferred package follow-up is documented.
