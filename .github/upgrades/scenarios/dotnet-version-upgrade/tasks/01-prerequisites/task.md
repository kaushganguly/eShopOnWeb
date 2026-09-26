# 01-prerequisites: Verify toolchain and update global.json

## Objective

Verify the .NET 10.0 SDK is installed and update `global.json` and the centrally-managed
`Directory.Packages.props` target-framework/version-alias properties so subsequent tasks can
bump each project's TFM and package versions consistently.

## Research

- `dotnet --list-sdks` confirms `10.0.401` (and other 10.0.x, 9.0.x, 8.0.x bands) are installed
  on this machine — .NET 10 SDK is available.
- `global.json` (repo root) currently pins:
  ```json
  { "sdk": { "version": "8.0.x", "rollForward": "latestFeature" } }
  ```
  Needs to move to a `10.0.x` band with the same roll-forward policy.
- `Directory.Packages.props` (repo root) has `ManagePackageVersionsCentrally=true` and declares:
  - `<TargetFramework>net8.0</TargetFramework>` — the central TFM property (not directly consumed
    by project files today — each `.csproj` sets its own `<TargetFramework>net8.0</TargetFramework>`
    — but kept in sync per repo convention for CPM consistency).
  - `<AspNetVersion>8.0.2</AspNetVersion>` — drives all `Microsoft.AspNetCore.*`/`Microsoft.Extensions.Identity.Core` package versions.
  - `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` — drives `System.Net.Http.Json`, `Microsoft.Extensions.Logging.Configuration`.
  - `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` — drives EF Core InMemory/SqlServer/Tools.
  - `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` — drives `Microsoft.VisualStudio.Web.CodeGeneration.Design`.
- No `.resx` with embedded images, no WPF/WinForms/COM/VSIX/T4/vcxproj projects detected — all
  10 projects are plain SDK-style (ClassLibrary/AspNetCore). `dotnet build`/`dotnet restore` is
  the correct tool for this whole solution (per `building-projects` skill decision guide).

## Plan

1. Update `global.json` → `"version": "10.0.x"` (keep `rollForward: latestFeature`).
2. Update `Directory.Packages.props`:
   - `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
   - `<AspNetVersion>8.0.2</AspNetVersion>` → latest net10.0-compatible ASP.NET Core version
   - `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` → latest net10.0-compatible version
   - `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` → latest net10.0-compatible EF Core version
   - `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` → latest net10.0-compatible version
   - Resolve exact versions via `get_supported_package_version` before writing them.
3. Verify `dotnet --info` picks up the new global.json SDK band without error.

## Done when

`global.json` targets an installed .NET 10 SDK version, `Directory.Packages.props` central
properties target `net10.0`, and `dotnet --version` (from repo root) resolves to a 10.0.x SDK.
