# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0 across the full solution using the confirmed Automatic, Top-Down strategy.
**Scope**: 10 SDK-style projects (3 shared libraries, 3 application entry points, 4 test projects) with centrally managed TargetFramework and package versions in `Directory.Packages.props`, 24 package updates, 5 incompatible or deprecated packages deferred for later resolution, 9 binary-incompatible APIs, 3 source-incompatible APIs, 49 behavioral changes, and an `Azure.Identity` security update to 1.21.0.

### Selected Strategy
**Top-Down (Application-First)** — Applications upgraded first, libraries multi-targeted temporarily.
**Rationale**: All projects are already modern .NET and SDK-style, the solution dependency graph fans into three application entry points (`PublicApi`, `BlazorAdmin`, `Web`), and temporary library multi-targeting can keep the repo buildable while each app slice absorbs its own package and API changes.

Applications are upgraded in this order: `PublicApi`, `BlazorAdmin`, then `Web`. Shared libraries that may need temporary multi-targeting are `ApplicationCore`, `Infrastructure`, and `BlazorShared`. The application dependency map is `PublicApi -> ApplicationCore, Infrastructure`; `BlazorAdmin -> BlazorShared`; `Web -> ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure`. Phase 2 starts only after all three application entry points and their attached test coverage no longer require net8.0.

## Tasks

### 01-toolchain-and-central-package-baseline: Verify .NET 10 prerequisites and prepare central package management

Confirm that the .NET 10 SDK and any pinned SDK settings are compatible with this repo before any project slice moves. Because every project inherits `TargetFramework` and most package versions from `Directory.Packages.props`, this task establishes the staging approach for the centrally managed `TargetFramework`, `AspNetVersion`, `EntityFrameworkCoreVersion`/current EF Core version property, `SystemExtensionVersion`, and `VSCodeGeneratorVersion` values so later app-first tasks can upgrade safely without accidentally forcing all untouched projects forward at once.

This task also captures the initial restore/build baseline, checks whether `global.json` or related tooling pins need adjustment, and inventories the five incompatible or deprecated packages that will be deferred unless they block progress. The outcome should make the central package file ready for staged net10.0 work while preserving a clean starting point for `PublicApi`, `BlazorAdmin`, and `Web`.

**Done when**: The .NET 10 SDK requirement is verified, any SDK pinning files are compatible, the staged edit plan for `Directory.Packages.props` is clear and applied as needed for incremental work, the central version variables needed for ASP.NET Core, EF Core, system extensions, and code generation are prepared for net10.0-compatible updates, and the current solution still restores before application upgrades begin.

---

### 02-public-api-slice: Upgrade PublicApi with ApplicationCore, Infrastructure, and API-facing tests

Upgrade the `PublicApi` application slice first, including the reusable code it directly depends on in `ApplicationCore` and `Infrastructure`, plus `PublicApiIntegrationTests` and the `PublicApi`-focused portions of `FunctionalTests`. Assessment data shows this slice carries 11 package issues and 5 API issues in `PublicApi`, along with 3 package issues in `ApplicationCore` and 4 package issues in `Infrastructure`, so it is the smallest server entry point that still exercises the shared domain and data layers.

This task should absorb the first coordinated `TargetFramework` and package version changes coming from `Directory.Packages.props`, especially the ASP.NET Core and EF Core version updates. It should also handle the initial binary/source compatibility fixes surfaced around configuration binding and options registration, while explicitly documenting any deferred package replacements such as deprecated DI or token packages if they do not block a green net10.0 build for this slice.

**Done when**: `PublicApi`, `ApplicationCore`, and `Infrastructure` build and run against net10.0 in the staged top-down setup, the API-facing tests that cover this slice are updated and passing on the new target, and any non-blocking unsupported package follow-ups for this slice are recorded for later resolution.

---

### 03-blazor-admin-slice: Upgrade BlazorAdmin and BlazorShared ahead of the Web host

Upgrade the `BlazorAdmin` WebAssembly client together with `BlazorShared`, which is its only direct project dependency and also a dependency of `Web`. This is the simplest application slice by structure, but it still carries 7 package issues and 3 API issues in `BlazorAdmin`, mostly around ASP.NET Core component packages, authentication packages, logging extensions, and `System.Net.Http.Json`.

