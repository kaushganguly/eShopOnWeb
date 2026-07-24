# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 (LTS). All 10 projects are upgraded simultaneously using the All-at-Once strategy. Central Package Management (Directory.Packages.props) is updated to use net10.0-compatible package versions.

**Progress**: 2/3 tasks complete <progress value="67" max="100"></progress> 67%

## Tasks

- ✅ 01-prerequisites: Verify SDK and Update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-projects: Upgrade All Projects to net10.0 ([Content](tasks/02-upgrade-projects/task.md), [Progress](tasks/02-upgrade-projects/progress-details.md))
- 🔄 03-final-validation: Run Tests and Confirm Upgrade ([Content](tasks/03-final-validation/task.md))
