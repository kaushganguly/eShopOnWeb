# .NET 10 LTS Upgrade - eShopOnWeb

## Upgrade Goal
Upgrade eShopOnWeb projects from .NET 8 (net8.0) to .NET 10 LTS (net10.0)

## Target Framework
- **Target**: net10.0 (LTS — Support ends Nov 2028)
- **Alternative considered**: net9.0 (STS), net11.0 (Preview)
- **Rationale**: LTS version provides long-term stability and support alignment with enterprise requirements

## Flow Mode
**Automatic** — No pauses for user review. Execute end-to-end without blocking for confirmation.

## Scope
- **Solution**: eShopOnWeb.sln
- **Projects to upgrade**:
  - ApplicationCore
  - BlazorAdmin
  - BlazorShared
  - Infrastructure
  - PublicApi
  - Web
  - FunctionalTests
  - IntegrationTests
  - PublicApiIntegrationTests
  - UnitTests

## Success Criteria
- ✅ All projects target net10.0
- ✅ Solution builds without errors
- ✅ All unit tests pass
- ✅ All package updates applied (no security vulnerabilities)

## User Preferences
- **Flow mode**: Automatic
- **Execution**: Run end-to-end without pauses
- **Pace**: Fast
- **Risk tolerance**: Accept standard breaking changes for LTS upgrade

## Decisions
- Use .NET 10 LTS for long-term support and stability
- Upgrade all application and test projects uniformly
