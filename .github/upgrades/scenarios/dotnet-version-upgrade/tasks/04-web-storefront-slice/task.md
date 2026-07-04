# 04-web-storefront-slice: 04-web-storefront-slice

Execute task 04-web-storefront-slice.

## Objective

Upgrade the `Web` application to net10.0, fix Razor view compilation errors, update xunit packages, and ensure all unit and functional tests pass.

## Scope

### Projects
- `src/Web/Web.csproj` — Main storefront MVC application
- `tests/UnitTests/UnitTests.csproj` — 44 unit tests
- `tests/FunctionalTests/FunctionalTests.csproj` — 12 functional tests

### Key Issues from Assessment
- Binary incompatible APIs: `ConfigurationBinder.Get<T>`, `Configure<T>` — not compile errors in practice
- `Exception(SerializationInfo, StreamingContext)` — already removed in task 01
- `TimeSpan.FromMinutes(double)` — uses const int, compiles fine
- Razor view compilation failures (the actual blockers)
- xunit 2.7.0 deprecated → update to 2.9.3

## Research Findings

### Root Cause: Razor 10 Parser Bug with HTML5 Semantic Elements

**Critical discovery**: The .NET 10 Razor source generator has a bug where `<section>`, `<article>`, and other HTML5 semantic block elements are treated as having "optional end tags" inside code blocks. When such an element contains a nested `@if` code block, the parser loses track of the enclosing element's stack, causing cascade errors.

Pattern that FAILS in .NET 10:
```cshtml
@if (outer_condition) {
    <section>            ← Razor parser loses this element's tracking
        @if (inner) { ... }   ← nested code block triggers the bug
    </section>           ← RZ1026: no matching start tag
}
```

Pattern that WORKS (using `<div>` instead):
```cshtml
@if (outer_condition) {
    <div>               ← <div> works fine
        @if (inner) { ... }
    </div>
}
```

### `Microsoft.VisualStudio.Web.CodeGeneration.Design` Package Issue
The package was pulling in `Microsoft.CodeAnalysis.Razor 6.0.24` (an old Razor generator). Added `PrivateAssets="All"` to prevent dependency leakage. Note: old package still shows in list but no longer contributes conflicting generators.

### Files to Fix

1. **`_LoginPartial.cshtml`**:
   - Root cause: All `<section>` elements used with nested `@if` blocks
   - Fix: Replace `<section>` with `<div>` throughout the view
   - Added `@model dynamic` directive (required for proper class generation)
   - Fixed `<img>` void elements to use self-closing syntax (`/>`)

2. **`_CookieConsentPartial.cshtml`**:
   - Root cause: `<script>` block with curly braces inside `@if` code block
   - Fix: Move `<script>` block outside the `@if` block; add null check in JS

3. **`ShowRecoverCodes.cshtml`**:
   - Root cause: `<text>` pseudo-element not supported in new Razor; multiple markup segments on same line after `@Html.Raw()`
   - Fix: Wrap recovery code pair in `<p>` element; use direct `&nbsp;` entity
