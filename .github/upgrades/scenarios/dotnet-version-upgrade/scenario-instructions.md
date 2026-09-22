# Scenario Instructions — dotnet-version-upgrade

## Scenario Parameters

- Target Framework: net10.0 (LTS, EOL 2028-11-14)
- Solution: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln (also Everything.sln)
- Scope: Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, and associated test projects
- Central Package Management: Directory.Packages.props — update shared TargetFramework and package versions
- global.json: update SDK version to a .NET 10 SDK version
- Docker/CI: update docker-compose.yml, docker-compose.override.yml, docker-compose.dcproj, and CI/CD workflow SDK pins if present
- Out of scope: no feature additions/refactors beyond what's required for the framework upgrade; no business logic changes unless required for .NET 10 API compatibility

## User Preferences

### Execution Style

- Flow Mode: **Automatic** — run end-to-end, only pause when genuinely blocked. Do not pause for review at assessment/plan/task boundaries. Print artifact paths inline and keep moving.

### Technical Preferences

- Success criteria: passBuild=true, generateNewUnitTests=false (do not generate new unit tests), passUnitTests=true, securityComplianceCheck=true (resolve known package vulnerabilities as part of the package updates)

## Source Control

- Working branch: upgrade/net10-001-upgrade-dotnet-to-net10 (created from main, working tree was clean at start)
- Commit Strategy: Single Commit at End (matches All-at-Once strategy)

## Upgrade Options

- **Upgrade Strategy**: All-at-Once — Rationale: 10 projects, all already on modern .NET 8.0 (SDK-style), moderate dependency depth (≤3 tiers: Core/Infra/BlazorShared → Web/PublicApi/BlazorAdmin → tests), only 1 incompatible package (Microsoft.VisualStudio.Azure.Containers.Tools.Targets) and 1 security vuln (Azure.Identity) — well within All-at-Once thresholds (≤20 projects, ≤2 high-risk migrations, ≤3 tier depth). No CI-green constraint stated.
- **Test Coverage**: Skip — success criteria explicitly says `generateNewUnitTests=false`; existing tests must continue to pass.

## Strategy
**Selected**: All-At-Once
**Rationale**: 10 projects, all on .NET 8.0 already modern/SDK-style, clear 3-tier dependency structure (ApplicationCore/Infrastructure/BlazorShared → Web/PublicApi/BlazorAdmin → test projects). Straightforward TFM bump + package version bumps + minor API/behavioral fixes.

### Execution Constraints
- Single atomic upgrade — all projects updated together (TFM + Directory.Packages.props + global.json) before build validation
- Update all project files and Directory.Packages.props first, then restore, then build solution and fix all compilation errors in one bounded pass (not iterative first-error-only fixes)
- Testing (unit/integration/functional) runs AFTER the atomic upgrade build succeeds
- Docker/CI SDK version pins updated alongside prerequisites (global.json) since they must stay consistent
- Security vulnerability (Azure.Identity) and deprecated/incompatible packages are fixed as part of the package update pass, not deferred
- Commit once at the end after build + tests pass (Single Commit at End)

## Execution Summary (Completed)

**Status**: ✅ Completed successfully — all 5 plan.md tasks executed (tracked via SQL todos due to a
start_task/break_down_task tool limitation that prevented registering tasks parsed from a manually
authored plan.md in this environment).

- Build: `dotnet build eShopOnWeb.sln -c Release` → 0 Errors, 0 Warnings.
- Tests: `dotnet test eShopOnWeb.sln -c Release --no-build` → 74/74 passed (UnitTests 44, IntegrationTests 3,
  FunctionalTests 12, PublicApiIntegrationTests 15).
- Security: `dotnet list eShopOnWeb.sln package --vulnerable --include-transitive` → no vulnerable packages
  reported for any project.
- `Everything.sln` contains the same project set as `eShopOnWeb.sln` — no additional scope.

Key fixes beyond mechanical version bumps:
- Removed `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no net10 support).
- Removed `System.Security.Claims` / `System.Text.Json` / `System.Net.Http.Json` explicit references
  (now framework-provided / auto-referenced, NU1510).
- Bumped `Azure.Identity` 1.10.4 → 1.21.0 (moderate CVE fix).
- Bumped `System.IdentityModel.Tokens.Jwt` to 8.23.0 to resolve NU1605 transitive downgrade conflicts.
- Replaced `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 (pins AutoMapper to exactly
  12.0.1, vulnerable to GHSA-rvv3-g6hj-g44x) with direct `AutoMapper` 16.1.1 (patched) + the
  `AddAutoMapper(cfg => {}, assembly)` overload now shipped directly in the core package.
- Pinned `NuGet.Packaging` / `NuGet.Protocol` to 6.12.5 (patched for GHSA-g4vj-cjjj-v7hg, transitive via
  `Microsoft.VisualStudio.Web.CodeGeneration.Design`).
- Fixed CS0433 ambiguous `Program` type in `PublicApiIntegrationTests` (both `PublicApi` and `Web` are
  referenced by that test project) via an `Aliases="WebAssembly"` extern alias on the `Web` ProjectReference.
- Removed obsolete `SYSLIB0051` binary-formatter serialization constructor from
  `EmptyBasketOnCheckoutException`.
- Fixed 3x `xUnit2013` analyzer warnings (`Assert.Equal(n, x.Count)` → `Assert.Single`/`Assert.Empty`).
