# 01-toolchain-baseline: Align SDK baseline and upgrade backlog

Confirm the repository can build against .NET 10 before project files start moving. This task covers the global.json update path, SDK availability, and a quick review of the deferred-package backlog so later tasks can focus on mandatory target-framework and API work instead of rediscovering package constraints.

It also captures the known exceptions that will intentionally be postponed until the dedicated cleanup task: deprecated or incompatible packages in Web, PublicApi, Infrastructure, and the test projects, plus framework-included packages that should be removed instead of upgraded.

**Done when**: global.json and toolchain expectations are aligned for net10.0, the solution has a clear deferred-package list, and the repository is ready for application-by-application execution.

---

## Findings

### .NET 10 SDK Availability

The following .NET 10 SDKs are installed on this machine:

- `10.0.109`
- `10.0.204`
- `10.0.301`
- `10.0.302` ← latest

**global.json update**: Changed `"version": "8.0.x"` → `"10.0.x"` with `"rollForward": "latestFeature"` to pin to the .NET 10 feature band and roll forward to the latest patch.

### Central Package Management

`Directory.Packages.props` sets `<TargetFramework>net8.0</TargetFramework>` centrally. This value will be changed to `net10.0` in the per-application upgrade tasks, not here.

### Project TargetFramework status (all net8.0 — unchanged by this task)

All 10 projects target `net8.0`. Upgrade to `net10.0` is deferred to their respective application tasks.

---

## Deferred Package Backlog

The following packages are flagged as deprecated, incompatible, or framework-included. They are **not** addressed in this task. They will be resolved in the dedicated package-cleanup task (after all projects are on net10.0) or in the application tasks if they block compilation.

### Deprecated packages (replace in cleanup task)

| Package | Version | Used In | Recommended Replacement |
| :--- | :--- | :--- | :--- |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | Web, PublicApi | Replace with AutoMapper 13.x direct DI registration or Mapperly |
| `xunit` | 2.7.0 | FunctionalTests, IntegrationTests, UnitTests | Upgrade to xunit v3 (3.x) |
| `xunit.runner.console` | 2.7.0 | UnitTests | Upgrade to xunit.runner.console v3 |
| `MSTest.TestAdapter` | 3.2.2 | PublicApiIntegrationTests | Replace with `Microsoft.Testing.Platform` / MSTest 3.4+ |
| `MSTest.TestFramework` | 3.2.2 | PublicApiIntegrationTests | Replace with `Microsoft.Testing.Platform` / MSTest 3.4+ |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | Infrastructure, PublicApi, Web | Replace with `Microsoft.IdentityModel.Tokens` (8.x+) |

### Incompatible packages (remove or replace in application tasks)

| Package | Version | Used In | Action |
| :--- | :--- | :--- | :--- |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | 1.19.6 | PublicApi | Remove — no net10.0-compatible version exists; VS tooling is not required at build time |

### Framework-included packages (remove — no NuGet reference needed)

| Package | Version | Used In | Reason |
| :--- | :--- | :--- | :--- |
| `System.Security.Claims` | 4.3.0 | ApplicationCore | Included in the .NET runtime; NuGet reference is redundant and should be removed |

### Security vulnerability (address in Web application task)

| Package | Current | Suggested | Used In |
| :--- | :--- | :--- | :--- |
| `Azure.Identity` | 1.10.4 | 1.21.0 | Web | 

### Packages requiring upgrade when TargetFramework is updated (handled per application task)

| Package | Current | Suggested |
| :--- | :--- | :--- |
| `Microsoft.AspNetCore.*` (all 8.0.x) | 8.0.2 | 10.0.x |
| `Microsoft.EntityFrameworkCore.*` (all 8.0.x) | 8.0.2 | 10.0.x |
| `Microsoft.Extensions.*` (all 8.0.x) | 8.0.0–8.0.2 | 10.0.x |
| `System.Net.Http.Json` | 8.0.0 | 10.0.x |
| `System.Text.Json` | 8.0.3 | 10.0.x |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 8.0.0 | 10.0.2 |
