# Task 02 Progress Details: Fix API Breaking Changes

## Changes Made

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed deprecated `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
- This constructor was removed from the base `Exception` class in .NET 9+ (breaking change from serialization infra removal)

### tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- Added `<Aliases>WebProject</Aliases>` to the `Web` project reference
- .NET 9+ changed the auto-generated `Program` entry point class visibility from `internal` to `public`
- Since both `PublicApi` and `Web` export a `public partial class Program`, referencing both caused CS0433 ambiguity
- Aliasing the `Web` project reference resolves the conflict

### tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs
- Added `extern alias WebProject;` at top of file
- Changed `using Microsoft.eShopWeb.Web.ViewModels;` to `using WebProject::Microsoft.eShopWeb.Web.ViewModels;`
- Required by the Web project alias change above
