# Progress Details: 03-fix-api-breaks

## Summary

Fixed breaking API changes identified by the upgrade assessment.

## Changes Made

### src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed the obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
- This constructor implemented the obsolete formatter-based serialization API (SYSLIB0051)
- In .NET 9+, custom serialization via `ISerializable` is no longer supported for exceptions
- The constructor served no functional purpose in modern .NET

## API Issues Status

| Issue | Type | File | Resolution |
|-------|------|------|------------|
| `Exception(SerializationInfo, StreamingContext)` | Api.0002 Source incompatible | ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs | **Fixed** — obsolete constructor removed |
| `ConfigurationBinder.Get<T>`, `Configure<T>` | Api.0001 Binary incompatible | PublicApi/Program.cs, Web/Configuration/*.cs | **Resolved by recompilation** — APIs still present in net10.0, old binaries incompatible but source compiles |
| `TimeSpan.FromMinutes(double)` | Api.0002 Source incompatible | Web/Configuration/ConfigureCookieSettings.cs | **Resolved** — overload still available in net10.0, compiles without changes |
| `HttpContent.ReadAsStringAsync()` (×2) | Api.0003 Behavioral change | Web/HealthChecks/*.cs | **No code change needed** — behavioral change doesn't affect test logic |
| `AddConsole()` | Api.0003 Behavioral change | PublicApi/Program.cs | **No code change needed** — logging behavior difference is non-breaking |
| `UseExceptionHandler(string path)` | Api.0003 Behavioral change | Web/Program.cs | **No code change needed** — middleware still works, behavior acceptable |

## Build Result
- 0 errors, 20 warnings (all pre-existing transitive dependency security warnings and SDK-level NU1510 warnings)
- SYSLIB0051 warning eliminated
