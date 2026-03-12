namespace Parameter.Parameters.Features.CreateParameter;

public record CreateParameterCommand(ParameterDto Parameter) : ICommand<CreateParameterResult>;