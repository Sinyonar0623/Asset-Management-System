using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.ReturnAsset;

public sealed class ReturnAssetCommandHandler(
    IAssetCommandHandlerService service)
    : ICommandHandler<ReturnAssetCommand, ReturnAssetResult>
{
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<ReturnAssetResult> Handle(ReturnAssetCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = await _service.ReturnVisibleAssetAsync(
            request.AssetId,
            request.UserId,
            request.RoleCode,
            cancellationToken);

        return new ReturnAssetResult(isSuccess);
    }
}
