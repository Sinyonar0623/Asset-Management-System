namespace Parameter.Parameters.Features.GetParameters;

public record GetParameterQuery(PaginationRequest PaginationRequest) : IQuery<GetParameterResult>;