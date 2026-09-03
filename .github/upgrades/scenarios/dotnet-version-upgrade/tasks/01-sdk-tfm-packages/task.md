# 01-sdk-tfm-packages: Update SDK, TFM, and NuGet package versions

## Objective
Update the .NET SDK in global.json, the target framework in Directory.Packages.props, and all package versions to net10.0-compatible releases.

## Affected Files
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
- `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props`
- `/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj`
- `/home/runner/work/eShopOnWeb/eShopOnWeb/src/PublicApi/PublicApi.csproj`

## Changes

### global.json
- `"version": "8.0.x"` → `"version": "10.0.x"`

### Directory.Packages.props
- `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`
- `<AspNetVersion>8.0.2</AspNetVersion>` → `<AspNetVersion>10.0.11</AspNetVersion>`
- `<SystemExtensionVersion>8.0.0</SystemExtensionVersion>` → `<SystemExtensionVersion>10.0.11</SystemExtensionVersion>`
- `<EntityFramworkCoreVersion>8.0.2</EntityFramworkCoreVersion>` → `<EntityFramworkCoreVersion>10.0.11</EntityFramworkCoreVersion>`
- `<VSCodeGeneratorVersion>8.0.0</VSCodeGeneratorVersion>` → `<VSCodeGeneratorVersion>10.0.2</VSCodeGeneratorVersion>`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.Text.Json`: `8.0.3` → `10.0.11`
- Remove `System.Security.Claims` entry (included in net10.0 framework)
- Remove `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` entry (incompatible, no net10.0 version)

### src/ApplicationCore/ApplicationCore.csproj
- Remove `<PackageReference Include="System.Security.Claims" />` (included in framework)

### src/PublicApi/PublicApi.csproj
- Remove `<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" />` (incompatible)
