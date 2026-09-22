# Projects Relationship Graph

[← Back to the assessment index](../assessment.md)

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
    P2["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
    P3["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
    P4["<b>📦&nbsp;UnitTests.csproj</b><br/><small>net8.0</small>"]
    P5["<b>📦&nbsp;IntegrationTests.csproj</b><br/><small>net8.0</small>"]
    P6["<b>📦&nbsp;FunctionalTests.csproj</b><br/><small>net8.0</small>"]
    P7["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
    P8["<b>📦&nbsp;BlazorAdmin.csproj</b><br/><small>net8.0</small>"]
    P9["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
    P10["<b>📦&nbsp;PublicApiIntegrationTests.csproj</b><br/><small>net8.0</small>"]
    P1 --> P3
    P1 --> P8
    P1 --> P9
    P1 --> P2
    P2 --> P3
    P3 --> P9
    P4 --> P3
    P4 --> P1
    P5 --> P2
    P5 --> P4
    P6 --> P3
    P6 --> P7
    P6 --> P1
    P7 --> P3
    P7 --> P2
    P8 --> P9
    P10 --> P7
    P10 --> P1
    click P1 "projects/Web.md"
    click P2 "projects/Infrastructure.md"
    click P3 "projects/ApplicationCore.md"
    click P4 "projects/UnitTests.md"
    click P5 "projects/IntegrationTests.md"
    click P6 "projects/FunctionalTests.md"
    click P7 "projects/PublicApi.md"
    click P8 "projects/BlazorAdmin.md"
    click P9 "projects/BlazorShared.md"
    click P10 "projects/PublicApiIntegrationTests.md"

```

