namespace Request.Requests.Features.Request.DeleteRequest;

public record DeleteRequestCommand(Guid Id) : ICommand<DeleteRequestResult>;
