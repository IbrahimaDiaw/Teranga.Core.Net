using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Teranga.Core.Exceptions;
using Teranga.Core.Models;

namespace Teranga.Core.Services
{
    /// <summary>
    /// Service to retrieve Teranga data.
    /// </summary>
    public class TerangaService : ITerangaService
    {
        private const string ResourceName = "Teranga.Core.Data.teranga-data.json";

        private readonly ILogger<TerangaService> _logger;
        private readonly IAnsdRegionalStatisticsService _ansdRegionalStatisticsService;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private DataSnapshot? _snapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerangaService"/> class.
        /// </summary>
        public TerangaService(
            ILogger<TerangaService> logger,
            IAnsdRegionalStatisticsService ansdRegionalStatisticsService)
        {
            _logger = logger;
            _ansdRegionalStatisticsService = ansdRegionalStatisticsService;
            LoadData();
        }

        /// <summary>
        /// Get the Teranga data.
        /// </summary>
        public Task<TerangaData> GetTerangaDataAsync()
        {
            _logger.LogInformation("Retrieving Teranga data");
            return Task.FromResult(EnsureSnapshotLoaded().TerangaData);
        }

        /// <summary>
        /// Get all the regions.
        /// </summary>
        public Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            _logger.LogInformation("Retrieving all regions");
            return Task.FromResult<IEnumerable<Region>>(EnsureSnapshotLoaded().Regions);
        }

        /// <summary>
        /// Get the region by code.
        /// </summary>
        public Task<Region?> GetRegionByCodeAsync(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Retrieving region with code: {Code}", code);
            snapshot.RegionsByCode.TryGetValue(code.Trim(), out var region);
            return Task.FromResult(region);
        }

        /// <summary>
        /// Get the aggregated region profile by code.
        /// </summary>
        public async Task<RegionProfile?> GetRegionProfileAsync(string regionCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(regionCode);
            var snapshot = EnsureSnapshotLoaded();

            if (!snapshot.RegionsByCode.TryGetValue(regionCode.Trim(), out var region))
            {
                _logger.LogInformation("No region profile found for region: {RegionCode}", regionCode);
                return null;
            }

            _logger.LogInformation("Retrieving aggregated region profile for: {RegionCode}", regionCode);
            return await CreateRegionProfileAsync(region);
        }

        /// <summary>
        /// Get the departments by region.
        /// </summary>
        public Task<IEnumerable<Departement>> GetDepartmentsByRegionAsync(string regionCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(regionCode);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Retrieving departments for region: {RegionCode}", regionCode);
            var departments = snapshot.RegionsByCode.TryGetValue(regionCode.Trim(), out var region)
                ? region.Departements
                : [];

            return Task.FromResult<IEnumerable<Departement>>(departments);
        }

        /// <summary>
        /// Get the department by code.
        /// </summary>
        public Task<Departement?> GetDepartmentByCodeAsync(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Retrieving department with code: {Code}", code);
            snapshot.DepartementsByCode.TryGetValue(code.Trim(), out var departement);
            return Task.FromResult(departement);
        }

        /// <summary>
        /// Get the communes by department.
        /// </summary>
        public Task<IEnumerable<Commune>> GetCommunesByDepartmentAsync(string departmentCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(departmentCode);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Retrieving communes for department: {DepartmentCode}", departmentCode);
            var communes = snapshot.DepartementsByCode.TryGetValue(departmentCode.Trim(), out var departement)
                ? departement.Communes
                : [];

            return Task.FromResult<IEnumerable<Commune>>(communes);
        }

        /// <summary>
        /// Get the commune by code.
        /// </summary>
        public Task<Commune?> GetCommuneByCodeAsync(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Retrieving commune with code: {Code}", code);
            snapshot.CommunesByCode.TryGetValue(code.Trim(), out var commune);
            return Task.FromResult(commune);
        }

        /// <summary>
        /// Search regions whose name contains the provided text.
        /// </summary>
        public Task<IEnumerable<Region>> SearchRegionsByNameAsync(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Searching regions with name: {Name}", name);
            return Task.FromResult<IEnumerable<Region>>(
                snapshot.Regions.Where(region => ContainsNormalized(region.Name, name)).ToArray());
        }

