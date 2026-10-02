# Projects Relationship Graph

[← Back to the assessment index](../assessment.md)

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;ApplicationCore.csproj</b><br/><small>net8.0</small>"]
    P2["<b>📦&nbsp;BlazorAdmin.csproj</b><br/><small>net8.0</small>"]
    P3["<b>📦&nbsp;BlazorShared.csproj</b><br/><small>net8.0</small>"]
    P4["<b>📦&nbsp;Infrastructure.csproj</b><br/><small>net8.0</small>"]
    P5["<b>📦&nbsp;PublicApi.csproj</b><br/><small>net8.0</small>"]
    P6["<b>📦&nbsp;Web.csproj</b><br/><small>net8.0</small>"]
    P7["<b>📦&nbsp;FunctionalTests.csproj</b><br/><small>net8.0</small>"]
    P8["<b>📦&nbsp;IntegrationTests.csproj</b><br/><small>net8.0</small>"]
    P9["<b>📦&nbsp;PublicApiIntegrationTests.csproj</b><br/><small>net8.0</small>"]
    P10["<b>📦&nbsp;UnitTests.csproj</b><br/><small>net8.0</small>"]
    P1 --> P3
    P2 --> P3
    P4 --> P1
    P5 --> P1
    P5 --> P4
    P6 --> P1
    P6 --> P2
    P6 --> P3
    P6 --> P4
    P7 --> P1
    P7 --> P5
    P7 --> P6
    P8 --> P4
    P8 --> P10
    P9 --> P5
    P9 --> P6
    P10 --> P1
    P10 --> P6
    click P1 "projects/ApplicationCore.md"
    click P2 "projects/BlazorAdmin.md"
    click P3 "projects/BlazorShared.md"
    click P4 "projects/Infrastructure.md"
    click P5 "projects/PublicApi.md"
    click P6 "projects/Web.md"
    click P7 "projects/FunctionalTests.md"
    click P8 "projects/IntegrationTests.md"
    click P9 "projects/PublicApiIntegrationTests.md"
    click P10 "projects/UnitTests.md"

```

