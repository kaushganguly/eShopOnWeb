# tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 0
- **Number of Files**: 8
- **Number of Files with Incidents**: 6
- **Lines of Code**: 318
- **Estimated LOC to modify**: 7+ (at least 2.2% of the project)

## Related Projects

**Depends on (2)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/PublicApi/PublicApi.csproj](../projects/PublicApi.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Web/Web.csproj](../projects/Web.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["PublicApiIntegrationTests.csproj"]
        MAIN["<b>📦&nbsp;PublicApiIntegrationTests.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/PublicApiIntegrationTests.md"
    end
    subgraph downstream["Dependencies (2)"]
        P5["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
        P6["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        click P5 "../projects/PublicApi.md"
        click P6 "../projects/Web.md"
    end
    MAIN --> P5
    MAIN --> P6

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 7 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 217 |  |
| ***Total APIs Analyzed*** | ***224*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| MSTest.TestAdapter | 3.2.2 | — | 🔵 Optional | NuGet package is deprecated |
| MSTest.TestFramework | 3.2.2 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

