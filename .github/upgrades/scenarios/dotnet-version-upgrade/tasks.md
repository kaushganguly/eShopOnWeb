# .NET Version Upgrade Progress

## Overview

Upgrade eShopOnWeb from net8.0 to net10.0 using an application-first plan. The work upgrades BlazorAdmin, Web, and PublicApi in order, keeps test updates with the application that triggers them, and finishes with deferred package cleanup plus full-solution validation.

**Progress**: 1/6 tasks complete <progress value="17" max="100"></progress> 17%

## Tasks

- ✅ 01-toolchain-baseline: Align SDK baseline and upgrade backlog ([Content](tasks/01-toolchain-baseline/task.md), [Progress](tasks/01-toolchain-baseline/progress-details.md))
- 🔄 02-blazor-admin-client: Upgrade the Blazor admin client path ([Content](tasks/02-blazor-admin-client/task.md))
- 🔲 03-web-storefront: Upgrade the customer-facing web application
- 🔲 04-public-api-surface: Upgrade the public API host and API test path
- 🔲 05-deferred-package-cleanup: Resolve deferred package and shared cleanup work
- 🔲 06-solution-validation: Validate the full net10.0 solution
