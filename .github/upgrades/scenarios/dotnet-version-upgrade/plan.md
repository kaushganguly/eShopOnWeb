# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the eShopOnWeb solution from net8.0 to net10.0.
**Scope**: 10 SDK-style projects across a 5-level dependency graph, with 3 ASP.NET Core applications, 7 supporting libraries/tests, 32 package upgrades, 12 deprecated packages, 2 security vulnerabilities, 9 binary incompatible APIs, and 3 source incompatible APIs.

### Selected Strategy
**Top-Down (Application-First)** — Upgrade entry-point applications first, carrying the libraries they immediately need.
**Rationale**: BlazorAdmin, PublicApi, and Web can be upgraded as distinct application slices, which keeps each checkpoint small while isolating the highest-risk web surface until its supporting libraries and lower-risk applications are already on net10.0.

**Applications (priority order)**: BlazorAdmin, PublicApi, Web.
**Supporting libraries and test projects in scope**: BlazorShared, ApplicationCore, Infrastructure, FunctionalTests, PublicApiIntegrationTests, UnitTests, IntegrationTests.
**Application dependency map**:
- BlazorAdmin → BlazorShared
- PublicApi → ApplicationCore, Infrastructure
- Web → ApplicationCore, BlazorAdmin, BlazorShared, Infrastructure
**Phase 2 trigger**: Not applicable as a separate consolidation phase for this modern-to-modern upgrade; once all three applications and their tests are on net10.0, move directly to full solution validation.

## Tasks

### 01-upgrade-prerequisites: Verify the net10 baseline and shared upgrade assumptions

This task is solution-wide. Confirm that the .NET 10 SDK and any `global.json` constraints support the target framework, then capture the baseline restore/build/test commands that already exist for the repo. The assessment shows 10 SDK-style projects already on net8.0, so no SDK-style conversion or framework-to-framework migration scaffolding is needed, but the upgrade still starts with a shared toolchain checkpoint.

Use this task to inventory the solution-level risks that cut across multiple slices: 32 package upgrades, 12 deprecated packages, 2 security vulnerabilities, one incompatible package in PublicApi, and the repeated configuration-binding API breaks that appear in both PublicApi and Web. Research should start with `global.json`, solution/package version conventions, and the current CI-equivalent build and test entry points so later application tasks can validate against the same baseline.

**Done when**: The repo can resolve a .NET 10 SDK, any `global.json` pin is confirmed compatible or updated, baseline restore/build/test commands are identified, and the shared package/API hotspots are mapped to the owning upgrade tasks.

---

### 02-blazor-admin-stack: Upgrade the Blazor admin application and shared UI contracts

This slice upgrades `src/BlazorAdmin/BlazorAdmin.csproj` together with `src/BlazorShared/BlazorShared.csproj`, which is its only internal dependency and the foundation library for the UI-facing path. BlazorAdmin is the lightest application entry point in the assessment: 7 recommended package upgrades, 3 behavioral API changes, and an estimated 3+ lines of code impact across 20 code files. Starting here gives the first net10.0 application checkpoint with the smallest dependency surface.

Known work in this slice includes aligning the Blazor WebAssembly package set to net10.0, checking `System.Net.Http.Json` usage, and reviewing behavioral changes around `JsonSerializer.Deserialize(...)` over HTTP content and `Uri`/`HttpClient` base-address construction. Research should begin in the admin client startup path and response-deserialization helpers, with attention to any behavior shared through BlazorShared components and validation models.

**Done when**: BlazorAdmin and BlazorShared target net10.0, package references for the Blazor/WebAssembly stack are aligned, flagged behavioral API call sites are reviewed and adjusted as needed, and the application-specific build/test coverage in the repo passes for this slice.

---

### 03-public-api-stack: Upgrade the API service with its domain and infrastructure dependencies

This task covers `src/ApplicationCore/ApplicationCore.csproj`, `src/Infrastructure/Infrastructure.csproj`, `src/PublicApi/PublicApi.csproj`, and `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`. PublicApi is the first service slice with deeper dependencies and the only incompatible package finding in the solution, while ApplicationCore and Infrastructure carry shared domain, EF Core, identity, and serialization concerns that must move with it.

Assessment context for this slice is rich: PublicApi has 11 package issues, including the incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` reference, deprecated AutoMapper/JWT dependencies, and 4 binary incompatible configuration-binding APIs plus a logging behavior change. ApplicationCore adds removal of `System.Security.Claims`, a vulnerable `System.Text.Json` version, and 2 source-incompatible exception-serialization constructor usages. Infrastructure contributes EF Core/Identity package upgrades and deprecated JWT usage, and PublicApiIntegrationTests adds MVC testing package updates plus 7 behavioral API checks around HTTP response handling. Research should begin with configuration binding (`Get<T>`, `Configure<T>(IConfiguration)`), package removal/replacement strategy for the incompatible tooling package, exception serialization patterns, and EF/Identity package alignment across the slice.

**Done when**: ApplicationCore, Infrastructure, PublicApi, and PublicApiIntegrationTests target net10.0, the incompatible PublicApi package is removed or replaced, package/security/deprecation findings in this slice are resolved inline, the flagged configuration and serialization API changes are fixed, and the API-specific integration tests/builds pass.

---

### 04-web-storefront-stack: Upgrade the storefront application and its downstream test suites

This task upgrades `src/Web/Web.csproj` together with `tests/FunctionalTests/FunctionalTests.csproj`, `tests/UnitTests/UnitTests.csproj`, and `tests/IntegrationTests/IntegrationTests.csproj`. Web is the highest-risk entry point in the assessment: 13 package findings, 15 API findings, 112 code files, and the remaining security vulnerability (`Azure.Identity`) along with several deprecated packages. Its downstream tests account for the largest behavioral-change surface, especially the 29 flagged API behavior changes in FunctionalTests.

Known risk areas include repeated configuration-binding breaks shared with PublicApi, `TimeSpan.FromMinutes(...)` source-compatibility work in auth/session settings, behavioral changes around `HttpContent.ReadAsStringAsync`, `Uri` construction, `UseExceptionHandler`, `AddAzureKeyVault`, `AddConsole`, and typed `HttpClient` setup, plus xUnit/MVC testing package churn in the dependent test projects. Research should start in Web startup/configuration code, auth configuration, Azure Key Vault integration, and the test helpers/assertions that currently depend on request URI and response-content behavior.

**Done when**: Web and its remaining test projects target net10.0, security and deprecated-package findings in this slice are resolved inline, the flagged configuration/auth/runtime behavioral changes are addressed, and the impacted functional, unit, and integration test suites pass on the upgraded framework.

---

### 05-solution-validation: Validate the full net10 solution and close remaining upgrade gaps

This final task is the completion gate for the whole repository. By the time it starts, all three applications and every supporting project should already be on net10.0, so the focus shifts to whole-solution validation, catching package/version drift, and confirming there are no stragglers left from the earlier slices.

Use this task to run the full solution restore/build/test workflow, verify that no project still targets net8.0, confirm the incompatible package in PublicApi is gone, and recheck the two security-vulnerability upgrades and deprecated-package replacements in their final assembled state. Research should focus on solution-level restore/build warnings, cross-project package alignment, and any remaining runtime or tooling concerns surfaced only after the full graph is upgraded together.

**Done when**: Every project in the solution targets net10.0, the full restore/build/test workflow succeeds, no incompatible package findings remain, security-sensitive package updates are verified in the final solution state, and any residual follow-up recommendations are documented for post-upgrade work rather than left implicit.
