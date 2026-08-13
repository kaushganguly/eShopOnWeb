# 01-update-tfm-and-sdk: Update target framework and SDK version

## Objective
Update the target framework from net8.0 to net10.0 and SDK version from 8.0.x to 10.0.x.

## Affected Files
- `/home/runner/work/eShopOnWeb/eShopOnWeb/Directory.Packages.props` — contains `<TargetFramework>net8.0</TargetFramework>` in PropertyGroup
- `/home/runner/work/eShopOnWeb/eShopOnWeb/global.json` — contains `"version": "8.0.x"`

## Steps
1. Update `Directory.Packages.props`: change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
2. Update `global.json`: change `"version": "8.0.x"` to `"version": "10.0.x"`
