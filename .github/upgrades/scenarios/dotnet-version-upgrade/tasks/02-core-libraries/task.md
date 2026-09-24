# 02-core-libraries: Upgrade ApplicationCore, BlazorShared, and Infrastructure to net10.0

## Objective
Bump package versions for `src/ApplicationCore/ApplicationCore.csproj`,
`src/BlazorShared/BlazorShared.csproj`, and `src/Infrastructure/Infrastructure.csproj` to
.NET 10-compatible versions, remove now-redundant explicit `PackageReference`s, and fix the one
source-incompatible API usage. TFM is already `net10.0` via the centrally managed
`Directory.Packages.props` property (task 01).

## Research Findings (from assessment + `query_dotnet_assessment` + `get_supported_package_version`)

### ApplicationCore.csproj
- 620/622 APIs compatible; 2 source-incompatible occurrences — both are the same line:
  `EmptyBasketOnCheckoutException.cs:12` — `: base(info, context)` calling the obsolete
  `System.Exception.#ctor(SerializationInfo, StreamingContext)` binary-serialization
  constructor (`SYSLIB0051`, obsolete since .NET 8, tightening further in .NET 10). **Fix**:
  remove the protected serialization constructor entirely — this is the recommended modern
  pattern since binary formatter-based exception serialization is obsolete/unsupported; no
  code elsewhere in the solution catches/serializes this exception type via `BinaryFormatter`.
- `System.Security.Claims` 4.3.0 — NuGet.0003 "functionality included with framework
  reference" (Mandatory) → **remove the explicit `PackageReference`** (matches NU1510 warning
  from task 01's restore).
- `System.Text.Json` 8.0.3 → **10.0.12** (confirmed via `get_supported_package_version`) —
  also has a known high-severity vulnerability at 8.0.3 (GHSA-8g4q-xg66-9fp4, GHSA-hh2w-p6rv-4g7w).
  Bumping the central `PackageVersion` to 10.0.12 resolves both NuGet.0002 and NuGet.0004.
  Since `System.Text.Json` 10.x ships as part of the net10.0 shared framework, also consider
  whether the explicit `PackageReference` is still needed — NU1510 was NOT raised for this
  package in task 01's restore output, so keep the explicit reference but at version 10.0.12.

### Infrastructure.csproj
- 0 API issues — no code changes needed, package-only task.
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 8.0.2 → **10.0.12** (confirmed).
- `Microsoft.EntityFrameworkCore.InMemory` 8.0.2 → **10.0.12** (confirmed).
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.2 → **10.0.12** (confirmed).
- `System.IdentityModel.Tokens.Jwt` 7.3.1 — assessment flags "deprecated" but
  `get_supported_package_version` confirms **8.23.0** is the latest supported stable version for
  net10.0 (package is still maintained, just has a newer major line) — bump version rather than
  replace the package/API surface (used for JWT handling in Infrastructure/Web/PublicApi).

### BlazorShared.csproj
- 0 package issues, 0 API issues — TFM bump (already done centrally) is sufficient. No further
  changes needed for this project.

### Central version properties in `Directory.Packages.props`
- `AspNetVersion` (8.0.2) drives many `Microsoft.AspNetCore.*`/`Microsoft.Extensions.Identity.*`
  packages used across Infrastructure/PublicApi/Web/BlazorAdmin → bump to **10.0.12**.
- `EntityFramworkCoreVersion` (8.0.2) drives the three EF Core packages above → bump to **10.0.12**.
- `SystemExtensionVersion` (8.0.0) drives `System.Net.Http.Json` (BlazorAdmin) and
  `Microsoft.Extensions.Logging.Configuration` (BlazorAdmin) → bump to **10.0.12** (per aggregate
  packages table).
- `VSCodeGeneratorVersion` (8.0.0) drives `Microsoft.VisualStudio.Web.CodeGeneration.Design` →
  assessment suggests 10.0.2 (design-time tool, versioned independently of runtime) — will bump
  in task 03 since that package is only referenced by PublicApi/Web, not the core libraries. No
  change needed in this task.

## Plan
1. In `Directory.Packages.props`: bump `AspNetVersion` → `10.0.12`, `EntityFramworkCoreVersion` →
   `10.0.12`; bump `System.Text.Json` `PackageVersion` → `10.0.12`; bump
   `System.IdentityModel.Tokens.Jwt` `PackageVersion` → `8.23.0`. (Leave `SystemExtensionVersion`
   and `VSCodeGeneratorVersion` for task 03 since they don't affect these three projects except
   where noted.)
2. In `src/ApplicationCore/ApplicationCore.csproj`: remove the `<PackageReference
   Include="System.Security.Claims" />` line (functionality now in framework).
3. In `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: remove the obsolete
   serialization constructor.
4. Restore + build the three projects; fix any new warnings.

## Done When
- ApplicationCore, BlazorShared, and Infrastructure all build with 0 errors and 0 warnings on
  net10.0.
- No source-incompatible API usages remain in these three projects.
- Central package versions for EF Core / ASP.NET Core Identity / System.Text.Json /
  System.IdentityModel.Tokens.Jwt are bumped to .NET 10-compatible versions.
