# 02-web-app

## Scope researched
- `src/Web/Web.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- central package versions in `Directory.Packages.props`
- upgrade assessment findings for Web and FunctionalTests

## Assessment findings
- `src/Web/Web.csproj` was flagged for package upgrades plus source/binary compatibility around configuration binding and Razor parsing.
- `tests/FunctionalTests/FunctionalTests.csproj` was flagged for `Microsoft.AspNetCore.Mvc.Testing` / EF Core alignment plus higher API churn from referenced web projects.
- The workspace already contains central net10 package baselines in `Directory.Packages.props` (`AspNetVersion=10.0.10`, `EntityFramworkCoreVersion=10.0.10`, `Azure.Identity=1.21.0`, `System.Text.Json=10.0.10`, `System.IdentityModel.Tokens.Jwt=8.19.2`).

## Package inventory for this scope
### Web
- Ardalis.ListStartupServices
- Ardalis.Specification
- Azure.Extensions.AspNetCore.Configuration.Secrets
- Azure.Identity
- MediatR
- BuildBundlerMinifier
- Microsoft.AspNetCore.Components.WebAssembly.Server
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.Web.LibraryManager.Build
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.Tools
- System.IdentityModel.Tokens.Jwt

### FunctionalTests
- Microsoft.AspNetCore.Mvc.Testing
- Microsoft.NET.Test.Sdk
- xunit
- xunit.runner.visualstudio
- Microsoft.EntityFrameworkCore.InMemory

## Upgrade notes
- Explicit `TargetFramework` entries were added to the two scoped project files so they target `net10.0` directly.
- Web source updates were required for .NET 10 Razor/source-generator behavior:
  - `ConfigurationBinder.GetValue` updated to generic form.
  - nullable-safe `BaseUrlConfiguration` binding added.
  - several Razor/Blazor files were normalized to self-closing tags or reshaped so Razor source generation succeeds on .NET 10.
- No additional `Directory.Packages.props` edits were required in this task because the central versions needed by Web/FunctionalTests were already updated in the workspace.

## Remaining follow-up outside this task's main code path
- `dotnet build src/Web/Web.csproj` still reports NU1510 warnings from referenced `ApplicationCore`/`BlazorAdmin` projects.
- `dotnet build tests/FunctionalTests/FunctionalTests.csproj` still reports NU1901/NU1903 warnings from transitive `PublicApi` dependencies (`AutoMapper`, `NuGet.Packaging`, `NuGet.Protocol`) plus the same NU1510 warnings from shared referenced projects.
