# Upgrade Plan: eShopOnWeb .NET 8.0 → .NET 10.0 LTS

## Summary
Upgrade all projects in the eShopOnWeb solution from .NET 8.0 to .NET 10.0 LTS.

## Strategy
Single-pass upgrade: update TFM, packages, fix API breaking changes, validate.

## Upgrade Targets
- **Target Framework**: net10.0 (LTS, support until Nov 2028)
- **Projects**: 10 projects (6 src + 4 tests)
- **Dependency Order**: BlazorShared → ApplicationCore/BlazorAdmin → Infrastructure → Web/PublicApi → Tests

## Package Version Changes
| Package | Old | New |
|---------|-----|-----|
| Microsoft.AspNetCore.* | 8.0.2 | 10.0.11 |
| Microsoft.EntityFrameworkCore.* | 8.0.2 | 10.0.11 |
| Microsoft.Extensions.* | 8.0.0 | 10.0.11 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 | 10.0.2 |
| System.Text.Json | 8.0.3 | 10.0.11 |
| System.Net.Http.Json | 8.0.0 | 10.0.11 |
| Azure.Identity | 1.10.4 | 1.21.0 (security fix) |

## Packages to Remove
| Package | Reason |
|---------|--------|
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | Incompatible with .NET 10 |
| System.Security.Claims | Now included in framework reference |

## API Breaking Changes to Fix
| Location | Issue | Fix |
|----------|-------|-----|
| ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs | Serialization constructor removed | Remove protected ctor |
| Web/Configuration/ConfigureCookieSettings.cs | TimeSpan.FromMinutes obsolete overload | Cast to double |

## Tasks
- 01-update-sdk-and-central-packages: Update global.json and Directory.Packages.props
- 02-fix-api-breaking-changes: Fix source-incompatible API usages
- 03-fix-project-files: Remove incompatible/redundant package references from csproj
- 04-build-and-fix: Build solution and resolve any remaining errors
- 05-run-tests: Run unit tests and validate
