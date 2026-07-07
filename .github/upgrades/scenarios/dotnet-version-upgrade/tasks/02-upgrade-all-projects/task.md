# 02-upgrade-all-projects: Upgrade all projects to .NET 10

## Objective
Upgrade all 10 projects from net8.0 to net10.0 via centralized package management.

## Strategy
All-at-once: Single `Directory.Packages.props` controls TFM and package versions for all projects.

## Scope
All 10 projects:
- **Source**: BlazorShared, ApplicationCore, Infrastructure, Web, PublicApi, BlazorAdmin
- **Tests**: UnitTests, FunctionalTests, IntegrationTests, PublicApiIntegrationTests

## Files Changed

### Directory.Packages.props (central)
- `TargetFramework`: net8.0 → net10.0
- `AspNetVersion`: 8.0.2 → 10.0.9
- `SystemExtensionVersion`: 8.0.0 → 10.0.9
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.9
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0 (security fix)
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.19.1 (deprecated)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible, no supported version for net10)
- Removed `System.Security.Claims` (now in framework)
- Removed `Microsoft.AspNetCore.Mvc 2.2.0` (now in framework, unused)
- Removed `System.Text.Json` PackageVersion (now in framework, NU1510)
- Removed `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` (high severity vulnerability GHSA-rvv3-g6hj-g44x, absorbed into AutoMapper 15+)
- Added `AutoMapper 15.1.3` (fixes vulnerability, DI integration is built-in)
- Added `CentralPackageTransitivePinningEnabled` = true
- Added `NuGet.Packaging 7.6.0` transitive pin (fixes GHSA-g4vj-cjjj-v7hg)
- Added `NuGet.Protocol 7.6.0` transitive pin (fixes GHSA-g4vj-cjjj-v7hg)

### src/PublicApi/PublicApi.csproj
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` PackageReference
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Security.Claims` PackageReference
- Removed `System.Text.Json` PackageReference (NU1510: unnecessary in net10)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (NU1510: unnecessary in net10)

### src/Web/Web.csproj
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper`

### src/PublicApi/Program.cs
- Updated `AddAutoMapper(typeof(MappingProfile).Assembly)` → `AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))`
  AutoMapper 15+ changed the DI API; `AddAutoMapper(Assembly)` removed, replaced with action-based configuration.

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete serialization constructor (SYSLIB0051: `Exception(SerializationInfo, StreamingContext)` obsolete in .NET 9+)

### tests/PublicApiIntegrationTests/ProgramTest.cs
- Changed `WebApplicationFactory<Program>` → `WebApplicationFactory<MappingProfile>`
  Both `Web` and `PublicApi` expose a `Program` class (top-level statements), causing CS0433 ambiguity.
  Using `MappingProfile` (only in PublicApi) as the assembly anchor.

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Fixed xUnit2013: `Assert.Equal(1, coll.Count)` → `Assert.Single(coll)` (x2)

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- Fixed xUnit2013: `Assert.Equal(0, coll.Count)` → `Assert.Empty(coll)`

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Fixed xUnit2013: `Assert.Equal(0, coll.Count)` → `Assert.Empty(coll)`

## Research Findings

### API Breaking Changes
The task-identified breaking changes (`ConfigurationBinder.Get<T>`, `OptionsConfigurationServiceCollectionExtensions.Configure<T>`, `TimeSpan.FromMinutes`) did NOT cause compilation errors. These are "binary incompatible" changes that manifest at runtime in old compiled code, but when recompiled against .NET 10 they compile and work fine.

### AutoMapper Migration (12.0.1 → 15.1.3)
AutoMapper 12-14.x all have GHSA-rvv3-g6hj-g44x (high severity). AutoMapper 15.x fixes it but changed the DI API:
- `AddAutoMapper(Assembly)` and `AddAutoMapper(Type)` overloads were removed
- Replaced with `AddAutoMapper(Action<IMapperConfigurationExpression>)`
- Migration: `services.AddAutoMapper(typeof(T).Assembly)` → `services.AddAutoMapper(cfg => cfg.AddMaps(typeof(T).Assembly))`
- `Profile`, `IMapper`, `CreateMap`, `ForMember`, `MapFrom` APIs unchanged

### Packages Now in Framework (NU1510)
In .NET 10, `System.Net.Http.Json` and `System.Text.Json` are included in the runtime framework. Explicit package references are unnecessary and generate NU1510 warnings.

### System.Security.Claims Removal  
`System.Security.Claims` 4.3.0 is fully integrated into .NET 10 framework. Explicit package reference not needed.

### CentralPackageTransitivePinningEnabled
Used to pin transitive versions of `NuGet.Packaging` and `NuGet.Protocol` to 7.6.0 to fix low-severity vulnerability GHSA-g4vj-cjjj-v7hg that comes transitively from `Microsoft.VisualStudio.Web.CodeGeneration.Design`.

## Build Result
- Errors: 0
- Warnings: 0
- All 10 projects: net10.0 ✓
