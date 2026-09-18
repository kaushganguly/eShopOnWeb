# Upgrade All Projects to net10.0

## Objective
Upgrade all 10 eShopOnWeb projects from .NET 8.0 to .NET 10.0 in a single atomic operation, including NuGet package updates and API/behavior fixes.

## Scope
**Production Projects** (6):
- src/ApplicationCore/ApplicationCore.csproj
- src/BlazorShared/BlazorShared.csproj
- src/BlazorAdmin/BlazorAdmin.csproj
- src/Infrastructure/Infrastructure.csproj
- src/PublicApi/PublicApi.csproj
- src/Web/Web.csproj

**Test Projects** (4):
- tests/UnitTests/UnitTests.csproj
- tests/IntegrationTests/IntegrationTests.csproj
- tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
- tests/FunctionalTests/FunctionalTests.csproj

## Key Challenges
- **7 Incompatible Packages**: Requiring deferred resolution or stubs (Defer Resolution strategy)
- **12 Breaking API Changes**: 9 binary incompatible + 3 source incompatible (Fix Inline strategy)
- **49 Behavioral Changes**: Especially HttpContent, Uri, Configuration bindings (test at runtime)

## Affected Dependencies
- **26 NuGet packages** need version updates
- **19 packages** have recommended upgrades
- **7 packages** have no direct compatible version

## Steps

### Phase 1: Project File Updates (All-at-Once)
1. Update all 10 project files: change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
2. Update global.json if it pins an SDK version (ensure net10.0 SDK is supported)
3. Commit: "Step 1: Update TargetFramework to net10.0"

### Phase 2: NuGet Package Updates
1. Run `dotnet restore` to fetch package metadata
2. Update all 26 packages to net10.0-compatible versions
3. Handle 7 incompatible packages:
   - Create stub types in temporary files (// STUB: markers) for missing packages
   - Document alternative packages in task.md enrichment
4. Commit: "Step 2: Update NuGet packages to net10.0"

### Phase 3: Fix API Incompatibilities (Fix Inline)
1. **Binary Incompatible (9 issues)**:
   - Microsoft.Extensions.Configuration.ConfigurationBinder.Get (4 instances)
   - Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue (1 instance)
   - Other binary breaks across projects

2. **Source Incompatible (3 issues)**:
   - System.Exception constructor (BinarySerializable)
   - System.TimeSpan.FromMinutes signature
   - Other source-level changes

3. **Behavioral Changes (49 issues** — test at runtime):
   - System.Net.Http.HttpContent (32 instances) — encoding/content-type behavior
   - System.Uri constructor (11 instances) — URI validation/normalization
   - Microsoft.Extensions.Logging.ConsoleLoggerExtensions (2 instances)
   - Other behavioral changes (test to verify)

4. Commit: "Step 3: Fix API incompatibilities and behavioral changes"

### Phase 4: Build & Compile Fixes
1. Run: `dotnet build eShopOnWeb.sln` to identify remaining compilation errors
2. Fix compilation errors:
   - API signature mismatches
   - Namespace/type resolution issues
   - Missing type stubs created in Phase 2
3. Iterate: build, fix, build (single bounded pass)
4. Ensure **0 compilation errors** and **0 warnings**
5. Commit: "Step 4: Fix compilation errors from net10.0 migration"

## Enrichment Data (Research Before Executing)

**Assessment Queries Needed**:
- Per-project issue summary for all 10 projects
- Package upgrade targets and incompatibility details
- API breakage instances (file, line, snippet)

**Key API Changes to Research**:
- HttpContent behavior in net10.0
- Uri validation in net10.0
- ConfigurationBinder generic methods in net10.0
- Exception serialization changes
- TimeSpan.FromMinutes signature

## Done When
- ✅ All 10 project TargetFramework values updated to net10.0
- ✅ All 26 NuGet packages resolved/updated
- ✅ 7 incompatible packages handled (stubs created if needed)
- ✅ All 9 binary incompatible APIs fixed
- ✅ All 3 source incompatible APIs fixed
- ✅ 49 behavioral changes reviewed and tested in validation task
- ✅ Solution builds with 0 errors and 0 warnings
- ✅ No unresolved API references
- ✅ All 4 commits pushed (or squashed per preference)

## Related Skills
- Assessment consultation (query_dotnet_assessment for per-project details)
- Multi-targeting (if any projects need conditional package refs)
- Stub marker format (for deferred incompatible packages)
