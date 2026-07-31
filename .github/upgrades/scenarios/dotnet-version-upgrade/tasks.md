# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0. All 10 projects are upgraded simultaneously (All-at-Once strategy) using centralized package management. Covers TFM updates, package version bumps, security fixes, and API breaking change resolution.

**Progress**: 2/3 tasks complete <progress value="67" max="100"></progress> 67%

## Tasks

- ✅ 01-prerequisites: Verify .NET 10 SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-projects: Upgrade all 10 projects to net10.0 ([Content](tasks/02-upgrade-projects/task.md), [Progress](tasks/02-upgrade-projects/progress-details.md))
- 🔲 03-final-validation: Validate build and run unit tests
