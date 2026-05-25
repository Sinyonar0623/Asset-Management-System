namespace Request.Requests.Features.Request.MarkApproved;

public record MarkApprovedCommand(
    Guid RequestId
) : ICommand<MarkApprovedResult>;
