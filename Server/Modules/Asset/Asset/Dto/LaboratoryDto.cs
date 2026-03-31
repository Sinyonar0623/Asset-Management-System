namespace Asset.Dto;

public sealed record LaboratoryDto
{
    public Guid Id { get; init; }
    public string LaboratoryName { get; init; } = string.Empty;
    public string RoomNo { get; init; } = string.Empty;
    public Guid TeacherId { get; init; }
    public string Description { get; init; } = string.Empty;
}
