# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS) using the All-at-Once strategy. All 10 projects will be upgraded simultaneously — updating the centrally managed target framework, bumping package versions, and fixing breaking API changes.

**Progress**: 2/3 tasks complete <progress value="67" max="100"></progress> 67%

## Tasks

- ✅ 01-prerequisites: Verify SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-all-projects: Upgrade all projects to net10.0 ([Content](tasks/02-upgrade-all-projects/task.md), [Progress](tasks/02-upgrade-all-projects/progress-details.md))
- 🔄 03-final-validation: Full solution build and test validation ([Content](tasks/03-final-validation/task.md))
