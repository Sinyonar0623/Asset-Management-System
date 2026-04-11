namespace Request.Requests.Features.Request.UpdateRequest;

public record UpdateRequestCommand(Guid Id, UpdateRequestDto Request) : ICommand<UpdateRequestResult>;
