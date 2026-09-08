# eShopOnWeb .NET Upgrade Plan

## Overview

**Target**: Upgrade all 10 projects from net8.0 to net10.0 (LTS)
**Scope**: Medium solution — 10 projects across 5 dependency tiers, all on modern .NET

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | 10 projects all on net8.0, ≤15 threshold; simultaneous upgrade is efficient |
| Unsupported Packages | Resolve Inline | 1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) — remove, no compatible version |
| Unsupported API Handling | Fix Inline | Binary and source incompatible APIs in Web and PublicApi; modern-to-modern upgrade with manageable change set |

**Selected Strategy**: All-At-Once — All projects upgraded simultaneously.
**Rationale**: 10 projects all on net8.0, 4-tier dependency graph; no .NET Framework projects; straightforward TFM bump.

## Tasks

### 01-prerequisites: Verify SDK and update toolchain configuration

Verify that .NET 10 SDK is installed. Update global.json to require SDK 10.0.x. Update version properties in Directory.Packages.props from 8.x to 10.x values.

**Done when**: dotnet --version confirms a .NET 10 SDK; global.json specifies SDK 10.0.x; version properties in Directory.Packages.props reference .NET 10.

---

### 02-upgrade-tfm-and-packages: Update all project TFMs and NuGet package versions

Update TargetFramework from net8.0 to net10.0 in all 10 project files. Update all PackageVersion entries in Directory.Packages.props to .NET 10-compatible versions. Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets (incompatible, no replacement).

**Done when**: All 10 projects target net10.0; dotnet restore completes without errors.

---

### 03-fix-breaking-changes: Resolve binary and source-incompatible API changes

Fix all Api.0001 and Api.0002 issues. Key areas: Web project (5 binary + 1 source incompatible) and PublicApi project (4 binary incompatible). Fix EmptyBasketOnCheckoutException SYSLIB0051 issue.

**Done when**: dotnet build eShopOnWeb.sln succeeds with 0 errors.

---

### 04-validate: Run tests and confirm upgrade success

Run full test suite. Verify UnitTests and IntegrationTests pass. Commit all changes.

**Done when**: dotnet test passes for UnitTests and IntegrationTests; solution builds with 0 errors.
