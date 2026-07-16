# Modernization Summary — 001-upgrade-dotnet-to-net10

- **finalStatus**: success
- **successCriteriaStatus**:
  - passBuild: true
  - generateNewUnitTests: false
  - passUnitTests: true

## Summary

Upgraded eShopOnWeb from .NET 8.0 to .NET 10.0 (LTS) across all ten projects (Web,
PublicApi, Infrastructure, ApplicationCore, BlazorShared, BlazorAdmin, IntegrationTests,
UnitTests, FunctionalTests, PublicApiIntegrationTests).

Changes made:
- `global.json`: SDK version bumped from `8.0.x` to `10.0.x`.
- `Directory.Packages.props`: `TargetFramework` changed to `net10.0`; central version
  variables (`AspNetVersion`, `SystemExtensionVersion`, `EntityFramworkCoreVersion`,
  `VSCodeGeneratorVersion`) bumped to their .NET 10-compatible releases (10.0.10 / 10.0.2).
  Individually upgraded packages flagged by the assessment as vulnerable/deprecated/
  incompatible: `Azure.Identity` (1.10.4→1.21.0, security fix), `Azure.Extensions.AspNetCore.Configuration.Secrets`
  (1.3.1→1.5.1), `System.IdentityModel.Tokens.Jwt` (7.3.1→8.19.2, deprecated package fix),
  `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (1.19.6→1.23.0, incompatible),
  `xunit`/`xunit.runner.console` (2.7.0→2.9.3), `xunit.runner.visualstudio` (2.5.6→3.1.5),
  `MSTest.TestAdapter`/`MSTest.TestFramework` (3.2.2→4.3.2), `System.Text.Json` (8.0.3→10.0.10).
  Replaced the deprecated `AutoMapper.Extensions.Microsoft.DependencyInjection` package
  (pinned to a vulnerable AutoMapper 12.0.1, CVE-2026-32933) with a direct reference to
  `AutoMapper` 16.2.0 in PublicApi (the only project using it); removed the unused reference
  from Web. Replaced the unmaintained, net10-incompatible `BlazorInputFile` package (its
  `ReadRequest` struct fails CLR type-layout validation on net10) with the built-in
  `Microsoft.AspNetCore.Components.Forms.IBrowserFile` API in `BlazorShared`/`BlazorAdmin`
  (the helper method using it was unused in the UI).
- `src/PublicApi/Program.cs`: updated the `AddAutoMapper` call for the new AutoMapper API
  (now requires an `Action<IMapperConfigurationExpression>` argument). Replaced
  `MinimalApi.Endpoint`'s `AddEndpoints()` (which reflects over every assembly loaded in
  the AppDomain) with an inline scan restricted to the PublicApi assembly, avoiding a
  `ReflectionTypeLoadException` thrown when `Microsoft.JSInterop.WebAssembly` (loaded
  transitively via the Web→BlazorAdmin project reference during MSTest discovery) is
  scanned — that assembly's WASM-only structs are not loadable under CoreCLR.
- `src/Web/Program.cs`: marked the implicit top-level-statements `Program` class
  `internal` explicitly, resolving a `CS0433` ambiguous-type build error against
  PublicApi's `public partial class Program` when both assemblies are referenced
  together (in `PublicApiIntegrationTests`).

## Validation

- `dotnet build Everything.sln` (clean, `--no-restore` free) — **Build succeeded**, 0 errors.
- `dotnet test Everything.sln --no-build` — **74/74 tests passed** (UnitTests 44,
  IntegrationTests 3, FunctionalTests 12, PublicApiIntegrationTests 15), 0 failures.
- `dotnet restore` — no remaining NuGet security-vulnerability (NU190x) warnings.
