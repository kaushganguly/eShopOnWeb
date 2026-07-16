## Files Modified
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — Removed obsolete SerializationInfo/StreamingContext constructor (SYSLIB0051)
- `src/ApplicationCore/ApplicationCore.csproj` — Removed System.Text.Json package ref (included in .NET 10 platform)
- `src/BlazorAdmin/BlazorAdmin.csproj` — Removed System.Net.Http.Json package ref (included in .NET 10 WASM)
- `src/Web/Web.csproj` — Removed unused AutoMapper.Extensions.Microsoft.DependencyInjection; added PrivateAssets=all to CodeGeneration.Design; added NuGet.Packaging/Protocol 6.12.5 overrides
- `Directory.Packages.props` — Added NuGet.Packaging 6.12.5 and NuGet.Protocol 6.12.5 entries
- `tests/UnitTests/UnitTests.Tests/CustomerOrdersWithItemsSpecification.cs` — Fixed xUnit2013: Assert.Equal(1, x.Count) → Assert.Single(x)
- `tests/UnitTests/UnitTests.Tests/BasketRemoveEmptyItems.cs` — Fixed xUnit2013: Assert.Equal(0, x.Count) → Assert.Empty(x)

## Build Result
- Errors: 0
- Warnings: 0
- Projects built: Web, ApplicationCore, Infrastructure, BlazorShared, UnitTests, FunctionalTests

## Test Result
- Tests run: 56
- Passed: 56
- Failed: 0

## Changes Summary
- Updated Web stack and shared dependencies (ApplicationCore, Infrastructure, BlazorShared) to net10.0
- Fixed deprecated serialization constructor in EmptyBasketOnCheckoutException
- Removed packages now included in the .NET 10 framework
- Fixed AutoMapper vulnerability by removing unused reference from Web.csproj
- Fixed NuGet.Packaging/Protocol vulnerability via version overrides
- Fixed xUnit2013 test warnings

## Issues Encountered
- TFM was already centrally set in Directory.Packages.props from task 01
- AutoMapper 12.0.1 vulnerability: removed unused package reference
- NuGet.Packaging/Protocol vulnerability: overrode transitive versions to 6.12.5
