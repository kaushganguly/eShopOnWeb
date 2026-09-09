# .NET Version Upgrade Plan: net8.0 → net10.0

## Overview

Upgrade the eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS).

**Solution**: eShopOnWeb.sln  
**Source**: net8.0  
**Target**: net10.0  
**Strategy**: In-place upgrade (all projects simultaneously)

## Projects in Scope

| Project | Type | Dependency Level |
|---------|------|-----------------|
| BlazorShared | ClassLibrary | 0 (foundation) |
| ApplicationCore | ClassLibrary | 1 |
| BlazorAdmin | AspNetCore | 1 |
| Infrastructure | ClassLibrary | 2 |
| PublicApi | AspNetCore | 3 |
| Web | AspNetCore | 3 |
| FunctionalTests | Test | 4 |
| PublicApiIntegrationTests | Test | 4 |
| UnitTests | Test | 4 |
| IntegrationTests | Test | 5 |

## Issues Summary

- **Mandatory (21)**: TFM changes, binary incompatible APIs, incompatible packages
- **Potential (84)**: Behavioral changes, package upgrades recommended
- **Optional (14)**: Deprecated packages, security vulnerabilities

## Upgrade Plan

### Task 01: Update global configuration
Update `global.json` SDK version and `Directory.Packages.props` with target framework and package versions.

**Changes:**
- `global.json`: SDK version `8.0.x` → `10.0.x`
- `Directory.Packages.props`: TargetFramework `net8.0` → `net10.0`
- Version variables: AspNetVersion=10.0.12, SystemExtensionVersion=10.0.12, EntityFramworkCoreVersion=10.0.12, VSCodeGeneratorVersion=10.0.2
- `System.Text.Json`: 8.0.3 → 10.0.12
- `Azure.Identity`: 1.10.4 → 1.21.0 (security fix)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no net10.0 version)

### Task 02: Fix ApplicationCore
- Remove `System.Security.Claims` PackageReference (now included in framework)
- Remove obsolete `Exception(SerializationInfo, StreamingContext)` constructor from `EmptyBasketOnCheckoutException`

### Task 03: Fix Web project source issues
- Fix `TimeSpan.FromMinutes(ValidityMinutesPeriod)` ambiguity in `ConfigureCookieSettings.cs`

### Task 04: Fix PublicApi project
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference (incompatible)

### Task 05: Build validation and test
- Restore, build, and run unit tests to confirm success

## Execution Constraints

- Automatic flow mode: no pauses
- Fix all build warnings in modified projects
- Run unit tests before marking complete
