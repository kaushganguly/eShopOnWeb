# 02-upgrade-class-libraries: Upgrade ApplicationCore, Infrastructure, BlazorShared to net10.0

## Scope

Projects: `src/ApplicationCore/ApplicationCore.csproj`, `src/Infrastructure/Infrastructure.csproj`,
`src/BlazorShared/BlazorShared.csproj`.

## Key discovery

The repo centralizes `TargetFramework` itself (not just package versions) as an MSBuild property
inside `Directory.Packages.props` (`<TargetFramework>net8.0</TargetFramework>`). No project `.csproj`
declares its own `<TargetFramework>`, and there is no `Directory.Build.props`. Because
`Directory.Packages.props` is imported early in SDK evaluation, this single property value flows to
every project in the solution. Consequently, bumping this property to `net10.0` upgrades the TFM for
**all 10 projects at once** — consistent with the chosen **All-at-Once** strategy — rather than needing
per-project edits.

## Assessment findings (query_dotnet_assessment)

- ApplicationCore.csproj (6 issues):
  - Project.0002: TFM net8.0 → net10.0 (mandatory)
  - NuGet.0003: `System.Security.Claims` 4.3.0 — functionality now included in framework reference;
    recommendation is to remove the package reference (namespace stays available via framework).
  - NuGet.0002 / NuGet.0004: `System.Text.Json` 8.0.3 → 10.0.12 (also flagged with a security advisory).
  - Api.0002 (source incompatible): `EmptyBasketOnCheckoutException` uses the legacy
    `Exception(SerializationInfo, StreamingContext)` binary-formatter serialization constructor
    (SYSLIB0051-style obsolete pattern). Not used anywhere else in the repo (`grep` confirmed only this
    one occurrence) — safe to delete since BinaryFormatter-style exception serialization isn't used by
    this web app.
- Infrastructure.csproj (5 issues):
  - Project.0002: TFM net8.0 → net10.0 (mandatory)
  - NuGet.0002: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.InMemory`,
    `Microsoft.EntityFrameworkCore.SqlServer` 8.0.2 → 10.0.12 (all driven by shared MSBuild properties).
  - NuGet.0005 (deprecated): `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.23.0 (per Microsoft LTS guidance).
- BlazorShared.csproj (1 issue):
  - Project.0002: TFM net8.0 → net10.0 (mandatory) — no package-level issues.

## Changes made

1. `Directory.Packages.props` (shared, affects whole solution):
   - `<TargetFramework>net8.0</TargetFramework>` → `net10.0`
   - `$(AspNetVersion)` 8.0.2 → 10.0.12
   - `$(SystemExtensionVersion)` 8.0.0 → 10.0.12
   - `$(EntityFramworkCoreVersion)` 8.0.2 → 10.0.12
   - `$(VSCodeGeneratorVersion)` 8.0.0 → 10.0.2
   - `Azure.Identity` 1.10.4 → 1.21.0 (resolves the flagged security vulnerability; also consumed by
     task 03's Web.csproj)
   - `System.Text.Json` 8.0.3 → 10.0.12
   - `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.23.0
   - Removed the `System.Security.Claims` PackageVersion entry (functionality merged into framework)
   - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.19.6 → 1.23.0 (latest stable on NuGet;
     `get_supported_package_version` reported no net10-specific mapping because this is a build-only VS
     Docker-debugging tooling package with no TFM-specific lib assets — bumping to latest stable resolves
     the "incompatible" flag safely since it isn't consumed via code APIs)
   - Test package versions also bumped here since they are shared entries (consumed later by task 04):
     `Microsoft.NET.Test.Sdk` 17.9.0 → 18.10.1, `xunit` 2.7.0 → 2.9.3,
     `xunit.runner.visualstudio` 2.5.6 → 4.0.0, `xunit.runner.console` 2.7.0 → 2.9.3,
     `MSTest.TestAdapter`/`MSTest.TestFramework` 3.2.2 → 4.4.1, `coverlet.collector` 6.0.2 → 10.1.0
2. `src/ApplicationCore/ApplicationCore.csproj` — removed `<PackageReference Include="System.Security.Claims" />`.
3. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — removed the obsolete
   `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor.

## Validation plan

- `dotnet restore` + `dotnet build` targeted at ApplicationCore, Infrastructure, BlazorShared individually.
- Fix any compiler errors/warnings introduced.
- Full solution build/test deferred to tasks 03-05 since Web/PublicApi/BlazorAdmin/tests depend on these libraries.
