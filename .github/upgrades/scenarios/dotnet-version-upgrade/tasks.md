# .NET Version Upgrade Progress

## Overview

Upgrade all 10 SDK-style eShopOnWeb projects from net8.0 to net10.0 using a top-down, application-first plan. The work starts with the lowest-dependency application slice, then upgrades the API and storefront surfaces with their supporting libraries and tests before whole-solution validation.

**Progress**: 3/5 tasks complete <progress value="60" max="100"></progress> 60%

## Tasks

- ✅ 01-upgrade-prerequisites: Verify the net10 baseline and shared upgrade assumptions ([Content](tasks/01-upgrade-prerequisites/task.md), [Progress](tasks/01-upgrade-prerequisites/progress-details.md))
- ✅ 02-blazor-admin-stack: Upgrade the Blazor admin application and shared UI contracts ([Content](tasks/02-blazor-admin-stack/task.md), [Progress](tasks/02-blazor-admin-stack/progress-details.md))
- ✅ 03-public-api-stack: Upgrade the API service with its domain and infrastructure dependencies ([Content](tasks/03-public-api-stack/task.md), [Progress](tasks/03-public-api-stack/progress-details.md))
- 🔲 04-web-storefront-stack: Upgrade the storefront application and its downstream test suites
- 🔲 05-solution-validation: Validate the full net10 solution and close remaining upgrade gaps
