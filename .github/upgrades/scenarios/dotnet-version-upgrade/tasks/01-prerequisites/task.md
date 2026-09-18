# Prerequisites & SDK Verification

## Objective
Verify .NET 10.0 SDK is available and validate global.json compatibility before proceeding with project upgrades.

## Scope
- Check .NET SDK installation
- Validate global.json for net10.0 compatibility
- Verify build tooling prerequisites

## Steps

1. **Verify .NET 10.0 SDK Installation**
   - Run: `dotnet --version` to confirm current SDK
   - Run: `dotnet sdk list` to see all available SDKs
   - Ensure .NET 10.0 SDK (or compatible version) is installed

2. **Check global.json**
   - Verify global.json exists in repo root
   - Check if it specifies an SDK version constraint
   - Update if needed to allow net10.0 SDK

3. **Verify Build Tooling**
   - Run: `dotnet --info` to check .NET runtime and SDK
   - Confirm MSBuild is accessible via dotnet CLI
   - Check NuGet configuration is correct

4. **Solution Restore Test**
   - Run: `dotnet restore eShopOnWeb.sln` to verify NuGet restore works
   - Verify no critical errors in restore output

## Done When
- ✅ .NET 10.0 SDK confirmed available
- ✅ global.json validated or updated for net10.0
- ✅ Build tooling prerequisites verified
- ✅ Solution restore succeeds without critical errors
