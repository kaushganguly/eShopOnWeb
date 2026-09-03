# Task 02-fix-source-breaks: Progress Details

## Summary
All source-incompatible issues were identified and fixed (most during task 01).

## Fixes Applied

### 1. SYSLIB0051 - Obsolete Serialization Constructor
**File**: `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`  
**Fix**: Removed `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor. This was obsoleted (SYSLIB0051) in .NET 5+ and causes warnings in .NET 10.

### 2. CS0433 - Program Type Ambiguity
**File**: `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`  
**Fix**: Added `<Aliases>WebApp</Aliases>` to the Web project reference to disambiguate between PublicApi's and Web's generated `Program` classes.

**File**: `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`  
**Fix**: Added `extern alias WebApp` and used alias for Web ViewModels type.

### 3. AutoMapper 16.x API Change
**File**: `src/PublicApi/Program.cs`  
**Fix**: Changed `services.AddAutoMapper(typeof(MappingProfile).Assembly)` to `services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly))` to conform with AutoMapper 16.x API.

### 4. Pre-existing xUnit2013 Warnings
Multiple test files updated to use proper xUnit assertion methods:
- `Assert.Equal(1, collection.Count)` → `Assert.Single(collection)`
- `Assert.Equal(0, collection.Count)` → `Assert.Empty(collection)`

## Notes
- Api.0001 (binary incompatible) issues for ConfigurationBinder methods resolved automatically after recompilation with updated net10.0 packages
- Api.0002 (source incompatible) for TimeSpan.FromMinutes - resolved without code change after package update
