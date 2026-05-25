using Asset.Data;
using Asset.Service.CommandHandlerService;
using MassTransit;
using Shared.Data.UnitOfWork;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Asset.Integration.Consumers;

public sealed class MarkAssetsInUseConsumer(
    IAssetCommandHandlerService service,
    IUnitOfWork<AssetDbContext> unitOfWork
) : IConsumer<MarkAssetsInUseCommand>
{
    private readonly IAssetCommandHandlerService _service = service;
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;

    public async Task Consume(ConsumeContext<MarkAssetsInUseCommand> context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

            var isSuccess = await _service.MarkAssetsInUseAsync(
                context.Message.AssetIds,
                context.Message.ResponsibleUserId,
                context.Message.PerformedBy,
                context.Message.RequestId,
                context.CancellationToken);

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

            await context.RespondAsync(new MarkAssetsInUseCommandResponse(isSuccess));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
            throw;
        }
    }
}
