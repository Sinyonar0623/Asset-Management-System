namespace Parameter.Parameters.Features.DeleteParameter;

public record DeleteParameterCommand(long Id) : ICommand<DeleteParameterResult>;
