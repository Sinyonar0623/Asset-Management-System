using Asset.Data;
using Asset.Service.CommandHandlerService;
using MassTransit;
using Shared.Data.UnitOfWork;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Asset.Integration.Consumers;

public sealed class AssignAssetsToLaboratoryConsumer(
    IAssetCommandHandlerService service,
    IUnitOfWork<AssetDbContext> unitOfWork
) : IConsumer<AssignAssetsToLaboratoryCommand>
{
    private readonly IAssetCommandHandlerService _service = service;
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;

    public async Task Consume(ConsumeContext<AssignAssetsToLaboratoryCommand> context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

            var isSuccess = await _service.AssignAssetsToLaboratoryAsync(
                context.Message.LaboratoryId,
                context.Message.AssetIds,
                context.Message.ApproverId,
                context.Message.RequestId,
                context.CancellationToken);

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

            await context.RespondAsync(new AssignAssetsToLaboratoryCommandResponse(isSuccess));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
            throw;
        }
    }
}
