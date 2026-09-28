# 05-final-validation: Full solution build and test validation

Perform a full-solution build and full test-suite run across all 10 projects
(`ApplicationCore`, `BlazorAdmin`, `BlazorShared`, `Infrastructure`, `PublicApi`, `Web`,
`FunctionalTests`, `IntegrationTests`, `PublicApiIntegrationTests`, `UnitTests`) to
confirm the net10.0 upgrade is complete and correct, and re-check for any remaining
vulnerable NuGet packages (the assessment's confirmed `Azure.Identity` 1.10.4 finding
should now be resolved by the 1.21.0 bump applied in task 01).

**Done when**: `dotnet build eShopOnWeb.sln` succeeds with 0 warnings/0 errors,
`dotnet test eShopOnWeb.sln` passes 100% across all 4 test projects, and
`dotnet list package --vulnerable` reports no vulnerabilities for any of the 10
projects.

## Research Findings

- Re-confirmed via `dotnet list <project> package --vulnerable --include-transitive`
  (run against all 10 projects, both via the solution file and individually for the 4
  projects the solution-level command didn't print, likely due to output buffering) —
  every project reports "has no vulnerable packages given the current sources". The
  previously-flagged `Azure.Identity` 1.10.4 vulnerability (used transitively by `Web`)
  is resolved by the 1.21.0 bump applied centrally in `Directory.Packages.props` (task
  01).
- `docker-compose.dcproj` is excluded from these checks (uses legacy `packages.config`,
  not compatible with the modern `dotnet list package` command) — this project is not
  part of the .NET 10 upgrade scope (it has no `TargetFramework` of its own; it
  orchestrates Docker Compose for local dev/debug only).

## Validation

- `dotnet build eShopOnWeb.sln` — see progress-details.md for full results.
- `dotnet test eShopOnWeb.sln --no-build` — see progress-details.md for full results.
- `dotnet list eShopOnWeb.sln package --vulnerable --include-transitive` (+ per-project
  for the 4 not printed at solution scope) — see progress-details.md.
