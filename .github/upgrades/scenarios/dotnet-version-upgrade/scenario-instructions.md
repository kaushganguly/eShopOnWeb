# .NET 8.0 → .NET 10.0 Upgrade — eShopOnWeb

## Scenario Metadata

- **Scenario**: dotnet-version-upgrade
- **Target Framework**: net10.0 (.NET 10 LTS)
- **Current Framework**: net8.0 (.NET 8.0)
- **Solution**: eShopOnWeb.sln
- **Execution Start**: Autonomous Automatic mode

## Upgrade Parameters

### Target Framework
- **Selected**: net10.0 (LTS — Support ends Nov 2028)
- **Rationale**: Long-term support, latest stable LTS, recommended for production

### Projects in Scope
1. ApplicationCore (Library)
2. BlazorAdmin (Blazor WebAssembly)
3. BlazorShared (Shared Library)
4. Infrastructure (Library)
5. PublicApi (API)
6. Web (Web Application)
7. FunctionalTests (Test Project)
8. IntegrationTests (Test Project)
9. PublicApiIntegrationTests (Test Project)
10. UnitTests (Test Project)

### Flow Mode
- **Mode**: Automatic
- **Behavior**: Execute end-to-end without pausing for user confirmations. Surface all artifacts and progress, but proceed autonomously.

## Execution Constraints
- Build must pass warning-free
- All unit tests must pass
- No breaking API changes without migration

## Progress Tracking

### Stage 1: Assessment
- Status: Pending
- Completion: [To be filled during assessment]

### Stage 2: Planning
- Status: Pending
- Completion: [To be filled during planning]

### Stage 3: Execution
- Status: Pending
- Completion: [To be filled during execution]

