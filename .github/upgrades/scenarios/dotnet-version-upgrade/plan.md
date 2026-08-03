# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from .NET 8 to .NET 10
**Scope**: 10 SDK-style projects across 3 AspNetCore applications, 3 shared libraries, and 4 test projects

### Selected Strategy
**Top-Down (Application-First)** — Applications are upgraded first, and shared libraries are only temporarily multi-targeted when the next application needs them.
**Rationale**: eShopOnWeb is already a modern, SDK-style solution, but it has three application entry points, a five-level dependency graph, 19 recommended package upgrades, one incompatible package, and concentrated API risk in PublicApi and Web. Using an application-first sequence keeps the solution mergeable and releasable while compatibility changes are introduced incrementally instead of forcing a single high-blast-radius cutover.

### Application and library classification
- **Applications (upgrade order)**: BlazorAdmin, PublicApi, Web
- **Shared libraries**: BlazorShared, ApplicationCore, Infrastructure
- **Tests aligned to application work**: PublicApiIntegrationTests with PublicApi; FunctionalTests, IntegrationTests, and UnitTests with Web

### Application dependency map
- **BlazorAdmin** depends on **BlazorShared**.
- **PublicApi** depends on **ApplicationCore** and **Infrastructure**; **Infrastructure** depends on **ApplicationCore**; **ApplicationCore** depends on **BlazorShared**.
- **Web** depends on **ApplicationCore**, **BlazorAdmin**, **BlazorShared**, and **Infrastructure**.

### Phase 2 trigger
Begin cleanup only after BlazorAdmin, PublicApi, Web, and their dependent test projects all build and run on net10.0. At that point, remove any temporary net8.0 targets, conditional package references, and compatibility branches added to keep earlier applications building during the transition.

## Tasks

### 01-toolchain-baseline: Verify the .NET 10 toolchain and solution-wide prerequisites

Scope this task to solution-level prerequisites before any project TFM changes land: confirm the .NET 10 SDK is available, check global.json compatibility, and inventory any build or CI assumptions that still pin the repo to .NET 8 behavior. The assessment shows all 10 projects are already SDK-style and on net8.0, so this task is about establishing a clean starting point rather than structural project conversion.

Research should start with the solution SDK selection rules, local and CI restore/build commands, and any package-management patterns that could amplify later version bumps. Pay particular attention to shared package families that recur across the solution (Microsoft.AspNetCore.*, Microsoft.EntityFrameworkCore.*, Microsoft.Extensions.*, and System.*) so later application tasks can upgrade them consistently.

**Done when**: The repo can select a .NET 10 SDK, any global.json or pipeline pinning issues are resolved or recorded, and the executor has a verified prerequisite baseline for upgrading solution projects from net8.0 to net10.0.

---

### 02-blazor-admin-client: Upgrade BlazorAdmin with the shared Blazor client library

This task covers src/BlazorAdmin/BlazorAdmin.csproj and the leaf dependency src/BlazorShared/BlazorShared.csproj. BlazorAdmin is the shallowest entry point in the graph, so it is the best first application for the top-down sequence. The assessment flags 7 package upgrades and 3 API incidents in BlazorAdmin, concentrated in Program.cs and Services/HttpService.cs, while BlazorShared is otherwise clean and only needs whatever temporary targeting or package alignment is required to let the first application move safely.

Known risks are concentrated in the Blazor WebAssembly package family, logging/identity package alignment, and behavioral differences around HttpContent and Uri handling in the client service layer. Research should start with the WebAssembly hosting/authentication package set, any temporary multi-targeting needed in BlazorShared so downstream net8 consumers continue to build, and whether Program.cs service registration changes are required when the project moves to net10.0.

**Done when**: BlazorShared supports the first application upgrade path, BlazorAdmin targets net10.0 with its required package updates and inline API fixes, and the client application builds cleanly without regressing the remaining net8.0 applications.

---

### 03-public-api-backend: Upgrade PublicApi with its backend libraries and API integration coverage

This task covers src/ApplicationCore/ApplicationCore.csproj, src/Infrastructure/Infrastructure.csproj, src/PublicApi/PublicApi.csproj, and tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj. The assessment concentrates backend risk here: 21 package findings across the four projects, 14 API incidents, a source-incompatible serialization constructor in ApplicationCore, and binary-incompatible configuration-binding issues in PublicApi/Program.cs. This is also where the one incompatible package appears: Microsoft.VisualStudio.Azure.Containers.Tools.Targets must be removed or otherwise neutralized so the application can advance.

