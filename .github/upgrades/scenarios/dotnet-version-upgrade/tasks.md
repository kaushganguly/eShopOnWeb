# .NET Version Upgrade Progress

## Overview

This upgrade moves eShopOnWeb's 10 SDK-style projects from net8.0 to net10.0 using a top-down, application-first sequence. Shared libraries may multi-target temporarily while BlazorAdmin, PublicApi, and Web are upgraded in order, followed by cleanup and full-solution validation.

**Progress**: 4/6 tasks complete <progress value="67" max="100"></progress> 67%

## Tasks

- ✅ 01-toolchain-baseline: Verify the .NET 10 toolchain and solution-wide prerequisites ([Content](tasks/01-toolchain-baseline/task.md), [Progress](tasks/01-toolchain-baseline/progress-details.md))
- ✅ 02-blazor-admin-client: Upgrade BlazorAdmin with the shared Blazor client library ([Content](tasks/02-blazor-admin-client/task.md), [Progress](tasks/02-blazor-admin-client/progress-details.md))
- ✅ 03-public-api-backend: Upgrade PublicApi with its backend libraries and API integration coverage ([Content](tasks/03-public-api-backend/task.md), [Progress](tasks/03-public-api-backend/progress-details.md))
- ✅ 04-web-storefront-tests: Upgrade Web and the remaining test projects on the full dependency graph ([Content](tasks/04-web-storefront-tests/task.md), [Progress](tasks/04-web-storefront-tests/progress-details.md))
- 🔲 05-shared-library-cleanup: Remove temporary compatibility scaffolding after all applications are on .NET 10
- 🔲 06-solution-validation: Validate the full net10 solution and capture deferred follow-up work
