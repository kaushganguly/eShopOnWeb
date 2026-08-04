# Modernization Summary: 001-upgrade-dotnet-to-net10

## finalStatus
success

## successCriteriaStatus
- passBuild: true
- passUnitTests: true

## summary
Upgraded all 10 projects in the eShopOnWeb solution from .NET 8.0 to .NET 10.0 (latest LTS).

### Key changes:
- **global.json**: SDK version updated from `8.0.x` to `10.0.x`
- **Directory.Packages.props**: `TargetFramework` changed from `net8.0` to `net10.0`
- **NuGet packages updated**:
  - `Microsoft.AspNetCore.*` / `Microsoft.EntityFrameworkCore.*`: `8.0.2` → `10.0.10`
  - `Azure.Identity`: `1.10.4` → `1.21.0`
  - `System.IdentityModel.Tokens.Jwt`: `7.3.1` → `8.22.0`
  - `AutoMapper`: Migrated from deprecated `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` → `AutoMapper 16.2.0`
  - `xunit`: `2.7.0` → `2.9.3`
  - `MSTest`: `3.2.2` → `3.7.3`
- **Removed packages**: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10.0 support), `System.Security.Claims` (included in framework)
- **API breaking changes resolved**:
  - Removed obsolete `Exception(SerializationInfo, StreamingContext)` constructor
  - Updated `AddAutoMapper()` call for AutoMapper 16.x API
  - Fixed `Program` class ambiguity in `ProgramTest.cs`
- **Security**: Pinned `NuGet.Packaging/Protocol` to `6.14.3` (vulnerability fix)

### Build result: 0 errors, 0 warnings
### Unit test result: 44/44 passed
