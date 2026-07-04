# Modernization Summary: 001-upgrade-dotnet-to-net10

## finalStatus
success

## successCriteriaStatus
- passBuild: true
- passUnitTests: true

## summary
Upgraded the entire eShopOnWeb solution from .NET 8.0 to .NET 10.0 (LTS).

### Changes Made

**Central Package Management (`Directory.Packages.props`, `global.json`)**
- `TargetFramework`: `net8.0` → `net10.0`
- `AspNetVersion` (Microsoft.AspNetCore.*): `8.0.2` → `10.0.9`
- `EntityFrameworkCoreVersion`: `8.0.2` → `10.0.9`
- `SystemExtensionVersion`: `8.0.0` → `10.0.9`
- `VSCodeGeneratorVersion`: `8.0.0` → `10.0.2`
- `Azure.Identity`: `1.10.4` → `1.21.0`
- `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.0.1`
- `xunit`: `2.7.0` → `2.9.3`
- SDK pin (`global.json`): `8.0.x` → `10.0.100`

**Source Code Fixes**
- ApplicationCore: Removed obsolete `Exception(SerializationInfo, StreamingContext)` constructor
- BlazorAdmin: Fixed Razor component elements in code blocks; self-closed `<img>` void elements
- Web: Fixed `<section>` → `<div>` in `_LoginPartial.cshtml`; fixed `<script>` in `@if` block; fixed `ShowRecoverCodes.cshtml` markup
- Tests: Fixed `WebApplicationFactory<Program>` ambiguity; fixed xUnit2013 assertions; removed legacy `DotNetCliToolReference`

### Test Results
| Suite | Tests | Passed | Failed |
|-------|-------|--------|--------|
| UnitTests | 44 | 44 | 0 |
| IntegrationTests | 3 | 3 | 0 |
| FunctionalTests | 12 | 12 | 0 |
| PublicApiIntegrationTests | 15 | 15 | 0 |
| **Total** | **74** | **74** | **0** |
