namespace Teranga.Core.Models
{
    /// <summary>
    /// Aggregated statistics for a region.
    /// </summary>
    public sealed class RegionStatistics
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
        /// Number of departments in the region.
        /// </summary>
        public int DepartmentCount { get; init; }

        /// <summary>
        /// Number of communes in the region.
        /// </summary>
        public int CommuneCount { get; init; }

        /// <summary>
        /// Average number of communes per department in the region.
        /// </summary>
        public double AverageCommunesPerDepartment { get; init; }
    }
}
