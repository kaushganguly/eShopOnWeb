# Scenario: Upgrade eShopOnWeb to .NET 10 LTS

## Upgrade Parameters

- **Solution**: eShopOnWeb.sln
- **Current Framework**: net8.0
- **Target Framework**: net10.0
- **Flow Mode**: Automatic

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported Packages: Defer Resolution (7 incompatible packages)
- Unsupported API Handling: Fix Inline

## User Preferences

- Automatic execution mode enabled
- No pauses for user confirmation
- Build and test validation required before task completion
