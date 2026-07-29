# 02-fix-project-files: Remove incompatible and redundant package references

## Objective
Remove package references that are incompatible with .NET 10.

## Files Modified
- src/PublicApi/PublicApi.csproj: removed Microsoft.VisualStudio.Azure.Containers.Tools.Targets
- src/ApplicationCore/ApplicationCore.csproj: removed System.Security.Claims
- Directory.Packages.props: removed PackageVersion entries for both packages
