using Asset.Data;
using Asset.Service.CommandHandlerService;
using MassTransit;
using Shared.Data.UnitOfWork;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Asset.Integration.Consumers;

public sealed class ReserveAssetConsumer(
    IAssetCommandHandlerService service,
    IUnitOfWork<AssetDbContext> unitOfWork
) : IConsumer<ReserveAssetCommand>
{
    private readonly IAssetCommandHandlerService _service = service;
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;

    public async Task Consume(ConsumeContext<ReserveAssetCommand> context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

            var isSuccess = await _service.ReserveAssetsAsync(
                context.Message.AssetIds,
                context.Message.ApproverId,
                context.Message.RequestId,
                context.CancellationToken);

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

            await context.RespondAsync(new ReserveAssetCommandResponse(isSuccess));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
            throw;
        }
    }
}
