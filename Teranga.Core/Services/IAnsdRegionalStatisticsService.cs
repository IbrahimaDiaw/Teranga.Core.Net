using Teranga.Core.Models;

namespace Teranga.Core.Services
{
    /// <summary>
    /// Provides normalized regional indicators sourced from ANSD.
    /// </summary>
    public interface IAnsdRegionalStatisticsService
    {
        /// <summary>
        /// Get dataset metadata.
        /// </summary>
        Task<AnsdDatasetMetadata> GetMetadataAsync();

        /// <summary>
        /// Get the list of supported indicators.
        /// </summary>
        Task<IReadOnlyList<AnsdIndicatorDefinition>> GetIndicatorDefinitionsAsync();

        /// <summary>
        /// Get all regional profiles.
        /// </summary>
        Task<IReadOnlyList<AnsdRegionalProfile>> GetAllRegionalProfilesAsync();

        /// <summary>
        /// Get a regional profile by region code.
        /// </summary>
        Task<AnsdRegionalProfile?> GetRegionalProfileAsync(string regionCode);

        /// <summary>
        /// Get a single indicator for a region.
        /// </summary>
        Task<AnsdRegionalIndicator?> GetRegionalIndicatorAsync(string regionCode, string indicatorCode);
    }
}
