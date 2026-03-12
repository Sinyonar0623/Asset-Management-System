namespace Parameter.Parameters.Features.DisableParameter;

public record DisableParameterCommand(long Id) : ICommand<DisableParameterResult>;
