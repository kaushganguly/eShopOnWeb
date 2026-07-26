# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 using All-at-Once strategy. All 10 projects (6 production, 4 test) are upgraded simultaneously with Central Package Management (Directory.Packages.props) as the primary version control lever.

**Progress**: 3/5 tasks complete <progress value="60" max="100"></progress> 60%

## Tasks

- ✅ 01-prerequisites: Verify SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-core-libraries: Upgrade foundation libraries ([Content](tasks/02-core-libraries/task.md), [Progress](tasks/02-core-libraries/progress-details.md))
- ✅ 03-applications: Upgrade web applications and Blazor frontend ([Content](tasks/03-applications/task.md), [Progress](tasks/03-applications/progress-details.md))
- 🔲 04-test-projects: Upgrade test projects
- 🔲 05-final-validation: Full solution build and test suite
