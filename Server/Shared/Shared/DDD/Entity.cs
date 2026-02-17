namespace Shared.DDD;

public class Entity<T> : IEntity<T>
{
    public T Id { get; set; }
    public DateTime? CreateOn { get; set; }
    public string CreateBy { get; set; } = string.Empty;
    public DateTime? UpdateOn { get; set; }
    public string? UpdateBy { get; set; }
}
