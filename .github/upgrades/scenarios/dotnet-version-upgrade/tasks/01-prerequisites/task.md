# 01-prerequisites: Verify SDK Compatibility and Update global.json

## Objective
Verify .NET 10 SDK is available and update `global.json` to allow the .NET 10 SDK for the upgrade.

## Scope
- Check installed .NET SDK versions
- Update `global.json` at repo root to reference .NET 10 SDK
- Verify the solution restores cleanly with the .NET 10 SDK

## Research Findings

### Installed .NET SDKs
| SDK Version | Path |
|---|---|
| 8.0.129 | /usr/share/dotnet/sdk |
| 8.0.206 | /usr/share/dotnet/sdk |
| 8.0.319 | /usr/share/dotnet/sdk |
| 8.0.423 | /usr/share/dotnet/sdk |
| 9.0.119 | /usr/share/dotnet/sdk |
| 9.0.205 | /usr/share/dotnet/sdk |
| 9.0.316 | /usr/share/dotnet/sdk |
| 10.0.110 | /usr/share/dotnet/sdk |
| 10.0.204 | /usr/share/dotnet/sdk |
| **10.0.302** | /usr/share/dotnet/sdk ← **highest .NET 10** |

### global.json (before)
```json
{
  "sdk": {
    "version": "8.0.x",
    "rollForward": "latestFeature"
  }
}
```

### global.json (after)
```json
{
  "sdk": {
    "version": "10.0.302",
    "rollForward": "latestMinor"
  }
}
```

`rollForward: latestMinor` allows any .NET 10 minor release ≥ 10.0.302, which is the right policy for a version upgrade scenario.

## Files Modified
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`

