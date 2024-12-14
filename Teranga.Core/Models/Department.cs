namespace Teranga.Core.Models
{
    public class Department : BaseEntity
    {
        public string RegionCode { get; set; } = default!;
        public ICollection<Commune> Communes { get; set; } = new List<Commune>();
    }
}
