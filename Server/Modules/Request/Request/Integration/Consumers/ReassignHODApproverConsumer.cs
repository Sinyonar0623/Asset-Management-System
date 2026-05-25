using MassTransit;
using Request.Data;
using Request.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Request.Integration.Consumers;

public sealed class ReassignHODApproverConsumer(
    IRequestCommandHandlerService service,
    IUnitOfWork<RequestDbContext> unitOfWork
) : IConsumer<ReassignHODApproverCommand>
{
    private readonly IRequestCommandHandlerService _service = service;
    private readonly IUnitOfWork<RequestDbContext> _unitOfWork = unitOfWork;

    public async Task Consume(ConsumeContext<ReassignHODApproverCommand> context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

            var isSuccess = await _service.ReassignHODApprover(
                context.Message.OldHODApproverId,
                context.Message.NewHODApproverId,
                context.CancellationToken);

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

            await context.RespondAsync(new ReassignHODApproverCommandResponse(isSuccess));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
            throw;
        }
    }
}
