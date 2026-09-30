# tests/IntegrationTests/IntegrationTests.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 6
- **Dependants**: 0
- **Number of Files**: 3
- **Number of Files with Incidents**: 1
- **Lines of Code**: 147
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depends on (6)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj](../projects/ApplicationCore.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorAdmin/BlazorAdmin.csproj](../projects/BlazorAdmin.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorShared/BlazorShared.csproj](../projects/BlazorShared.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Infrastructure/Infrastructure.csproj](../projects/Infrastructure.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Web/Web.csproj](../projects/Web.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/UnitTests/UnitTests.csproj](../projects/UnitTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["IntegrationTests.csproj"]
        MAIN["<b>📦&nbsp;IntegrationTests.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/IntegrationTests.md"
    end
    subgraph downstream["Dependencies (6)"]
        P2["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
        P4["<b>📦&nbsp;UnitTests.csproj</b><br/><small>net8.0</small>"]
        P3["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
        P8["<b>📦&nbsp;BlazorAdmin.csproj</b><br/><small>net8.0</small>"]
        P9["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        click P2 "../projects/Infrastructure.md"
        click P4 "../projects/UnitTests.md"
        click P3 "../projects/ApplicationCore.md"
        click P8 "../projects/BlazorAdmin.md"
        click P9 "../projects/BlazorShared.md"
        click P1 "../projects/Web.md"
    end
    MAIN --> P2
    MAIN --> P4
    MAIN --> P3
    MAIN --> P8
    MAIN --> P9
    MAIN --> P1

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 234 |  |
| ***Total APIs Analyzed*** | ***234*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| xunit | 2.7.0 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

