# 03-web-and-api-hosts: 03-web-and-api-hosts

Execute task 03-web-and-api-hosts.

## Research Findings
See `progress-details.md` in this task folder for the full breakdown of API breaking changes
discovered and fixed (Microsoft.OpenApi namespace consolidation, OpenApiReference removal,
AddSecurityRequirement signature change, AutoMapper.Extensions.Microsoft.DependencyInjection
migration, NU1510/NU1901 warning cleanup).

## Actions Taken
1. Fixed `CustomSchemaFilters.cs` and `Program.cs` in PublicApi for the Microsoft.OpenApi 2.x / Swashbuckle.AspNetCore 10.x breaking changes.
2. Migrated PublicApi off `AutoMapper.Extensions.Microsoft.DependencyInjection` (vulnerable, unmaintained) to the core `AutoMapper` 16.2.0 package; removed the same unused reference from Web.
3. Removed NU1510-flagged explicit `PackageReference`s that are now part of the net10.0 shared framework (`System.Security.Claims`, `System.Text.Json` in ApplicationCore — task 02; `System.Net.Http.Json` in BlazorAdmin — this task).
4. Removed the unused `Microsoft.VisualStudio.Web.CodeGeneration.Design` scaffolding tool package from Web and PublicApi (eliminated NU1901 transitive vulnerability warnings; package had no newer version to fix them and is unused by application code).
5. Validated clean builds (0 warnings/0 errors) for PublicApi, BlazorAdmin, and Web.
