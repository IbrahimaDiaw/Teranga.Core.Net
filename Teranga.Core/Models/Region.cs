
namespace Teranga.Core.Models
{
    /// <summary>
    /// The region of a country
    /// </summary>
    public class Region : BaseEntity
    {
        /// <summary>
        /// The departements of the region
        /// </summary>
        public virtual ICollection<Departement> Departements { get; set; } = new List<Departement>();
    }
}
