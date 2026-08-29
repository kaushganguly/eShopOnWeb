# Assessment: .NET 8 → .NET 10 Upgrade

**Solution**: eShopOnWeb.sln  
**Source**: net8.0  
**Target**: net10.0 (LTS, GA, supported until 2028-11-14)  
**Date**: 2026-08-29

## Projects (10 total)

| # | Project | SDK | Current TFM | Source |
|---|---------|-----|-------------|--------|
| 1 | ApplicationCore | Microsoft.NET.Sdk | net8.0 (inherited) | src/ApplicationCore |
| 2 | BlazorAdmin | Microsoft.NET.Sdk.BlazorWebAssembly | net8.0 (inherited) | src/BlazorAdmin |
| 3 | BlazorShared | Microsoft.NET.Sdk | net8.0 (inherited) | src/BlazorShared |
| 4 | Infrastructure | Microsoft.NET.Sdk | net8.0 (inherited) | src/Infrastructure |
| 5 | PublicApi | Microsoft.NET.Sdk.Web | net8.0 (inherited) | src/PublicApi |
| 6 | Web | Microsoft.NET.Sdk.Web | net8.0 (inherited) | src/Web |
| 7 | UnitTests | Microsoft.NET.Sdk | net8.0 (inherited) | tests/UnitTests |
| 8 | IntegrationTests | Microsoft.NET.Sdk | net8.0 (inherited) | tests/IntegrationTests |
| 9 | FunctionalTests | Microsoft.NET.Sdk | net8.0 (inherited) | tests/FunctionalTests |
| 10 | PublicApiIntegrationTests | Microsoft.NET.Sdk | net8.0 (inherited) | tests/PublicApiIntegrationTests |

**Key finding**: All projects inherit `<TargetFramework>` from `Directory.Packages.props`. Changing it once upgrades all 10 projects.

## Package Changes Required

### Framework-versioned (via version variables)
| Variable | Current | Target |
|----------|---------|--------|
| AspNetVersion | 8.0.2 | 10.0.11 |
| SystemExtensionVersion | 8.0.0 | 10.0.11 |
| EntityFramworkCoreVersion | 8.0.2 | 10.0.11 |
| VSCodeGeneratorVersion | 8.0.0 | 10.0.2 |

### Third-party packages requiring updates
| Package | Current | Target | Notes |
|---------|---------|--------|-------|
| Ardalis.Specification | 7.0.0 | 9.3.1 | Breaking changes possible |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 | Requires Ardalis.Specification 9.x |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 | Check API |
| Ardalis.Result | 7.0.0 | 10.1.0 | Check API |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 | Compatible |
| Azure.Identity | 1.10.4 | 1.21.0 | Compatible |
| FluentValidation | 11.9.0 | 12.1.1 | Check breaking changes |
| MediatR | 12.0.1 | 14.2.0 | Check breaking changes |
| NSubstitute | 5.1.0 | 6.2.0 | Check API |
| System.Text.Json | 8.0.3 | 10.0.11 | Bundled in SDK but explicit ref |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.22.0 | Breaking changes possible |
| Swashbuckle.AspNetCore | 6.5.0 | 10.2.3 | Major version - breaking changes |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | 10.2.3 | |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | 10.2.3 | |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 | Compatible |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.23.0 | Compatible |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.9.0 | Compatible |
| xunit | 2.7.0 | 2.9.3 | Compatible |
| xunit.runner.visualstudio | 2.5.6 | 4.0.0 | Major version |
| xunit.runner.console | 2.7.0 | 2.9.3 | Compatible |
| MSTest.TestAdapter | 3.2.2 | 4.3.3 | Compatible |
| MSTest.TestFramework | 3.2.2 | 4.3.3 | Compatible |
| coverlet.collector | 6.0.2 | 10.0.1 | Compatible |
| Microsoft.AspNetCore.Mvc | 2.2.0 | 2.3.12 | Standalone 2.x (for .NET Standard) |

## SDK Updates
- `global.json`: `8.0.x` → `10.0.x`
- CI workflow `.github/workflows/dotnetcore.yml`: `dotnet-version: '8.0.x'` → `10.0.x`

## Risks

| Risk | Severity | Mitigation |
|------|----------|------------|
| Swashbuckle 6→10 major version | High | Review API config changes |
| MediatR 12→14 breaking changes | Medium | Review handler and notification changes |
| Ardalis.Specification 7→9 | Medium | Review repository pattern API |
| Ardalis.Result 7→10 | Medium | Review result type API |
| FluentValidation 11→12 | Low | Usually backward compatible |
| xunit.runner.visualstudio 2→4 | Low | Test runner changes |
| System.IdentityModel.Tokens.Jwt 7→8 | Medium | JWT validation API changes |
| DotNetCliToolReference in FunctionalTests | Low | Remove obsolete tool reference |

## Notes

- `Microsoft.AspNetCore.Mvc 2.2.0` is a standalone package for .NET Standard projects; may be removable or needs version bump to 2.3.x
- `BlazorInputFile 0.2.0` - old Blazor component library, no updates; may have compatibility issues with Blazor WebAssembly 10
- `MinimalApi.Endpoint 1.3.0` - no newer version; likely compatible
- `Ardalis.ApiEndpoints 4.1.0` - no newer version; verify compatibility
- `DotNetCliToolReference` in FunctionalTests.csproj is obsolete and should be removed
