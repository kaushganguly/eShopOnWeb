# Progress Details: 03-fix-api-breaking-changes

## Changes Made

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed obsolete protected serialization constructor `Exception(SerializationInfo, StreamingContext)` (obsolete in .NET 9+)

### src/Web/Configuration/ConfigureCookieSettings.cs
- Fixed `TimeSpan.FromMinutes(ValidityMinutesPeriod)` → `TimeSpan.FromMinutes((double)ValidityMinutesPeriod)` to resolve CS0121 ambiguity between `FromMinutes(double)` and `FromMinutes(long)` overloads in .NET 9+
