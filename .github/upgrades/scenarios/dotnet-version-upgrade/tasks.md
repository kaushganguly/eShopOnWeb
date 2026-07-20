# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0. All 10 projects are upgraded simultaneously using the All-at-Once strategy. CPM (Central Package Management) is already in place, simplifying package version updates.

**Progress**: 1/3 tasks complete <progress value="33" max="100"></progress> 33%

## Tasks

- ✅ 01-prerequisites: Update SDK and toolchain configuration for .NET 10 ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- 🔄 02-core-upgrade: Upgrade all projects to net10.0 — TFM, packages, and code fixes ([Content](tasks/02-core-upgrade/task.md))
- 🔲 03-validation: Full solution build and test suite
