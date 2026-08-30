# Modernization Summary: 001-upgrade-dotnet-to-net10

## finalStatus
success

## successCriteriaStatus
- passBuild: true
- passUnitTests: true

## summary
Successfully upgraded eShopOnWeb from .NET 8.0 to .NET 10.0 (latest LTS). Key changes:

- **global.json**: SDK version updated from `8.0.x` to `10.0.x`
- **Directory.Packages.props**: TFM changed `net8.0` → `net10.0`; all Microsoft.* packages bumped to `10.0.11`; AutoMapper upgraded to `16.2.0`
- **ApplicationCore.csproj**: Removed `System.Security.Claims` and `System.Text.Json` (now built into framework)
- **ApplicationCore/Exceptions**: Removed obsolete `protected (SerializationInfo, StreamingContext)` constructor (API removed in .NET 9+)
- **BlazorAdmin.csproj**: Removed `System.Net.Http.Json` (now built into framework)
- **PublicApi.csproj**: Removed incompatible `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`; pinned `NuGet.Packaging/Protocol 7.9.0`; added AutoMapper 16.2.0
- **PublicApi/Program.cs**: Updated `AddAutoMapper` call to use AutoMapper 16 API (`cfg => cfg.AddProfile<MappingProfile>()`)
- **Web.csproj**: Made CodeGeneration.Design private assets; pinned `NuGet.Packaging/Protocol 7.9.0`
- **ProgramTest.cs**: Fixed ambiguous `Program` type reference
- **Test files (3)**: Fixed xUnit2013 warnings — `Assert.Equal(0/1, count)` → `Assert.Empty()`/`Assert.Single()`

## Build & Test Results
- Build: ✅ 0 errors, 0 warnings
- Unit Tests: ✅ 44/44 passed
- Target Framework: `net10.0` (LTS, support ends November 2028)
