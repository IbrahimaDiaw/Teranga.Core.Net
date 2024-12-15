namespace Teranga.Core.Models
{
    /// <summary>
    /// The departement of a region
    /// </summary>
    public class Departement : BaseEntity
    {
        /// <summary>
        /// The region code of the departement
        /// </summary>
        public string RegionCode { get; set; } = default!;
        /// <summary>
        /// The communes of the departement
        /// </summary>
        public ICollection<Commune> Communes { get; set; } = new List<Commune>();
    }
}
