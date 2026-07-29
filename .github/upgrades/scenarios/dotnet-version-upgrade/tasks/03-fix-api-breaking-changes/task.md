# 03-fix-api-breaking-changes: Fix API Breaking Changes (.NET 8 → .NET 10)

## Objective

Fix source and binary-incompatible API usages identified in the upgrade assessment for .NET 10.

---

## Research Findings

### Breaking Change 1 — Serialization Constructor (Api.0002 — Source Incompatible) ✅ FIXED

**File**: `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`

The `protected Exception(SerializationInfo, StreamingContext)` constructor was removed in .NET 9 as part of the removal of `BinaryFormatter` serialization. This is **source incompatible** — the code will not compile.

**Action taken**: Removed the constructor (lines 12-14 of the original file).

---

### Breaking Change 2 — `Configure<T>(IServiceCollection, IConfiguration)` (Api.0001 — Binary Incompatible)

Assessment flags these as binary incompatible. Since this is binary (not source) incompatible, the code may still compile after package updates to 10.x. Deferred to build step (task 04).

**Affected files**:
- `src/Web/Configuration/ConfigureWebServices.cs` line 16: `services.Configure<CatalogSettings>(configuration)`
- `src/PublicApi/Program.cs` line 42: `builder.Services.Configure<CatalogSettings>(builder.Configuration)`
- `src/PublicApi/Program.cs` line 49: `builder.Services.Configure<BaseUrlConfiguration>(configSection)`
- `src/Web/Program.cs` line 98: `builder.Services.Configure<BaseUrlConfiguration>(configSection)`

**If build fails**, replace each with a lambda overload:
```csharp
// e.g., ConfigureWebServices.cs
services.Configure<CatalogSettings>(settings => configuration.Bind(settings));
// e.g., PublicApi/Program.cs line 42
builder.Services.Configure<CatalogSettings>(settings => builder.Configuration.Bind(settings));
// e.g., PublicApi/Program.cs line 49
builder.Services.Configure<BaseUrlConfiguration>(settings => configSection.Bind(settings));
// e.g., Web/Program.cs line 98
builder.Services.Configure<BaseUrlConfiguration>(settings => configSection.Bind(settings));
```

---

### Breaking Change 3 — `ConfigurationBinder.Get<T>(IConfiguration)` (Api.0001 — Binary Incompatible)

Same rationale — binary incompatible, may still compile. Deferred to task 04.

**Affected files**:
- `src/Web/Configuration/ConfigureCoreServices.cs` line 22: `configuration.Get<CatalogSettings>()`
- `src/PublicApi/Program.cs` line 43: `builder.Configuration.Get<CatalogSettings>()`
- `src/PublicApi/Program.cs` line 50: `configSection.Get<BaseUrlConfiguration>()`
- `src/Web/Program.cs` line 99: `configSection.Get<BaseUrlConfiguration>()`

**If build fails**, add binder options:
```csharp
// e.g., ConfigureCoreServices.cs
var catalogSettings = configuration.Get<CatalogSettings>(c => c.ErrorOnUnknownConfiguration = false) ?? new CatalogSettings();
// e.g., PublicApi/Program.cs line 43
var catalogSettings = builder.Configuration.Get<CatalogSettings>(c => c.ErrorOnUnknownConfiguration = false) ?? new CatalogSettings();
// e.g., PublicApi/Program.cs line 50
var baseUrlConfig = configSection.Get<BaseUrlConfiguration>(c => c.ErrorOnUnknownConfiguration = false);
// e.g., Web/Program.cs line 99
var baseUrlConfig = configSection.Get<BaseUrlConfiguration>(c => c.ErrorOnUnknownConfiguration = false);
```

---

## Summary of Actions

| # | File | Action | Status |
|---|------|--------|--------|
| 1 | `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | Removed serialization constructor | ✅ Done |
| 2 | `src/Web/Configuration/ConfigureWebServices.cs` | Deferred — binary compat, verify at build | ⏳ Task 04 |
| 3 | `src/Web/Configuration/ConfigureCoreServices.cs` | Deferred — binary compat, verify at build | ⏳ Task 04 |
| 4 | `src/PublicApi/Program.cs` | Deferred — binary compat, verify at build | ⏳ Task 04 |
| 5 | `src/Web/Program.cs` | Deferred — binary compat, verify at build | ⏳ Task 04 |

