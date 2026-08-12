# Progress Details: 03-api-fixes

## Changes Made

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
  - This overload was removed from .NET 10 (Api.0002: source incompatible)

### tests/PublicApiIntegrationTests/ProgramTest.cs
- Changed `WebApplicationFactory<Program>` to `WebApplicationFactory<MappingProfile>`
  - .NET 10 changed auto-generated `Program` class visibility for top-level statement programs to public
  - This created a CS0433 ambiguity between `Program` in PublicApi and `Program` in Web assemblies
  - Fix: use `MappingProfile` (unique to PublicApi's `Microsoft.eShopWeb.PublicApi` namespace) as the generic type anchor

## Notes on Binary Incompatible APIs
- `ConfigurationBinder.Get<T>()` and `Configure<T>(IConfiguration)` usages (Api.0001) compiled cleanly
  after recompilation against the updated packages — no source changes were needed for these items.
- `TimeSpan.FromMinutes(double)` compiled cleanly — the overload resolution was not ambiguous in practice.
