# Progress — 02-upgrade-core-libraries

## Changes Made

- `src/ApplicationCore/ApplicationCore.csproj`: removed `PackageReference` for
  `System.Security.Claims` and `System.Text.Json` (both now framework-provided in net10.0;
  removing `System.Text.Json` also eliminates the NU1903 security vulnerability advisory).
- `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`: removed the obsolete
  `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` binary-formatter
  serialization constructor (SYSLIB0051). No other code in the repo referenced it.
- `Directory.Packages.props`:
  - `Ardalis.GuardClauses` 4.0.1 → 5.0.0
  - `Ardalis.Result` 7.0.0 → 10.1.0
  - `Ardalis.Specification` 7.0.0 → 9.3.1
  - `Ardalis.Specification.EntityFrameworkCore` 7.0.0 → 9.3.1
  - `System.IdentityModel.Tokens.Jwt` 7.3.1 → 8.23.0 (was flagged deprecated by assessment)
  - `FluentValidation` 11.9.0 → 12.1.1
  - Removed `System.Security.Claims` and `System.Text.Json` `PackageVersion` entries (no longer
    referenced by any project after the ApplicationCore cleanup above).
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.InMemory`,
    `Microsoft.EntityFrameworkCore.SqlServer` already bumped to `10.0.12` via the `$(EntityFramworkCoreVersion)`/
    `$(AspNetVersion)` alias properties changed in task 01 — no direct edit needed here.
- `ApplicationCore`, `Infrastructure`, `BlazorShared` all now target `net10.0` automatically — this
  repo has no per-project `<TargetFramework>`; it's centrally defined once in
  `Directory.Packages.props` (imported early via CPM) and inherited by every project, so task 01's
  central bump retargeted the whole solution already.

## Validation

- `dotnet build src/Infrastructure/Infrastructure.csproj` (which also restores/builds its
  project references `ApplicationCore` and `BlazorShared`): **Build succeeded, 0 Warning(s), 0
  Error(s)**. Output assemblies confirmed under `bin/Debug/net10.0/` for all three projects.

## Notes for Later Tasks

- The "no per-project TargetFramework" pattern means tasks 03/04 do **not** need to edit
  `<TargetFramework>` in any `.csproj` — only package references/versions and any breaking-API
  code fixes.
- `System.Text.Json` / `System.Security.Claims` `PackageVersion` entries were removed from CPM;
  if a project in tasks 03/04 needs to reference either explicitly, re-add the `PackageVersion`
  with a net10.0-compatible version at that time (none currently do, per repo-wide grep run in
  task 02).
