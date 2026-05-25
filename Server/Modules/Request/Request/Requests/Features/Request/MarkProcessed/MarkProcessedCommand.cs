namespace Request.Requests.Features.Request.MarkProcessed;

public record MarkProcessedCommand(
    Guid RequestId,
    Guid ApproverId,
    string ApproverRoleCode,
    string Decision,
    string? Comment,
    List<Guid> AssetIds
) : ICommand<MarkProcessedResult>;
