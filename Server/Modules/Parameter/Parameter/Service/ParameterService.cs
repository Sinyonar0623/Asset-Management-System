using Mapster;
using Parameter.Data.Repository;
using Parameter.Dto;

namespace Parameter.Service;

public class ParameterService(IParameterRepository parameterRepository) : IParameterService
{
    private readonly IParameterRepository _repository = parameterRepository;

    public async Task<long> CreateParameter(ParameterDto parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var newParameter = Parameters.Model.Parameter.Create(parameter.Group, parameter.Value, parameter.Description);

        await _repository.AddAsync(newParameter);

        await _repository.SaveChangeAsync();

        return newParameter.Id;
    }

    public async Task DisableParameter(long id)
    {
        var p = await GetRequiredParameter(id);

        p.Disable();
        await _repository.SaveChangeAsync();
    }

    public async Task EnableParameter(long id)
    {
        var p = await GetRequiredParameter(id);

        p.Enable();
        await _repository.SaveChangeAsync();
    }

    public async Task UpdateParameter(long id, ParameterDto parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var entity = await GetRequiredParameter(id);

        entity.Update(parameter.Group, parameter.Value, parameter.Description);

        if (parameter.Active)
        {
            entity.Enable();
        }
        else
        {
            entity.Disable();
        }

        await _repository.SaveChangeAsync();
    }

    public async Task DeleteParameter(long id)
    {
        var entity = await GetRequiredParameter(id);

        await _repository.DeleteAsync(entity);
        await _repository.SaveChangeAsync();
    }

    public async Task<List<ParameterDto>> GetParameters()
    {
        var parameters = await _repository.GetAllAsync();

        var result = parameters.Adapt<List<ParameterDto>>();

        return result;
    }

    public async Task<List<ParameterDto>> GetParameterByGroup(string group)
    {
        var parameters = await _repository.GetParameterByGroup(group);

        var result = parameters.Adapt<List<ParameterDto>>();

        return result;
    }

    public async Task<ParameterDto> GetParameter(long id)
    {
        var parameter = await GetRequiredParameter(id);

        var result = parameter.Adapt<ParameterDto>();

        return result;
    }

    private async Task<Parameters.Model.Parameter> GetRequiredParameter(long id)
    {
        var parameter = await _repository.GetByIdAsync(id) ?? 
            throw new KeyNotFoundException($"Parameter with id {id} was not found.");

        return parameter;
    }
}
