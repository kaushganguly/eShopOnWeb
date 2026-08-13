# 03-fix-api-breaking-changes: Fix source-incompatible API breaking changes

## Objective
Fix source-incompatible API changes between .NET 8 and .NET 10.

## Issues to Fix

### 1. EmptyBasketOnCheckoutException.cs - Obsolete serialization constructor
The protected constructor `Exception(SerializationInfo, StreamingContext)` is obsolete in .NET 9+.
Remove the obsolete constructor.
File: `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`

### 2. ConfigureCookieSettings.cs - TimeSpan.FromMinutes ambiguity
In .NET 9+, `TimeSpan.FromMinutes(long)` overload was added, causing CS0121 ambiguity when passing a `const int`.
Fix: cast to `double` explicitly or use `(double)ValidityMinutesPeriod`.
File: `src/Web/Configuration/ConfigureCookieSettings.cs` line 26

### 3. ConfigureWebServices.cs - Configure<T>(IConfiguration) binary incompatible
`services.Configure<CatalogSettings>(configuration)` - binary incompatible, needs recompile. May not need source changes.
File: `src/Web/Configuration/ConfigureWebServices.cs` line 16

### 4. ConfigureCoreServices.cs - Get<T>() returns nullable
`configuration.Get<CatalogSettings>()` - already has null-coalescing operator, should be fine.
File: `src/Web/Configuration/ConfigureCoreServices.cs` line 22

### 5. PublicApi/Program.cs - Multiple Configure<T> and Get<T> calls
These are flagged as binary incompatible (need recompile, not source changes).
Files: `src/PublicApi/Program.cs`
