# 03-fix-api-breaks: Resolve breaking API changes in source code

## Objective
Fix source-level breaking API changes identified by the assessment after the TFM upgrade.
The build already passes with 0 errors — remaining work is to eliminate the SYSLIB0051 warning
for the removed serialization API and verify behavioral changes (Api.0003) are acceptable.

## Scope
- **ApplicationCore** (Api.0002 source incompatible):
  - `EmptyBasketOnCheckoutException.cs` line 11: `System.Exception(SerializationInfo, StreamingContext)` constructor is obsolete (SYSLIB0051). Fix: remove this obsolete constructor.
- **Binary-incompatible APIs (Api.0001)**: No code changes needed — already resolved by recompilation targeting net10.0.
  - `ConfigurationBinder.Get<T>` and `Configure<T>(IConfiguration)` still compile with .NET 10.
- **Behavioral changes (Api.0003)**: 49 occurrences across projects — compile fine, runtime behavior slightly changed. No code changes needed unless tests fail.

## Done When
- SYSLIB0051 warning eliminated in ApplicationCore
- Build passes with 0 errors and 0 new warnings from modified files
