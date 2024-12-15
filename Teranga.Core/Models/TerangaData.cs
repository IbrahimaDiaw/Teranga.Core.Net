namespace Teranga.Core.Models
{
    /// <summary>
    /// The data of a country
    /// </summary>
    public class TerangaData : BaseEntity
    {
        /// <summary>
        /// The population of the country
        /// </summary>
        public int Population { get; set; }
        /// <summary>
        /// The area of the country
        /// </summary>
        public double Area { get; set; }
        /// <summary>
        /// The flag of the country
        /// </summary>
        public string Flag { get; set; } = default!;
        /// <summary>
        /// The local language name of the country
        /// </summary>
        public List<string> LocalLanguages { get; set; } = new();
        /// <summary>
        /// The capital of the country
        /// </summary>
        public string Capital { get; set; } = default!;
        /// <summary>
        /// The currency of the country
        /// </summary>
        public string Currency { get; set; } = default!;
        /// <summary>
        /// The official language of the country
        /// </summary>
        public string Language { get; set; } = default!;
        /// <summary>
        /// The continent of the country
        /// </summary>
        public string Continent { get; set; } = default!;
        /// <summary>
        /// The timezone of the country
        /// </summary>
        public string TimeZone { get; set; } = default!;
        /// <summary>
        /// The regions of the country
        /// </summary>
        public virtual ICollection<Region> Regions { get; set; } = new List<Region>();
    }
}
