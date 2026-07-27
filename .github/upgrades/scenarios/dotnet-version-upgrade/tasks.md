# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 using the All-at-Once strategy. All 10 projects will be upgraded simultaneously — TFM update in Directory.Packages.props, package version updates, and API breaking change fixes.

**Progress**: 3/3 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-prerequisites: Validate .NET 10 SDK and update toolchain configuration ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-all: Upgrade all projects to net10.0 and update all packages ([Content](tasks/02-upgrade-all/task.md), [Progress](tasks/02-upgrade-all/progress-details.md))
- ✅ 03-validate: Run full test suite and finalize ([Content](tasks/03-validate/task.md), [Progress](tasks/03-validate/progress-details.md))
