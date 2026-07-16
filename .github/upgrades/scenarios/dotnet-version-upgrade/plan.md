# .NET Version Upgrade Plan

> **Strategy**: Top-Down
> **Target Framework**: net10.0
> **Flow Mode**: Automatic
> **Package Handling**: Resolve Inline
> **API Handling**: Fix Inline

## Overview

**Target**: Upgrade the eShopOnWeb solution from net8.0 to net10.0.
**Scope**: 10 SDK-style modern .NET projects with 119 assessment findings, including 21 mandatory issues, 19 package updates, 2 security vulnerabilities, and application-focused API breaking changes in PublicApi and Web.

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Confirm the development environment is ready for a net10.0 upgrade before project changes begin. This includes validating that the required .NET SDK is available, aligning any pinned SDK version in global.json, and confirming the repository is still on a clean modern .NET baseline with no SDK-style or multi-targeting remediation required.

This task also establishes the guardrails for the rest of the execution flow: all projects remain single-targeted, package and API issues are handled inline, and each later task can assume the toolchain is consistent across the solution.

**Done when**: the net10.0 SDK requirement is verified, global.json is updated or confirmed compatible, and the solution is confirmed ready for a straight net8.0 to net10.0 upgrade without SDK-style conversion or temporary multi-targeting.

---

### 02-foundation-libs: Upgrade BlazorShared, ApplicationCore, BlazorAdmin, and Infrastructure

Upgrade the shared and foundational projects that sit underneath the applications: BlazorShared, ApplicationCore, BlazorAdmin, and Infrastructure. This task covers target framework changes, package version updates in these projects, removal of packages that are now unnecessary on modern .NET, and any source-level fixes required by the assessment before downstream applications are rebuilt.

Special attention should go to ApplicationCore source-incompatible APIs, the removal of System.Security.Claims from framework-covered references, Infrastructure package updates, and the BlazorAdmin/BlazorShared dependencies that support the web-facing projects. Completing these projects first keeps the application task focused on the higher-risk runtime and hosting changes.

**Done when**: all four foundation projects target net10.0, their project files no longer carry obsolete or incompatible package references, mandatory issues in these projects are fixed inline, and the foundation set builds successfully together.

---

### 03-applications: Upgrade PublicApi and Web applications

Upgrade the two primary application entry points, PublicApi and Web, after the shared libraries are stable on net10.0. This task carries the heaviest compatibility work in the plan because these projects contain most of the binary-incompatible API findings, a share of the source-incompatible changes, and the highest volume of package updates.

The task should include framework and package updates, removal of Microsoft.VisualStudio.Azure.Containers.Tools.Targets from PublicApi, remediation of the Azure.Identity and System.Text.Json security findings where applicable, and inline fixes for configuration binding, options configuration, request/response handling, logging, exception handling, and other runtime-sensitive API changes called out by the assessment. Behavioral changes must be validated as part of the application upgrade rather than deferred.

**Done when**: PublicApi and Web both target net10.0, incompatible or redundant packages are removed, recommended package updates are applied, mandatory API fixes are completed inline, and both applications build successfully against the upgraded foundation projects.

---

### 04-tests: Upgrade all test projects

Upgrade the test estate after the production projects have been moved to net10.0. This includes FunctionalTests, PublicApiIntegrationTests, UnitTests, and IntegrationTests, with package alignment to the upgraded application stack and code fixes for any test-only compatibility changes.

Because the assessment shows API and package issues concentrated in FunctionalTests and PublicApiIntegrationTests, this task should absorb the remaining test-hosting, HTTP, serialization, and framework-package adjustments needed to run the full validation suite reliably on net10.0.

**Done when**: all four test projects target net10.0, test package references are aligned with the upgraded solution, mandatory issues in the test projects are fixed inline, and the complete test set builds successfully.

---

### 05-final-validation: Run full solution build and test validation

Perform the final end-to-end validation pass once every project has been upgraded. This task verifies the solution as an integrated whole, confirms that cross-project references are healthy, and checks that behavioral changes introduced by the new runtime have not left hidden regressions in application startup, API behavior, or test infrastructure.

This validation step is where the upgrade is closed out: the full solution build, the relevant automated test suites, and any final cleanup needed to leave the branch in an execution-ready state should all be completed here.

**Done when**: the full solution restores, builds, and runs its existing automated tests successfully on net10.0, no upgrade-specific obsolete package references remain, and the repository is ready for final review or merge.
