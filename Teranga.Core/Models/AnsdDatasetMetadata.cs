namespace Teranga.Core.Models
{
    /// <summary>
    /// Metadata describing the normalized ANSD dataset bundled with the package.
    /// </summary>
    public sealed class AnsdDatasetMetadata
    {
        /// <summary>
        /// Display name of the normalized dataset.
        /// </summary>
        public string DatasetName { get; init; } = default!;

        /// <summary>
        /// Source organization.
        /// </summary>
        public string SourceOrganization { get; init; } = default!;

        /// <summary>
        /// ANSD catalog or landing page used as reference.
        /// </summary>
        public string SourceCatalogUrl { get; init; } = default!;

        /// <summary>
        /// Covered administrative level.
        /// </summary>
        public string CoverageLevel { get; init; } = default!;

        /// <summary>
        /// Date when the normalized dataset was prepared.
        /// </summary>
        public string NormalizedOn { get; init; } = default!;

        /// <summary>
        /// Short note about the normalization strategy.
        /// </summary>
        public string Notes { get; init; } = default!;
    }
}
