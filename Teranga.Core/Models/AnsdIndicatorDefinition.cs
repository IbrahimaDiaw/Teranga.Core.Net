namespace Teranga.Core.Models
{
    /// <summary>
    /// Public description of an ANSD indicator exposed by the package.
    /// </summary>
    public sealed class AnsdIndicatorDefinition
    {
        /// <summary>
        /// Stable indicator code.
        /// </summary>
        public string IndicatorCode { get; init; } = default!;

        /// <summary>
        /// Human-readable indicator name.
        /// </summary>
        public string IndicatorName { get; init; } = default!;

        /// <summary>
        /// Value unit.
        /// </summary>
        public string Unit { get; init; } = default!;

        /// <summary>
        /// Optional short description.
        /// </summary>
        public string? Description { get; init; }
    }
}
