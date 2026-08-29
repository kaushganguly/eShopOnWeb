# Scenario Instructions: .NET Version Upgrade

## Scenario Parameters

- **Scenario**: dotnet-version-upgrade
- **Source Framework**: net8.0
- **Target Framework**: net10.0
- **Solution**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- **Working Branch**: upgrade/dotnet-net10
- **Flow Mode**: Automatic (no pauses)

## User Preferences

### Execution Style
- Fully autonomous execution, no user confirmation required.
- Accept all defaults and complete the entire upgrade end-to-end.

### Flow Mode
Automatic

## Decisions

- Target framework: net10.0 (GA, LTS, supported until 2028-11-14)
- SDK version: 10.0.400 is installed; global.json will use 10.0.x
- Upgrade strategy: Update all 10 projects simultaneously via Directory.Packages.props (centralized TFM)

## Execution Constraints

- All 10 projects inherit TargetFramework from Directory.Packages.props (centralized)
- Update TargetFramework once in Directory.Packages.props to upgrade all projects
- Update all version variables (AspNetVersion, SystemExtensionVersion, EntityFramworkCoreVersion, VSCodeGeneratorVersion)
- Update individual package versions for third-party packages
- Update global.json SDK version
- Update CI workflow .NET version
- Fix any breaking changes

## Package Version Targets (for net10.0)

### Framework-versioned packages
- AspNetVersion: 10.0.11 (was 8.0.2)
- SystemExtensionVersion: 10.0.11 (was 8.0.0)
- EntityFramworkCoreVersion: 10.0.11 (was 8.0.2)
- VSCodeGeneratorVersion: 10.0.2 (was 8.0.0)

### Third-party packages (already compatible or updated)
- Ardalis.Specification: 9.3.1 (was 7.0.0)
- Ardalis.Specification.EntityFrameworkCore: 9.3.1 (was 7.0.0)
- Ardalis.GuardClauses: 5.0.0 (was 4.0.1)
- Ardalis.Result: 10.1.0 (was 7.0.0)
- Ardalis.ApiEndpoints: 4.1.0 (unchanged)
- Ardalis.ListStartupServices: 1.1.4 (unchanged)
- Azure.Extensions.AspNetCore.Configuration.Secrets: 1.5.2 (was 1.3.1)
- Azure.Identity: 1.21.0 (was 1.10.4)
- AutoMapper.Extensions.Microsoft.DependencyInjection: 12.0.1 (unchanged)
- BlazorInputFile: 0.2.0 (unchanged)
- Blazored.LocalStorage: 4.5.0 (unchanged)
- FluentValidation: 12.1.1 (was 11.9.0)
- MediatR: 14.2.0 (was 12.0.1)
- MinimalApi.Endpoint: 1.3.0 (unchanged)
- NSubstitute: 6.2.0 (was 5.1.0)
- NSubstitute.Analyzers.CSharp: 1.0.17 (unchanged)
- System.Security.Claims: 4.3.0 (unchanged)
- System.Text.Json: 10.0.11 (was 8.0.3)
- System.Net.Http.Json: 10.0.11 (was 8.0.0)
- System.IdentityModel.Tokens.Jwt: 8.22.0 (was 7.3.1)
- Swashbuckle.AspNetCore: 10.2.3 (was 6.5.0)
- Swashbuckle.AspNetCore.SwaggerUI: 10.2.3 (was 6.5.0)
- Swashbuckle.AspNetCore.Annotations: 10.2.3 (was 6.5.0)
- Microsoft.Web.LibraryManager.Build: 3.0.114 (was 2.1.175)
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets: 1.23.0 (was 1.19.6)
- Microsoft.NET.Test.Sdk: 18.9.0 (was 17.9.0)
- xunit: 2.9.3 (was 2.7.0)
- xunit.runner.visualstudio: 4.0.0 (was 2.5.6)
- xunit.runner.console: 2.9.3 (was 2.7.0)
- MSTest.TestAdapter: 4.3.3 (was 3.2.2)
- MSTest.TestFramework: 4.3.3 (was 3.2.2)
- coverlet.collector: 10.0.1 (was 6.0.2)
- BuildBundlerMinifier: 3.2.449 (unchanged)
- Microsoft.AspNetCore.Mvc: 2.3.12 (was 2.2.0)
