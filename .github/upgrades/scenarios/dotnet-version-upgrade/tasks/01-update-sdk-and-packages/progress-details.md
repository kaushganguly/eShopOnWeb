# Progress Details — Task 01

## Files Modified

1. `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json`
   - SDK version: `8.0.x` → `10.0.x`

2. `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props`
   - TargetFramework: `net8.0` → `net10.0`
   - AspNetVersion: `8.0.2` → `10.0.10`
   - SystemExtensionVersion: `8.0.0` → `10.0.10`
   - EntityFramworkCoreVersion: `8.0.2` → `10.0.10`
   - VSCodeGeneratorVersion: `8.0.0` → `10.0.2`
   - Azure.Identity: `1.10.4` → `1.21.0`
   - System.Text.Json: `8.0.3` → `10.0.10`
   - System.IdentityModel.Tokens.Jwt: `7.3.1` → `8.22.0`
   - Removed: System.Security.Claims
   - Removed: Microsoft.VisualStudio.Azure.Containers.Tools.Targets

3. `/home/runner/work/eShopOnWeb/eShopOnWeb/src/ApplicationCore/ApplicationCore.csproj`
   - Removed PackageReference: System.Security.Claims

4. `/home/runner/work/eShopOnWeb/eShopOnWeb/src/PublicApi/PublicApi.csproj`
   - Removed PackageReference: Microsoft.VisualStudio.Azure.Containers.Tools.Targets
