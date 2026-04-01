namespace Teranga.Core.Models
{
    /// <summary>
    /// Aggregated statistics for the Teranga dataset.
    /// </summary>
    public sealed class TerangaStatistics
    {
        /// <summary>
        /// Country code.
        /// </summary>
        public string CountryCode { get; init; } = default!;

        /// <summary>
        /// Country name.
        /// </summary>
        public string CountryName { get; init; } = default!;

        /// <summary>
        /// Number of regions.
        /// </summary>
        public int RegionCount { get; init; }

        /// <summary>
        /// Number of departments.
        /// </summary>
        public int DepartmentCount { get; init; }

        /// <summary>
        /// Number of communes.
        /// </summary>
        public int CommuneCount { get; init; }

        /// <summary>
        /// Number of local languages.
        /// </summary>
        public int LocalLanguageCount { get; init; }

        /// <summary>
        /// Average number of departments per region.
        /// </summary>
        public double AverageDepartmentsPerRegion { get; init; }

        /// <summary>
        /// Average number of communes per department.
        /// </summary>
        public double AverageCommunesPerDepartment { get; init; }

        /// <summary>
        /// Region with the most departments.
        /// </summary>
        public RegionStatistics? RegionWithMostDepartments { get; init; }

        /// <summary>
        /// Region with the most communes.
        /// </summary>
        public RegionStatistics? RegionWithMostCommunes { get; init; }

        /// <summary>
        /// Department with the most communes.
        /// </summary>
        public DepartmentStatistics? DepartmentWithMostCommunes { get; init; }

        /// <summary>
        /// Region-level statistics.
        /// </summary>
        public IReadOnlyList<RegionStatistics> Regions { get; init; } = [];

        /// <summary>
        /// Top departments by commune count.
        /// </summary>
        public IReadOnlyList<DepartmentStatistics> TopDepartmentsByCommuneCount { get; init; } = [];
    }
}
