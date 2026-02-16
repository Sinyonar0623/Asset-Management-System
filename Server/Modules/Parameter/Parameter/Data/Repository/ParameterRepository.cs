using Shared.Data.Repository;

namespace Parameter.Data.Repository;
public class ParameterRepository : Repository<Parameters.Model.Parameter, long>, IParameterRepository
{
    public ParameterRepository(ParameterDbContext dbContext) : base(dbContext)
    {
        
    }
}