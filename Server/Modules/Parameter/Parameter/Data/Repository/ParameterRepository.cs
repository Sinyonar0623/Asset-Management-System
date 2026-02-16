using Mapster;
using Microsoft.EntityFrameworkCore;
using Parameter.Dto;
using Shared.Data.Repository;

namespace Parameter.Data.Repository;
public class ParameterRepository(ParameterDbContext dbContext) : Repository<Parameters.Model.Parameter, long>(dbContext), IParameterRepository
{
    private readonly ParameterDbContext _context = dbContext;

    public async Task<List<ParameterDto>> GetParameterByGroup(string group)
    {
        var query = await _context.Parameter.Where(p => p.Group == group).AsNoTracking().ToListAsync();

        var result = query.Adapt<List<ParameterDto>>();

        return result;
    }
}