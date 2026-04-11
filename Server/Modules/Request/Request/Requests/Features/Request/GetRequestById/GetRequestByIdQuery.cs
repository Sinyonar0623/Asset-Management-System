namespace Request.Requests.Features.Request.GetRequestById;

public record GetRequestByIdQuery(Guid Id) : IQuery<GetRequestByIdResult>;
