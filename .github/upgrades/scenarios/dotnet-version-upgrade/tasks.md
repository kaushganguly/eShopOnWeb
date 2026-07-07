# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from .NET 8 (net8.0) to .NET 10 (net10.0) using the All-at-Once strategy. All 10 projects use centralized package management via Directory.Packages.props, simplifying the upgrade to a focused set of changes.

**Progress**: 3/3 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-prerequisites: Verify SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-all-projects: Upgrade all projects to .NET 10 ([Content](tasks/02-upgrade-all-projects/task.md), [Progress](tasks/02-upgrade-all-projects/progress-details.md))
- ✅ 03-final-validation: Run full test suite ([Content](tasks/03-final-validation/task.md), [Progress](tasks/03-final-validation/progress-details.md))
