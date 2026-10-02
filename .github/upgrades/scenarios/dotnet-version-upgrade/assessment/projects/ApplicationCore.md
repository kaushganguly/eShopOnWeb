# src/ApplicationCore/ApplicationCore.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 5
- **Number of Files**: 40
- **Number of Files with Incidents**: 2
- **Lines of Code**: 792
- **Estimated LOC to modify**: 2+ (at least 0.3% of the project)

## Related Projects

**Depends on (1)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorShared/BlazorShared.csproj](../projects/BlazorShared.md)

**Depended on by (5)** — projects that reference this one:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Infrastructure/Infrastructure.csproj](../projects/Infrastructure.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/PublicApi/PublicApi.csproj](../projects/PublicApi.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Web/Web.csproj](../projects/Web.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/FunctionalTests/FunctionalTests.csproj](../projects/FunctionalTests.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/UnitTests/UnitTests.csproj](../projects/UnitTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (5)"]
        P4["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
        P5["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
        P6["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        P7["<b>📦&nbsp;FunctionalTests.csproj</b><br/><small>net8.0</small>"]
        P10["<b>📦&nbsp;UnitTests.csproj</b><br/><small>net8.0</small>"]
        click P4 "../projects/Infrastructure.md"
        click P5 "../projects/PublicApi.md"
        click P6 "../projects/Web.md"
        click P7 "../projects/FunctionalTests.md"
        click P10 "../projects/UnitTests.md"
    end
    subgraph current["ApplicationCore.csproj"]
        MAIN["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/ApplicationCore.md"
    end
    subgraph downstream["Dependencies (1)"]
        P3["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
        click P3 "../projects/BlazorShared.md"
    end
    P4 --> MAIN
    P5 --> MAIN
    P6 --> MAIN
    P7 --> MAIN
    P10 --> MAIN
    MAIN --> P3

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 2 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 620 |  |
| ***Total APIs Analyzed*** | ***622*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| System.Security.Claims | 4.3.0 | — | 🔴 Mandatory | NuGet package functionality is included with framework reference |
| System.Text.Json | 8.0.3 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| System.Text.Json | 8.0.3 | 10.0.12 | 🔵 Optional | NuGet package contains security vulnerability |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

