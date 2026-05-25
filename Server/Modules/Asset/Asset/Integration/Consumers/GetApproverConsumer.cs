using Asset.Service.CommandHandlerService;
using MassTransit;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Asset.Integration.Consumers;

public sealed class GetApproverConsumer(
    ILaboratoryCommandHandlerService laboratoryService
) : IConsumer<GetApproverIntegration>
{
    private readonly ILaboratoryCommandHandlerService _laboratoryService = laboratoryService;

    public async Task Consume(ConsumeContext<GetApproverIntegration> context)
    {
        var laboratory = await _laboratoryService.GetLaboratoryById(
            context.Message.LabId,
            context.CancellationToken);

        if (!laboratory.TeacherId.HasValue)
        {
            throw new KeyNotFoundException($"Teacher approver was not found for laboratory id {context.Message.LabId}.");
        }

        await context.RespondAsync(new GetApproverIntegrationResponse(laboratory.TeacherId.Value));
    }
}
