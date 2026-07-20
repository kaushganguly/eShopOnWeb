# 02-core-upgrade: Upgrade all projects to net10.0 — TFM, packages, and code fixes

## Research Findings

### CatalogSettings Configuration
- `CatalogBaseUrl` is defined at **root level** in appsettings.json (not in a named section)
- Therefore `configuration.GetSection("").Get<CatalogSettings>()` and `services.AddOptions<CatalogSettings>().Bind(configuration)` are correct for binding from root

### AutoMapper Migration
- Original: `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1 → transitioned to `AutoMapper` package
- Vulnerability GHSA-rvv3-g6hj-g44x affects AutoMapper < 15.1.1 and 16.0.0 ≤ AutoMapper < 16.1.1
- Updated to AutoMapper 15.1.3 (safe, from LuckyPennySoftware fork)
- API changed in 15.x: `AddAutoMapper(Assembly)` removed; use `AddAutoMapper(cfg => cfg.AddProfile<T>())`

### NuGet.Packaging / NuGet.Protocol
- Vulnerability GHSA-g4vj-cjjj-v7hg: affects NuGet.Packaging 6.12.0–6.12.4; fixed in 6.12.5
- Source: `Microsoft.VisualStudio.Web.CodeGeneration.Design` 10.0.2 depends on NuGet.Packaging 6.12.1
- Fix: Made CodeGeneration.Design `PrivateAssets="all"` + directly referenced NuGet.Packaging/Protocol at 6.12.5

### .NET 10 Breaking Changes Applied
1. `EmptyBasketOnCheckoutException` serialization constructor removed (SYSLIB0051)
2. `Configure<T>(IConfiguration)` replaced with `AddOptions<T>().Bind(configuration)` (binary incompatible)
3. `IConfiguration.Get<T>()` replaced with `configuration.GetSection("").Get<T>()` (binary incompatible)
4. `TimeSpan.FromMinutes(int)` cast to `(double)` to resolve ambiguity (source incompatible)
5. Top-level `Program` class conflict: added `internal partial class Program {}` to Web/Program.cs

## Status: COMPLETED
