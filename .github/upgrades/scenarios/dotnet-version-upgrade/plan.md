# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the eShopOnWeb solution from net8.0 to net10.0.
**Scope**: 10 SDK-style projects across 3 ASP.NET Core applications and 7 supporting library/test projects, with a 5-level dependency chain, 19 recommended package upgrades, 7 incompatible or deprecated package findings, and 61 API compatibility findings.

### Selected Strategy
**Top-Down (Application-First)** — Upgrade application entry points first, then finish shared cleanup once all application surfaces are stable on net10.0.
**Rationale**: The solution is already on modern .NET and does not need SDK-style conversion, but it is still a complex upgrade because risk is concentrated in the application hosts: Web carries the Azure.Identity vulnerability and the largest API-change surface, PublicApi has binary-incompatible configuration calls plus an incompatible package, and BlazorAdmin brings a separate WebAssembly runtime path. Sequencing by application keeps the most business-visible entry points moving first while deferring deprecated-package replacement until the net10.0 baseline is stable.

**Application priority**: BlazorAdmin, Web, PublicApi.
**Supporting libraries**: BlazorShared, ApplicationCore, Infrastructure.
**Application dependency map**:
- BlazorAdmin → BlazorShared
- Web → ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure
- PublicApi → ApplicationCore, Infrastructure

**Phase 2 trigger**: Start deferred package cleanup only after BlazorAdmin, Web, PublicApi, and their attached test suites are all running on net10.0.

## Tasks

### 01-toolchain-baseline: Align SDK baseline and upgrade backlog

Confirm the repository can build against .NET 10 before project files start moving. This task covers the global.json update path, SDK availability, and a quick review of the deferred-package backlog so later tasks can focus on mandatory target-framework and API work instead of rediscovering package constraints.

It should also capture the known exceptions that will intentionally be postponed until the dedicated cleanup task: deprecated or incompatible packages in Web, PublicApi, Infrastructure, and the test projects, plus framework-included packages that should be removed instead of upgraded.

**Done when**: global.json and toolchain expectations are aligned for net10.0, the solution has a clear deferred-package list, and the repository is ready for application-by-application execution.

---

### 02-blazor-admin-client: Upgrade the Blazor admin client path

Upgrade BlazorAdmin first because it is the smallest application surface and depends only on BlazorShared. This isolates the WebAssembly package wave early, resolves the HttpContent and Uri behavioral checks in BlazorAdmin/Services/HttpService.cs and BlazorAdmin/Program.cs, and establishes a low-risk net10.0 application baseline before the larger web hosts move.

Any BlazorShared changes needed to keep the client compiling and running should be handled inside this task. Test work stays attached to the consuming application, so only validation needed for the Blazor admin path belongs here.

**Done when**: BlazorAdmin and BlazorShared target net10.0 where required for this path, the identified behavioral API issues are addressed or validated, and the admin client builds cleanly on the new framework.

---

### 03-web-storefront: Upgrade the customer-facing web application

Upgrade Web next, including the shared project changes it consumes from ApplicationCore, Infrastructure, BlazorAdmin, and BlazorShared. This is the highest-risk task because Web has the largest issue count, the Azure.Identity security finding, multiple deprecated packages, binary-incompatible configuration binding calls, a source-incompatible TimeSpan usage, and several runtime-behavior checks concentrated in Program.cs and the Configure* classes.

This task also absorbs the Web-focused test fallout. FunctionalTests and UnitTests should be updated here wherever they validate the storefront, authentication, basket, checkout, or page/controller behavior affected by the Web upgrade.

**Done when**: Web runs on net10.0, the Azure.Identity vulnerability is remediated, mandatory Web and shared-library API issues needed by the storefront are fixed inline, and the Web-aligned FunctionalTests and UnitTests pass against the upgraded host.

---

### 04-public-api-surface: Upgrade the public API host and API test path

Upgrade PublicApi after the storefront work has stabilized the shared core and infrastructure dependencies. The main focus is the binary-incompatible configuration and options binding calls in PublicApi/Program.cs, plus the package set shared with Web for ASP.NET Core, Identity, Entity Framework, and tooling.

Because Unsupported Packages is set to Defer Resolution, this task should get PublicApi working on net10.0 first and carry only the package changes required for that outcome. PublicApiIntegrationTests, along with any FunctionalTests that exercise API behavior, should be updated in the same task so the host and its tests stay aligned.

**Done when**: PublicApi targets net10.0, its mandatory binary/API issues are resolved, required package upgrades for the host are applied, and API-focused integration coverage passes on the upgraded runtime.

---

### 05-deferred-package-cleanup: Resolve deferred package and shared cleanup work

Once all three application surfaces are on net10.0, finish the package work that was intentionally deferred to keep earlier tasks focused. This includes the incompatible Microsoft.VisualStudio.Azure.Containers.Tools.Targets reference in PublicApi, deprecated AutoMapper.Extensions.Microsoft.DependencyInjection and System.IdentityModel.Tokens.Jwt usage, framework-included System.Security.Claims in ApplicationCore, and deprecated test dependencies across xUnit and MSTest projects.

This is also the right place to normalize any remaining shared Infrastructure, ApplicationCore, BlazorShared, and test-project package decisions that were not strictly required to get the application hosts running. If package replacement requires code changes, they should be limited to the affected shared/test areas and kept separate from the earlier application-upgrade objectives.

**Done when**: all deferred incompatible, deprecated, or removable package findings have been resolved or intentionally documented, shared projects no longer carry obsolete package references needed only for the pre-upgrade state, and the solution package graph is consistent on net10.0.

---

### 06-solution-validation: Validate the full net10.0 solution

Run the full solution validation pass after application upgrades and deferred cleanup are complete. This task is where the team proves that the upgrade is coherent end to end: build output, test coverage, and the runtime-sensitive areas flagged by the assessment such as HttpContent handling, Uri behavior, exception serialization, and logging/exception-handler behavior.

Any remaining issues discovered here should be limited to stabilization, not broad rescoping of the plan. The goal is to leave the scenario with a clean execution finish line and an explicit record of any non-blocking follow-up items.

**Done when**: the full solution builds on net10.0, all existing test projects pass, high-risk behavioral-change areas have been revalidated, and any leftover non-blocking recommendations are documented for follow-up outside the upgrade path.
