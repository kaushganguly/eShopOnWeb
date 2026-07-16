# Progress Details: 02-foundation-libs

## Status: Completed

## Summary
Upgraded four foundation library projects from net8.0 to net10.0 via Central Package Management updates. Fixed source-incompatible APIs and removed framework-included packages.

## Files Modified

| File | Change |
|------|--------|
| `Directory.Packages.props` | Updated TargetFramework to net10.0; bumped all version variables (AspNetVersion → 10.0.10, SystemExtensionVersion → 10.0.10, EntityFramworkCoreVersion → 10.0.10, VSCodeGeneratorVersion → 10.0.2); Azure.Identity 1.10.4 → 1.21.0; System.Text.Json 8.0.3 → 10.0.10; System.IdentityModel.Tokens.Jwt 7.3.1 → 8.19.2 |
| `src/ApplicationCore/ApplicationCore.csproj` | Removed System.Security.Claims and System.Text.Json references (now in framework) |
| `src/BlazorAdmin/BlazorAdmin.csproj` | Removed System.Net.Http.Json reference (now in framework) |
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Removed obsolete serialization constructor (source incompatible in .NET 10) |

## Build Results

| Project | Target | Errors | Warnings |
|---------|--------|--------|----------|
| BlazorShared | net10.0 | 0 | 0 |
| ApplicationCore | net10.0 | 0 | 0 |
| BlazorAdmin | net10.0 | 0 | 0 |
| Infrastructure | net10.0 | 0 | 0 |

## Issues Resolved

1. **NU1605 (Error): System.IdentityModel.Tokens.Jwt downgrade** — JwtBearer 10.0.10 transitively requires System.IdentityModel.Tokens.Jwt >= 8.19.2 but the CPM had 7.3.1 pinned. Fixed by updating to 8.19.2.
2. **NU1510: System.Text.Json unnecessarily referenced** — Removed from ApplicationCore.csproj since it's included in .NET 10 framework.
3. **NU1510: System.Net.Http.Json unnecessarily referenced** — Removed from BlazorAdmin.csproj since it's included in .NET 10 framework.
4. **NuGet.0003: System.Security.Claims included in framework** — Removed from ApplicationCore.csproj.
5. **Api.0002: Source-incompatible constructor** — Removed obsolete `Exception(SerializationInfo, StreamingContext)` constructor from EmptyBasketOnCheckoutException.
6. **NuGet.0004: Azure.Identity security vulnerability** — Updated to 1.21.0.
