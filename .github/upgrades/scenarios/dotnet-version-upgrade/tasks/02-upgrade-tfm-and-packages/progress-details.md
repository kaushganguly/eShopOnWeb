# Task 02: Update all project TFMs and NuGet package versions

## Changes Made

### Directory.Packages.props
- `<TargetFramework>`: net8.0 → net10.0
- `<AspNetVersion>`: 8.0.2 → 10.0.11 (drives all Microsoft.AspNetCore.* packages)
- `<EntityFramworkCoreVersion>`: 8.0.2 → 10.0.11 (drives all EF Core packages)
- `<SystemExtensionVersion>`: 8.0.0 → 10.0.11
- `<VSCodeGeneratorVersion>`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0 (security: MODERATE vuln fixed)
- `System.Text.Json`: 8.0.3 → 10.0.11 (security: HIGH vuln fixed)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.19.2 (needed by JwtBearer 10.x)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` (deprecated) → `AutoMapper 16.2.0`
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (NuGet.0001 incompatible)
- Removed `System.Security.Claims` (NuGet.0003 framework-included in net10.0)

### Project file changes
- `ApplicationCore.csproj`: removed System.Security.Claims, System.Text.Json (framework-included)
- `BlazorAdmin.csproj`: removed System.Net.Http.Json (framework-included)
- `PublicApi.csproj`: removed incompatible Containers.Tools.Targets; replaced AutoMapper reference
- `Web.csproj`: replaced AutoMapper.Extensions reference with AutoMapper
- `FunctionalTests.csproj`: removed deprecated DotNetCliToolReference (dotnet-xunit 2.3.1)

## Issues Resolved
- NU1903: System.Text.Json HIGH severity — FIXED
- NU1902: Azure.Identity MODERATE severity — FIXED
- NU1001: Microsoft.VisualStudio.Azure.Containers.Tools.Targets incompatible — REMOVED
- NU1003: System.Security.Claims framework-included — REMOVED
- NU1605: System.IdentityModel.Tokens.Jwt downgrade — FIXED

## Known Remaining Warnings
- NU1901 (LOW): NuGet.Packaging 6.12.1 / NuGet.Protocol 6.12.1 — transitive dependencies
  of Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2 (latest version available).
  Cannot be overridden in CPM without breaking the scaffolding tool dependency graph.
  These are build-time tool vulnerabilities, not runtime code.
