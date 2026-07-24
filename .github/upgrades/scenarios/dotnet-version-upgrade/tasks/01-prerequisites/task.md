# 01-prerequisites: Verify SDK and Update global.json

## Objective
Confirm the .NET 10 SDK is available and update `global.json` so the build toolchain resolves to .NET 10 before any project TFMs are changed.

## Research Findings

### Available .NET SDKs
| SDK Version | Location |
|---|---|
| 8.0.129 | /usr/share/dotnet/sdk |
| 8.0.206 | /usr/share/dotnet/sdk |
| 8.0.319 | /usr/share/dotnet/sdk |
| 8.0.423 | /usr/share/dotnet/sdk |
| 9.0.119 | /usr/share/dotnet/sdk |
| 9.0.205 | /usr/share/dotnet/sdk |
| 9.0.316 | /usr/share/dotnet/sdk |
| **10.0.110** | /usr/share/dotnet/sdk |
| **10.0.204** | /usr/share/dotnet/sdk |
| **10.0.302** | /usr/share/dotnet/sdk ✅ (selected by rollForward) |

### Original global.json
```json
{
  "sdk": {
    "version": "8.0.x",
    "rollForward": "latestFeature"
  }
}
```

### Files Affected
- `global.json` — SDK version constraint updated from `8.0.x` to `10.0.100` (latestFeature selects 10.0.302)

### CPM Notes
`Directory.Packages.props` uses version variables (`AspNetVersion`, `EntityFramworkCoreVersion`, `SystemExtensionVersion`, `VSCodeGeneratorVersion`). These will be updated in task 02 (update package versions).

## Outcome
- `dotnet --version` → `10.0.302` ✅
- `dotnet restore eShopOnWeb.sln` → exit 0 ✅ (warnings are pre-existing CVEs, not SDK errors)

