# 01-update-sdk-and-packages

## Objective

Update the SDK version in `global.json` and all centrally managed package versions in `Directory.Packages.props` to target .NET 10.

## Scope

This is a centralized upgrade — the `TargetFramework` property and all shared version variables are defined in `Directory.Packages.props`, so a single file update propagates to all 10 projects in the solution. No individual project files need to be edited for this task.

## Files Affected

| File | Change |
|------|--------|
| `global.json` | SDK version `8.0.x` → `10.0.x` (rollForward stays `latestFeature`) |
| `Directory.Packages.props` | TargetFramework + 4 version variables + 3 individual package versions |

## Research Findings

### global.json
- Uses `"rollForward": "latestFeature"` — this is appropriate for .NET 10 and requires no change.
- SDK version pattern `10.0.x` will resolve to the latest .NET 10 feature band installed.

### Directory.Packages.props — Version Variables
All of the following variables are used via `$(VariableName)` references in the `<PackageVersion>` items below, so updating the variables cascades to all dependent packages:

| Variable | Old Value | New Value | Used By |
|----------|-----------|-----------|---------|
| `TargetFramework` | net8.0 | net10.0 | All 10 projects |
| `AspNetVersion` | 8.0.2 | 10.0.10 | 10 ASP.NET/Identity/Blazor packages |
| `SystemExtensionVersion` | 8.0.0 | 10.0.10 | `Microsoft.Extensions.Logging.Configuration`, `System.Net.Http.Json` |
| `EntityFramworkCoreVersion` | 8.0.2 | 10.0.10 | `EF Core InMemory`, `SqlServer`, `Tools` |
| `VSCodeGeneratorVersion` | 8.0.0 | 10.0.2 | `Microsoft.VisualStudio.Web.CodeGeneration.Design` |

### Directory.Packages.props — Individual Package Versions
| Package | Old Version | New Version | Notes |
|---------|-------------|-------------|-------|
| `Azure.Identity` | 1.10.4 | 1.21.0 | Azure SDK; not tied to .NET version cadence |
| `System.Text.Json` | 8.0.3 | 10.0.10 | Ships with .NET runtime; align with TFM |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | 8.21.0 | Microsoft.IdentityModel package; 8.x series targets .NET 8+ |

### Packages NOT Changed
- Third-party packages (Ardalis.*, FluentValidation, MediatR, AutoMapper, NSubstitute, Swashbuckle, xunit, etc.) — these are version-agnostic and remain at their current versions.
- `Microsoft.AspNetCore.Mvc` Version="2.2.0" — this is pinned to an older version and requires separate investigation.
- Test SDK packages (`Microsoft.NET.Test.Sdk`, `MSTest.*`, `coverlet.collector`) — remain at current versions; compatible with .NET 10.

## Execution Steps

1. Edit `global.json`: change SDK version `8.0.x` → `10.0.x`
2. Edit `Directory.Packages.props`: update 4 version variables in `<PropertyGroup>` and 3 `<PackageVersion>` entries
3. Verify changes with grep
4. Write progress-details.md

