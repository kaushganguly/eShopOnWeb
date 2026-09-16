# Scenario: .NET Version Upgrade (dotnet-version-upgrade)

## Execution Parameters

### Target Framework
- **Proposed**: net10.0 (LTS, support until 2028-11-14)
- **Choice**: net10.0 ✓

### Solution
- **Path**: /home/runner/work/eShopOnWeb/eShopOnWeb/eShopOnWeb.sln

### Flow Mode
- **Selected**: automatic
- **Behavior**: Run end-to-end without pausing for confirmation after assessment and plan. Only pause if blocked or missing critical information.

### Source Control
- **Type**: git
- **Working Branch**: upgrade/dotnet-10-lts
- **Base Commit**: 87e8b9e (HEAD -> main)
- **Strategy**: Create workflow folder on working branch, commit changes at end of each stage

## Execution Strategy

1. **Assessment** — Analyze all projects, dependencies, breaking changes
2. **Planning** — Create upgrade plan with task breakdown
3. **Execution** — Apply target framework changes, resolve breaking changes, validate

## Success Criteria
- All projects compile cleanly (zero warnings)
- All unit tests pass
- All integration tests pass
- No security vulnerabilities introduced

## User Preferences
- Flow Mode: automatic (no pauses except when blocked)
- Accept all defaults for package versions and compatibility recommendations

---
*Document created during workflow initialization*
