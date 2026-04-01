namespace Teranga.Core.Models
{
    /// <summary>
    /// ANSD indicator profile for a region.
    /// </summary>
    public sealed class AnsdRegionalProfile
    {
        /// <summary>
        /// Region code aligned with the package administrative dataset.
        /// </summary>
        public string RegionCode { get; init; } = default!;

        /// <summary>
        /// Region name.
        /// </summary>
        public string RegionName { get; init; } = default!;

        /// <summary>
        /// ANSD page used as source reference for the region profile.
        /// </summary>
        public string ProfilePageUrl { get; init; } = default!;

        /// <summary>
        /// Regional indicators.
        /// </summary>
        public IReadOnlyList<AnsdRegionalIndicator> Indicators { get; init; } = [];
    }
}
