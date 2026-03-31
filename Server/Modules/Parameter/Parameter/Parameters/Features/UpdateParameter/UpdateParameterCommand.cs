namespace Parameter.Parameters.Features.UpdateParameter;

public record UpdateParameterCommand(long Id, ParameterDto Parameter) : ICommand<UpdateParameterResult>;
