# Modernization Summary: 01-toolchain-baseline

## Task
Align SDK baseline and upgrade backlog for .NET 10 migration.

## Changes Made

### `global.json`
Updated SDK version pin from `8.0.x` to `10.0.x` with `rollForward: latestFeature`. This ensures the entire solution uses the .NET 10 toolchain for all subsequent build and migration steps. The latest available .NET 10 SDK on this machine is `10.0.302`.

### `tasks/01-toolchain-baseline/task.md`
Enriched with research findings:
- Available .NET 10 SDKs listed
- Deferred package backlog documented (6 deprecated, 1 incompatible, 1 framework-included, 1 security-vulnerable, 13 version-bump packages)
- Clear scope boundaries: TargetFramework and NuGet package changes deferred to application tasks

## Build Result
✅ **Build succeeded** (0 errors, 13 pre-existing warnings) after switching to the .NET 10 SDK. All 10 projects compile successfully at `net8.0` using the .NET 10 SDK toolchain.

## Deferred Work Captured

| Category | Packages | Addressed In |
| :--- | :--- | :--- |
| Deprecated packages | AutoMapper.Extensions.DI, xunit v2, xunit.runner.console v2, MSTest.TestAdapter, MSTest.TestFramework, System.IdentityModel.Tokens.Jwt | Package cleanup task |
| Incompatible | Microsoft.VisualStudio.Azure.Containers.Tools.Targets | Application task (PublicApi) |
| Framework-included | System.Security.Claims | ApplicationCore task |
| Security vulnerability | Azure.Identity 1.10.4 | Web application task |
| Version bumps (ASP.NET Core, EF Core, System.*) | 13 packages | Per-application tasks |
