# src/BlazorAdmin/BlazorAdmin.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** AspNetCore
- **Dependencies**: 1
- **Dependants**: 5
- **Number of Files**: 48
- **Number of Files with Incidents**: 3
- **Lines of Code**: 969
- **Estimated LOC to modify**: 6+ (at least 0.6% of the project)

## Related Projects

**Depends on (1)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorShared/BlazorShared.csproj](../projects/BlazorShared.md)

**Depended on by (5)** — projects that reference this one:

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
    subgraph upstream["Dependants (5)"]
        P1["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
        P4["<b>📦&nbsp;UnitTests.csproj</b><br/><small>net8.0</small>"]
        P5["<b>📦&nbsp;IntegrationTests.csproj</b><br/><small>net8.0</small>"]
        P6["<b>📦&nbsp;FunctionalTests.csproj</b><br/><small>net8.0</small>"]
        P10["<b>📦&nbsp;PublicApiIntegrationTests.csproj</b><br/><small>net8.0</small>"]
        click P1 "../projects/Web.md"
        click P4 "../projects/UnitTests.md"
        click P5 "../projects/IntegrationTests.md"
        click P6 "../projects/FunctionalTests.md"
        click P10 "../projects/PublicApiIntegrationTests.md"
    end
    subgraph current["BlazorAdmin.csproj"]
        MAIN["<b>📦&nbsp;BlazorAdmin.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/BlazorAdmin.md"
    end
    subgraph downstream["Dependencies (1)"]
        P9["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
        click P9 "../projects/BlazorShared.md"
    end
    P1 --> MAIN
    P4 --> MAIN
    P5 --> MAIN
    P6 --> MAIN
    P10 --> MAIN
    MAIN --> P9

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 1 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 5 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 882 |  |
| ***Total APIs Analyzed*** | ***888*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.AspNetCore.Components.Authorization | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.Components.WebAssembly | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.Components.WebAssembly.Authentication | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.Extensions.Identity.Core | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.Extensions.Logging.Configuration | 8.0.0 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| System.Net.Http.Json | 8.0.0 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

