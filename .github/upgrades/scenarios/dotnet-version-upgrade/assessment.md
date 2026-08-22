# .NET Version Upgrade Assessment
## eShopOnWeb: net8.0 → net10.0

**Assessment Date**: 2025  
**Target Framework**: net10.0 (LTS, EOL: 2028-11-14)  
**Source Framework**: net8.0  
**Solution**: eShopOnWeb.sln

---

## Projects in Scope

| Project | Type | TF Override |
|---------|------|-------------|
| src/Web | ASP.NET Core MVC/Razor | None (uses central) |
| src/ApplicationCore | Class Library | None |
| src/Infrastructure | Class Library | None |
| src/PublicApi | ASP.NET Core Web API | None |
| src/BlazorShared | Class Library | None |
| src/BlazorAdmin | Blazor WebAssembly | None |
| tests/UnitTests | Test | None |
| tests/IntegrationTests | Test | None |
| tests/PublicApiIntegrationTests | Test | None |
| tests/FunctionalTests | Test | None |

All TargetFramework declarations are centralized in `Directory.Packages.props`.

---

## Package Analysis

### Microsoft Framework Packages (need version update)

| Package | Current | Target | Notes |
|---------|---------|--------|-------|
| AspNetVersion (AspNetCore.*) | 8.0.2 | 10.0.11 | In-box framework |
| SystemExtensionVersion (Extensions.*) | 8.0.0 | 10.0.0 | In-box framework |
| EntityFramworkCoreVersion (EFCore.*) | 8.0.2 | 10.0.11 | Major version aligned |
| VSCodeGeneratorVersion | 8.0.0 | 10.0.2 | Design-time tool |
| System.Text.Json | 8.0.3 | 10.0.0 | In-box in net10 |
| System.Net.Http.Json | 8.0.0 | 10.0.0 | In-box in net10 |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.9.0 | Latest stable |

### Third-Party Packages (compatibility updates)

| Package | Current | Target | Breaking Changes Risk |
|---------|---------|--------|----------------------|
| Ardalis.Specification | 7.0.0 | 9.3.1 | Low — core API stable |
| Ardalis.Specification.EntityFrameworkCore | 7.0.0 | 9.3.1 | Low — required for EF Core 10 |
| Ardalis.Result | 7.0.0 | 10.1.0 | Low — `Result<T>.NotFound()`, implicit cast used |
| Ardalis.GuardClauses | 4.0.1 | 5.0.0 | Low — `Guard.Against.*()` API stable |
| Azure.Identity | 1.10.4 | 1.21.0 | Low — config pattern unchanged |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.1 | 1.5.2 | Low |
| System.IdentityModel.Tokens.Jwt | 7.3.1 | 8.22.0 | Low — `SymmetricSecurityKey` used |
| Swashbuckle.AspNetCore | 6.5.0 | 10.2.3 | Medium — major version, but ISchemaFilter stable |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | 10.2.3 | Medium |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | 10.2.3 | Medium |
| MediatR | 12.0.1 | 14.2.0 | Low — handler signatures unchanged |
| FluentValidation | 11.9.0 | 12.1.1 | Low — not used in source code |
| NSubstitute | 5.1.0 | 6.2.0 | Low — test mocking API stable |
| MSTest.TestAdapter | 3.2.2 | 4.3.3 | Low |
| MSTest.TestFramework | 3.2.2 | 4.3.3 | Low |
| coverlet.collector | 6.0.2 | 10.0.1 | Low |
| xunit | 2.7.0 | 2.9.3 | Low |
| xunit.runner.visualstudio | 2.5.6 | 4.0.0 | Low — supports xunit 2.x |
| xunit.runner.console | 2.7.0 | 2.9.3 | Low |

### Packages Kept Same (already latest)
- Ardalis.ApiEndpoints 4.1.0
- Ardalis.ListStartupServices 1.1.4
- AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
- BlazorInputFile 0.2.0
- Blazored.LocalStorage 4.5.0
- MinimalApi.Endpoint 1.3.0
- System.Security.Claims 4.3.0 (in-box, unused as explicit package)

---

## Risk Items

### High Priority
1. **Swashbuckle 6→10**: Major version jump. `ISchemaFilter.Apply(OpenApiSchema, SchemaFilterContext)` signature is stable but configuration API may differ. `CustomSchemaFilters` uses stable interface — minimal risk.

### Medium Priority
2. **FunctionalTests deprecated tool**: `DotNetCliToolReference` for `dotnet-xunit` v2.3.1 is obsolete and must be removed.
3. **Ardalis.Result 7→10**: Major jump but only `Result<T>.NotFound()` and implicit cast to `T` used — these are stable patterns.

### Low Priority
4. **global.json SDK**: Must be updated from `8.0.x` to `10.0.x`.
5. **Microsoft.AspNetCore.Mvc 2.2.0**: Listed in Directory.Packages.props but not used in any project — harmless, keep.

---

## Summary

- **Projects to upgrade**: 10
- **Packages to update**: 25
- **Deprecated items to remove**: 1 (DotNetCliToolReference)
- **Code changes expected**: Minimal (possible Swashbuckle config adjustment)
- **Overall risk**: Low-Medium
