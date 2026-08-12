# Progress Details: 01-central-config

## Changes Made

### global.json
- SDK version: `8.0.x` → `10.0.x`

### Directory.Packages.props
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.11`
- `SystemExtensionVersion`: `8.0.0` → `10.0.11`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.11`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security fix)
- `System.Text.Json`: `8.0.3` → `10.0.11`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
- Removed `System.Security.Claims` (now part of framework)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10 support)
