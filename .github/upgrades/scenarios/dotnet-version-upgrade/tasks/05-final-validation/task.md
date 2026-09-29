# 05-final-validation: Full solution build and test validation

## Scope

Final end-to-end validation of the whole eShopOnWeb solution after tasks 01-04:
- Full solution build (`eShopOnWeb.sln` and `Everything.sln`) in both Debug and Release configurations.
- Full solution test run (`dotnet test`) across all 4 test projects.
- Solution-wide NuGet vulnerability re-scan (`dotnet list package --vulnerable --include-transitive`)
  to confirm `securityComplianceCheck=true` is satisfied.

## Plan

1. `dotnet build eShopOnWeb.sln -c Debug` and `dotnet build Everything.sln -c Release` — must both
   succeed with 0 errors/warnings (aside from the documented unavoidable NU1901).
2. `dotnet test eShopOnWeb.sln -c Debug` and `-c Release` — must show 0 failures across all 74 tests
   (44 UnitTests + 3 IntegrationTests + 12 FunctionalTests + 15 PublicApiIntegrationTests).
3. `dotnet list Everything.sln package --vulnerable --include-transitive` — confirm only the single
   known/accepted low-severity NU1901 (NuGet.Packaging/NuGet.Protocol 6.12.1, transitive via
   `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2, design-time-only scaffolding tool,
   confirmed latest available version — no fix exists yet) remains, and no High/Critical/Medium
   findings exist anywhere in the graph.
4. Record final results in progress-details.md and mark scenario 100% complete.
5. Per `scenario-instructions.md` Commit Strategy ("Single Commit at End"), stage and commit all
   changes in a single commit once all 5 tasks are complete.
