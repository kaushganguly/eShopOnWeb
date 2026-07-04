# .NET Version Upgrade Progress

## Overview

This upgrade moves eShopOnWeb's 10 SDK-style projects from net8.0 to net10.0 using a top-down, application-first sequence. Shared libraries are prepared only when a consuming app needs them, then consolidated back to a single net10.0 target before final validation.

**Progress**: 6/6 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-toolchain-and-central-package-baseline: Verify .NET 10 prerequisites and prepare central package management ([Content](tasks/01-toolchain-and-central-package-baseline/task.md), [Progress](tasks/01-toolchain-and-central-package-baseline/progress-details.md))
- ✅ 02-public-api-slice: Upgrade PublicApi with ApplicationCore, Infrastructure, and API-facing tests ([Content](tasks/02-public-api-slice/task.md), [Progress](tasks/02-public-api-slice/progress-details.md))
- ✅ 03-blazor-admin-slice: Upgrade BlazorAdmin and BlazorShared ahead of the Web host ([Content](tasks/03-blazor-admin-slice/task.md), [Progress](tasks/03-blazor-admin-slice/progress-details.md))
- ✅ 04-web-storefront-slice: Upgrade Web and complete the highest-risk application migration ([Content](tasks/04-web-storefront-slice/task.md), [Progress](tasks/04-web-storefront-slice/progress-details.md))
- ✅ 05-library-and-test-consolidation: Remove temporary net8.0 compatibility from shared libraries and remaining tests ([Content](tasks/05-library-and-test-consolidation/task.md), [Progress](tasks/05-library-and-test-consolidation/progress-details.md))
- ✅ 06-solution-validation-and-deferred-followups: Validate the full net10.0 solution and capture remaining deferred work ([Content](tasks/06-solution-validation-and-deferred-followups/task.md), [Progress](tasks/06-solution-validation-and-deferred-followups/progress-details.md))
