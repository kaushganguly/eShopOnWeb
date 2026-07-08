# .NET Version Upgrade Progress

## Overview

Upgrade eShopOnWeb's 10 SDK-style net8.0 projects to net10.0 using a top-down, application-first sequence. Shared projects will only carry temporary coexistence support where a net10.0 application still needs to live beside a remaining net8.0 consumer, and incompatible packages will be deferred unless they block the migration.

**Progress**: 3/6 tasks complete <progress value="50" max="100"></progress> 50%

## Tasks

- ✅ 01-toolchain-baseline: Align SDK and centralized package management for net10.0 ([Content](tasks/01-toolchain-baseline/task.md), [Progress](tasks/01-toolchain-baseline/progress-details.md))
- ✅ 02-blazor-admin-surface: Upgrade BlazorAdmin and BlazorShared for the first net10.0 entry point ([Content](tasks/02-blazor-admin-surface/task.md), [Progress](tasks/02-blazor-admin-surface/progress-details.md))
- ✅ 03-public-api-surface: Upgrade PublicApi with ApplicationCore and Infrastructure compatibility work ([Content](tasks/03-public-api-surface/task.md), [Progress](tasks/03-public-api-surface/progress-details.md))
- 🔲 04-web-storefront: Upgrade Web and its dependent test surface to complete the application cutover
- 🔲 05-shared-target-consolidation: Remove temporary coexistence targets and clean central version drift
- 🔲 06-solution-validation: Validate the complete net10.0 solution and record deferred follow-ups
