namespace Teranga.Core.Models
{
    /// <summary>
    /// Detailed statistics for a single region.
    /// </summary>
    public sealed class RegionDetailStatistics
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

        /// <summary>
        /// Department with the most communes in the region.
        /// </summary>
        public DepartmentStatistics? DepartmentWithMostCommunes { get; init; }

        /// <summary>
        /// Department-level statistics for the region.
        /// </summary>
        public IReadOnlyList<DepartmentStatistics> Departments { get; init; } = [];
    }
}
