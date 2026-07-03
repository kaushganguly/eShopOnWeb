# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb from net8.0 to net10.0 LTS. Using a Top-Down (Application-First) strategy: Web and PublicApi applications upgraded with their library dependencies inline, followed by full solution validation.

**Progress**: 4/4 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-prerequisites: Verify .NET 10 SDK and update global.json ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-upgrade-web: Upgrade Web application and its library dependencies ([Content](tasks/02-upgrade-web/task.md), [Progress](tasks/02-upgrade-web/progress-details.md))
- ✅ 03-upgrade-publicapi: Upgrade PublicApi and fix binary-incompatible APIs ([Content](tasks/03-upgrade-publicapi/task.md), [Progress](tasks/03-upgrade-publicapi/progress-details.md))
- ✅ 04-final-validation: Full solution build and test suite ([Content](tasks/04-final-validation/task.md), [Progress](tasks/04-final-validation/progress-details.md))
