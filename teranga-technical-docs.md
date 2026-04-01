# Technical Documentation - Teranga.Core

## Overview

Teranga.Core is a `.NET 10` package that embeds:

- the Senegal administrative reference dataset
- a normalized ANSD regional indicators dataset

The main entry point is now `ITerangaService`, which acts as a facade over both administrative data and ANSD regional data.

## Public services

- [ITerangaService](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Services/ITerangaService.cs)
- [IAnsdRegionalStatisticsService](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Services/IAnsdRegionalStatisticsService.cs)

`IAnsdRegionalStatisticsService` remains public for compatibility and specialized ANSD-only usage.

## Embedded datasets

- [teranga-data.json](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Data/teranga-data.json)
- [ansd-regional-profiles.json](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Data/ansd-regional-profiles.json)

## Administrative capabilities

`ITerangaService` exposes:

- lookup by region, department, and commune code
- name search for regions, departments, and communes
- global administrative statistics
- detailed statistics for one region

Key models:

- `TerangaData`
- `Region`
- `Departement`
- `Commune`
- `TerangaStatistics`
- `RegionDetailStatistics`

## ANSD facade on ITerangaService

`ITerangaService` also exposes:

- `GetAnsdMetadataAsync()`
- `GetAnsdIndicatorDefinitionsAsync()`
- `GetAnsdRegionalProfilesAsync()`
- `GetAnsdRegionalProfileAsync(regionCode)`
- `GetAnsdRegionalIndicatorAsync(regionCode, indicatorCode)`

This is a facade over [AnsdRegionalStatisticsService.cs](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Services/AnsdRegionalStatisticsService.cs). The dedicated ANSD service remains the source of truth for loading the normalized ANSD dataset.

## Aggregated regional profile

The package adds an aggregated read model:

- `RegionProfile`

`GetRegionProfileAsync(regionCode)` returns:

- the `Region`
- its `Departments`
- a flat `Communes` list
- `RegionDetailStatistics`
- the ANSD regional profile for the same region when available

This method does not change `GetRegionByCodeAsync()`, which still returns the plain `Region` model.

## Search and pagination

The package also exposes:

- `SearchRegionProfilesAsync(query)`
- `GetRegionProfilesAsync(page, pageSize)`

Behavior:

- search matches region code or name
- search ignores case and accents
- pagination uses `page` and `pageSize`
- paginated results are wrapped in `PagedResult<T>`
- paginated region profiles are ordered by region name

## Dependency injection

[ServiceCollectionExtensions.cs](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Extensions/ServiceCollectionExtensions.cs) registers:

```csharp
services.AddTerangaCore();
services.AddTerangaCore(ServiceLifetime.Scoped);
```

The same lifetime is applied to:

- `ITerangaService`
- `IAnsdRegionalStatisticsService`

## Example usage

```csharp
var region = await terangaService.GetRegionByCodeAsync("DK");
var profile = await terangaService.GetRegionProfileAsync("DK");
var page = await terangaService.GetRegionProfilesAsync(1, 5);
var matches = await terangaService.SearchRegionProfilesAsync("thies");

var metadata = await terangaService.GetAnsdMetadataAsync();
var ansdProfile = await terangaService.GetAnsdRegionalProfileAsync("DK");
```

## Testing focus

The test suite should cover:

- existing administrative lookups and statistics
- ANSD facade methods on `ITerangaService`
- `RegionProfile` consistency
- paginated region profile listing
- region profile search by code and by accent-insensitive name
- DI resolution for `TerangaService` with its ANSD dependency

Key test files:

- [TerangaServiceTests.cs](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core.Tests/TerangaServiceTests.cs)
- [TerangaDataIntegrityTests.cs](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core.Tests/TerangaDataIntegrityTests.cs)
- [AnsdRegionalStatisticsServiceTests.cs](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core.Tests/AnsdRegionalStatisticsServiceTests.cs)
- [TerangaServicePerformanceTests.cs](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core.Tests/TerangaServicePerformanceTests.cs)

## Maintenance notes

- keep the administrative dataset and ANSD dataset as separate domains
- use `RegionProfile` for aggregated reads instead of enriching `Region`
- keep `IAnsdRegionalStatisticsService` compatible while `ITerangaService` remains the main facade
- update package docs and release notes whenever public methods or read models change
