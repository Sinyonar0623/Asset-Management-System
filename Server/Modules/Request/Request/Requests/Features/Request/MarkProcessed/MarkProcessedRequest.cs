namespace Request.Requests.Features.Request.MarkProcessed;

public record MarkProcessedRequest(
    Guid RequestId,
    string Decision,
    string? Comment,
    List<Guid>? AssetIds
);