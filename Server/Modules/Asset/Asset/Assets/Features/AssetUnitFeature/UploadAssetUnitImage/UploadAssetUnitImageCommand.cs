namespace Asset.Assets.Features.AssetUnitFeature.UploadAssetUnitImage;

public record UploadAssetUnitImageCommand(
    Guid AssetUnitId,
    List<CreateAssetUnitImageDto> Images) : ICommand<UploadAssetUnitImageResult>;
