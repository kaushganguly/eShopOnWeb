# Upgrade Plan — eShopOnWeb .NET 8.0 → .NET 10.0

## Selected Strategy

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all currently on `net8.0`, all SDK-style, centrally managed via
`Directory.Packages.props` (single `TargetFramework` property shared by all projects), shallow
dependency graph (≤ 3 tiers), no .NET Framework projects, no OData/Newtonsoft→STJ-class risky
migrations. Clear case for All-At-Once.

## Projects In Scope

**Libraries**
- BlazorShared (`src/BlazorShared/BlazorShared.csproj`)
- ApplicationCore (`src/ApplicationCore/ApplicationCore.csproj`)
- Infrastructure (`src/Infrastructure/Infrastructure.csproj`)
- BlazorAdmin (`src/BlazorAdmin/BlazorAdmin.csproj`)

**Applications**
- Web (`src/Web/Web.csproj`)
- PublicApi (`src/PublicApi/PublicApi.csproj`)

**Tests**
- UnitTests (`tests/UnitTests/UnitTests.csproj`)
- IntegrationTests (`tests/IntegrationTests/IntegrationTests.csproj`)
- FunctionalTests (`tests/FunctionalTests/FunctionalTests.csproj`)
- PublicApiIntegrationTests (`tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`)

## Tasks

### 001-upgrade-all-projects: Upgrade SDK, TFM, and centrally managed packages; fix breaking changes; validate
1. Update `global.json` SDK version from `8.0.x` to `10.0.x` (rollForward `latestFeature`).
2. Update `Directory.Packages.props`:
   - `TargetFramework`: `net8.0` → `net10.0`
   - Bump centrally managed version properties (`AspNetVersion`, `SystemExtensionVersion`,
     `EntityFramworkCoreVersion`, `VSCodeGeneratorVersion`) to their .NET 10–compatible versions.
   - Bump remaining package versions to latest .NET 10–compatible versions, keeping packages
     with commercial-licensing transitions (MediatR, AutoMapper) pinned at their last
     Apache-2.0/MIT-licensed release to avoid introducing a license obligation.
   - Replace `AutoMapper.Extensions.Microsoft.DependencyInjection` with the core `AutoMapper`
     package (DI registration is built-in since AutoMapper 12).
   - Remove the unused, legacy `Microsoft.AspNetCore.Mvc` 2.2.0 CPM entry (no project references it).
3. Update `src/Web/Dockerfile` and `src/PublicApi/Dockerfile` base images from `8.0` to `10.0`.
4. Build the full solution; fix any breaking API changes surfaced by the .NET 10 / ASP.NET Core 10 /
   EF Core 10 upgrade.
5. Run the full test suite (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests);
   fix any failures without changing test intent (no new tests are being added per success criteria).
6. Re-check for vulnerable packages (`dotnet list package --vulnerable --include-transitive`) and
   confirm no known vulnerabilities remain.

**Done when**: solution builds with 0 errors/0 warnings on `net10.0`, full test suite passes,
and `dotnet list package --vulnerable` reports no vulnerabilities.
