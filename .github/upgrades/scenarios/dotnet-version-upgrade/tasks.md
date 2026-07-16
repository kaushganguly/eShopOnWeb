# .NET Version Upgrade Progress

## Overview

This upgrade moves the 10-project eShopOnWeb solution from net8.0 to net10.0 using a Top-Down, application-first strategy. Shared SDK and package changes land first, then each ASP.NET Core application is upgraded with its impacted libraries and tests before final cleanup and validation.

**Progress**: 1/6 tasks complete <progress value="17" max="100"></progress> 17%

## Tasks

- ✅ 01-prerequisites-and-central-package-baseline: Prepare SDK and central package versions ([Content](tasks/01-prerequisites-and-central-package-baseline/task.md), [Progress](tasks/01-prerequisites-and-central-package-baseline/progress-details.md))
- 🔲 02-upgrade-web-stack: Upgrade Web and its shared backend path
- 🔲 03-upgrade-publicapi-stack: Upgrade PublicApi and its integration coverage
- 🔲 04-upgrade-blazoradmin-stack: Upgrade BlazorAdmin and finalize client package alignment
- 🔲 05-consolidate-remaining-libraries-and-dependent-tests: Finish shared-library cleanup after all apps move
- 🔲 06-final-validation: Validate the full solution and document deferred follow-up
