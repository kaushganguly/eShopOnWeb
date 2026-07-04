# Progress Details: 04-web-storefront-slice

## Build Status
- **Web.csproj**: ✅ 0 errors, 6 NuGet warnings (AutoMapper vulnerability deferred per plan)
- **UnitTests.csproj**: ✅ 0 errors, 0 non-NuGet warnings
- **FunctionalTests.csproj**: ✅ 0 errors, 0 non-NuGet warnings
- **Full solution**: ✅ 0 errors, 28 NuGet warnings only

## Test Results
- **UnitTests**: 44/44 passed ✅
- **FunctionalTests**: 12/12 passed ✅

## Files Changed

### `src/Web/Web.csproj`
- Added `PrivateAssets="All"` and `IncludeAssets` to `Microsoft.VisualStudio.Web.CodeGeneration.Design` to prevent old `Microsoft.CodeAnalysis.Razor 6.0.24` from leaking into build

### `src/Web/Views/Shared/_LoginPartial.cshtml`
**Root cause**: .NET 10 Razor source generator bug — HTML5 semantic elements (`<section>`, `<article>`, etc.) are treated as having optional end tags inside `@if` code blocks. When a nested `@if` appears inside such elements, the parser loses track of the enclosing element stack.

**Changes**:
- Added `@model dynamic` directive at top (required for proper class generation)
- Replaced all `<section>` elements with `<div>` elements (workaround for Razor 10 parser bug)
- Fixed `<img>` void elements to use XHTML self-closing syntax (`/>`)
- Form tag helper (`asp-area`, `asp-page`) restored — works correctly with `<div>` elements

**Behavioral note**: Visual appearance unchanged (CSS class names preserved). The `col-lg-*`, `esh-identity-*` classes are the same on `<div>` as they were on `<section>`.

### `src/Web/Views/Shared/_CookieConsentPartial.cshtml`
**Root cause**: `<script>` block with JavaScript curly braces `{` `}` inside an `@if` code block causes the Razor parser to confuse JavaScript curly braces with Razor code block delimiters.

**Changes**:
- Moved `<script>` block outside the `@if` block
- Added null check in JavaScript: `if (button)` before `addEventListener` call
- Cookie consent behavior unchanged (script runs on every page load but only wires up when the banner is present)

### `src/Web/Views/Manage/ShowRecoverCodes.cshtml`
**Root cause**: Two issues in line 23:
1. `<text>&nbsp;</text>` — `<text>` pseudo-element behavior changed in .NET 10 Razor; `</text>` causes RZ1026 error
2. Multiple markup segments on one line after `@Html.Raw()` confuses Razor expression boundary detection

**Changes**:
- Replaced the complex inline `<code>@val</code><text>&nbsp;</text><code>@val2</code><br />` with a `<p>` wrapper: `<p><code>@val</code>&nbsp;<code>@val2</code></p>`
- Direct `&nbsp;` HTML entity used inside the `<p>` wrapper (no raw output needed)

### `Directory.Packages.props`
- Updated `xunit` from 2.7.0 (deprecated) to 2.9.3
- Updated `xunit.runner.visualstudio` from 2.5.6 to 2.8.2
- Updated `xunit.runner.console` from 2.7.0 to 2.9.3

### `tests/FunctionalTests/FunctionalTests.csproj`
- Removed legacy `DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1"` (incompatible with .NET 10)

### `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
- Fixed xUnit2013 warnings: `Assert.Equal(1, col.Count)` → `Assert.Single(col)`

### `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`
- Fixed xUnit2013 warning: `Assert.Equal(0, col.Count)` → `Assert.Empty(col)`

### `tests/IntegrationTests/Repositories/BasketRepositoryTests/SetQuantities.cs`
- Fixed xUnit2013 warning: `Assert.Equal(0, col.Count)` → `Assert.Empty(col)`

## Key Diagnostics Performed

### Razor 10 Parser Bug Investigation
Confirmed that `.NET 10 Razor source generator treats <section>, <article>, and other HTML5 semantic elements as optionally-closeable`. Test cases:
- `@if → <div> → @if` → ✅ Works
- `@if → <section> → @if` → ❌ RZ1021/RZ1026 errors
- `@if → <article> → @if` → ❌ Same errors

This is a regression from .NET 8 where the same views compiled correctly. The workaround (replacing `<section>` with `<div>`) is the correct approach since the semantic meaning is preserved via CSS classes.

### Deferred Items (per task plan)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 high severity vulnerability — still present; AutoMapper deferred per task plan
- `ConfigurationBinder`, `OptionsConfigurationServiceCollectionExtensions` — binary incompatible APIs listed in assessment; verified not compile-time errors in practice
- `System.IdentityModel.Tokens.Jwt` deprecated — deferred per task plan; build passes
