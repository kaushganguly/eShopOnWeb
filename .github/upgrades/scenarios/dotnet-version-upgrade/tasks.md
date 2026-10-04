# Tasks — dotnet-version-upgrade (net8.0 -> net10.0)

- ✅ 01-update-sdk-and-tfm: Update global.json SDK pin and central TargetFramework
- ✅ 02-update-package-versions: Update all PackageVersion entries to net10.0-compatible versions
- ✅ 03-update-docker-base-images: Bump Dockerfile base images to .NET 10
- ✅ 04-build-and-fix-applicationcore-chain: Build & fix BlazorShared/ApplicationCore/Infrastructure
- ✅ 05-build-and-fix-web-publicapi-blazoradmin: Build & fix BlazorAdmin/Web/PublicApi (Swashbuckle/Microsoft.OpenApi v2 migration, AutoMapper v16 migration, BlazorInputFile removal)
- ✅ 06-build-and-fix-tests: Build & fix all test projects (xUnit2013/MSTEST0044 warnings, Program ambiguity via extern alias, Blazor WASM assembly isolation)
- ✅ 07-run-tests-and-validate: Run full test suite and validate success criteria — 74/74 tests passed, 0 warnings, 0 errors, 0 vulnerable packages
