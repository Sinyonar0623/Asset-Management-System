using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Request.Data.Repository.Read;

public class RequestReadRepository(RequestDbContext dbContext)
    : BaseReadRepository<Requests.Model.Request, Guid>(dbContext), IRequestReadRepository
{
    private readonly RequestDbContext _context = dbContext;

    protected override IQueryable<Requests.Model.Request> GetReadQuery()
    {
        return _context.Requests
            .AsNoTracking()
            .Include(x => x.Detail)
            .Include(x => x.Items)
            .Include(x => x.Trackings);
    }
}
