namespace Request.Requests.Features.Request.UpdateRequest;

public record UpdateRequestCommand(
    Guid Id,
    UpdateRequestDto Request,
    Guid UserId,
    string RoleCode) : ICommand<UpdateRequestResult>;
