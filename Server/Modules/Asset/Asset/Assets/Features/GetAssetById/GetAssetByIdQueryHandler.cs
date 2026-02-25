using Asset.Data.Repository;
using Asset.Dto;
using Shared.CQRS;

namespace Asset.Assets.Features.GetAssetById;

public sealed class GetAssetByIdQueryHandler(IAssetRepository assetRepository)
    : IQueryHandler<GetAssetByIdQuery, GetAssetByIdResult>
{
    private readonly IAssetRepository _repository = assetRepository;

    public async Task<GetAssetByIdResult> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (asset is null)
            return new GetAssetByIdResult(null, false);

        var dto = new AssetDto(
            asset.Id,
            asset.RealWorldId,
            asset.Brand,
            asset.Name,
            asset.SerialNo,
            asset.Description,
            asset.Type,
            asset.Status,
            asset.Amount,
            asset.Remark,
            asset.OwnerId,
            asset.Laboratory is null ? null : new AssetLaboratoryDto(
                asset.Laboratory.Id,
                asset.Laboratory.LaboratoryName,
                asset.Laboratory.RoomNo,
                asset.Laboratory.TeacherId,
                asset.Laboratory.Description
            ),
            asset.Components.Select(c => new AssetComponentDto(
                c.Id,
                c.RealWorldId,
                c.Brand,
                c.Name,
                c.SerialNo,
                c.Description,
                c.Type,
                c.Remark
            )).ToList()
        );

        return new GetAssetByIdResult(dto, true);
    }
}
