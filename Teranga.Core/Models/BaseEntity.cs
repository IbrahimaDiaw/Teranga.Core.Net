namespace Teranga.Core.Models
{
    public abstract class BaseEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
