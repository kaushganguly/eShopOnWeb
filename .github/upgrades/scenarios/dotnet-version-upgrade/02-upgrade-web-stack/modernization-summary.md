# Task 02-upgrade-web-stack: Modernization Summary

## Overview

Upgraded the Web application stack and its shared dependencies to net10.0. The `TargetFramework` was already set to `net10.0` in `Directory.Packages.props` (done in task 01), so this task focused on resolving all build warnings and ensuring clean compilation across all in-scope projects.

## Scope

Projects upgraded:
- `src/Web/Web.csproj`
- `src/ApplicationCore/ApplicationCore.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/BlazorShared/BlazorShared.csproj`
- `src/BlazorAdmin/BlazorAdmin.csproj` (Web dependency, included to fix NU1510 warning)
- `tests/UnitTests/UnitTests.csproj`
- `tests/FunctionalTests/FunctionalTests.csproj`
- `Directory.Packages.props` (security overrides)

## Changes Made

### 1. Removed Obsolete Binary Serialization Constructor (SYSLIB0051)

**File**: `src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs`

Removed the `protected EmptyBasketOnCheckoutException(SerializationInfo, StreamingContext)` constructor. This constructor called the base `Exception(SerializationInfo, StreamingContext)` which is marked as obsolete in .NET 10 (binary serialization is deprecated). The remaining constructors cover all real-world exception creation scenarios.

### 2. Removed Platform-Bundled Package References (NU1510)

**Files**: `src/ApplicationCore/ApplicationCore.csproj`, `src/BlazorAdmin/BlazorAdmin.csproj`

- Removed `System.Text.Json` from `ApplicationCore.csproj` — bundled in .NET 10 BCL
- Removed `System.Net.Http.Json` from `BlazorAdmin.csproj` — bundled in .NET 10 WASM platform

Both namespaces remain fully available at compile and runtime through the framework.

### 3. Removed Unused AutoMapper Reference (NU1903)

**File**: `src/Web/Web.csproj`

Removed `AutoMapper.Extensions.Microsoft.DependencyInjection` package reference from `Web.csproj`. The package was referenced but AutoMapper is only used in `PublicApi`, not in `Web` code. Removing it also eliminates the high-severity vulnerability warning (GHSA-rvv3-g6hj-g44x) for AutoMapper 12.0.1.

### 4. Security: NuGet.Packaging/Protocol Vulnerability Fix (NU1901)

**Files**: `Directory.Packages.props`, `src/Web/Web.csproj`

Added explicit version overrides for NuGet.Packaging and NuGet.Protocol to version **6.12.5** (patched), overriding the vulnerable transitive version 6.12.1 brought in by `Microsoft.VisualStudio.Web.CodeGeneration.Design`. Advisory: GHSA-g4vj-cjjj-v7hg (Defense in Depth update for NuGet Client).

Applied `PrivateAssets="all"` to both overrides to prevent them from leaking into consumers. Also applied proper `PrivateAssets="all"` metadata to `Microsoft.VisualStudio.Web.CodeGeneration.Design` itself (development-only scaffolding tool).

### 5. Fixed xUnit Assertion Style Warnings (xUnit2013)

**Files**: 
- `tests/UnitTests/ApplicationCore/Specifications/CustomerOrdersWithItemsSpecification.cs`
- `tests/UnitTests/ApplicationCore/Entities/BasketTests/BasketRemoveEmptyItems.cs`

Updated collection size assertions to use more expressive xUnit methods:
- `Assert.Equal(1, collection.Count)` → `Assert.Single(collection)`
- `Assert.Equal(0, collection.Count)` → `Assert.Empty(collection)`

## Build Results

| Project | Warnings | Errors |
|---------|----------|--------|
| Web | 0 | 0 |
| ApplicationCore | 0 | 0 |
| Infrastructure | 0 | 0 |
| BlazorShared | 0 | 0 |
| BlazorAdmin | 0 | 0 |

## Test Results

| Test Suite | Passed | Failed | Skipped |
|-----------|--------|--------|---------|
| UnitTests | 44 | 0 | 0 |
| FunctionalTests | 12 | 0 | 0 |

## Target Framework

All projects build on `net10.0` as confirmed by build output showing `bin/Debug/net10.0/` artifacts.
