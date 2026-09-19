# .NET Version Upgrade — scenario-instructions.md

## Upgrade Target

- **Target Framework**: .NET 10.0 (LTS — Support ends Nov 2028)
- **Source Framework**: .NET 8.0
- **Solution**: eShopOnWeb.sln
- **Scope**: All projects in solution (Web, PublicApi, ApplicationCore, Infrastructure, BlazorAdmin, BlazorShared, test projects)

## Flow Mode

- **Selected Mode**: Automatic
- **Behavior**: Run end-to-end, only pause if genuinely blocked. Surface assessment, plan, and progress inline — proceed without waiting for user approval.

## Pre-Initialization Status

- ✅ Target framework confirmed: .NET 10.0
- ✅ Solution path verified: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln
- ✅ Flow mode set to Automatic
- ✅ Ready to proceed to Assessment stage

## Decisions

- Upgrade strategy: Single-shot upgrade to .NET 10.0 across all projects
- All NuGet packages will be updated to versions compatible with .NET 10.0
- Follow .NET 10.0 best practices and migration guidance

## User Preferences

(None recorded yet)

