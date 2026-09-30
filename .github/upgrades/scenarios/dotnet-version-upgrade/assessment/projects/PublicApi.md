# src/PublicApi/PublicApi.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** AspNetCore
- **Dependencies**: 3
- **Dependants**: 2
- **Number of Files**: 38
- **Number of Files with Incidents**: 2
- **Lines of Code**: 1070
- **Estimated LOC to modify**: 15+ (at least 1.4% of the project)

## Related Projects

**Depends on (3)** — projects this one references:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj](../projects/ApplicationCore.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/BlazorShared/BlazorShared.csproj](../projects/BlazorShared.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/src/Infrastructure/Infrastructure.csproj](../projects/Infrastructure.md)

**Depended on by (2)** — projects that reference this one:

- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/FunctionalTests/FunctionalTests.csproj](../projects/FunctionalTests.md)
- [/home/runner/work/eShopOnWeb/eShopOnWeb/tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj](../projects/PublicApiIntegrationTests.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (2)"]
        P6["<b>📦&nbsp;FunctionalTests.csproj</b><br/><small>net8.0</small>"]
        P10["<b>📦&nbsp;PublicApiIntegrationTests.csproj</b><br/><small>net8.0</small>"]
        click P6 "../projects/FunctionalTests.md"
        click P10 "../projects/PublicApiIntegrationTests.md"
    end
    subgraph current["PublicApi.csproj"]
        MAIN["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
        click MAIN "../projects/PublicApi.md"
    end
    subgraph downstream["Dependencies (3)"]
        P3["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
        P2["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
        P9["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
        click P3 "../projects/ApplicationCore.md"
        click P2 "../projects/Infrastructure.md"
        click P9 "../projects/BlazorShared.md"
    end
    P6 --> MAIN
    P10 --> MAIN
    MAIN --> P3
    MAIN --> P2
    MAIN --> P9

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 5 | High - Require code changes |
| 🟡 Source Incompatible | 9 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 1 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1249 |  |
| ***Total APIs Analyzed*** | ***1264*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | — | 🔵 Optional | NuGet package is deprecated |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.AspNetCore.Identity.UI | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.Tools | 8.0.2 | 10.0.12 | 🟡 Potential | NuGet package upgrade is recommended |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | — | 🔴 Mandatory | NuGet package is incompatible |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 | 10.0.2 | 🟡 Potential | NuGet package upgrade is recommended |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

