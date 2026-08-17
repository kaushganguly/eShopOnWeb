# Dependency Map

eShopOnWeb declares 36 central NuGet package versions across production and test projects, with ASP.NET Core, EF Core, Identity, Blazor, and Ardalis libraries as the primary dependency groups.

## Dependencies

```mermaid
flowchart LR
    App["eShopOnWeb"]

    subgraph Web["Web Frameworks"]
        AspNetCore["ASP NET Core packages v8.0.2"]
        Blazor["Blazor WebAssembly v8.0.2"]
        ArdalisApi["Ardalis ApiEndpoints v4.1.0"]
        MinimalApi["MinimalApi Endpoint v1.3.0"]
    end
    subgraph DB["Database ORM"]
        EFSql["EF Core SQL Server v8.0.2"]
        EFMemory["EF Core InMemory v8.0.2"]
        IdentityEf["Identity EF Core v8.0.2"]
        ArdalisSpecEf["Ardalis Specification EF v7.0.0"]
    end
    subgraph Sec["Security"]
        JwtBearer["JWT ******"]
        JwtTokens["JWT Tokens v7.3.1"]
        Claims["System Security Claims v4.3.0"]
    end
    subgraph Config["Configuration"]
        AzureSecrets["Azure Key Vault Secrets v1.3.1"]
        AzureIdentity["Azure Identity v1.10.4"]
        LoggingConfig["Logging Configuration v8.0.0"]
    end
    subgraph ApiDocs["API Documentation"]
        Swagger["Swashbuckle v6.5.0"]
    end
    subgraph Util["Utilities"]
        AutoMapper["AutoMapper DI v12.0.1"]
        MediatR["MediatR v12.0.1"]
        Guard["Ardalis GuardClauses v4.0.1"]
        Result["Ardalis Result v7.0.0"]
        FluentValidation["FluentValidation v11.9.0"]
        LocalStorage["Blazored LocalStorage v4.5.0"]
        Bundler["Bundler Minifier v3.2.449"]
    end

    App -->|"web"| Web
    App -->|"persistence"| DB
    App -->|"security"| Sec
    App -->|"configuration"| Config
    App -->|"documentation"| ApiDocs
    App -->|"utilities"| Util
    AspNetCore -.->|"framework reference"| IdentityEf
    EFSql -.->|"supports"| IdentityEf
```

### Dependency Summary

| Category | Count | Key Libraries | Notes |
|---|---:|---|---|
| Web Frameworks | 8 | ASP.NET Core packages, Blazor WebAssembly, Ardalis.ApiEndpoints, MinimalApi.Endpoint | Main web and API hosting stack |
| Database / ORM | 4 | EF Core SQL Server, EF Core InMemory, Identity EF Core, Ardalis Specification EF | SQL Server runtime and InMemory test option |
| Security | 3 | JWT Bearer, System.IdentityModel.Tokens.Jwt, System.Security.Claims | API bearer auth and claims generation |
| Observability / API Docs | 3 | Swashbuckle.AspNetCore packages | Swagger and OpenAPI UI for PublicApi |
| Configuration | 3 | Azure Key Vault Secrets, Azure Identity, Logging Configuration | Production secret loading and logging configuration |
| Utilities | 10 | AutoMapper, MediatR, Ardalis, FluentValidation, Blazored.LocalStorage | Mapping, mediator handlers, guard clauses, admin client helpers |
| Build Tooling | 5 | Web CodeGeneration, Containers Tools Targets, LibraryManager, BundlerMinifier | Build and development support packages |

### Version & Compatibility Risks

The solution targets net8.0 and uses centrally managed package versions. The .NET upgrade assessment flags several NuGet upgrade, deprecation, compatibility, and vulnerability findings for a future net10.0 move; packages such as older MVC tooling, container tooling, and some framework-included packages should be reviewed during modernization.

### Notable Observations

- Central package management in `Directory.Packages.props` keeps versions consistent across projects.
- EF Core SQL Server and EF Core InMemory are both declared, enabling runtime SQL Server and test or demo in-memory modes.
- PublicApi combines Minimal API endpoint registration with Ardalis.ApiEndpoints for authentication.
- Azure Key Vault and Azure Identity dependencies are only used by the Web project production startup path.

## Test Dependencies

| Framework | Version | Notes |
|---|---:|---|
| xUnit | 2.7.0 | Unit, integration, and functional tests |
| xUnit runner visualstudio | 2.5.6 | Test discovery in Visual Studio and dotnet test |
| xUnit runner console | 2.7.0 | Console runner dependency in UnitTests |
| Microsoft.NET.Test.Sdk | 17.9.0 | Common test SDK |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.2 | Functional and API integration test hosting |
| MSTest TestFramework and Adapter | 3.2.2 | PublicApiIntegrationTests use MSTest |
| NSubstitute and analyzers | 5.1.0 / 1.0.17 | Test doubles and analyzer support |
| coverlet.collector | 6.0.2 | Coverage collector for PublicApiIntegrationTests |

Total test-scope dependencies: 9
The repository has mature unit, integration, functional, and API integration test infrastructure. The mixed xUnit and MSTest usage is intentional by project but should be considered when consolidating test tooling.
