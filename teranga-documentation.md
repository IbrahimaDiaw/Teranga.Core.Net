# Teranga Documentation

## Purpose

This document describes the administrative code system used by `Teranga.Core` and the practical rules for consuming the package datasets.

It complements:

- [README.md](D:/Projects/Nugets/Teranga.Core.Net/README.md)
- [teranga-technical-docs.md](D:/Projects/Nugets/Teranga.Core.Net/teranga-technical-docs.md)

## Package Scope

`Teranga.Core` currently exposes two separate data domains:

- the Senegal administrative reference dataset
- the ANSD regional indicators dataset

The administrative reference dataset is the source of truth for region, department, and commune codes. The ANSD dataset is normalized and mapped onto those region codes.

## Administrative Code Structure

The code system is hierarchical.

- Region: 2 letters
  - Example: `DK`
- Department: region code + numeric suffix
  - Example: `DK1`
- Commune: department code + `C` + numeric suffix
  - Example: `DK1C17`

This hierarchy allows deterministic parent-child relationships:

- a department belongs to one region
- a commune belongs to one department
- commune and department codes inherit the prefix of their parent territory

## Region Codes

The embedded administrative dataset currently uses these region codes:

- `DK` Dakar
- `DL` Diourbel
- `FK` Fatick

The authoritative list is:

- `DK` Dakar
- `DL` Diourbel
- `FK` Fatick
- `KF` Kaffrine
- `KD` Kolda
- `KL` Kaolack
- `KE` Kedougou
- `LG` Louga
- `MT` Matam
- `SL` Saint-Louis
- `SD` Sedhiou
- `TC` Tambacounda
- `TH` Thies
- `ZG` Ziguinchor

Important:

- older informal mappings such as `DB`, `KG`, or `SE` are not the package contract
- consumers should always use the codes returned by `ITerangaService`

## Dataset Coverage

Current administrative dataset totals:

- `14` regions
- `46` departments
- `552` communes

These values are exposed by `GetStatisticsAsync()`.

## Administrative Service API

Main entry point:

- [ITerangaService](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Services/ITerangaService.cs)

Typical usage:

```csharp
var region = await terangaService.GetRegionByCodeAsync("DK");
var departments = await terangaService.GetDepartmentsByRegionAsync("DK");
var commune = await terangaService.GetCommuneByCodeAsync("DK1C17");

var regionMatches = await terangaService.SearchRegionsByNameAsync("dakar");
var departmentMatches = await terangaService.SearchDepartmentsByNameAsync("keur");
var communeMatches = await terangaService.SearchCommunesByNameAsync("medina");
```

Statistics usage:

```csharp
var statistics = await terangaService.GetStatisticsAsync();
var regionStatistics = await terangaService.GetRegionStatisticsAsync("DK");
```

Aggregated regional profile usage:

```csharp
var profile = await terangaService.GetRegionProfileAsync("DK");
var matches = await terangaService.SearchRegionProfilesAsync("dakar");
var page = await terangaService.GetRegionProfilesAsync(1, 5);
```

## Validation Rules

Consumers should assume:

- codes are case-insensitive for lookup purposes
- blank codes are invalid
- unknown codes return `null` for single-entity lookup methods
- search is case-insensitive and accent-insensitive

Recommended validation patterns:

- region code shape: `^[A-Z]{2}$`
- department code shape: `^[A-Z]{2}[0-9]+$`
- commune code shape: `^[A-Z]{2}[0-9]+C[0-9]+$`

These regexes are convenience checks only. The real source of truth is the embedded dataset.

## ANSD Regional Mapping

Main entry point:

- [IAnsdRegionalStatisticsService](D:/Projects/Nugets/Teranga.Core.Net/Teranga.Core/Services/IAnsdRegionalStatisticsService.cs)

The ANSD module:

- is limited to the `Region` level in v1
- does not modify `Region`, `Departement`, or `Commune`
- keeps `Year`, `SourceLabel`, and `SourceReferenceUrl` on each indicator
- reads only the normalized internal dataset bundled with the package

Typical usage:

```csharp
var metadata = await ansdService.GetMetadataAsync();
var profile = await ansdService.GetRegionalProfileAsync("DK");
var indicator = await ansdService.GetRegionalIndicatorAsync("DK", "population_total");
```

The same ANSD data is also available through `ITerangaService`:

```csharp
var metadata = await terangaService.GetAnsdMetadataAsync();
var ansdProfile = await terangaService.GetAnsdRegionalProfileAsync("DK");
var population = await terangaService.GetAnsdRegionalIndicatorAsync("DK", "population_total");
```

## Dependency Injection

Register the package with:

```csharp
services.AddTerangaCore();
```

Or choose a different lifetime:

```csharp
services.AddTerangaCore(ServiceLifetime.Scoped);
```

This registers:

- `ITerangaService`
- `IAnsdRegionalStatisticsService`

## Maintenance Notes

When the administrative dataset changes:

- keep the published code scheme stable where possible
- update tests that validate counts and hierarchical consistency
- avoid documenting hard-coded commune lists unless they come directly from the embedded dataset and are maintained with it

When the ANSD dataset changes:

- update the normalized internal dataset, not the runtime contract shape
- preserve region code alignment with the administrative dataset
- keep documentation aligned with [README.md](D:/Projects/Nugets/Teranga.Core.Net/README.md) and [teranga-technical-docs.md](D:/Projects/Nugets/Teranga.Core.Net/teranga-technical-docs.md)
