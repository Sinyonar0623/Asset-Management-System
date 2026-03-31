namespace Parameter.Parameters.Features.GetParameterByGroup;

public record GetParameterByGroupQuery(PaginationRequest PaginationRequest, string Group) : IQuery<GetParameterByGroupResult>;