Because `Web` references `BlazorAdmin`, this task must leave the hosted relationship in a state that the later `Web` task can consume without backtracking. The key planning concern is to update the centrally managed ASP.NET Core and system extension package variables in a way that keeps the hosted admin UI and its shared contracts compatible during the transition, while treating any deferred unsupported package decisions as follow-up work unless they block the staged upgrade.

**Done when**: `BlazorAdmin` and `BlazorShared` are aligned to the net10.0 migration path, the hosted client/server relationship needed by `Web` remains buildable for the next slice, and any Blazor-specific package or API adjustments required by the assessment are completed or explicitly documented as deferred non-blockers.

---

### 04-web-storefront-slice: Upgrade Web and complete the highest-risk application migration

Upgrade the `Web` application last among the entry points, after `ApplicationCore`, `Infrastructure`, `BlazorAdmin`, and `BlazorShared` have been prepared by earlier slices. This is the heaviest application task in the assessment: `Web` alone carries 13 package issues, 15 API issues, and the largest estimated code impact, while `UnitTests` and the remaining `FunctionalTests` also depend on it and must move with the storefront host.

This task is where the most visible behavioral work lands. It includes applying the `Azure.Identity` security update to 1.21.0, reconciling ASP.NET Core/EF Core package updates inherited from `Directory.Packages.props`, and fixing the highest-frequency behavioral and binary issues that are most likely to surface in startup, configuration binding, HTTP content handling, URI behavior, and exception-handling flows. Deprecated packages such as `AutoMapper.Extensions.Microsoft.DependencyInjection` and `System.IdentityModel.Tokens.Jwt` can remain deferred only if the upgraded `Web` slice restores, builds, and tests cleanly without immediate replacement.

**Done when**: `Web` runs on net10.0 with its dependent projects and centrally managed package versions aligned, the `Azure.Identity` vulnerability is removed by upgrading to 1.21.0 or later, `UnitTests` and the Web-facing `FunctionalTests` pass on the new target, and the main application behavioral regressions identified by assessment have been addressed.

---

### 05-library-and-test-consolidation: Remove temporary net8.0 compatibility from shared libraries and remaining tests

After `PublicApi`, `BlazorAdmin`, and `Web` are all moved, consolidate the solution by removing temporary multi-targeting or compatibility scaffolding from `ApplicationCore`, `Infrastructure`, and `BlazorShared`. This is the explicit Phase 2 cleanup required by the Top-Down strategy, and it also closes out the remaining test alignment for `IntegrationTests`, cross-slice `FunctionalTests`, and any shared test infrastructure still carrying old-target assumptions.

This task should leave the repo in a single-target net10.0 state, eliminate conditional compilation or transitional package references that were only needed to bridge application slices, and revisit any deferred unsupported package items that naturally belong with shared libraries or test infrastructure. It is also the point to decide whether incompatible/deprecated packages such as the container tooling or xUnit runner artifacts stay as documented follow-ups or require immediate action for long-term maintainability.

**Done when**: All shared libraries and remaining test projects no longer depend on temporary net8.0 compatibility, transitional conditions added for the top-down migration are removed, `IntegrationTests` are aligned with the upgraded infrastructure stack, and the solution is ready for a single-target net10.0 validation pass.

---

### 06-solution-validation-and-deferred-followups: Validate the full net10.0 solution and capture remaining deferred work

Run the final validation pass across the entire solution after consolidation. This task confirms that all 10 projects restore, build, and test successfully together on net10.0, and that the staged application-first migration did not leave behind hidden compatibility gaps between the three application entry points, the shared libraries, and the four test projects.

The task also closes the loop on the confirmed "Defer Resolution" package strategy by producing an explicit record of any unsupported or deprecated packages that remain intentionally unresolved after the successful upgrade. Security-related work must be complete at this point, especially the `Azure.Identity` update, while non-blocking follow-ups such as package replacements or tooling modernization are documented so execution can end with a clean, reviewable outcome.

**Done when**: The full solution restores, builds, and runs its existing test suite on net10.0, no blocking security vulnerability remains, all 10 projects are confirmed on the target framework, and any intentionally deferred unsupported package follow-ups are documented with their final disposition.
