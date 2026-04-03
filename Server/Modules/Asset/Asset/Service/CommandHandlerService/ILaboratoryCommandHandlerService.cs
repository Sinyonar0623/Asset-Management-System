namespace Asset.Service.CommandHandlerService;

public interface ILaboratoryCommandHandlerService
{
    Task<Guid> CreateLaboratory(LaboratoryDto laboratory, CancellationToken cancellationToken = default);
    Task<List<LaboratoryDto>> GetLaboratories(CancellationToken cancellationToken = default);
    Task<LaboratoryDto> GetLaboratoryById(Guid laboratoryId, CancellationToken cancellationToken = default);
    Task<bool> UpdateLaboratory(Guid laboratoryId, LaboratoryDto laboratory, CancellationToken cancellationToken = default);
    Task<DeleteLaboratoryOperationResult> DeleteLaboratory(Guid laboratoryId, CancellationToken cancellationToken = default);
    Task<bool> AssignLaboratory(Guid assetId, Guid labId, CancellationToken cancellationToken = default);
}

public sealed record DeleteLaboratoryOperationResult(bool IsSuccess, string? Message = null);
