# src/Infrastructure/Infrastructure.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 3
- **Number of Files**: 30
- **Number of Files with Incidents**: 1
- **Lines of Code**: 2923
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depends on (1)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj](../projects/ApplicationCore.md)

**Depended on by (3)** — projects that reference this one:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/PublicApi/PublicApi.csproj](../projects/PublicApi.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Web/Web.csproj](../projects/Web.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/IntegrationTests/IntegrationTests.csproj](../projects/IntegrationTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (3)"]
        P1["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        P5["<b>📦&nbsp;IntegrationTests.csproj</b><br/><small>net8.0</small>"]
        P7["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
        click P1 "../projects/Web.md"
        click P5 "../projects/IntegrationTests.md"
        click P7 "../projects/PublicApi.md"
    end
    subgraph current["Infrastructure.csproj"]
        MAIN["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/Infrastructure.md"
    end
    subgraph downstream["Dependencies (1)"]
        P3["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
        click P3 "../projects/ApplicationCore.md"
    end
    P1 --> MAIN
    P5 --> MAIN
    P7 --> MAIN
    MAIN --> P3

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 200 |  |
| ***Total APIs Analyzed*** | ***200*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

