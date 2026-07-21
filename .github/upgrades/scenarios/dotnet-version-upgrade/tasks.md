# .NET Version Upgrade Progress

## Overview

Upgrading the eShopOnWeb solution from net8.0 to net10.0 LTS using the All-at-Once strategy. All 10 projects are upgraded simultaneously in a single atomic pass.

**Progress**: 1/3 tasks complete <progress value="33" max="100"></progress> 33%

## Tasks

- ✅ 01-prerequisites: Verify .NET 10 SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- 🔄 02-upgrade-all-projects: Upgrade all 10 projects to net10.0 ([Content](tasks/02-upgrade-all-projects/task.md))
- 🔲 03-final-validation: Run tests and verify full solution health
