# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 using an All-at-Once strategy. TargetFramework is centrally managed, so all 10 projects are upgraded in a single coordinated pass covering TFM, packages, and API breaking changes.

**Progress**: 3/3 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-prerequisites: Verify SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade: Update TFM, packages, and fix breaking API changes ([Content](tasks/02-upgrade/task.md), [Progress](tasks/02-upgrade/progress-details.md))
- ✅ 03-validation: Full solution build and test verification ([Content](tasks/03-validation/task.md), [Progress](tasks/03-validation/progress-details.md))
