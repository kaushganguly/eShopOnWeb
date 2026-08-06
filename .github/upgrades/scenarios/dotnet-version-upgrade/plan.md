# .NET Version Upgrade Plan: net8.0 → net10.0

## Upgrade Overview
- **Solution**: eShopOnWeb.sln
- **Target**: net10.0 (LTS)
- **Strategy**: In-place upgrade — all projects upgraded together
- **Working Branch**: upgrade/dotnet-10

## Assessment Summary
- 10 projects all targeting net8.0
- 119 issues (21 mandatory, 84 potential, 14 optional)
- 32 affected files
- Key issues: TFM changes, package upgrades, API breaking changes

## Key Issues to Resolve
1. All 10 projects need TFM updated from net8.0 → net10.0
2. Directory.Packages.props version variables need updating
3. System.Security.Claims must be removed (now in framework)
4. Microsoft.VisualStudio.Azure.Containers.Tools.Targets must be removed (incompatible)
5. EmptyBasketOnCheckoutException: remove obsolete serialization constructor
6. TimeSpan.FromMinutes(double) → TimeSpan.FromMinutes(long)
7. Azure.Identity security vulnerability fix: 1.10.4 → 1.21.0

## Tasks

- ⬜ 01-tfm-and-packages: Update TFM and package versions
- ⬜ 02-fix-source-breaking-changes: Fix source-incompatible API changes
- ⬜ 03-build-and-fix: Build solution and fix remaining errors
- ⬜ 04-run-unit-tests: Run unit tests to verify upgrade
