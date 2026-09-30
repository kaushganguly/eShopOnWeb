# tests/UnitTests/UnitTests.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 5
- **Dependants**: 1
- **Number of Files**: 23
- **Number of Files with Incidents**: 1
- **Lines of Code**: 933
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depends on (5)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj](../projects/ApplicationCore.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorAdmin/BlazorAdmin.csproj](../projects/BlazorAdmin.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorShared/BlazorShared.csproj](../projects/BlazorShared.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Infrastructure/Infrastructure.csproj](../projects/Infrastructure.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Web/Web.csproj](../projects/Web.md)

**Depended on by (1)** — projects that reference this one:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/IntegrationTests/IntegrationTests.csproj](../projects/IntegrationTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P5["<b>📦&nbsp;IntegrationTests.csproj</b><br/><small>net8.0</small>"]
        click P5 "../projects/IntegrationTests.md"
    end
    subgraph current["UnitTests.csproj"]
        MAIN["<b>📦&nbsp;UnitTests.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/UnitTests.md"
    end
    subgraph downstream["Dependencies (5)"]
        P3["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        P8["<b>📦&nbsp;BlazorAdmin.csproj</b><br/><small>net8.0</small>"]
        P9["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
        P2["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
        click P3 "../projects/ApplicationCore.md"
        click P1 "../projects/Web.md"
        click P8 "../projects/BlazorAdmin.md"
        click P9 "../projects/BlazorShared.md"
        click P2 "../projects/Infrastructure.md"
    end
    P5 --> MAIN
    MAIN --> P3
    MAIN --> P1
    MAIN --> P8
    MAIN --> P9
    MAIN --> P2

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1008 |  |
| ***Total APIs Analyzed*** | ***1008*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| xunit | 2.7.0 | — | 🔵 Optional | NuGet package is deprecated |
| xunit.runner.console | 2.7.0 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

