# Progress Details — Task 02 (API Breaking Changes)

## Files Modified

1. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`
   - Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo info, StreamingContext context)` constructor
   - This constructor was removed from `System.Exception` in .NET 9+

2. `src/Web/Configuration/ConfigureCookieSettings.cs`
   - Changed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((long)ValidityMinutesPeriod)`
   - `TimeSpan.FromMinutes(double)` is now obsolete-as-error in .NET 10; use `long` overload

3. `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`
   - Added `<Aliases>global,publicapi</Aliases>` to PublicApi project reference
   - Needed because .NET 9+ made implicit `Program` class `public`, causing CS0433 ambiguity

4. `tests/PublicApiIntegrationTests/ProgramTest.cs`
   - Added `extern alias publicapi;`
   - Changed `WebApplicationFactory<Program>` → `WebApplicationFactory<publicapi::Program>`
   - Disambiguates the `Program` type between `PublicApi` and `Web` assemblies
