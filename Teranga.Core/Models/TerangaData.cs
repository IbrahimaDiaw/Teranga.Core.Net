namespace Teranga.Core.Models
{
    public class TerangaData : BaseEntity
    {
        public int Population { get; set; }
        public double Area { get; set; }
        public string Capital { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public string Language { get; set; } = default!;
        public string Continent { get; set; } = default!;
        public virtual ICollection<Region> Regions { get; set; } = new List<Region>();
    }
}
