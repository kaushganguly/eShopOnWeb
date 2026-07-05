# 02-upgrade-all: 02-upgrade-all

Execute task 02-upgrade-all.

## Research findings

- `query_dotnet_assessment` was unavailable in this session (`StateManager is not initialized`), so findings below were taken directly from `assessment.md`.
- `src/ApplicationCore/ApplicationCore.csproj`: 3 issues, including `System.Security.Claims` included in the framework, `System.Text.Json` upgrade recommendation, and source-incompatible exception serialization API usage.
- `src/BlazorAdmin/BlazorAdmin.csproj`: 7 package/API issues, mostly ASP.NET/Extensions package upgrades plus behavioral-change verification.
- `src/BlazorShared/BlazorShared.csproj`: 0 issues reported.
- `src/Infrastructure/Infrastructure.csproj`: 4 issues, primarily EF Core/Identity package upgrades and deprecated `System.IdentityModel.Tokens.Jwt` verification.
- `src/PublicApi/PublicApi.csproj`: 11 issues, including binary/behavioral API changes, ASP.NET/EF upgrades, and incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` removal.
- `src/Web/Web.csproj`: 13 issues, including binary/source/behavioral API changes plus `Azure.Identity` vulnerability remediation.
- `tests/UnitTests/UnitTests.csproj`: 2 issues, including deprecated xUnit packages.
- `tests/IntegrationTests/IntegrationTests.csproj`: 2 issues, including deprecated xUnit packages.
- `tests/FunctionalTests/FunctionalTests.csproj`: 3 issues, including deprecated xUnit packages and behavioral-change verification.
- `tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj`: 1 issue, primarily behavioral-change verification around upgraded ASP.NET testing packages.
