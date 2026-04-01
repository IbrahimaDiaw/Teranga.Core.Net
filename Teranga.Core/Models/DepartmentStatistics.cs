namespace Teranga.Core.Models
{
    /// <summary>
    /// Aggregated statistics for a department.
    /// </summary>
    public sealed class DepartmentStatistics
    {
        /// <summary>
        /// Department code.
        /// </summary>
        public string DepartmentCode { get; init; } = default!;

        /// <summary>
        /// Department name.
        /// </summary>
        public string DepartmentName { get; init; } = default!;

        /// <summary>
        /// Parent region code.
        /// </summary>
        public string RegionCode { get; init; } = default!;

        /// <summary>
        /// Parent region name.
        /// </summary>
        public string RegionName { get; init; } = default!;

        /// <summary>
        /// Number of communes in the department.
        /// </summary>
        public int CommuneCount { get; init; }
    }
}
