using Teranga.Core.Models;

namespace Teranga.Core.Services
{
    /// <summary>
    /// The Teranga service.
    /// </summary>
    public interface ITerangaService
    {
        /// <summary>
        /// Get the Teranga data.
        /// </summary>
        Task<TerangaData> GetTerangaDataAsync();

        /// <summary>
        /// Get all the regions.
        /// </summary>
        Task<IEnumerable<Region>> GetAllRegionsAsync();

        /// <summary>
        /// Get the region by code.
        /// </summary>
        Task<Region?> GetRegionByCodeAsync(string code);

        /// <summary>
        /// Get the aggregated region profile by code.
        /// </summary>
        Task<RegionProfile?> GetRegionProfileAsync(string regionCode);

        /// <summary>
        /// Get the departments by region.
        /// </summary>
        Task<IEnumerable<Departement>> GetDepartmentsByRegionAsync(string regionCode);

        /// <summary>
        /// Get the department by code.
        /// </summary>
        Task<Departement?> GetDepartmentByCodeAsync(string code);

        /// <summary>
        /// Get the communes by department.
        /// </summary>
        Task<IEnumerable<Commune>> GetCommunesByDepartmentAsync(string departmentCode);

        /// <summary>
        /// Get the commune by code.
        /// </summary>
        Task<Commune?> GetCommuneByCodeAsync(string code);

        /// <summary>
        /// Search regions whose name contains the provided text.
        /// </summary>
        Task<IEnumerable<Region>> SearchRegionsByNameAsync(string name);

        /// <summary>
        /// Search aggregated region profiles by code or name.
        /// </summary>
        Task<IReadOnlyList<RegionProfile>> SearchRegionProfilesAsync(string query);

        /// <summary>
        /// Search departments whose name contains the provided text.
        /// </summary>
        Task<IEnumerable<Departement>> SearchDepartmentsByNameAsync(string name);

        /// <summary>
        /// Search communes whose name contains the provided text.
        /// </summary>
        Task<IEnumerable<Commune>> SearchCommunesByNameAsync(string name);

        /// <summary>
        /// Get aggregated statistics for the dataset.
        /// </summary>
        Task<TerangaStatistics> GetStatisticsAsync();

        /// <summary>
        /// Get detailed statistics for a single region.
        /// </summary>
        Task<RegionDetailStatistics?> GetRegionStatisticsAsync(string regionCode);

        /// <summary>
        /// Get dataset metadata for ANSD regional indicators.
        /// </summary>
        Task<AnsdDatasetMetadata> GetAnsdMetadataAsync();

        /// <summary>
        /// Get available ANSD regional indicator definitions.
        /// </summary>
        Task<IReadOnlyList<AnsdIndicatorDefinition>> GetAnsdIndicatorDefinitionsAsync();

        /// <summary>
        /// Get all ANSD regional profiles.
        /// </summary>
        Task<IReadOnlyList<AnsdRegionalProfile>> GetAnsdRegionalProfilesAsync();

        /// <summary>
        /// Get a single ANSD regional profile by region code.
        /// </summary>
        Task<AnsdRegionalProfile?> GetAnsdRegionalProfileAsync(string regionCode);

        /// <summary>
        /// Get a single ANSD regional indicator for a region.
        /// </summary>
        Task<AnsdRegionalIndicator?> GetAnsdRegionalIndicatorAsync(string regionCode, string indicatorCode);

        /// <summary>
        /// Get paginated aggregated region profiles.
        /// </summary>
        Task<PagedResult<RegionProfile>> GetRegionProfilesAsync(int page, int pageSize);

        /// <summary>
        /// Reload the data.
        /// </summary>
        Task ReloadDataAsync();
    }
}
