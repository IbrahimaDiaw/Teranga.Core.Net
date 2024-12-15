namespace Teranga.Core.Models
{
    /// <summary>
    /// The base entity of the application
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// The code of the entity
        /// </summary>
        public string Code { get; set; } = default!;
        /// <summary>
        /// The name of the entity
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// The description of the entity
        /// </summary>
        public string? Description { get; set; }
    }
}
