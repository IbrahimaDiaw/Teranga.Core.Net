# Changelog

## 2.1.0 - 2026-04-01

- Migrated the package to `.NET 10`
- Added a normalized ANSD regional module for `14` regions
- Added `IAnsdRegionalStatisticsService`
- Exposed ANSD data directly through `ITerangaService`
- Added `RegionProfile` aggregated read model
- Added `GetRegionProfileAsync(regionCode)`
- Added `SearchRegionProfilesAsync(query)`
- Added `GetRegionProfilesAsync(page, pageSize)`
- Added global and per-region administrative statistics
- Added case-insensitive and accent-insensitive name search
- Cleaned, validated, and hardened the embedded datasets
- Updated the sample UI and documentation
