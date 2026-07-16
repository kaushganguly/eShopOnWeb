# 04-upgrade-blazoradmin-stack: Upgrade BlazorAdmin Stack to .NET 10

## Objective

Align BlazorAdmin and BlazorShared to the net10.0 baseline established by prior tasks,
fix all assessment findings, and ensure both projects build cleanly with 0 warnings.

## Scope

- `src/BlazorAdmin/BlazorAdmin.csproj` — Blazor WebAssembly client
- `src/BlazorShared/BlazorShared.csproj` — shared Blazor models and interfaces

## Research Findings

### BlazorAdmin.csproj Assessment Issues

| Issue | Category | Description | Outcome |
|-------|----------|-------------|---------|
| Api.0003 | Behavioral | Behavioral changes in .NET 10 | No code changes required; existing code is compatible |
| NuGet.0002 | Packages | Package upgrades recommended | Already resolved in Task 01 via Central Package Management |

### BlazorShared.csproj Assessment Issues

| Issue | Category | Description | Outcome |
|-------|----------|-------------|---------|
| Project.0002 | Project | Target framework needs to be changed | Resolved: net10.0 inherited from Directory.Packages.props |

## Execution

### Phase 1: Research

Both projects were examined for outstanding issues. The assessment findings were fully
addressed by prior tasks:

- **Task 01** updated all package versions in `Directory.Packages.props` to net10.0 and
  removed `System.Net.Http.Json` from BlazorAdmin (now included in the framework).
- **Task 02** completed any shared BlazorShared adjustments required by the Web project.

The `TargetFramework` property set to `net10.0` in `Directory.Packages.props` is
inherited by both `BlazorShared.csproj` (uses `Microsoft.NET.Sdk`) and
`BlazorAdmin.csproj` (uses `Microsoft.NET.Sdk.BlazorWebAssembly`), so no explicit
`<TargetFramework>` node is needed in either project file.

### Phase 2: Build Verification

```
dotnet build src/BlazorAdmin/BlazorAdmin.csproj
  → Build succeeded. 0 Warning(s). 0 Error(s).

dotnet build src/BlazorShared/BlazorShared.csproj
  → Build succeeded. 0 Warning(s). 0 Error(s).
```

No additional code changes were required in this task.

## Outcome

Both projects build on `net10.0` with **0 warnings and 0 errors**. All assessment findings
are resolved. Central Package Management alignment is complete for both projects.
