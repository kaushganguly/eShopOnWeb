# Upgrade Options

## Context
- Scenario: .NET version upgrade
- Target Framework: net10.0
- Complexity: Simple
- Projects: 10 total (6 source, 4 test)
- Current State: All projects already use SDK-style format, central package management, and a centrally managed TargetFramework.

## Recommended Options

### 1. Upgrade Strategy
- **Recommended**: All-at-Once
- **Why**: All projects already target net8.0, use the same centrally managed TargetFramework, and have no .NET Framework or legacy project system blockers. Updating the shared target framework and package set together is the lowest-complexity path.
- **Alternative**: Staged by dependency layer, only if validation reveals unexpected package or API regressions.

### 2. Package Management
- **Recommended**: Keep Central Package Management as-is
- **Why**: Directory.Packages.props is already in use, so package upgrades can be coordinated once for the whole solution.
- **Notes**:
  - Prioritize Azure.Identity due to the reported security vulnerability.
  - Upgrade the 19 recommended packages during the same pass.
  - Review deprecated packages for replacement or removal where practical.
  - Remove or replace Microsoft.VisualStudio.Azure.Containers.Tools.Targets if it blocks net10.0 compatibility.

### 3. Nullable Annotations
- **Recommended**: Defer nullable annotation expansion
- **Why**: This upgrade is a target framework and package alignment exercise, not a language-safety modernization. Enabling or tightening nullable analysis can be handled separately to reduce churn.
- **Alternative**: Optionally enable stricter nullable analysis after the framework upgrade is stable.

### 4. Validation Scope
- **Recommended**: Full solution restore, build, and test after the single-pass upgrade
- **Why**: The dependency graph is interconnected, so validation should cover the whole solution once the central framework and package versions are updated.
- **Notes**:
  - Pay extra attention to ASP.NET Core, EF Core, and test host packages moving from 8.x to 10.x.
  - Re-run affected functional and integration tests to catch behavioral changes noted in assessment.

## Summary
This upgrade is best handled as a single coordinated solution-wide update: bump the centrally managed TargetFramework to net10.0, update centrally managed packages together, resolve the known incompatible/deprecated package exceptions, and validate the full solution in one pass.
