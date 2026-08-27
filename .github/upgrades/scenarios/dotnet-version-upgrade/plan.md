# .NET Version Upgrade Plan: net8.0 → net10.0

## Overview
Upgrade eShopOnWeb solution from .NET 8 (net8.0) to .NET 10 (net10.0).

- **Solution**: eShopOnWeb.sln
- **Projects**: 10 projects (6 src + 4 tests)
- **Target Framework**: net10.0 (LTS)
- **Strategy**: Single-solution upgrade — update all projects together

## Assessment Summary
- 119 issues (21 mandatory, 84 potential, 14 optional)
- 10 projects all at net8.0
- Key issues: TFM changes, package upgrades, API breaking changes (EmptyBasketOnCheckoutException serialization constructor, Configure/Get API)

## Tasks

### 01-tfm-and-packages
Update target framework and package versions across the solution.
- Update global.json SDK version from 8.0.x to 10.0.x
- Update Directory.Packages.props: TargetFramework, version vars, package versions
- Remove incompatible packages: System.Security.Claims, Microsoft.VisualStudio.Azure.Containers.Tools.Targets
- Upgrade platform packages to .NET 10 versions

### 02-api-breaking-changes
Fix API breaking changes identified in assessment.
- Fix EmptyBasketOnCheckoutException: remove deprecated (SerializationInfo, StreamingContext) constructor
- Fix Configure<T>(services, configuration) calls if they fail to compile
- Fix ConfigurationBinder.Get<T> calls if they fail to compile

### 03-build-and-fix
Build the solution and fix any remaining compilation errors.

### 04-run-tests
Run unit tests and fix any test failures.