        /// <summary>
        /// Search aggregated region profiles by code or name.
        /// </summary>
        public async Task<IReadOnlyList<RegionProfile>> SearchRegionProfilesAsync(string query)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(query);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Searching aggregated region profiles with query: {Query}", query);

            var matches = snapshot.Regions
                .Select(region => new
                {
                    Region = region,
                    Score = GetRegionSearchScore(region, query)
                })
                .Where(item => item.Score != int.MaxValue)
                .OrderBy(item => item.Score)
                .ThenBy(item => item.Region.Name, StringComparer.OrdinalIgnoreCase)
                .Select(item => item.Region)
                .ToArray();

            var profiles = await Task.WhenAll(matches.Select(CreateRegionProfileAsync));
            return profiles;
        }

        /// <summary>
        /// Search departments whose name contains the provided text.
        /// </summary>
        public Task<IEnumerable<Departement>> SearchDepartmentsByNameAsync(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Searching departments with name: {Name}", name);
            return Task.FromResult<IEnumerable<Departement>>(
                snapshot.Departements.Where(departement => ContainsNormalized(departement.Name, name)).ToArray());
        }

        /// <summary>
        /// Search communes whose name contains the provided text.
        /// </summary>
        public Task<IEnumerable<Commune>> SearchCommunesByNameAsync(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            var snapshot = EnsureSnapshotLoaded();

            _logger.LogInformation("Searching communes with name: {Name}", name);
            return Task.FromResult<IEnumerable<Commune>>(
                snapshot.Communes.Where(commune => ContainsNormalized(commune.Name, name)).ToArray());
        }

        /// <summary>
        /// Get aggregated statistics for the dataset.
        /// </summary>
        public Task<TerangaStatistics> GetStatisticsAsync()
        {
            var snapshot = EnsureSnapshotLoaded();
            var regionStatistics = snapshot.Regions
                .Select(region => new RegionStatistics
                {
                    RegionCode = region.Code,
                    RegionName = region.Name,
                    DepartmentCount = region.Departements.Count,
                    CommuneCount = region.Departements.Sum(departement => departement.Communes.Count),
                    AverageCommunesPerDepartment = region.Departements.Count == 0
                        ? 0
                        : (double)region.Departements.Sum(departement => departement.Communes.Count) / region.Departements.Count
                })
                .OrderByDescending(region => region.CommuneCount)
                .ThenBy(region => region.RegionName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var departmentStatistics = snapshot.Regions
                .SelectMany(region => region.Departements.Select(departement => new DepartmentStatistics
                {
                    DepartmentCode = departement.Code,
                    DepartmentName = departement.Name,
                    RegionCode = region.Code,
                    RegionName = region.Name,
                    CommuneCount = departement.Communes.Count
                }))
                .ToArray();

            var departmentWithMostCommunes = departmentStatistics
                .OrderByDescending(departement => departement.CommuneCount)
                .ThenBy(departement => departement.DepartmentName, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();

            var topDepartments = departmentStatistics
                .Select(departement => new DepartmentStatistics
                {
                    DepartmentCode = departement.DepartmentCode,
                    DepartmentName = departement.DepartmentName,
                    RegionCode = departement.RegionCode,
                    RegionName = departement.RegionName,
                    CommuneCount = departement.CommuneCount
                })
                .OrderByDescending(departement => departement.CommuneCount)
                .ThenBy(departement => departement.DepartmentName, StringComparer.OrdinalIgnoreCase)
                .Take(10)
                .ToArray();

            var statistics = new TerangaStatistics
            {
                CountryCode = snapshot.TerangaData.Code,
                CountryName = snapshot.TerangaData.Name,
                RegionCount = snapshot.Regions.Length,
                DepartmentCount = snapshot.Departements.Length,
                CommuneCount = snapshot.Communes.Length,
                LocalLanguageCount = snapshot.TerangaData.LocalLanguages.Count,
                AverageDepartmentsPerRegion = snapshot.Regions.Length == 0 ? 0 : (double)snapshot.Departements.Length / snapshot.Regions.Length,
                AverageCommunesPerDepartment = snapshot.Departements.Length == 0 ? 0 : (double)snapshot.Communes.Length / snapshot.Departements.Length,
                RegionWithMostDepartments = regionStatistics
                    .OrderByDescending(region => region.DepartmentCount)
                    .ThenByDescending(region => region.CommuneCount)
                    .ThenBy(region => region.RegionName, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault(),
                RegionWithMostCommunes = regionStatistics.FirstOrDefault(),
                DepartmentWithMostCommunes = departmentWithMostCommunes,
                Regions = regionStatistics,
                TopDepartmentsByCommuneCount = topDepartments
            };

            _logger.LogInformation("Retrieving Teranga statistics");
            return Task.FromResult(statistics);
        }

        /// <summary>
        /// Get detailed statistics for a single region.
        /// </summary>
        public Task<RegionDetailStatistics?> GetRegionStatisticsAsync(string regionCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(regionCode);
            var snapshot = EnsureSnapshotLoaded();

            if (!snapshot.RegionsByCode.TryGetValue(regionCode.Trim(), out var region))
            {
                _logger.LogInformation("No statistics found for region: {RegionCode}", regionCode);
                return Task.FromResult<RegionDetailStatistics?>(null);
            }

            _logger.LogInformation("Retrieving detailed statistics for region: {RegionCode}", regionCode);
            return Task.FromResult<RegionDetailStatistics?>(CreateRegionDetailStatistics(region));
        }

        /// <summary>
        /// Get dataset metadata for ANSD regional indicators.
        /// </summary>
        public Task<AnsdDatasetMetadata> GetAnsdMetadataAsync()
        {
            return _ansdRegionalStatisticsService.GetMetadataAsync();
        }

        /// <summary>
        /// Get available ANSD regional indicator definitions.
        /// </summary>
        public Task<IReadOnlyList<AnsdIndicatorDefinition>> GetAnsdIndicatorDefinitionsAsync()
        {
            return _ansdRegionalStatisticsService.GetIndicatorDefinitionsAsync();
        }

        /// <summary>
        /// Get all ANSD regional profiles.
        /// </summary>
        public Task<IReadOnlyList<AnsdRegionalProfile>> GetAnsdRegionalProfilesAsync()
        {
            return _ansdRegionalStatisticsService.GetAllRegionalProfilesAsync();
        }

        /// <summary>
        /// Get a single ANSD regional profile by region code.
        /// </summary>
        public Task<AnsdRegionalProfile?> GetAnsdRegionalProfileAsync(string regionCode)
        {
            return _ansdRegionalStatisticsService.GetRegionalProfileAsync(regionCode);
        }

        /// <summary>
        /// Get a single ANSD regional indicator for a region.
        /// </summary>
        public Task<AnsdRegionalIndicator?> GetAnsdRegionalIndicatorAsync(string regionCode, string indicatorCode)
        {
            return _ansdRegionalStatisticsService.GetRegionalIndicatorAsync(regionCode, indicatorCode);
        }

        /// <summary>
        /// Get paginated aggregated region profiles.
        /// </summary>
        public async Task<PagedResult<RegionProfile>> GetRegionProfilesAsync(int page, int pageSize)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
            var snapshot = EnsureSnapshotLoaded();

            var orderedRegions = snapshot.Regions
                .OrderBy(region => region.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var totalCount = orderedRegions.Length;
            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
            var pageRegions = orderedRegions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            var items = await Task.WhenAll(pageRegions.Select(CreateRegionProfileAsync));

            return new PagedResult<RegionProfile>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = page > 1 && totalPages > 0,
                HasNextPage = page < totalPages
            };
        }

        /// <summary>
        /// Reload the data.
        /// </summary>
        public async Task ReloadDataAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                LoadData();
                _logger.LogInformation("Teranga data reloaded successfully");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void LoadData()
        {
            try
            {
                var assembly = typeof(TerangaService).Assembly;

                using var stream = assembly.GetManifestResourceStream(ResourceName);
                if (stream == null)
                {
                    throw new TerangaException("Resource not found: " + ResourceName);
                }

                using var reader = new StreamReader(stream);
                var jsonContent = reader.ReadToEnd();

                var terangaData = JsonSerializer.Deserialize<TerangaData>(
                    jsonContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (terangaData == null)
                {
                    throw new TerangaException("Failed to deserialize Teranga data");
                }

                var snapshot = BuildSnapshot(terangaData);
                Interlocked.Exchange(ref _snapshot, snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Teranga data");
                throw new TerangaException("Failed to load Teranga data", ex);
            }
        }

        private DataSnapshot EnsureSnapshotLoaded()
        {
            return Volatile.Read(ref _snapshot) ?? throw new TerangaException("Teranga data not loaded");
        }

        private static DataSnapshot BuildSnapshot(TerangaData terangaData)
        {
            var regions = terangaData.Regions.ToArray();
            var departements = regions.SelectMany(region => region.Departements).ToArray();
            var communes = departements.SelectMany(departement => departement.Communes).ToArray();

            return new DataSnapshot(
                terangaData,
                regions,
                departements,
                communes,
                regions.ToDictionary(region => region.Code, StringComparer.OrdinalIgnoreCase),
                departements.ToDictionary(departement => departement.Code, StringComparer.OrdinalIgnoreCase),
                communes.ToDictionary(commune => commune.Code, StringComparer.OrdinalIgnoreCase));
        }

        private RegionDetailStatistics CreateRegionDetailStatistics(Region region)
        {
            var departments = region.Departements
                .Select(departement => new DepartmentStatistics
                {
                    DepartmentCode = departement.Code,
                    DepartmentName = departement.Name,
                    RegionCode = region.Code,
                    RegionName = region.Name,
                    CommuneCount = departement.Communes.Count
                })
                .OrderByDescending(departement => departement.CommuneCount)
                .ThenBy(departement => departement.DepartmentName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new RegionDetailStatistics
            {
                RegionCode = region.Code,
                RegionName = region.Name,
                DepartmentCount = region.Departements.Count,
                CommuneCount = region.Departements.Sum(departement => departement.Communes.Count),
                AverageCommunesPerDepartment = region.Departements.Count == 0
                    ? 0
                    : (double)region.Departements.Sum(departement => departement.Communes.Count) / region.Departements.Count,
                DepartmentWithMostCommunes = departments.FirstOrDefault(),
                Departments = departments
            };
        }

        private async Task<RegionProfile> CreateRegionProfileAsync(Region region)
        {
            var departments = region.Departements
                .OrderBy(departement => departement.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var communes = departments
                .SelectMany(departement => departement.Communes)
                .OrderBy(commune => commune.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new RegionProfile
            {
                RegionCode = region.Code,
                RegionName = region.Name,
                Region = region,
                Departments = departments,
                Communes = communes,
                Statistics = CreateRegionDetailStatistics(region),
                AnsdProfile = await _ansdRegionalStatisticsService.GetRegionalProfileAsync(region.Code)
            };
        }

        private static int GetRegionSearchScore(Region region, string query)
        {
            var normalizedQuery = Normalize(query);
            var normalizedCode = Normalize(region.Code);
            var normalizedName = Normalize(region.Name);

            if (string.Equals(normalizedCode, normalizedQuery, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (string.Equals(normalizedName, normalizedQuery, StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            if (normalizedCode.StartsWith(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            {
                return 2;
            }

            if (normalizedName.StartsWith(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            {
                return 3;
            }

            if (normalizedCode.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            {
                return 4;
            }

            if (normalizedName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            {
                return 5;
            }

            return int.MaxValue;
        }

        private static bool ContainsNormalized(string source, string value)
        {
            var normalizedSource = Normalize(source);
            var normalizedValue = Normalize(value);
            return normalizedSource.Contains(normalizedValue, StringComparison.OrdinalIgnoreCase);
        }

        private static string Normalize(string value)
        {
            var normalized = value.Trim().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var character in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private sealed record DataSnapshot(
            TerangaData TerangaData,
            Region[] Regions,
            Departement[] Departements,
            Commune[] Communes,
            Dictionary<string, Region> RegionsByCode,
            Dictionary<string, Departement> DepartementsByCode,
            Dictionary<string, Commune> CommunesByCode);
    }
}
