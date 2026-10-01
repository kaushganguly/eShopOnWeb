# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 (LTS)
**Scope**: 10 projects (eShopOnWeb.sln / Everything.sln, same projects), all SDK-style, Central Package Management via `Directory.Packages.props` which sets `TargetFramework` once for every project

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | 10 projects (≤15), all already on net8.0 (modern-to-modern), low complexity per assessment, no CI-green constraint |
| Unsupported Packages | Resolve Inline | Only `src/PublicApi` flags an incompatible package (`Microsoft.AspNetCore.Mvc` 2.2.0) — small enough to research and resolve inline |
| Unsupported API Handling | Fix Inline | Modern-to-modern upgrade with few flagged API issues (Api.0001/0002/0003) — no bottom-up strategy in play |

## Tasks

### 01-prerequisites: Verify toolchain and baseline

Verify the .NET 10 SDK is installed and available, confirm `global.json` can roll forward to `10.0.x`, and capture a pre-upgrade baseline (current build state, current test pass/fail counts) so later validation has a comparison point. Review the centrally-managed MSBuild properties in `Directory.Packages.props` (`TargetFramework`, `AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`, `VSCodeGeneratorVersion`) since these drive every project's framework and package versions from one place.

**Done when**: .NET 10 SDK confirmed available, `global.json` update plan confirmed, and baseline build/test status recorded.


### 02-upgrade-all-projects: Upgrade all projects to net10.0

Update the centrally-managed `TargetFramework` property (and `AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`, `VSCodeGeneratorVersion`) in `Directory.Packages.props` from net8.0/8.x to net10.0/10.x, and update `global.json` to target the .NET 10 SDK. Update all centrally-managed NuGet package versions in `Directory.Packages.props` to net10-compatible, non-vulnerable versions — including `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*`, test packages (`Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.*`, `MSTest.*`, `coverlet.collector`), and third-party packages (`Ardalis.*`, `MediatR`, `AutoMapper.Extensions.Microsoft.DependencyInjection`, `FluentValidation`, `Swashbuckle.AspNetCore*`, `Azure.Identity`, `Azure.Extensions.AspNetCore.Configuration.Secrets`, `System.IdentityModel.Tokens.Jwt`). Give special attention to `Microsoft.AspNetCore.Mvc` in `src/PublicApi`, pinned at `2.2.0` independent of `$(AspNetVersion)` and flagged incompatible — likely a legacy leftover that conflicts with the framework-provided shared framework and should be reconciled rather than version-bumped.

Covers all 10 projects in one pass: ApplicationCore, BlazorShared, Infrastructure (libraries); Web, PublicApi, BlazorAdmin (applications — watch Blazor WebAssembly/Authentication breaking changes); UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests (test projects — watch `Microsoft.AspNetCore.Mvc.Testing` `WebApplicationFactory` behavior changes). Fix all resulting compilation errors from ASP.NET Core, EF Core, Identity, and Blazor WebAssembly breaking API changes identified by the assessment (Api.0001 binary-incompatible in PublicApi/Web, Api.0002 source-incompatible in ApplicationCore, Api.0003 behavioral changes in BlazorAdmin/PublicApi/Web/test projects) inline — do not stub or defer. Address flagged security-vulnerable (NuGet.0004) and deprecated (NuGet.0005) packages as part of this task.

**Done when**: All 10 projects target `net10.0`, `global.json` targets a .NET 10 SDK, all centrally-managed package versions are net10-compatible with no known vulnerabilities, and the solution builds with 0 errors and 0 warnings.


### 03-final-validation: Validate build, tests, and security

Run a full solution build confirming 0 errors/0 warnings, then run the full test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) confirming all tests pass. Re-run package vulnerability detection to confirm no known-vulnerable packages remain in the restored graph.

**Done when**: Solution builds with 0 errors/0 warnings, all tests pass, and no known security vulnerabilities remain.
