# src/Infrastructure/Infrastructure.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 6
- **Number of Files**: 30
- **Number of Files with Incidents**: 2
- **Lines of Code**: 2923
- **Estimated LOC to modify**: 4+ (at least 0.1% of the project)

## Related Projects

**Depends on (2)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj](../projects/ApplicationCore.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorShared/BlazorShared.csproj](../projects/BlazorShared.md)

**Depended on by (6)** — projects that reference this one:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/PublicApi/PublicApi.csproj](../projects/PublicApi.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Web/Web.csproj](../projects/Web.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/FunctionalTests/FunctionalTests.csproj](../projects/FunctionalTests.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/IntegrationTests/IntegrationTests.csproj](../projects/IntegrationTests.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj](../projects/PublicApiIntegrationTests.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/UnitTests/UnitTests.csproj](../projects/UnitTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (6)"]
        P1["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        P4["<b>📦&nbsp;UnitTests.csproj</b><br/><small>net8.0</small>"]
        P5["<b>📦&nbsp;IntegrationTests.csproj</b><br/><small>net8.0</small>"]
        P6["<b>📦&nbsp;FunctionalTests.csproj</b><br/><small>net8.0</small>"]
        P7["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
        P10["<b>📦&nbsp;PublicApiIntegrationTests.csproj</b><br/><small>net8.0</small>"]
        click P1 "../projects/Web.md"
        click P4 "../projects/UnitTests.md"
        click P5 "../projects/IntegrationTests.md"
        click P6 "../projects/FunctionalTests.md"
        click P7 "../projects/PublicApi.md"
        click P10 "../projects/PublicApiIntegrationTests.md"
    end
    subgraph current["Infrastructure.csproj"]
        MAIN["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/Infrastructure.md"
    end
    subgraph downstream["Dependencies (2)"]
        P3["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
        P9["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
        click P3 "../projects/ApplicationCore.md"
        click P9 "../projects/BlazorShared.md"
    end
    P1 --> MAIN
    P4 --> MAIN
    P5 --> MAIN
    P6 --> MAIN
    P7 --> MAIN
    P10 --> MAIN
    MAIN --> P3
    MAIN --> P9

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 4 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 4248 |  |
| ***Total APIs Analyzed*** | ***4252*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

## Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| IdentityModel & Claims-based Security | 4 | 100.0% | Windows Identity Foundation (WIF), SAML, and claims-based authentication APIs that have been replaced by modern identity libraries. WIF was the original identity framework for .NET Framework. Migrate to Microsoft.IdentityModel.* packages (modern identity stack). |

