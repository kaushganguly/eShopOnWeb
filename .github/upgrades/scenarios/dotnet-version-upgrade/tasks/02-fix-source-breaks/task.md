# 02-fix-source-breaks: Fix source-incompatible API usages

## Objective
Verify and document all source-incompatible API usages that were identified in the assessment.

## Assessment Findings

### Completed During Task 01
The following source-incompatible issues were identified by the assessment and fixed:

1. **SYSLIB0051** - `EmptyBasketOnCheckoutException.cs`: Removed obsolete `(SerializationInfo, StreamingContext)` constructor. This constructor was marked obsolete with SYSLIB0051 and removed from the recommended patterns for .NET 10.

2. **CS0433 Program ambiguity** - `PublicApiIntegrationTests`: Both PublicApi and Web expose an internal `Program` class (from top-level statements). Fixed with extern alias on Web project reference.

3. **xUnit2013 analyzer warnings** - Fixed 4 pre-existing test assertion warnings.

4. **AutoMapper API update** - `AutoMapper.AddAutoMapper(Assembly)` signature changed in 16.x. Updated to use `cfg.AddMaps(Assembly)`.

### Api.0001 Binary Incompatible Issues
The assessment flagged several `ConfigurationBinder.Get<T>()`, `Configure<T>(IServiceCollection, IConfiguration)`, and `ConfigurationBinder.GetValue(...)` calls as binary incompatible. These resolved automatically after recompilation against net10.0 packages — the new package versions provide updated, compatible API implementations.

## Build Status
✅ Build succeeded — 0 errors, 0 compiler warnings
