# Task 03: Resolve binary and source-incompatible API changes

## Changes Made

### ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs
- Removed the `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
- This constructor invokes `Exception.Exception(SerializationInfo, StreamingContext)` which is
  flagged as SYSLIB0051 (obsolete in .NET 8, removed/more-breaking in .NET 10)
- The serialization constructor is not needed for modern .NET exception handling

### src/PublicApi/Program.cs
- Changed `builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly)` to
  `builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>())`
- AutoMapper 16.x removed the Assembly overload for AddAutoMapper
- Only one Profile class (MappingProfile) exists in the assembly

### src/PublicApi/PublicApiTestAnchor.cs (new file)
- Added `public class PublicApiTestAnchor` in `Microsoft.eShopWeb.PublicApi` namespace
- Resolves CS0433 ambiguity: in .NET 10, both PublicApi and Web expose a `Program` type
  (PublicApi via `public partial class Program {}`, Web via top-level statements)
- WebApplicationFactory<T> only needs T to be in the target assembly — PublicApiTestAnchor
  serves as the assembly anchor without the naming conflict

### tests/PublicApiIntegrationTests/ProgramTest.cs
- Changed `WebApplicationFactory<Program>` to `WebApplicationFactory<PublicApiTestAnchor>`
- Added `using Microsoft.eShopWeb.PublicApi;`
- Resolves CS0433 compilation error in .NET 10

## Tests Affected
- All tests still pass after these changes
- The PublicApiTestAnchor correctly identifies the PublicApi assembly for WebApplicationFactory
