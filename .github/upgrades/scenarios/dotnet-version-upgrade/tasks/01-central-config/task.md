# 01-central-config: Update central configuration and package versions

## Affected Files
- `global.json` — SDK version pin
- `Directory.Packages.props` — Central Package Management versions

## Changes

### global.json
- SDK version: `8.0.x` → `10.0.x`

### Directory.Packages.props
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.11`
- `SystemExtensionVersion`: `8.0.0` → `10.0.11`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.11`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.Text.Json`: `8.0.3` → `10.0.11`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
- REMOVE `System.Security.Claims` (NuGet.0003 — included in framework)
- REMOVE `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (NuGet.0001 — no net10 support)
