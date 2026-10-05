# Task 03 — Final Validation

## Description

Run a complete, clean validation pass over the fully-upgraded solution: full solution build in
Release configuration, the complete test suite across all 4 test projects, and a final security
vulnerability scan — confirming the upgrade from net8.0 to net10.0 is complete, builds cleanly,
and all tests pass before making the final commit.

## Done when

- `dotnet build eShopOnWeb.sln -c Release` succeeds with 0 errors and 0 code warnings.
- `dotnet test eShopOnWeb.sln -c Release --no-build` passes all tests in all 4 test projects
  (UnitTests, IntegrationTests, FunctionalTests, PublicApiIntegrationTests).
- `dotnet list eShopOnWeb.sln package --vulnerable --include-transitive` output reviewed; any
  remaining vulnerabilities are either fixed or explicitly documented as accepted/deferred risks.
- All project TargetFrameworks confirmed as `net10.0` (via the central `Directory.Packages.props`
  `TargetFramework` property, with no per-project overrides).
