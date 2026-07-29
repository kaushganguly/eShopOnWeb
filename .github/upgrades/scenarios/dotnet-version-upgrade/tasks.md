# .NET Version Upgrade Progress

## Overview

Upgrading eShopOnWeb solution from net8.0 to net10.0. All 10 projects are upgraded together using a direct in-place strategy, updating the centrally managed TargetFramework and NuGet packages, removing incompatible references, and fixing API breaking changes.

**Progress**: 1/4 tasks complete <progress value="25" max="100"></progress> 25%

## Tasks

- ✅ 01-update-sdk-and-packages: Update SDK and NuGet packages ([Content](tasks/01-update-sdk-and-packages/task.md), [Progress](tasks/01-update-sdk-and-packages/progress-details.md))
- 🔲 02-fix-project-files: Remove incompatible and redundant package references
- 🔲 03-fix-api-breaking-changes: Resolve breaking API changes
- 🔲 04-build-and-validate: Build solution and run unit tests
