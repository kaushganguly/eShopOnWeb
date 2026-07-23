# .NET Version Upgrade Progress

## Overview

Upgrade eShopOnWeb from net8.0 to net10.0 using an application-first plan. The work upgrades BlazorAdmin, Web, and PublicApi in order, keeps test updates with the application that triggers them, and finishes with deferred package cleanup plus full-solution validation.

**Progress**: 5/6 tasks complete <progress value="83" max="100"></progress> 83%

## Tasks

- ✅ 01-toolchain-baseline: Align SDK baseline and upgrade backlog ([Content](tasks/01-toolchain-baseline/task.md), [Progress](tasks/01-toolchain-baseline/progress-details.md))
- ✅ 02-blazor-admin-client: Upgrade the Blazor admin client path ([Content](tasks/02-blazor-admin-client/task.md), [Progress](tasks/02-blazor-admin-client/progress-details.md))
- ✅ 03-web-storefront: Upgrade the customer-facing web application ([Content](tasks/03-web-storefront/task.md), [Progress](tasks/03-web-storefront/progress-details.md))
- ✅ 04-public-api-surface: Upgrade the public API host and API test path ([Content](tasks/04-public-api-surface/task.md), [Progress](tasks/04-public-api-surface/progress-details.md))
- ✅ 05-deferred-package-cleanup: Resolve deferred package and shared cleanup work ([Content](tasks/05-deferred-package-cleanup/task.md), [Progress](tasks/05-deferred-package-cleanup/progress-details.md))
- 🔲 06-solution-validation: Validate the full net10.0 solution
