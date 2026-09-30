# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

Detailed findings live alongside this file in `assessment/`. This page is the index: read it first, then open only the documents you need.

## Table of Contents

- [Executive Summary](#executive-summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Detailed Reports](#detailed-reports)
  - [Projects Relationship Graph](assessment/project-graph.md)
  - [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
  - [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)
  - [Project Details](#project-details)

## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 10 | All require upgrade |
| Total NuGet Packages | 301 | 26 need upgrade |
| Total Code Files | 302 |  |
| Total Code Files with Incidents | 34 |  |
| Total Lines of Code | 12159 |  |
| Total Number of Issues | 154 |  |
| Proposed Target Framework | net10.0 |  |
| Estimated LOC to modify | 96+ | at least 0.8% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [src/ApplicationCore/ApplicationCore.csproj](assessment/projects/ApplicationCore.md) | net8.0 | 🟢 Low | 3 | 2 | 0 | 2+ | ClassLibrary, Sdk Style = True |
| [src/BlazorAdmin/BlazorAdmin.csproj](assessment/projects/BlazorAdmin.md) | net8.0 | 🟢 Low | 7 | 6 | 0 | 6+ | AspNetCore, Sdk Style = True |
| [src/BlazorShared/BlazorShared.csproj](assessment/projects/BlazorShared.md) | net8.0 | 🟢 Low | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src/Infrastructure/Infrastructure.csproj](assessment/projects/Infrastructure.md) | net8.0 | 🟢 Low | 4 | 4 | 0 | 4+ | ClassLibrary, Sdk Style = True |
| [src/PublicApi/PublicApi.csproj](assessment/projects/PublicApi.md) | net8.0 | 🟢 Low | 11 | 15 | 0 | 15+ | AspNetCore, Sdk Style = True |
| [src/Web/Web.csproj](assessment/projects/Web.md) | net8.0 | 🟢 Low | 13 | 24 | 0 | 24+ | AspNetCore, Sdk Style = True |
| [tests/FunctionalTests/FunctionalTests.csproj](assessment/projects/FunctionalTests.md) | net8.0 | 🟢 Low | 3 | 34 | 0 | 34+ | ClassLibrary, Sdk Style = True |
| [tests/IntegrationTests/IntegrationTests.csproj](assessment/projects/IntegrationTests.md) | net8.0 | 🟢 Low | 2 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj](assessment/projects/PublicApiIntegrationTests.md) | net8.0 | 🟢 Low | 3 | 11 | 0 | 11+ | ClassLibrary, Sdk Style = True |
| [tests/UnitTests/UnitTests.csproj](assessment/projects/UnitTests.md) | net8.0 | 🟢 Low | 2 | 0 | 0 |  | ClassLibrary, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 275 | 91.4% |
| ⚠️ Incompatible | 7 | 2.3% |
| 🔄 Upgrade Recommended | 19 | 6.3% |
| ***Total NuGet Packages*** | ***301*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 24 | High - Require code changes |
| 🟡 Source Incompatible | 20 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 52 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 13838 |  |
| ***Total APIs Analyzed*** | ***13934*** |  |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| IdentityModel & Claims-based Security | 12 | 12.5% | Windows Identity Foundation (WIF), SAML, and claims-based authentication APIs that have been replaced by modern identity libraries. WIF was the original identity framework for .NET Framework. Migrate to Microsoft.IdentityModel.* packages (modern identity stack). |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:System.Net.Http.HttpContent | 32 | 33.3% | Behavioral Change |
| T:System.Uri | 13 | 13.5% | Behavioral Change |
| M:Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure''1(Microsoft.Extensions.DependencyInjection.IServiceCollection,Microsoft.Extensions.Configuration.IConfiguration) | 5 | 5.2% | Binary Incompatible |
| M:Microsoft.Extensions.Configuration.ConfigurationBinder.Get''1(Microsoft.Extensions.Configuration.IConfiguration) | 4 | 4.2% | Binary Incompatible |
| M:System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.#ctor | 3 | 3.1% | Binary Incompatible |
| M:System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.CreateToken(Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor) | 3 | 3.1% | Binary Incompatible |
| M:System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.WriteToken(Microsoft.IdentityModel.Tokens.SecurityToken) | 3 | 3.1% | Binary Incompatible |
| M:System.Uri.#ctor(System.String) | 3 | 3.1% | Behavioral Change |
| T:System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler | 3 | 3.1% | Binary Incompatible |
| M:Microsoft.Extensions.DependencyInjection.IdentityEntityFrameworkBuilderExtensions.AddEntityFrameworkStores''1(Microsoft.AspNetCore.Identity.IdentityBuilder) | 2 | 2.1% | Source Incompatible |

The table above is the top 10. See [the complete list](assessment/api-issues/most-frequent-api-issues.md) for every affected API.

## Detailed Reports

- [Projects Relationship Graph](assessment/project-graph.md)
- [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
- [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)

### Project Details

- [src/ApplicationCore/ApplicationCore.csproj](assessment/projects/ApplicationCore.md)
- [src/BlazorAdmin/BlazorAdmin.csproj](assessment/projects/BlazorAdmin.md)
- [src/BlazorShared/BlazorShared.csproj](assessment/projects/BlazorShared.md)
- [src/Infrastructure/Infrastructure.csproj](assessment/projects/Infrastructure.md)
- [src/PublicApi/PublicApi.csproj](assessment/projects/PublicApi.md)
- [src/Web/Web.csproj](assessment/projects/Web.md)
- [tests/FunctionalTests/FunctionalTests.csproj](assessment/projects/FunctionalTests.md)
- [tests/IntegrationTests/IntegrationTests.csproj](assessment/projects/IntegrationTests.md)
- [tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj](assessment/projects/PublicApiIntegrationTests.md)
- [tests/UnitTests/UnitTests.csproj](assessment/projects/UnitTests.md)


