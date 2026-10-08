# Scenario Instructions — dotnet-version-upgrade

## Parameters
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- Target Framework: net10.0
- Scope: All projects (Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests) plus centrally managed packages in Directory.Packages.props
- Provisioning tool: n/a (code upgrade only)

## User Preferences

### Execution Style
- Flow Mode: **Automatic** — do not pause for confirmation at any stage; surface assessment/plan/progress and keep moving unless genuinely blocked.
- generateNewUnitTests: false — do not add new unit tests, only fix/migrate existing ones.
- Success criteria: passBuild=true, passUnitTests=true, securityComplianceCheck=true.

### Source Control
- Git repo detected; working branch created: `upgrade/dotnet10-net10` (from `main`, clean working tree at start).

## Decisions
- Using eShopOnWeb.sln (not Everything.sln) as it is the canonical solution covering all projects named in the task plus docker-compose item; Everything.sln is an equivalent alternate grouping.

## Final Outcome (post-execution)

- **Build**: `dotnet build eShopOnWeb.sln` (Debug and Release) — **0 Errors, 0 Warnings** across all 11 projects, verified on a fully clean (`bin`/`obj` removed) rebuild.
- **Tests**: All 4 test projects pass — UnitTests 44/44, IntegrationTests 3/3, FunctionalTests 12/12, PublicApiIntegrationTests 15/15 (**74/74 total**).
- **Security compliance**: `dotnet list package --vulnerable --include-transitive` reports no vulnerable packages (direct or transitive) in any project.
- MCP orchestrator task-tracking tools (`generate_dotnet_upgrade_assessment`, skill lookups, `break_down_task`/`start_task`) were unavailable this session ("Orchestrator channel was not available"); assessment/plan/execution were carried out manually with direct file edits and `dotnet build`/`dotnet test` validation loops instead of the tracked task workflow.

### Significant discovery: .NET 10 Razor compiler regression

During the upgrade, a previously unknown **.NET 10 Razor compiler bug** was discovered and had to be worked around in both Blazor (`.razor`) and MVC/Razor Pages (`.cshtml`) views. It only reproduces on a **fully clean build** (stale `obj/` Razor-generated files mask it on incremental builds).

**Symptom 1 (markup-in-code-block corruption)**: An element that is a *direct child* of a C# code block (`@if`/`@else`/`@for`/`@foreach`) and is recognized by the Razor/TagHelper pipeline (any Blazor component or plain HTML tag whose name is ≥6 characters, OR any element matched by a TagHelper — e.g. `<form>`, `<partial>`, any `asp-*`-attributed element) causes the Razor tokenizer to drop/garble the tag, producing cascading `RZ1021`/`RZ1026`/`CS1525`/`CS0119`/`CS1010` errors in that file and often subsequent files.
  - **Workaround**: insert a Razor comment (`@* ... *@`) immediately before the affected element. Required for every such element at every nesting level where it is the direct child of its own enclosing code block (including when a code block is nested inside another code block's already-commented child) — each occurrence needs its own comment.

**Symptom 2 (attribute-expression truncation)**: When two or more TagHelper-bound elements using an inline Razor expression in an attribute value (e.g. `asp-for="@someVar.Prop"`) appear inside a code block nested two or more levels deep (e.g. `@foreach` inside `@if`, or similar), the generated attribute value is truncated by one character, causing `CS1061`/"does not contain a definition for '<truncated-name>'" errors. The comment workaround does not fix this variant.
  - **Workaround applied**: replaced `asp-for="@expr"` with plain `name="..."/value="@expr"` HTML attributes for simple hidden-input cases (functionally equivalent since no client-side validation was needed on those fields).

This is a genuine, reproducible .NET 10 SDK/Razor compiler issue (isolated and confirmed via minimal standalone repro projects), not an artifact of this codebase. All instances found in eShopOnWeb were fixed; see file list below for exact locations.

### CS0433 ambiguous `Program` type (PublicApiIntegrationTests)

In .NET 10, the SDK's implicitly generated top-level-statement `Program` class is now **`public`** by default (previously `internal`). Since `tests/PublicApiIntegrationTests` references both `Web.csproj` and `PublicApi.csproj` (and `PublicApi/Program.cs` already declares an explicit `public partial class Program`), both assemblies now expose a public `Program` type, making `WebApplicationFactory<Program>` ambiguous (CS0433). Fixed by switching `ProgramTest.cs` to use the existing `AuthenticateEndpoint` marker type (the same pattern already used by `FunctionalTests/PublicApi/ApiTestFixture.cs`) instead of `Program`.

### Files changed
See git diff on branch `upgrade/dotnet10-net10` for the full list; summary:
- `global.json`, `Directory.Packages.props` — SDK/TFM → net10.0, package version bumps, AutoMapper DI package swap, security pins, CPM transitive pinning.
- `src/Web/Web.csproj`, `src/PublicApi/PublicApi.csproj` — AutoMapper package reference swap.
- `src/BlazorAdmin/BlazorAdmin.csproj`, `src/ApplicationCore/ApplicationCore.csproj` — removed unneeded implicit package references.
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — removed obsolete serialization ctor.
- `src/PublicApi/CustomSchemaFilters.cs`, `src/PublicApi/Program.cs` — Microsoft.OpenApi 2.x / Swashbuckle 10.x API updates.
- `src/BlazorAdmin/Pages/CatalogItemPage/List.razor` — Razor compiler workaround comments.
- `src/Web/Views/Manage/{ShowRecoverCodes,MyAccount,ExternalLogins}.cshtml`, `src/Web/Views/Shared/{_LoginPartial,_CookieConsentPartial}.cshtml`, `src/Web/Pages/Index.cshtml`, `src/Web/Pages/Basket/{Index,Checkout}.cshtml` — Razor compiler workarounds (comments / attribute rewrites).
- `tests/PublicApiIntegrationTests/ProgramTest.cs` — marker-type fix for CS0433.
- `tests/PublicApiIntegrationTests/AssemblyInfo.cs` (new) — `[assembly: DoNotParallelize]` to silence MSTEST0001 (shared static WebApplicationFactory).
- `tests/PublicApiIntegrationTests/CatalogItemEndpoints/CatalogItemListPagedEndpoint.cs` — replaced obsolete `[DataTestMethod]` with `[TestMethod]` (MSTEST0044).
- `tests/UnitTests/...`, `tests/IntegrationTests/...` — minor xUnit2013 analyzer warning fixes (`Assert.Equal(1, ...)` → `Assert.Single(...)`).
