# Progress Details: 02-core-libraries

## Changes Made
1. `Directory.Packages.props`:
   - `AspNetVersion` 8.0.2 → 10.0.12
   - `EntityFramworkCoreVersion` 8.0.2 → 10.0.12
   - `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.23.0
   - Removed unused central `<PackageVersion Include="System.Security.Claims" ...>` entry
     (package no longer referenced by any project).
   - `System.Text.Json` `PackageVersion` bumped to 10.0.12, then the entry was **removed
     entirely** after confirming the explicit `PackageReference` was redundant on net10.0
     (NU1510) and no other project references it.
2. `src/ApplicationCore/ApplicationCore.csproj`:
   - Removed `<PackageReference Include="System.Security.Claims" />` (functionality now in
     framework reference — NuGet.0003).
   - Removed `<PackageReference Include="System.Text.Json" />` (functionality now in framework
     reference on net10.0 — NU1510 raised during build).
3. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`:
   - Removed the obsolete binary-serialization constructor
     `protected EmptyBasketOnCheckoutException(SerializationInfo info, StreamingContext context)
     : base(info, context)` — this called `System.Exception`'s obsolete serialization
     constructor (SYSLIB0051, source-incompatible per assessment Api.0002). No other code in
     the solution serializes this exception via `BinaryFormatter`/`ISerializable`, so removal
     is safe and matches Microsoft's recommended modernization path.

## Validation
- `dotnet build src/ApplicationCore/ApplicationCore.csproj` → **0 warnings, 0 errors**, output
  in `bin/Debug/net10.0/`.
- `dotnet build src/BlazorShared/BlazorShared.csproj` → **0 warnings, 0 errors**.
- `dotnet build src/Infrastructure/Infrastructure.csproj` → **0 warnings, 0 errors**.
- Confirmed via `get_supported_package_version` that 10.0.12 is the latest supported version of
  `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer`,
  `Microsoft.EntityFrameworkCore.InMemory`, `Microsoft.EntityFrameworkCore.Tools`, and
  `System.Text.Json` for net10.0, and 8.23.0 for `System.IdentityModel.Tokens.Jwt`.
- No remaining source-incompatible or binary-incompatible API usages in these three projects
  (verified via `query_dotnet_assessment` before the fix — only 1 file/2 occurrences existed,
  both resolved by the single constructor removal).

## Notes
- `SystemExtensionVersion` and `VSCodeGeneratorVersion` central properties were intentionally
  left unchanged in this task — they only affect BlazorAdmin/PublicApi/Web, which are in scope
  for task 03.
- Downstream projects (Web, PublicApi, BlazorAdmin, all four test projects) will pick up the
  `AspNetVersion`/`EntityFramworkCoreVersion` bumps automatically once they restore against the
  updated `Directory.Packages.props` in tasks 03/04.
