using Parameter.Dto;

namespace Parameter.Service;

public interface IParameterService
{
    Task<List<ParameterDto>> GetParameterByGroup(string group);
    Task<long> CreateParameter(ParameterDto parameter);
    Task EnableParameter(long Id);
    Task DisableParameter(long Id);
}