# 04-blazor: Upgrade BlazorAdmin and BlazorShared to .NET 10

## Findings
- `BlazorAdmin.csproj` and `BlazorShared.csproj` were effectively building on `net10.0` via central props, but neither project declared the target framework locally.
- `BlazorAdmin` already had its Blazor package versions resolved to `10.0.10` from `Directory.Packages.props`.
- `BlazorAdmin` produced `NU1510` because `System.Net.Http.Json` was explicitly referenced even though it is included in .NET 10.
- `BlazorShared` had no project-specific warnings or API issues beyond the target framework update.

## Planned changes
- Declare `net10.0` explicitly in both scoped project files.
- Remove the redundant `System.Net.Http.Json` package reference from `BlazorAdmin`.
- Rebuild `BlazorAdmin` (which also builds `BlazorShared`) and verify zero warnings in scope.
