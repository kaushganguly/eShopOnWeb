# 06-solution-validation-and-deferred-followups

## Objective
Validate the full net10.0 solution and capture remaining deferred work.

## Scope

### All 10 Projects (net10.0 via Directory.Packages.props)

**Source projects (6):**
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj`
- `src/BlazorShared/BlazorShared.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/PublicApi/PublicApi.csproj`
- `src/Web/Web.csproj`

**Test projects (4):**
- `tests/UnitTests/UnitTests.csproj`
- `tests/IntegrationTests/IntegrationTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`

All 10 projects inherit `<TargetFramework>net10.0</TargetFramework>` from `Directory.Packages.props`.

### Key Facts Discovered
- **Azure.Identity**: Upgraded to 1.21.0 (safe — CVE was in older versions) ✅
- **xunit.runner.console**: Already updated to 2.9.3 (not the deprecated 2.7.0) ✅
- **AutoMapper 12.0.1**: Known CVE (GHSA-rvv3-g6hj-g44x, high severity) — intentionally deferred, requires migration to AutoMapper 13+
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6**: Non-critical tooling, deferred
- **NuGet.Packaging / NuGet.Protocol 6.12.1**: Low severity vulnerability (GHSA-g4vj-cjjj-v7hg) — transitive, deferred

## Steps

### Phase 1: Full solution validation
1. `dotnet restore eShopOnWeb.sln` — confirms all packages resolve
2. `dotnet build eShopOnWeb.sln --no-restore` — confirms all 10 projects compile
3. `dotnet test tests/UnitTests/UnitTests.csproj --no-build -v minimal` — unit tests pass

### Phase 2: Document deferred items
- Record intentionally-deferred packages in progress-details.md

### Phase 3: Verify all 10 projects target net10.0
- Confirmed via Directory.Packages.props `<TargetFramework>net10.0</TargetFramework>`

## Done When
- Full solution restores, builds, and unit tests pass on net10.0 ✅
- All 10 projects confirmed on net10.0 ✅
- Azure.Identity vulnerability fix confirmed ✅
- Deferred packages documented ✅
