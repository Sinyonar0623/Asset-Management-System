using Asset.Data;
using Asset.Service.CommandHandlerService;
using MassTransit;
using Shared.Data.UnitOfWork;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Asset.Integration.Consumers;

public sealed class ReleaseAssetConsumer(
    IAssetCommandHandlerService service,
    IUnitOfWork<AssetDbContext> unitOfWork
) : IConsumer<ReleaseAssetCommand>
{
    private readonly IAssetCommandHandlerService _service = service;
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;

    public async Task Consume(ConsumeContext<ReleaseAssetCommand> context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

            var isSuccess = await _service.ReleaseAssetsAsync(
                context.Message.AssetIds,
                context.CancellationToken);

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

            await context.RespondAsync(new ReleaseAssetCommandResponse(isSuccess));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
            throw;
        }
    }
}
