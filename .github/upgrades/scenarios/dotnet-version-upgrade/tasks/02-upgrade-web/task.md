# 02-upgrade-web: Upgrade Web and Dependent Libraries to .NET 10

## Objective
Upgrade the Web project and all its library dependencies (BlazorShared, ApplicationCore, BlazorAdmin, Infrastructure) plus UnitTests to target net10.0 and use .NET 10-compatible package versions.

## Projects in Scope
1. `src/BlazorShared/BlazorShared.csproj` - Level 0 (no project deps)
2. `src/ApplicationCore/ApplicationCore.csproj` - Level 1 (depends on BlazorShared)
3. `src/BlazorAdmin/BlazorAdmin.csproj` - Level 1 (depends on BlazorShared)
4. `src/Infrastructure/Infrastructure.csproj` - Level 2 (depends on ApplicationCore)
5. `src/Web/Web.csproj` - Level 3 (depends on all above)
6. `tests/UnitTests/UnitTests.csproj` - depends on Web and ApplicationCore

## Research Findings

### TargetFramework Status
All projects inherit `<TargetFramework>net10.0</TargetFramework>` from `Directory.Packages.props` (set in task 01). No individual .csproj files needed TFM changes.

### Package Version Issues Found
| Variable/Package | Old | New | Reason |
|---|---|---|---|
| `EntityFramworkCoreVersion` | 8.0.2 | 10.0.9 | Upgrade to .NET 10 |
| `SystemExtensionVersion` | 8.0.0 | 10.0.0 | System/Extensions packages follow .NET version |
| `VSCodeGeneratorVersion` | 8.0.0 | 10.0.2 | Upgrade to .NET 10 |
| `Azure.Identity` | 1.10.4 | 1.14.2 | Security vuln fix; EF Core SqlServer 10.0.9 requires >=1.14.2 |
| `System.Text.Json` | 8.0.3 | 10.0.0 | .NET 10 version |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | 8.0.1 | JwtBearer 10.0.9 requires >=8.0.1 transitively |

### Source Code Issues Found
| File | Issue | Fix Applied |
|---|---|---|
| `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` | `Exception(SerializationInfo, StreamingContext)` constructor removed in .NET 9+ (source incompatible) | Removed the obsolete serialization constructor |

### Unnecessary Packages Removed
| Project | Package | Reason |
|---|---|---|
| `ApplicationCore.csproj` | `System.Security.Claims` | Included in .NET 10 framework (NU1510) |
| `ApplicationCore.csproj` | `System.Text.Json` | Included in .NET 10 framework (NU1510) |
| `BlazorAdmin.csproj` | `System.Net.Http.Json` | Included in .NET 10 framework (NU1510) |
| `Web.csproj` | `AutoMapper.Extensions.Microsoft.DependencyInjection` | Not used in Web project |
| `Web.csproj` | `Microsoft.VisualStudio.Web.CodeGeneration.Design` | Dev scaffolding tool; its NuGet tooling transitive deps had low severity vulns |

### Test Code Warnings Fixed
| File | Warning | Fix |
|---|---|---|
| `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs:22` | xUnit2013: Use Assert.Single instead of Assert.Equal(1,...) | Changed to `Assert.Single()` |
| `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs:35` | xUnit2013: Use Assert.Single instead of Assert.Equal(1,...) | Changed to `Assert.Single()` |
| `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs:19` | xUnit2013: Use Assert.Empty instead of Assert.Equal(0,...) | Changed to `Assert.Empty()` |

## Execution Steps

1. Update `Directory.Packages.props`: Update version variables and individual package versions
2. Remove unused/framework-included packages from individual .csproj files
3. Fix source-incompatible `EmptyBasketOnCheckoutException` serialization constructor
4. Fix xUnit analyzer warnings in test files
5. Build and verify all projects
6. Run UnitTests

## Build Results
- All 6 in-scope projects: Build succeeded with 0 errors, 0 warnings
- UnitTests: 44/44 passed
