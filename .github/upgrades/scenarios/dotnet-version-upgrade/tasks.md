# .NET Version Upgrade Progress

## Overview

This upgrade moves the 10-project eShopOnWeb solution from net8.0 to net10.0 using a Top-Down, application-first strategy. Shared SDK and package changes land first, then each ASP.NET Core application is upgraded with its impacted libraries and tests before final cleanup and validation.

**Progress**: 5/6 tasks complete <progress value="83" max="100"></progress> 83%

## Tasks

- ✅ 01-prerequisites-and-central-package-baseline: Prepare SDK and central package versions ([Content](tasks/01-prerequisites-and-central-package-baseline/task.md), [Progress](tasks/01-prerequisites-and-central-package-baseline/progress-details.md))
- ✅ 02-upgrade-web-stack: Upgrade Web and its shared backend path ([Content](tasks/02-upgrade-web-stack/task.md), [Progress](tasks/02-upgrade-web-stack/progress-details.md))
- ✅ 03-upgrade-publicapi-stack: Upgrade PublicApi and its integration coverage ([Content](tasks/03-upgrade-publicapi-stack/task.md), [Progress](tasks/03-upgrade-publicapi-stack/progress-details.md))
- ✅ 04-upgrade-blazoradmin-stack: Upgrade BlazorAdmin and finalize client package alignment ([Content](tasks/04-upgrade-blazoradmin-stack/task.md), [Progress](tasks/04-upgrade-blazoradmin-stack/progress-details.md))
- ✅ 05-consolidate-remaining-libraries-and-dependent-tests: Finish shared-library cleanup after all apps move ([Content](tasks/05-consolidate-remaining-libraries-and-dependent-tests/task.md), [Progress](tasks/05-consolidate-remaining-libraries-and-dependent-tests/progress-details.md))
- 🔲 06-final-validation: Validate the full solution and document deferred follow-up