Research should start with temporary multi-targeting in BlazorShared, ApplicationCore, and Infrastructure; EF Core 10 alignment across runtime and tooling packages; JWT/authentication package guidance; and the replacement or documented deferral path for deprecated packages such as AutoMapper.Extensions.Microsoft.DependencyInjection, System.IdentityModel.Tokens.Jwt, and MSTest components that are not required to unblock the net10 build. Fix API issues inline during this task, especially the exception serialization constructor and any configuration binder call sites surfaced by the assessment.

**Done when**: ApplicationCore, Infrastructure, PublicApi, and PublicApiIntegrationTests can build against the net10 upgrade path; blocking incompatible references are removed or conditioned; required package upgrades land; and the API-focused automated coverage runs successfully on the new target.

---

### 04-web-storefront-tests: Upgrade Web and the remaining test projects on the full dependency graph

This task covers src/Web/Web.csproj plus tests/FunctionalTests/FunctionalTests.csproj, tests/IntegrationTests/IntegrationTests.csproj, and tests/UnitTests/UnitTests.csproj. It is intentionally last in Phase 1 because Web has the widest dependency surface and the highest concentrated upgrade risk: 13 package findings and 15 API incidents in Web itself, 29 additional API incidents in FunctionalTests, and the solution's Azure.Identity security vulnerability lives here. By this point, the shared libraries and other entry points should already carry whatever temporary compatibility is needed for Web to become the final net10 application.

Research should start with Web's configuration/bootstrap files, health check HTTP semantics, identity and EF package alignment, and the behavior changes around HttpContent and Uri that dominate the functional test incidents. Also review the deprecated xUnit and JWT-related packages in the remaining test and web projects: if they do not block the TFM move, keep the upgrade scoped by documenting the deferred replacement path rather than turning this task into a framework-agnostic testing modernization effort.

**Done when**: Web targets net10.0, the Azure.Identity vulnerability is remediated through the required package update, inline API fixes are applied in Web and the affected tests, and the remaining test projects execute successfully against the upgraded application graph.

---

### 05-shared-library-cleanup: Remove temporary compatibility scaffolding after all applications are on .NET 10

This task is the Phase 2 consolidation point for any projects that were temporarily multi-targeted or conditioned during the application-first rollout, primarily BlazorShared, ApplicationCore, Infrastructure, and any test helpers or application projects that briefly carried dual-targeting. The top-down strategy only works if temporary compatibility does not become permanent technical debt, so this task intentionally happens after every application and dependent test project has crossed to net10.0.

Research should start with TargetFrameworks entries, conditional ItemGroup/PropertyGroup sections, compile constants, and per-framework package references introduced in earlier tasks. Also verify that project references no longer require net8.0 compatibility and that any deferred package decisions are clearly isolated from the temporary framework-bridging work so cleanup does not accidentally re-expand scope.

**Done when**: No project keeps a temporary net8.0 target or conditional compatibility path solely for the upgrade sequence, and the solution's project/package graph is simplified back to a stable .NET 10 steady state with deferred package follow-ups explicitly documented.

---

### 06-solution-validation: Validate the full net10 solution and capture deferred follow-up work

This task covers end-to-end validation across all 10 projects once the framework and package migration is complete. The assessment baseline includes 119 incidents with 21 mandatory findings, 84 potential findings, and 14 optional findings, so final validation must confirm that mandatory and upgrade-blocking issues have actually been closed and that remaining deferred items are understood rather than accidentally ignored.

Research should start with the full solution restore/build/test flow, runtime smoke checks for the three application entry points, and a concise list of package or behavior items intentionally deferred under the selected compatibility option. Use this task to confirm that inline API fixes addressed the source/binary incompatibilities in ApplicationCore, PublicApi, and Web, and that no temporary upgrade accommodations remain hidden outside the documented follow-up list.

**Done when**: The solution restores, builds, and runs on .NET 10; the existing automated test suites pass; the upgraded applications have been smoke-validated at a high level; and any intentionally deferred package replacements or runtime investigations are captured for post-upgrade follow-up.
