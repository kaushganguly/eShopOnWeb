# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 LTS using the All-at-Once strategy. TFM and package versions are managed centrally in Directory.Packages.props, enabling a single-file update to upgrade all 10 projects simultaneously.

**Progress**: 2/3 tasks complete <progress value="67" max="100"></progress> 67%

## Tasks

- ✅ 01-prerequisites: Verify SDK and toolchain for net10.0 ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-all: Upgrade all projects to net10.0 ([Content](tasks/02-upgrade-all/task.md), [Progress](tasks/02-upgrade-all/progress-details.md))
- 🔲 03-validation: Final build and test suite validation
