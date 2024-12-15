namespace Teranga.Core.Models
{
    /// <summary>
    /// The commune of a departement
    /// </summary>
    public class Commune : BaseEntity
    {
        /// <summary>
        /// The departement code of the commune
        /// </summary>
        public string DepartementCode { get; set; } = default!;
    }
}
