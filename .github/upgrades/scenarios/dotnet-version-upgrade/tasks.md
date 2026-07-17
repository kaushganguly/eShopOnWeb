# .NET 10 Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0. All 10 projects use centrally-managed TFM (Directory.Build.props) and packages (Directory.Packages.props). Using All-at-Once strategy.

**Progress**: 3/3 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-prerequisites: Verify and update SDK toolchain ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-solution: Upgrade TFM, packages, and fix breaking API changes ([Content](tasks/02-upgrade-solution/task.md), [Progress](tasks/02-upgrade-solution/progress-details.md))
- ✅ 03-validate: Run unit tests and confirm upgrade success ([Content](tasks/03-validate/task.md), [Progress](tasks/03-validate/progress-details.md))
