namespace Request.Requests.Features.Request.CreateRequest;

public record CreateRequestCommand(
    CreateRequestDto Request,
    Guid RequesterId,
    string RequesterRoleCode
) : ICommand<CreateRequestResult>;
