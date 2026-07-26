# 03-applications: Upgrade web applications and Blazor frontend

## Objective

Upgrade BlazorAdmin, PublicApi, and Web from net8.0 to net10.0. These are the most complex projects with API compatibility issues.

## Scope

- `src/BlazorAdmin/BlazorAdmin.csproj` — Blazor WebAssembly app, behavioral change
- `src/PublicApi/PublicApi.csproj` — ASP.NET Core API, binary-incompatible APIs, 17 issues (6 mandatory)
- `src/Web/Web.csproj` — Main MVC web app, binary + source incompatible APIs, 29 issues (6 mandatory)

## Context

**Directory.Packages.props already updated** to net10.0 versions. All AspNet packages are at 10.0.10.

**BlazorAdmin**: TFM change already handled by Directory.Packages.props TargetFramework property. Need to check for behavioral changes.

**PublicApi**: The `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference was already removed in task 01. Has binary-incompatible APIs — check assessment for details. JwtBearer is at 10.0.10. Deprecated: AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1, System.IdentityModel.Tokens.Jwt (updated to 8.19.2).

**Web**: Has binary + source incompatible APIs. Azure.Identity (1.21.0) already fixed. Microsoft.VisualStudio.Web.CodeGeneration.Design updated to 10.0.2. 

## Done when

- BlazorAdmin, PublicApi, and Web all build with 0 errors and 0 warnings
- All mandatory API incompatibilities resolved
- No references to removed/incompatible packages
