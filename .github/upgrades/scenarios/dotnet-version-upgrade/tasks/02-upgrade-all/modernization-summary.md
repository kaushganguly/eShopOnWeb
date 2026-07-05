# Modernization Summary

Completed task `02-upgrade-all` for the eShopOnWeb .NET version upgrade.

## Summary
- Upgraded the centrally managed target framework from `net8.0` to `net10.0`.
- Updated ASP.NET Core, Extensions, EF Core, Azure Identity, System.Text.Json, JWT, and xUnit package versions for .NET 10 compatibility.
- Removed obsolete or framework-provided package references (`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`, `System.Security.Claims`, `Microsoft.AspNetCore.Mvc`, redundant `System.Text.Json`, redundant `System.Net.Http.Json`).
- Replaced deprecated/vulnerable AutoMapper DI packaging with `AutoMapper` `16.2.0` and updated DI registration code.
- Fixed build-time API/test issues and warning-producing assertions.
- Verified `dotnet build /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln --nologo` completes with `0 Warning(s)` and `0 Error(s)`.
