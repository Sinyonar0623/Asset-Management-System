namespace Shared.DDD;

public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}

public interface IEntity
{
    public DateTime? CreateOn { get; set; }
    public string CreateBy { get; set; }
    public DateTime? UpdateOn { get; set; }
    public string? UpdateBy { get; set; }
}
