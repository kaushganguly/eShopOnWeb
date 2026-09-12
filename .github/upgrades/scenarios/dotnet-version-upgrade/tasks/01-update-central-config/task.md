# Task: Update Central Configuration to .NET 10

## Objective
Update Directory.Packages.props and global.json to target .NET 10.0, and update all centrally managed package versions for .NET 10 compatibility.

## Files to Modify

1. **global.json**
   - Update SDK version from "8.0.x" to "10.0.x"

2. **Directory.Packages.props**
   - Update `<TargetFramework>` from net8.0 to net10.0
   - Update `<AspNetVersion>` from 8.0.2 to 10.0.0
   - Update `<SystemExtensionVersion>` from 8.0.0 to 10.0.0
   - Update `<EntityFramworkCoreVersion>` from 8.0.2 to 10.0.0
   - Update `<VSCodeGeneratorVersion>` from 8.0.0 to 10.0.0
   - Update package versions to .NET 10 compatible versions

## Research Notes

### Centrally Managed Packages
The solution uses `Directory.Packages.props` with `<ManagePackageVersionsCentrally>true`. All package versions are defined in this file and inherited by all projects.

### Key Package Updates Needed
- Microsoft.AspNetCore.* packages → 10.0.0
- Microsoft.EntityFrameworkCore.* packages → 10.0.0
- System.* packages → 10.0.0+
- Test packages (xunit, MSTest, etc.) → compatible versions
- Swashbuckle.AspNetCore → 6.6.0+

## Execution Steps

1. Update global.json SDK version
2. Update Directory.Packages.props TargetFramework
3. Update all version properties and package versions
4. Save and validate

## Expected Outcome

✅ global.json configured for .NET 10
✅ Directory.Packages.props updated to .NET 10
✅ All package versions compatible with .NET 10
