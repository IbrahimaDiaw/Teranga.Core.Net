namespace Teranga.Core.Models
{
    /// <summary>
    /// Aggregated profile for a single region.
    /// </summary>
    public sealed class RegionProfile
    {
        /// <summary>
        /// Region code.
        /// </summary>
        public string RegionCode { get; init; } = default!;

        /// <summary>
        /// Region name.
        /// </summary>
        public string RegionName { get; init; } = default!;

        /// <summary>
        /// Administrative region model.
        /// </summary>
        public Region Region { get; init; } = default!;

        /// <summary>
        /// Departments belonging to the region.
        /// </summary>
        public IReadOnlyList<Departement> Departments { get; init; } = [];

        /// <summary>
        /// Flat commune list for the region.
        /// </summary>
        public IReadOnlyList<Commune> Communes { get; init; } = [];

        /// <summary>
        /// Administrative statistics for the region.
        /// </summary>
        public RegionDetailStatistics Statistics { get; init; } = default!;

        /// <summary>
        /// ANSD indicators for the region when available.
        /// </summary>
        public AnsdRegionalProfile? AnsdProfile { get; init; }
    }
}
