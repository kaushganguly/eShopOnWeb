# Upgrade Plan: eShopOnWeb .NET 8.0 → .NET 10.0

## Objective
Upgrade the entire eShopOnWeb solution from net8.0 to net10.0. The TFM is defined centrally in Directory.Packages.props and applies to all 10 projects.

## Strategy
Direct upgrade (net8.0 → net10.0), solution-wide, using Central Package Management.

## Assessment Summary
- 10 projects, all targeting net8.0
- 119 issues: 21 mandatory, 84 potential, 14 optional
- Key issues: TFM update, NuGet package upgrades, 1 incompatible package, 1 built-in package, API breaking changes

## Execution Constraints
- All package versions managed centrally in Directory.Packages.props
- TFM defined centrally — one change covers all projects
- Build tool: dotnet build (all SDK-style projects, no WPF/WinForms/net4xx)

## Tasks

### 01-central-version-update
Update Directory.Packages.props (TFM net8.0 → net10.0, all package versions) and global.json (SDK version 8.0.x → 10.0.x).

Package version changes:
- TargetFramework: net8.0 → net10.0
- AspNetVersion: 8.0.2 → 10.0.11
- SystemExtensionVersion: 8.0.0 → 10.0.11
- EntityFramworkCoreVersion: 8.0.2 → 10.0.11
- VSCodeGeneratorVersion: 8.0.0 → 10.0.2
- System.Text.Json: 8.0.3 → 10.0.11
- Azure.Identity: 1.10.4 → 1.21.0
- System.IdentityModel.Tokens.Jwt: 7.3.1 → 8.22.0
- global.json SDK: 8.0.x → 10.0.x

### 02-remove-incompatible-packages
Remove packages from individual .csproj files:
- System.Security.Claims from ApplicationCore.csproj (now included in framework)
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets from PublicApi.csproj (no .NET 10 support)
- Remove System.Security.Claims and Microsoft.VisualStudio.Azure.Containers.Tools.Targets from Directory.Packages.props

### 03-fix-api-breaking-changes
Fix source-incompatible API changes:
- Remove obsolete serialization constructor in EmptyBasketOnCheckoutException.cs
- Fix TimeSpan.FromMinutes ambiguity in ConfigureCookieSettings.cs
- Fix configuration.Get<T>() calls if compilation fails

### 04-build-and-validate
Build the full solution, fix any remaining compilation errors.

### 05-run-unit-tests
Run unit tests to validate the upgrade.
