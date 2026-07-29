# Progress Details: 03-fix-api-breaking-changes

## Changes Made

### 1. Removed Serialization Constructor — `EmptyBasketOnCheckoutException.cs`

**File**: `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`

**Reason**: `protected Exception(SerializationInfo, StreamingContext)` was removed in .NET 9 as part of eliminating `BinaryFormatter` serialization support. This is a **source-incompatible** breaking change (Api.0002) — leaving it in would cause a compile error.

**Change**: Removed lines 12-14:
```csharp
// REMOVED:
protected EmptyBasketOnCheckoutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
{
}
```

The class now contains only the three standard public constructors (parameterless, message-only, message+inner-exception).

---

## Deferred Items (pending build verification in task 04)

The assessment flagged the following patterns as **binary incompatible** (Api.0001). Because they are not *source* incompatible, they will likely still compile after the package upgrades to 10.x. These are noted in task.md with suggested fixes to apply if the build in task 04 fails:

| Pattern | Locations |
|---------|-----------|
| `services.Configure<T>(IConfiguration)` | ConfigureWebServices.cs:16, PublicApi/Program.cs:42, PublicApi/Program.cs:49, Web/Program.cs:98 |
| `configuration.Get<T>()` | ConfigureCoreServices.cs:22, PublicApi/Program.cs:43, PublicApi/Program.cs:50, Web/Program.cs:99 |

Suggested fixes (lambda bind + binder options) are documented in `task.md` for use during task 04 if compilation fails.

---

## Build Status

Not run in this task — deferred to task 04 (`04-build-and-fix`).
