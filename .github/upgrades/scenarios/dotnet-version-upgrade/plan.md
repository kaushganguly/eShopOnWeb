# Upgrade Plan: .NET 8 → .NET 10

## Goal
Upgrade eShopOnWeb from net8.0 to net10.0 LTS, updating all projects, packages, and fixing breaking changes.

## Strategy
In-place upgrade of all projects simultaneously (single-phase). All projects are Low difficulty.

## Execution Constraints
- Target Framework: net10.0
- SDK: 10.0.x
- All packages must be updated to .NET 10-compatible versions
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` must be removed (no .NET 10 support)
- `System.Security.Claims` must be removed from ApplicationCore (included in framework)
- Obsolete serialization constructor in EmptyBasketOnCheckoutException must be removed

## 001-upgrade-dotnet-to-net10: Upgrade eShopOnWeb from .NET 8.0 to .NET 10.0 LTS

Upgrade all projects from net8.0 to net10.0. Update global.json, Directory.Packages.props, fix incompatible packages, fix breaking changes, and validate with build and tests.

Subtasks:
- 001.01-sdk-and-tfm: Update global.json SDK to 10.0.x and Directory.Packages.props versions (TargetFramework=net10.0, AspNetVersion=10.0.11, SystemExtensionVersion=10.0.11, EntityFramworkCoreVersion=10.0.11, VSCodeGeneratorVersion=10.0.2, Azure.Identity=1.21.0, System.IdentityModel.Tokens.Jwt=8.22.0)
- 001.02-fix-incompatible-packages: Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets from PublicApi.csproj and Directory.Packages.props; remove System.Security.Claims from ApplicationCore.csproj
- 001.03-fix-breaking-changes: Remove obsolete serialization constructor from EmptyBasketOnCheckoutException.cs; fix other compilation errors
- 001.04-build-and-validate: Restore, build, and run tests; fix any remaining errors
