using Mapster;
using Parameter.Data.Repository;
using Parameter.Dto;

namespace Parameter.Service;

public class ParameterService(IParameterRepository parameterRepository) : IParameterService
{
    private readonly IParameterRepository _repository = parameterRepository;
    public async Task<long> CreateParameter(ParameterDto parameter)
    {
        var p = parameter.Adapt<Parameters.Model.Parameter>();

        await _repository.AddAsync(p);

        return p.Id;
    }

    public async Task DisableParameter(long Id)
    {
        var p = await _repository.GetByIdAsync(Id);

        p.Disable();
    }

    public async Task EnableParameter(long Id)
    {
        var p = await _repository.GetByIdAsync(Id);

        p.Enable();
    }

    public Task<List<ParameterDto>> GetParameterByGroup(string group)
    {
        throw new NotImplementedException();
    }
}