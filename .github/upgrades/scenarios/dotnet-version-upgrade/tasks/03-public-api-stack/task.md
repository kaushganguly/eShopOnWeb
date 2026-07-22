# 03-public-api-stack: Upgrade API Service Stack to net10.0

## Objective
Upgrade ApplicationCore, Infrastructure, PublicApi, and PublicApiIntegrationTests projects to target net10.0 as part of the .NET 10 migration.

## Scope
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/PublicApi/PublicApi.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
- `Directory.Packages.props` (shared package version updates)

## Key Changes

### TFM Upgrades
- Added `<TargetFramework>net10.0</TargetFramework>` to all 4 project files.
- ApplicationCore, Infrastructure, PublicApi, and PublicApiIntegrationTests now target net10.0.

### Package Removals
- `System.Security.Claims` removed from ApplicationCore.csproj — built into .NET 5+ framework.
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` removed from PublicApi.csproj — incompatible with net10.0.

### Source Incompatibilities Fixed
- Removed `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor from ApplicationCore — obsolete binary serialization pattern (SYSLIB0051).

### AutoMapper Migration (12.0.1 → 16.2.0)
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` with `AutoMapper 16.2.0` in PublicApi.
- In AutoMapper 13+, DI support was merged into the main package; `AddAutoMapper` no longer takes an `Assembly` directly.
- Updated `Program.cs`: `AddAutoMapper(typeof(MappingProfile).Assembly)` → `AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))`.
- Used `VersionOverride="16.2.0"` in PublicApi.csproj to avoid conflicting with Web project's AutoMapper.Extensions 12.0.1.
- `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` retained in Directory.Packages.props for Web project (task 04).

### Security Package Updates
| Package | Old Version | New Version | Reason |
|---------|-------------|-------------|--------|
| `System.Text.Json` | 8.0.3 | 9.0.0 | Vulnerable version |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | 8.20.0 | JWT security update |
| `Microsoft.IdentityModel.Protocols` | (transitive) | 8.20.0 | Version alignment with JWT 8.x |
| `Microsoft.IdentityModel.Protocols.OpenIdConnect` | (transitive) | 8.20.0 | Version alignment with JWT 8.x |
| `Azure.Identity` | 1.10.4 | 1.21.0 | Moderate severity CVEs (GHSA-m5vv-6r4h-3vj9, GHSA-wvxc-855f-jvrv) |
| `Microsoft.Extensions.Caching.Memory` | (transitive 8.0.0) | 9.0.18 | High severity CVE (GHSA-qj66-m88j-hmgj) |
| `NuGet.Packaging` | (transitive 6.3.1) | 6.14.3 | Critical CVE GHSA-68w7-72jg-6qpp via CodeGeneration.Design |
| `NuGet.Protocol` | (transitive 6.3.1) | 6.14.3 | High CVE GHSA-6qmf-mmc7-6c2p via CodeGeneration.Design |

### Package Version Pins (Infrastructure.csproj)
Added explicit references to pin transitive dependency versions:
- `Azure.Identity` 1.21.0
- `Microsoft.Extensions.Caching.Memory` 9.0.18
- `Microsoft.IdentityModel.Protocols` 8.20.0
- `Microsoft.IdentityModel.Protocols.OpenIdConnect` 8.20.0

### CodeGeneration.Design Upgrade
- PublicApi.csproj uses `VersionOverride="10.0.2"` for `Microsoft.VisualStudio.Web.CodeGeneration.Design`.
- Global version kept at `8.0.0` (via `$(VSCodeGeneratorVersion)`) for Web project compatibility.

## Build Results
| Project | Errors | Warnings |
|---------|--------|----------|
| ApplicationCore | 0 | 0 |
| Infrastructure | 0 | 0 |
| PublicApi | 0 | 0 |
| PublicApiIntegrationTests | N/A (pre-existing failure: Web still net8.0) | - |

## Test Results
- PublicApiIntegrationTests cannot be run in isolation because it references Web.csproj which still targets net8.0 and cannot reference net10.0 dependencies (ApplicationCore, BlazorShared, Infrastructure).
- This is a pre-existing condition from task 02 (BlazorShared/BlazorAdmin upgraded to net10.0).
- Integration tests will be runnable after task 04 upgrades Web to net10.0.

## Done Criteria Status
- ✅ ApplicationCore targets net10.0
- ✅ Infrastructure targets net10.0
- ✅ PublicApi targets net10.0
- ✅ PublicApiIntegrationTests targets net10.0
- ✅ Incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` removed
- ✅ AutoMapper updated (16.2.0)
- ✅ JWT updated (8.20.0)
- ✅ Security findings resolved
- ✅ Serialization constructor removed
- ⚠️ Integration tests pending until task 04 (Web upgrade)
