using Asset.Assets.Events;
using Asset.Assets.Model;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;
using Mapster;

namespace Asset.Service.CommandHandlerService;

public class LaboratoryCommandHandlerService(
    ILaboratoryReadRepository laboratoryReadRepository,
    ILaboratoryWriteRepository laboratoryWriteRepository) : ILaboratoryCommandHandlerService
{
    private readonly ILaboratoryReadRepository _laboratoryReadRepository = laboratoryReadRepository;
    private readonly ILaboratoryWriteRepository _laboratoryWriteRepository = laboratoryWriteRepository;

    public async Task<Guid> CreateLaboratory(LaboratoryDto laboratory, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(laboratory);

        var newLaboratory = Laboratory.Create(
            laboratory.LaboratoryName,
            laboratory.RoomNo,
            laboratory.TeacherId,
            laboratory.Description);

        await _laboratoryWriteRepository.AddAsync(newLaboratory, cancellationToken);

        return newLaboratory.Id;
    }

    public async Task<List<LaboratoryDto>> GetLaboratories(CancellationToken cancellationToken = default)
    {
        var laboratories = await _laboratoryReadRepository.GetAllAsync(cancellationToken);
        return laboratories.Adapt<List<LaboratoryDto>>();
    }

    public async Task<LaboratoryDto> GetLaboratoryById(Guid laboratoryId, CancellationToken cancellationToken = default)
    {
        var laboratory = await _laboratoryReadRepository.GetByIdAsync(laboratoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Laboratory with id {laboratoryId} was not found.");

        return laboratory.Adapt<LaboratoryDto>();
    }

    public async Task<bool> UpdateLaboratory(
        Guid laboratoryId,
        LaboratoryDto laboratory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(laboratory);

        var currentLaboratory = await _laboratoryWriteRepository.GetByIdAsync(laboratoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Laboratory with id {laboratoryId} was not found.");

        currentLaboratory.Update(
            laboratory.LaboratoryName,
            laboratory.RoomNo,
            laboratory.TeacherId,
            laboratory.Description);

        return true;
    }

    public async Task<bool> AssignTeacher(
        Guid laboratoryId,
        Guid teacherId,
        CancellationToken cancellationToken = default)
    {
        if (laboratoryId == Guid.Empty)
            throw new ArgumentException("Laboratory id is required.", nameof(laboratoryId));

        if (teacherId == Guid.Empty)
            throw new ArgumentException("Teacher id is required.", nameof(teacherId));

        var laboratory = await _laboratoryWriteRepository.GetByIdAsync(laboratoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Laboratory with id {laboratoryId} was not found.");

        laboratory.AssignTeacher(teacherId);

        return true;
    }

    public async Task<DeleteLaboratoryOperationResult> DeleteLaboratory(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        var laboratory = await _laboratoryWriteRepository.GetByIdAsync(laboratoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Laboratory with id {laboratoryId} was not found.");

        var hasAssetModels = await _laboratoryReadRepository.HasAssetModelsAsync(laboratoryId, cancellationToken);
        if (hasAssetModels)
        {
            return new DeleteLaboratoryOperationResult(
                false,
                "Cannot delete laboratory because it is assigned to assets.");
        }

        await _laboratoryWriteRepository.DeleteAsync(laboratory, cancellationToken);

        return new DeleteLaboratoryOperationResult(true);
    }

    public async Task<bool> AssignLaboratory(Guid assetId, Guid labId, CancellationToken cancellationToken = default)
    {
        if (assetId == Guid.Empty)
            throw new ArgumentException("Asset id is required.", nameof(assetId));

        if (labId == Guid.Empty)
            throw new ArgumentException("Laboratory id is required.", nameof(labId));

        var lab = await _laboratoryWriteRepository.GetByIdAsync(labId, cancellationToken)
            ?? throw new KeyNotFoundException($"Laboratory with id {labId} was not found.");

        lab.AddDomainEvent(new AssignLaboratoryEvent(assetId, lab));

        return true;
    }
}
