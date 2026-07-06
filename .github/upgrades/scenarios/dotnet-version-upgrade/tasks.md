# .NET Version Upgrade Progress

## Overview

Upgrading all 10 eShopOnWeb projects from net8.0 to net10.0 (LTS) using the All-at-Once strategy. Central package management (Directory.Packages.props) allows TFM and package versions to be updated in one place.

**Progress**: 3/3 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-prerequisites: Verify SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-core-upgrade: Upgrade all projects to net10.0 ([Content](tasks/02-core-upgrade/task.md), [Progress](tasks/02-core-upgrade/progress-details.md))
- ✅ 03-test-validation: Run unit tests and confirm passing ([Content](tasks/03-test-validation/task.md), [Progress](tasks/03-test-validation/progress-details.md))
