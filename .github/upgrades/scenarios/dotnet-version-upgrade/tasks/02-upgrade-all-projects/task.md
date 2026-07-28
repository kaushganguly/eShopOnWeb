# 02-upgrade-all-projects: Upgrade all projects to net10.0 with package updates

## Research Findings

### Environment
- .NET SDK: 10.0.302 (confirmed installed)
- global.json: already pinned to SDK 10.0.302
- ASP.NET Core runtime 10.0.10 confirmed present at `/usr/share/dotnet/packs/Microsoft.AspNetCore.App.Ref/10.0.10`
- **Build tool**: `dotnet build` (all SDK-style, no legacy features)

### TFM Analysis
- `Directory.Packages.props` sets `<TargetFramework>net8.0</TargetFramework>` globally
- None of the 10 individual .csproj files override `TargetFramework` — they all inherit from Directory.Packages.props
- **Action**: Update only Directory.Packages.props TFM from net8.0 to net10.0

### Package Removal Requirements
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`: Referenced in `PublicApi.csproj` AND has a `PackageVersion` entry in Directory.Packages.props — must remove from both
- `System.Security.Claims`: Referenced in `ApplicationCore.csproj` AND has a `PackageVersion` entry — must remove from both
- `Microsoft.AspNetCore.Mvc` 2.2.0: In Directory.Packages.props but NOT referenced by any project file — remove for cleanliness
- `FunctionalTests.csproj` has a legacy `<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" />` — must remove (not supported in modern SDK)

### Source-Incompatible APIs (require code changes)
1. **`Exception(SerializationInfo, StreamingContext)` constructor** — SYSLIB0051 removed in .NET 9+
   - File: `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` line 12
   - Fix: Remove the serialization constructor (only 1 place)

2. **`TimeSpan.FromMinutes(double)`** — SYSLIB0106 obsolete-as-error in .NET 9+
   - File: `src/Web/Configuration/ConfigureCookieSettings.cs` line 26
   - Fix: Change to `TimeSpan.FromMinutes((long)ValidityMinutesPeriod)`
   - Also check: `TimeSpan.FromSeconds(double)` in BlazorAdmin/CustomAuthStateProvider.cs and Web/Extensions/CacheHelpers.cs

### Binary-Incompatible APIs (recompile fixes automatically)
- `ConfigurationBinder.Get<T>(IConfiguration)` — 4 occurrences in PublicApi/Program.cs and Web/Program.cs
- `OptionsConfigurationServiceCollectionExtensions.Configure<T>(IServiceCollection, IConfiguration)` — 4 occurrences
- `ConfigurationBinder.GetValue(...)` — 1 occurrence in Web/Program.cs
- These are binary-incompatible but source-compatible: recompiling against net10.0 should resolve them without code changes

### Packages Kept (deprecated but functional)
- `System.IdentityModel.Tokens.Jwt`: Still actively used in IdentityTokenClaimService.cs and ApiTokenHelper.cs files
- `AutoMapper.Extensions.Microsoft.DependencyInjection`: Actively used in Web and PublicApi DI setup
- `xunit` 2.7.0: Deprecated in favor of v3 but still compatible with net10.0 (targets netstandard2.0)

### Files to Modify
1. `Directory.Packages.props` — version updates, removals
2. `src/PublicApi/PublicApi.csproj` — remove Containers.Tools.Targets reference
3. `src/ApplicationCore/ApplicationCore.csproj` — remove System.Security.Claims reference
4. `tests/FunctionalTests/FunctionalTests.csproj` — remove DotNetCliToolReference
5. `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs` — remove serialization ctor
6. `src/Web/Configuration/ConfigureCookieSettings.cs` — fix TimeSpan.FromMinutes
7. Possibly: `src/BlazorAdmin/CustomAuthStateProvider.cs` — fix TimeSpan.FromSeconds
8. Possibly: `src/Web/Extensions/CacheHelpers.cs` — fix TimeSpan.FromSeconds
