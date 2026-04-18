namespace Request.Requests.Features.Request.CreateRequest;

public record CreateRequestCommand(
    CreateRequestDto Request,
    Guid RequesterId
) : ICommand<CreateRequestResult>;
