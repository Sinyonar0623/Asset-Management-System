namespace Parameter.Parameters.Features.EndableParameter;

public record EndableParameterCommand(long Id) : ICommand<EndableParameterResult>;
