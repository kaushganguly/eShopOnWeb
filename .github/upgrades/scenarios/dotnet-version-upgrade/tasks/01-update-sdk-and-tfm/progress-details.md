# Progress Details: 01-update-sdk-and-tfm

## Changes Made

### global.json
- Updated SDK version from `8.0.x` to `10.0.x`

### Directory.Packages.props
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion`: `8.0.2` → `10.0.11`
- `SystemExtensionVersion`: `8.0.0` → `10.0.11`
- `EntityFramworkCoreVersion`: `8.0.2` → `10.0.11`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0` (security vulnerability fix)
- `System.Text.Json`: `8.0.3` → `10.0.11`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
- Removed `System.Security.Claims` (functionality included in net10.0 framework)

## Affected Projects
All 10 projects in the solution inherit net10.0 from Directory.Packages.props.
