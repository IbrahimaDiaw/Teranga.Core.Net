
namespace Teranga.Core.Models
{
    public class Region : BaseEntity
    {
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
