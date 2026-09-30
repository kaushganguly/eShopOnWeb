# tests/FunctionalTests/FunctionalTests.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 6
- **Dependants**: 0
- **Number of Files**: 12
- **Number of Files with Incidents**: 9
- **Lines of Code**: 633
- **Estimated LOC to modify**: 34+ (at least 5.4% of the project)

## Related Projects

**Depends on (6)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj](../projects/ApplicationCore.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorAdmin/BlazorAdmin.csproj](../projects/BlazorAdmin.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorShared/BlazorShared.csproj](../projects/BlazorShared.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Infrastructure/Infrastructure.csproj](../projects/Infrastructure.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/PublicApi/PublicApi.csproj](../projects/PublicApi.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Web/Web.csproj](../projects/Web.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["FunctionalTests.csproj"]
        MAIN["<b>📦&nbsp;FunctionalTests.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/FunctionalTests.md"
    end
    subgraph downstream["Dependencies (6)"]
        P3["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
        P7["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        P8["<b>📦&nbsp;BlazorAdmin.csproj</b><br/><small>net8.0</small>"]
        P9["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
        P2["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
        click P3 "../projects/ApplicationCore.md"
        click P7 "../projects/PublicApi.md"
        click P1 "../projects/Web.md"
        click P8 "../projects/BlazorAdmin.md"
        click P9 "../projects/BlazorShared.md"
        click P2 "../projects/Infrastructure.md"
    end
    MAIN --> P3
    MAIN --> P7
    MAIN --> P1
    MAIN --> P8
    MAIN --> P9
    MAIN --> P2

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 4 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 30 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 882 |  |
| ***Total APIs Analyzed*** | ***916*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| xunit | 2.7.0 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

## Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| IdentityModel & Claims-based Security | 4 | 11.8% | Windows Identity Foundation (WIF), SAML, and claims-based authentication APIs that have been replaced by modern identity libraries. WIF was the original identity framework for .NET Framework. Migrate to Microsoft.IdentityModel.* packages (modern identity stack). |

