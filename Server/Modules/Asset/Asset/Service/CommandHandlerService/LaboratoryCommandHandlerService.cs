using Asset.Assets.Model;
using Asset.Data;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;
using Mapster;
using Shared.Data.UnitOfWork;

namespace Asset.Service.CommandHandlerService;

public class LaboratoryCommandHandlerService(
    ILaboratoryReadRepository laboratoryReadRepository,
    ILaboratoryWriteRepository laboratoryWriteRepository,
    IUnitOfWork<AssetDbContext> unitOfWork) : ILaboratoryCommandHandlerService
{
    private readonly ILaboratoryReadRepository _laboratoryReadRepository = laboratoryReadRepository;
    private readonly ILaboratoryWriteRepository _laboratoryWriteRepository = laboratoryWriteRepository;
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;

    public async Task<Guid> CreateLaboratory(LaboratoryDto laboratory, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(laboratory);

        var newLaboratory = Laboratory.Create(
            laboratory.LaboratoryName,
            laboratory.RoomNo,
            laboratory.TeacherId,
            laboratory.Description);

        await _laboratoryWriteRepository.AddAsync(newLaboratory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeleteLaboratoryOperationResult(true);
    }
}
