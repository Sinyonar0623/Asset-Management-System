using Parameter.Dto;

namespace Parameter.Service;

public interface IParameterService
{
    Task<List<ParameterDto>> GetParameterByGroup(string group);
    Task<long> CreateParameter(ParameterDto parameter);
    Task UpdateParameter(long id, ParameterDto parameter);
    Task DeleteParameter(long id);
    Task<List<ParameterDto>> GetParameters();
    Task EnableParameter(long id);
    Task DisableParameter(long id);
    Task<ParameterDto> GetParameter(long id);
}
