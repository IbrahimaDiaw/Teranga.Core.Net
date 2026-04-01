# Teranga.Core

[![NuGet](https://img.shields.io/nuget/v/Teranga.Core.svg)](https://www.nuget.org/packages/Teranga.Core/)
[![NuGet](https://img.shields.io/nuget/dt/Teranga.Core.svg)](https://www.nuget.org/packages/Teranga.Core/)

Teranga.Core is a `.NET 10` library for Senegal administrative data, now exposing ANSD regional indicators through the main `ITerangaService` facade.

Current package version: `2.1.0`

## Features

- Embedded administrative dataset for Senegal: regions, departments, and communes
- Embedded normalized ANSD regional indicators dataset
- Fast in-memory lookup by code
- Name search ignoring case and accents
- Global and per-region administrative statistics
- Aggregated regional profile combining administrative data and ANSD indicators
- Search and paginated listing of aggregated region profiles
- Configurable dependency injection lifetime

## Installation

### Package Manager Console

```powershell
Install-Package Teranga.Core -Version 2.1.0
```

### .NET CLI

```bash
dotnet add package Teranga.Core --version 2.1.0
```

## Registration

```csharp
using Teranga.Core.Extensions;

services.AddTerangaCore();
services.AddTerangaCore(ServiceLifetime.Scoped);
```

Registered services:

- `ITerangaService`
- `IAnsdRegionalStatisticsService`

## Main usage

```csharp
var region = await terangaService.GetRegionByCodeAsync("DK");
var profile = await terangaService.GetRegionProfileAsync("DK");
var search = await terangaService.SearchRegionProfilesAsync("dakar");
var page = await terangaService.GetRegionProfilesAsync(1, 5);

var statistics = await terangaService.GetStatisticsAsync();
var regionStatistics = await terangaService.GetRegionStatisticsAsync("DK");

var ansdMetadata = await terangaService.GetAnsdMetadataAsync();
var ansdProfile = await terangaService.GetAnsdRegionalProfileAsync("DK");
var population = await terangaService.GetAnsdRegionalIndicatorAsync("DK", "population_total");
```

## Public APIs

```csharp
public interface ITerangaService
{
    Task<TerangaData> GetTerangaDataAsync();
    Task<IEnumerable<Region>> GetAllRegionsAsync();
    Task<Region?> GetRegionByCodeAsync(string code);
    Task<RegionProfile?> GetRegionProfileAsync(string regionCode);
    Task<IEnumerable<Departement>> GetDepartmentsByRegionAsync(string regionCode);
    Task<Departement?> GetDepartmentByCodeAsync(string code);
    Task<IEnumerable<Commune>> GetCommunesByDepartmentAsync(string departmentCode);
    Task<Commune?> GetCommuneByCodeAsync(string code);
    Task<IEnumerable<Region>> SearchRegionsByNameAsync(string name);
    Task<IReadOnlyList<RegionProfile>> SearchRegionProfilesAsync(string query);
    Task<IEnumerable<Departement>> SearchDepartmentsByNameAsync(string name);
    Task<IEnumerable<Commune>> SearchCommunesByNameAsync(string name);
    Task<TerangaStatistics> GetStatisticsAsync();
    Task<RegionDetailStatistics?> GetRegionStatisticsAsync(string regionCode);
    Task<AnsdDatasetMetadata> GetAnsdMetadataAsync();
    Task<IReadOnlyList<AnsdIndicatorDefinition>> GetAnsdIndicatorDefinitionsAsync();
    Task<IReadOnlyList<AnsdRegionalProfile>> GetAnsdRegionalProfilesAsync();
    Task<AnsdRegionalProfile?> GetAnsdRegionalProfileAsync(string regionCode);
    Task<AnsdRegionalIndicator?> GetAnsdRegionalIndicatorAsync(string regionCode, string indicatorCode);
    Task<PagedResult<RegionProfile>> GetRegionProfilesAsync(int page, int pageSize);
    Task ReloadDataAsync();
}
```

## ANSD notes

- ANSD data remains modeled separately from `Region`, `Departement`, and `Commune`
- `GetRegionProfileAsync` is the aggregated read model for one region
- each ANSD indicator preserves its own source and year
- `IAnsdRegionalStatisticsService` remains available for specialized usage

## Package artifacts

- `teranga-data.json` for the administrative dataset
- `ansd-regional-profiles.json` for normalized ANSD regional indicators

## Running tests

```bash
dotnet test
```
