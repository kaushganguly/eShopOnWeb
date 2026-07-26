# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 using All-at-Once strategy. All 10 projects (6 production, 4 test) are upgraded simultaneously with Central Package Management (Directory.Packages.props) as the primary version control lever.

**Progress**: 1/5 tasks complete <progress value="20" max="100"></progress> 20%

## Tasks

- ✅ 01-prerequisites: Verify SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- 🔲 02-core-libraries: Upgrade foundation libraries
- 🔲 03-applications: Upgrade web applications and Blazor frontend
- 🔲 04-test-projects: Upgrade test projects
- 🔲 05-final-validation: Full solution build and test suite
