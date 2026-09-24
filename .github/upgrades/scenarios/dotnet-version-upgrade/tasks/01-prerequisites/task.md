# 01-prerequisites: Verify SDK toolchain and update global.json

## Objective
Establish the .NET 10 toolchain foundation for the rest of the upgrade: update `global.json`'s
SDK version and the centrally managed `TargetFramework` in `Directory.Packages.props` from
`net8.0` to `net10.0`. No project-level code changes in this task.

## Research Findings

- **global.json** (repo root) currently pins `"version": "8.0.x"` with `"rollForward":
  "latestFeature"`. Needs to become a .NET 10 SDK version.
- **Installed SDKs on this machine** (`dotnet --list-sdks`): 8.0.131, 8.0.206, 8.0.319, 8.0.425,
  9.0.121, 9.0.205, 9.0.318, 10.0.112, 10.0.204, 10.0.303, **10.0.401** (latest .NET 10 SDK
  installed).
- **Central TFM management is unusual but present**: this repo does NOT set `TargetFramework`
  in any individual `.csproj` file (confirmed — none of the 10 project files declare
  `<TargetFramework>`). Instead, `Directory.Packages.props` (at repo root) declares:
  ```xml
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <TargetFramework>net8.0</TargetFramework>
    <AspNetVersion>8.0.2</AspNetVersion>
    <SystemExtensionVersion>8.0.0</SystemExtensionVersion>
    <EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>
    <VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>
  </PropertyGroup>
  ```
  Since `Directory.Packages.props` is auto-imported by MSBuild/NuGet (like `Directory.Build.props`),
  this one line drives the TFM for **all 10 projects** simultaneously. This means the "central TFM
  bump" required by tasks 02/03/04 is actually satisfied by a single edit here in task 01 — those
  tasks focus on per-group **package version bumps** and **code fixes**, not TFM edits.
- **No `Directory.Build.props`/`Directory.Build.targets`** file exists in the repo — only
  `Directory.Packages.props` carries shared MSBuild properties.
- **No legacy/non-SDK-style projects** — all 10 `.csproj` files use `<Project Sdk="Microsoft.NET.Sdk">`.
  No `packages.config` files found (only NuGet-generated `obj/*.nuget.g.props` files reference an
  old `ToolsVersion="14.0"` XML namespace — this is normal NuGet-generated tooling metadata, not a
  legacy project format signal).

## Plan
1. Update `global.json`: `"version": "10.0.x"`, keep `rollForward: latestFeature`.
2. Update `Directory.Packages.props`: change `<TargetFramework>net8.0</TargetFramework>` to
   `<TargetFramework>net10.0</TargetFramework>`.
3. Run `dotnet restore` on `eShopOnWeb.sln` to confirm the toolchain resolves correctly before
   handing off to tasks 02-04 (which bump individual package versions).

## Done When
- `global.json` specifies a .NET 10 SDK version.
- `dotnet --version` (with global.json in effect) confirms a matching SDK.
- `dotnet restore` on the solution succeeds (package version bumps happen in later tasks, so some
  packages may still show as incompatible with net10.0 until tasks 02-04 run — that's expected and
  not a blocker for this task).
