# Progress Details — 02-public-api-slice

## Summary

The PublicApi slice (ApplicationCore → Infrastructure → PublicApi) already compiled against `net10.0`
(inherited from `Directory.Packages.props`) without any source errors. Three warnings and one compile
error were resolved; the remaining warnings are deferred transitive-package security advisories that
require larger package migrations (AutoMapper 13 migration) covered in future tasks.

---

## Changes Made

### 1. `src/ApplicationCore/ApplicationCore.csproj`

**What changed:** Removed two `<PackageReference>` entries that became unnecessary (inbox) on net10.0.

```diff
-   <PackageReference Include="System.Security.Claims" />
-   <PackageReference Include="System.Text.Json" />
+   <!-- System.Security.Claims and System.Text.Json are inbox on net10.0; removed per NU1510 -->
```

**Why:** Both packages are part of the .NET 10 framework; keeping explicit references produces
`NU1510` ("will not be pruned") warnings on every build.

---

### 2. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`

**What changed:** Removed the obsolete serialization constructor.

```diff
-   protected EmptyBasketOnCheckoutException(
-       System.Runtime.Serialization.SerializationInfo info,
-       System.Runtime.Serialization.StreamingContext context) : base(info, context)
-   {
-   }
```

**Why:** `Exception(SerializationInfo, StreamingContext)` is decorated with `[Obsolete]` (SYSLIB0051)
in .NET 5+ and will be removed in a future version. BinaryFormatter-based serialization of exceptions
is not used in this codebase. Removing the constructor eliminates the `SYSLIB0051` warning and moves
the class toward modern patterns.

---

### 3. `tests/PublicApiIntegrationTests/ProgramTest.cs`

**What changed:** Replaced `WebApplicationFactory<Program>` with
`WebApplicationFactory<MappingProfile>`.

```diff
+using Microsoft.eShopWeb.PublicApi;
 ...
-private static WebApplicationFactory<Program> _application = new();
+private static WebApplicationFactory<MappingProfile> _application = new();
 ...
-    _application = new WebApplicationFactory<Program>();
+    _application = new WebApplicationFactory<MappingProfile>();
```

**Why:** Both `PublicApi` and `Web` assemblies use top-level statements, which means both expose
an implicit `Program` class. The test project references both assemblies, causing
`CS0433: The type 'Program' exists in both 'PublicApi' and 'Web'`.

Using `MappingProfile` (a `public` type in `Microsoft.eShopWeb.PublicApi`) as the anchor type
unambiguously targets the PublicApi entry-point assembly. `WebApplicationFactory<T>` only needs
`T` to identify the assembly — any public type from the assembly works.

---

## Build Verification

| Project | Target | Errors | Actionable Warnings |
|---|---|---|---|
| ApplicationCore | net10.0 | 0 | 0 |
| Infrastructure | net10.0 | 0 | 0 |
| PublicApi | net10.0 | 0 | 0 (3 deferred NU1901/NU1903) |
| PublicApiIntegrationTests | net10.0 | 0 | 0 (deferred, same as PublicApi) |

---

## Test Verification

```
Passed! - Failed: 0, Passed: 15, Skipped: 0, Total: 15
```

All 15 integration tests pass (auth endpoint, catalog item CRUD, parallel-call stress tests).

---

## Deferred Follow-up Items

| Item | Tracking |
|---|---|
| **AutoMapper 12.0.1 HIGH vulnerability** (GHSA-rvv3-g6hj-g44x) — requires migrating from `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 to AutoMapper 13+ with built-in DI | Future task (Blazor/Web slice or dedicated package-hygiene task) |
| **NuGet.Packaging/Protocol 6.12.1 LOW vulnerability** (GHSA-g4vj-cjjj-v7hg) — transitive from `Microsoft.VisualStudio.Web.CodeGeneration.Design` / EF tooling | Same future task |
| **`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6** — assessment marks as package-incompatible; does not block build; no replacement version available yet | Keep until a compatible release; revisit in container publishing task |
