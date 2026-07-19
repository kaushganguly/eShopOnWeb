# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb (10 projects) from net8.0 to net10.0 using the All-at-Once strategy. Centralized package management via Directory.Packages.props means TFM and package updates apply to all projects in a single file change, followed by inline API break fixes and full test validation.

**Progress**: 2/4 tasks complete <progress value="50" max="100"></progress> 50%

## Tasks

- ✅ 01-prerequisites: Verify SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-tfm-and-packages: Update TFM and NuGet packages in Directory.Packages.props ([Content](tasks/02-upgrade-tfm-and-packages/task.md), [Progress](tasks/02-upgrade-tfm-and-packages/progress-details.md))
- 🔲 03-fix-api-breaks: Resolve breaking API changes in source code
- 🔲 04-final-validation: Run full test suite
