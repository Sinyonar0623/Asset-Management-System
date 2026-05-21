namespace Request.Dto;

public sealed record CreateRequestDto
{
    public string RequestType { get; init; } = string.Empty;
    public Guid TargetLaboratoryId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public RequestDetailDto? Detail { get; init; }
    public List<RequestItemDto>? Items { get; init; }
}
