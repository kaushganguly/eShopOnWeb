# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 using All-at-Once strategy. All 10 projects will be upgraded simultaneously with centralized package management updates in Directory.Packages.props, removal of incompatible packages, and inline fixes for breaking API changes.

**Progress**: 1/3 tasks complete <progress value="33" max="100"></progress> 33%

## Tasks

- ✅ 01-prerequisites: Verify SDK and toolchain ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- 🔄 02-upgrade-all-projects: Upgrade TFM, packages, and fix breaking API changes ([Content](tasks/02-upgrade-all-projects/task.md))
- 🔲 03-final-validation: Run full test suite and confirm upgrade complete
