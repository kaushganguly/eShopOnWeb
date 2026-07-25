# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade eShopOnWeb from net8.0 to net10.0
**Scope**: 10 projects (6 source, 4 test), all SDK-style with centralized package management via Directory.Packages.props

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects on net8.0, mechanical TFM bump to net10.0, CPM active for centralized package updates.

## Tasks

### 01-prerequisites: Verify SDK and toolchain

Confirm .NET 10 SDK is installed and global.json does not constrain the upgrade. The SDK validation tool has already confirmed SDK compatibility and no global.json changes are needed. This task records the verification as a baseline checkpoint before code changes begin.

**Done when**: .NET 10 SDK is confirmed installed and global.json is compatible with net10.0.

---

### 02-upgrade-all-projects: Upgrade TFM, packages, and fix breaking API changes

Update all 10 projects from net8.0 to net10.0 in a single pass. Since centralized package management is active, the primary changes are in `Directory.Packages.props` where the `TargetFramework` property and all versioned package MSBuild properties need updating.

**TFM change**: Update `<TargetFramework>net8.0</TargetFramework>` to `net10.0` in `Directory.Packages.props`. All projects inherit this centrally.

**Package updates** (in Directory.Packages.props):
- `AspNetVersion`: 8.0.2 → 10.0.10 (Microsoft.AspNetCore.* packages)
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.10 (Microsoft.EntityFrameworkCore.* packages)
- `SystemExtensionVersion`: 8.0.0 → 10.0.10 (Microsoft.Extensions.*, System.Net.Http.Json)
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2 (Microsoft.VisualStudio.Web.CodeGeneration.Design)
- `System.Text.Json`: 8.0.3 → 10.0.10
- `Azure.Identity`: 1.10.4 → 1.21.0 (security vulnerability fix)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.x latest (deprecated, move to latest LTS)

**Packages to remove**:
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` — no compatible net10.0 version; remove from Directory.Packages.props and all project files that reference it
- `System.Security.Claims` — functionality included in net10.0 framework reference; remove from ApplicationCore

**Breaking API fixes** (inline):
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: Remove obsolete `Exception(SerializationInfo, StreamingContext)` constructor — the protected serialization constructor was removed in .NET 9+. Remove the constructor and `[Serializable]` attribute if present.
- `src/Web/Configuration/ConfigureWebServices.cs` and `src/Web/Configuration/ConfigureCoreServices.cs`: `OptionsConfigurationServiceCollectionExtensions.Configure<T>(services, configuration)` and `ConfigurationBinder.Get<T>(configuration)` overloads changed; update callers to use explicit section-based binding.
- `src/PublicApi/Program.cs`: Same Configure<T>/Get<T> pattern fixes as above.
- `src/Web/Configuration/ConfigureCookieSettings.cs`: `TimeSpan.FromMinutes()` behavioral change — verify the usage is safe or switch to `TimeSpan.FromMinutes(double)` with explicit validation.
- `src/Web/Program.cs`: `UseExceptionHandler(string path)` behavioral change — check if usage needs updating.
- `src/PublicApi/Program.cs`: `AddConsole()` behavioral change — verify or update console logging setup.

**Done when**: All 10 projects target net10.0, all packages updated, incompatible packages removed, solution builds without errors or warnings, unit tests pass.

---

### 03-final-validation: Run full test suite and confirm upgrade complete

Run the complete test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) to confirm the upgraded solution behaves correctly. Document any test failures and resolve them. Confirm all projects target net10.0.

**Done when**: All tests pass (or known-broken tests are documented with reason), full solution builds clean, and all projects show net10.0 as TargetFramework.
