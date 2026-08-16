# .NET 10 Upgrade Plan for eShopOnWeb

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 10 projects, all on net8.0, SDK-style, clear dependency structure. Low complexity.

## Projects

**Libraries**: ApplicationCore, BlazorShared, Infrastructure
**Applications**: Web, PublicApi, BlazorAdmin
**Tests**: UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests

---

### 01-upgrade-all: Upgrade all projects from net8.0 to net10.0

All 10 projects are upgraded simultaneously. The TargetFramework is defined centrally in Directory.Packages.props, so updating it once applies to all projects. Package versions are also managed centrally via Directory.Packages.props version variables.

Key changes required:
- global.json: SDK version 8.0.x → 10.0.x
- Directory.Packages.props: TargetFramework net8.0 → net10.0, version variables updated for all Microsoft.* packages
- Remove System.Security.Claims (now included in framework ref)
- Remove Microsoft.VisualStudio.Azure.Containers.Tools.Targets (incompatible, no supported version)
- Fix EmptyBasketOnCheckoutException.cs: remove obsolete serialization constructor
- Fix any API breaking changes identified after first build

**Done when**: Solution builds successfully with no errors after all changes applied.

### 02-final-validation: Run tests and validate

Run unit tests and confirm all pass. Fix any test failures caused by behavioral changes.

**Done when**: All unit tests pass (dotnet test UnitTests).
