namespace Teranga.Core.Models
{
    /// <summary>
    /// A single ANSD indicator value for a region.
    /// </summary>
    public sealed class AnsdRegionalIndicator
    {
        /// <summary>
        /// Stable indicator code.
        /// </summary>
        public string IndicatorCode { get; init; } = default!;

        /// <summary>
        /// Indicator label.
        /// </summary>
        public string IndicatorName { get; init; } = default!;

        /// <summary>
        /// Indicator numeric value.
        /// </summary>
        public decimal Value { get; init; }

        /// <summary>
        /// Unit of the value.
        /// </summary>
        public string Unit { get; init; } = default!;

        /// <summary>
        /// Source year when available.
        /// </summary>
        public int? Year { get; init; }

        /// <summary>
        /// Source label published by ANSD.
        /// </summary>
        public string SourceLabel { get; init; } = default!;

        /// <summary>
        /// Source reference URL.
        /// </summary>
        public string SourceReferenceUrl { get; init; } = default!;
    }
}
