# Upgrade Plan: .NET 8 → .NET 10

## Solution
- **Solution**: eShopOnWeb.sln
- **From**: net8.0 (.NET 8)
- **To**: net10.0 (.NET 10 LTS)
- **Strategy**: In-place upgrade (all projects upgraded together)

## Assessment Summary

### Projects
All 10 projects inherit `TargetFramework` from `Directory.Packages.props`. No individual project TFM declarations needed.

### Key Changes Required
1. **global.json**: SDK `8.0.x` → `10.0.x`
2. **Directory.Packages.props**: TargetFramework + all package versions

### Package Updates
| Package | From | To |
|---|---|---|
| AspNetVersion (all Microsoft.AspNetCore.*) | 8.0.2 | 10.0.11 |
| SystemExtensionVersion | 8.0.0 | 10.0.11 |
| EntityFrameworkCoreVersion | 8.0.2 | 10.0.11 |
| VSCodeGeneratorVersion | 8.0.0 | 10.0.2 |
| System.Text.Json | 8.0.3 | 10.0.11 |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.22.0 |
| Swashbuckle.AspNetCore | 6.5.0 | 10.2.3 |
| MediatR | 12.0.1 | 14.2.0 |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | 12.0.1 |
| Ardalis.Specification | 7.0.0 | 9.3.1 |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 |
| Ardalis.Result | 7.0.0 | 10.1.0 |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 |
| Ardalis.ApiEndpoints | 4.1.0 | 4.1.0 |
| Ardalis.ListStartupServices | 1.1.4 | 1.1.4 |
| FluentValidation | 11.9.0 | 12.1.1 |
| Blazored.LocalStorage | 4.5.0 | 4.5.0 |
| BlazorInputFile | 0.2.0 | 0.2.0 |
| Azure.Identity | 1.10.4 | 1.21.0 |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 |
| NSubstitute | 5.1.0 | 6.2.0 |
| NSubstitute.Analyzers.CSharp | 1.0.17 | 1.0.17 |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.9.0 |
| xunit | 2.7.0 | 2.9.3 |
| xunit.runner.visualstudio | 2.5.6 | 3.1.5 |
| xunit.runner.console | 2.7.0 | 2.9.3 |
| MSTest.TestAdapter | 3.2.2 | 4.3.3 |
| MSTest.TestFramework | 3.2.2 | 4.3.3 |
| coverlet.collector | 6.0.2 | 10.0.1 |
| MinimalApi.Endpoint | 1.3.0 | 1.3.0 |
| Microsoft.Web.LibraryManager.Build | 2.1.175 | 3.0.114 |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.19.6 |

## Tasks

### 01-sdk-and-tfm
Update global.json and Directory.Packages.props target framework

### 02-update-packages
Update all NuGet package versions in Directory.Packages.props

### 03-build-and-fix
Build the solution and fix any breaking changes

### 04-run-tests
Run unit tests to verify correctness
