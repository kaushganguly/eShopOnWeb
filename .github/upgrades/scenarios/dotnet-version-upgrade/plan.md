# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0 (LTS)
**Scope**: 10 projects — 3 ASP.NET Core apps (Web, PublicApi, BlazorAdmin), 4 class libraries (ApplicationCore, BlazorShared, Infrastructure, BlazorShared), 4 test projects — all using centralized package management via Directory.Packages.props

### Selected Strategy
**All-At-Once** — All 10 projects upgraded simultaneously in a single atomic operation.
**Rationale**: All 10 projects already target net8.0 (modern .NET), TFM and package versions are centrally managed via Directory.Packages.props, eliminating per-project changes. A single edit to that file effectively upgrades the entire solution at once.

## Tasks

### 01-prerequisites: Verify SDK and update global.json

Verify the .NET 10 SDK is installed on this machine and update global.json to require the .NET 10 SDK. The current global.json pins `8.0.x` with `rollForward: latestFeature` — this must be changed to the equivalent .NET 10 SDK specification before the TFM change, or the SDK will refuse to build the upgraded projects.

**Done when**: global.json references a .NET 10 SDK version (e.g., `10.0.x`) and `dotnet --version` confirms a .NET 10 SDK is available.

---

### 02-upgrade-tfm-and-packages: Update TFM and NuGet packages in Directory.Packages.props

Update the centrally managed TargetFramework from `net8.0` to `net10.0` and all package versions in Directory.Packages.props. Key version variables to update: `AspNetVersion` (8.0.2 → 10.x), `SystemExtensionVersion` (8.0.0 → 10.x), `EntityFramworkCoreVersion` (8.0.2 → 10.x), `VSCodeGeneratorVersion` (8.0.0 → 10.x). Third-party packages (Ardalis.*, AutoMapper, FluentValidation, MediatR, Swashbuckle, etc.) must also be updated to versions compatible with net10.0.

One incompatible package must be resolved inline: `Microsoft.AspNetCore.Mvc 2.2.0` in PublicApi — this is a legacy package whose types are now part of the ASP.NET Core framework reference (NuGet.0003). The package reference must be removed from PublicApi.csproj.

Security vulnerabilities in ApplicationCore and Web (`System.Security.Claims 4.3.0`, `System.Text.Json 8.0.3`) are addressed as part of this task by updating to the current safe versions or removing the package if its functionality is included in net10.0.

Deprecated packages (xunit 2.x, NSubstitute.Analyzers, coverlet, etc.) should be updated to their current recommended versions.

**Done when**: All package versions in Directory.Packages.props reference net10.0-compatible versions, `dotnet restore` succeeds with no NU* errors, and the incompatible Microsoft.AspNetCore.Mvc reference is removed from PublicApi.csproj.

---

### 03-fix-api-breaks: Resolve breaking API changes in source code

Fix binary-incompatible and source-incompatible API usages flagged by the assessment. PublicApi has 9 binary-incompatible API usages (Api.0001) and behavioral changes (Api.0003). Web has binary-incompatible APIs plus 3 source-incompatible API usages (Api.0002). ApplicationCore has source-incompatible APIs (Api.0002).

All identified breaking changes should be fixed inline per the "Fix Inline" option confirmed in upgrade-options.md. For each flagged file, apply the known .NET 10 replacement API or remove usage of removed APIs. Behavioral changes (Api.0003 — 49 occurrences across FunctionalTests, BlazorAdmin, Web, PublicApi, PublicApiIntegrationTests) should be reviewed and code updated where the new behavior differs from expected.

After fixes, the full solution must build with zero errors. Warnings should be reviewed and addressed in modified files.

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 errors. All projects in the solution compile targeting net10.0.

---

### 04-final-validation: Run full test suite

Run all tests across the solution — UnitTests, IntegrationTests, FunctionalTests, and PublicApiIntegrationTests. Address any test failures that result from net10.0 behavioral changes or package version differences. Document any skipped tests with clear rationale.

**Done when**: All test projects execute without build errors. Test pass rate matches or exceeds the pre-upgrade baseline. No test failures caused by the framework upgrade remain unaddressed.
