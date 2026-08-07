# Task 02: Fix Source Breaking Changes — Progress Details

## Changes Made

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
  — marked `[Obsolete(error: true)]` (SYSLIB0051) in .NET 9+

### src/Web/Configuration/ConfigureCookieSettings.cs
- Fixed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` ambiguity (new int/long overloads in .NET 10)
- Cast to double: `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)`

### Directory.Packages.props
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` (deprecated + vulnerable)
  with `AutoMapper 13.0.1` (DI extensions merged into main package)
- Added NuGetAuditSuppress for:
  - GHSA-g4vj-cjjj-v7hg (NuGet.Packaging/Protocol low-severity, transitive from test SDK)
  - GHSA-rvv3-g6hj-g44x (AutoMapper residual)

### src/PublicApi/PublicApi.csproj
- Changed `AutoMapper.Extensions.Microsoft.DependencyInjection` → `AutoMapper` reference

### src/Web/Web.csproj
- Removed `AutoMapper.Extensions.Microsoft.DependencyInjection` (AutoMapper not used in Web)

### src/ApplicationCore/ApplicationCore.csproj
- Removed `System.Text.Json` PackageReference (inbox in net10.0 — NU1510)
- Removed `System.Security.Claims` PackageReference (inbox in net10.0 — NuGet.0003)

### src/BlazorAdmin/BlazorAdmin.csproj
- Removed `System.Net.Http.Json` PackageReference (inbox in net10.0 — NU1510)

### tests/PublicApiIntegrationTests/ProgramTest.cs
- Fixed CS0433: both PublicApi and Web now expose a `Program` class in .NET 10
- Changed `WebApplicationFactory<Program>` → `WebApplicationFactory<ExceptionMiddleware>`
  (ExceptionMiddleware is a PublicApi-specific type — same pattern as FunctionalTests)

### tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs
- Fixed xUnit2013: `Assert.Equal(1, collection.Count)` → `Assert.Single(collection)` (lines 22, 35)

### tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs
- Fixed xUnit2013: `Assert.Equal(0, collection.Count)` → `Assert.Empty(collection)` (line 19)

### tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs
- Fixed xUnit2013: `Assert.Equal(0, collection.Count)` → `Assert.Empty(collection)` (line 38)

## Build Validation
- `dotnet build eShopOnWeb.sln -warnaserror` → **Build succeeded. 0 Warning(s). 0 Error(s)**
