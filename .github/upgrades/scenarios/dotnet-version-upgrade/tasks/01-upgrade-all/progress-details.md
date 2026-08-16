# Task 01-upgrade-all Progress

## Summary
Successfully upgraded all 10 projects from .NET 8 (net8.0) to .NET 10 (net10.0).

## Files Modified

### Configuration Files
- `global.json`: SDK version 8.0.x → 10.0.x
- `Directory.Packages.props`: TFM net8.0 → net10.0, package versions updated

### Package Updates (Directory.Packages.props)
- `AspNetVersion`: 8.0.2 → 10.0.11 (all Microsoft.AspNetCore.* packages)
- `SystemExtensionVersion`: 8.0.0 → 10.0.11
- `EntityFramworkCoreVersion`: 8.0.2 → 10.0.11
- `VSCodeGeneratorVersion`: 8.0.0 → 10.0.2
- `Azure.Identity`: 1.10.4 → 1.21.0 (security fix)
- `System.Text.Json`: 8.0.3 → 10.0.11
- `System.IdentityModel.Tokens.Jwt`: 7.3.1 → 8.22.0
- Removed `System.Security.Claims` (now in framework)
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (incompatible)

### Project File Updates
- `src/ApplicationCore/ApplicationCore.csproj`: removed System.Text.Json, System.Security.Claims refs
- `src/PublicApi/PublicApi.csproj`: removed Microsoft.VisualStudio.Azure.Containers.Tools.Targets ref
- `src/BlazorAdmin/BlazorAdmin.csproj`: retained System.Net.Http.Json (needed for WASM)
- `src/Web/Web.csproj`: removed RazorLangVersion property
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: added WebProject alias on Web ref

### Breaking Changes Fixed

#### API Breaking Changes
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`:
  Removed obsolete `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor
  (Exception(SerializationInfo, StreamingContext) removed in .NET 9+)

#### .NET 10 Razor Source Generator Breaking Changes
The .NET 10 Razor source generator has a bug where certain HTML5 semantic elements and
TagHelper attributes inside `@if`/`@for` code blocks generate incorrect column offsets.
Workarounds applied:

- `src/BlazorAdmin/Pages/CatalogItemPage/List.razor`: 
  - `<Spinner>` wrapped in `<div>` to fix RZ1021
  - Dialog components wrapped in `<div>` to fix RZ1021
  - `<img>` tags made self-closing
- `src/BlazorAdmin/Pages/CatalogItemPage/Details.razor`, Delete.razor, Create.razor, Edit.razor:
  - `<img>` tags made self-closing
- `src/Web/Views/Shared/_LoginPartial.cshtml`:
  - Added Razor comment at top, `<section>` → `<div>`
- `src/Web/Views/Shared/_CookieConsentPartial.cshtml`:
  - Moved `<script>` block out of `@if` using Html.Raw
- `src/Web/Views/Manage/ExternalLogins.cshtml`:
  - Replaced `<form asp-action>` + `<input asp-for>` inside @if with plain HTML + Url.Action
- `src/Web/Views/Manage/MyAccount.cshtml`:
  - Moved `@if`/`@else` logic to pre-form `@{}` block to avoid TagHelper column offset bug
- `src/Web/Views/Manage/ShowRecoverCodes.cshtml`:
  - Used `@:` prefix for inline elements in `@for` block, replaced `<text>` with direct `&nbsp;`
- `src/Web/Pages/Index.cshtml`:
  - Replaced `<partial for="...">` inside `@if`/`@foreach` with `@await Html.PartialAsync()`
- `src/Web/Pages/Basket/Index.cshtml`, `Basket/Checkout.cshtml`:
  - Replaced `<div asp-validation-summary="All">` with `@Html.ValidationSummary()`
- `src/Web/*.cshtml` (various): `<section>` → `<div>`, `<article>` → `<div>` where inside code blocks
- `src/Web/Views/Order/Detail.cshtml`, `Manage/ExternalLogins.cshtml`:
  - `<img>` tags made self-closing
- `src/Web/Program.cs`: removed `internal partial class Program {}` (caused Razor compiler issues)
- `tests/PublicApiIntegrationTests/ProgramTest.cs`: reverted to standard `WebApplicationFactory<Program>` 
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs`:
  Added `extern alias WebProject` to resolve Web.ViewModels type reference

## Results
- Build: ✅ All 10 projects build successfully
- Unit Tests: ✅ 44/44 pass
