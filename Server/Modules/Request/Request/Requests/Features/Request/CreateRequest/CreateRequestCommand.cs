namespace Request.Requests.Features.Request.CreateRequest;

public record CreateRequestCommand(CreateRequestDto Request) : ICommand<CreateRequestResult>;
