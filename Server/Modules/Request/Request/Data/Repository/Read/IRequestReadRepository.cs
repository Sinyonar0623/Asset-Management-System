using Shared.Data;

namespace Request.Data.Repository.Read;

public interface IRequestReadRepository : IReadRepository<Requests.Model.Request, Guid>
{
}
