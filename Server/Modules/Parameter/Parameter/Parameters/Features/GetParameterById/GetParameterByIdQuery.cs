namespace Parameter.Parameters.Features.GetParameterById;

public record GetParameterByIdQuery(long Id) : IQuery<GetParameterByIdResult>;