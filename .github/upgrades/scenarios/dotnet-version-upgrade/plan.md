# .NET 10 Upgrade Plan — eShopOnWeb

## Objective
Upgrade eShopOnWeb solution from .NET 8.0 to .NET 10.0 LTS.

## Strategy
In-place upgrade following topological dependency order. Uses Central Package Management (Directory.Packages.props) to coordinate version updates across all projects.

## Assessment Summary
- 10 projects to upgrade (all currently net8.0)
- 119 issues total: 21 mandatory, 84 potential, 14 optional
- Key mandatory issues: TF change (10), binary incompatible APIs (9), incompatible package (1), redundant package (1)

## Tasks

### Phase 1 — Infrastructure

- ✅ 01-global-json: Update global.json SDK version to 10.0.x
- ✅ 02-central-packages: Update Directory.Packages.props versions for net10.0
- ✅ 03-blazorshared: Upgrade BlazorShared project (Level 0 — no project dependencies)

### Phase 2 — Core Libraries

- ✅ 04-applicationcore: Upgrade ApplicationCore project (Level 1 — remove System.Security.Claims)
- ✅ 05-infrastructure: Upgrade Infrastructure project (Level 2)
- ✅ 06-blazoradmin: Upgrade BlazorAdmin project (Level 1)

### Phase 3 — Applications

- ✅ 07-web: Upgrade Web project (Level 3 — fix API breaking changes)
- ✅ 08-publicapi: Upgrade PublicApi project (Level 3 — remove incompatible VS Containers package, fix API breaking changes)

### Phase 4 — Tests

- ✅ 09-unittests: Upgrade UnitTests project (Level 4)
- ✅ 10-integrationtests: Upgrade IntegrationTests project (Level 5)
- ✅ 11-functionaltests: Upgrade FunctionalTests project (Level 4)
- ✅ 12-publicapiintegrationtests: Upgrade PublicApiIntegrationTests project (Level 4)

### Phase 5 — Validation

- ✅ 13-build-validate: Build solution and run unit tests
