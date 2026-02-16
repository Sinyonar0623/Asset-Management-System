using Parameter.Dto;
using Shared.Data.Repository;

namespace Parameter.Data.Repository;

public interface IParameterRepository : IRepository<Parameters.Model.Parameter, long>
{
    Task<List<ParameterDto>> GetParameterByGroup(string group);
}