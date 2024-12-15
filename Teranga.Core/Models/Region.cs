
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
        public virtual ICollection<Departement> Departments { get; set; } = new List<Departement>();
    }
}
