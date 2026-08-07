# Upgrade Plan: net8.0 → net10.0

## Objective

Upgrade all 10 projects in the eShopOnWeb solution from `net8.0` to `net10.0` (LTS, supported until Nov 2028).

## Strategy

Single-pass upgrade: update TFM + SDK + packages in one commit, fix source-code breaking changes, then validate build and tests.

## Projects in Scope

| Project | Difficulty | Notes |
|---|---|---|
| src/ApplicationCore | 🟢 Low | Source-incompatible serialization constructor |
| src/BlazorAdmin | 🟢 Low | Package upgrades only |
| src/BlazorShared | 🟢 Low | TFM only |
| src/Infrastructure | 🟢 Low | Package upgrades only |
| src/PublicApi | 🟢 Low | Incompatible package, binary + behavioral API changes |
| src/Web | 🟢 Low | Security vuln, API changes, source-incompatible TimeSpan.FromMinutes |
| tests/FunctionalTests | 🟢 Low | Package upgrades, behavioral changes |
| tests/IntegrationTests | 🟢 Low | Package upgrades |
| tests/PublicApiIntegrationTests | 🟢 Low | Package upgrades |
| tests/UnitTests | 🟢 Low | TFM only |

## Tasks

### 01-update-sdk-packages
Update global.json SDK version and Directory.Packages.props (TargetFramework, version variables, package versions).

### 02-fix-source-breaking-changes
Fix source-incompatible APIs:
- Remove obsolete serialization constructor from `EmptyBasketOnCheckoutException`
- Fix `TimeSpan.FromMinutes` ambiguity
- Remove incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from PublicApi

### 03-build-and-fix
Build the full solution, resolve any remaining compile errors and warnings.

### 04-run-tests
Run all tests and fix any failures.
