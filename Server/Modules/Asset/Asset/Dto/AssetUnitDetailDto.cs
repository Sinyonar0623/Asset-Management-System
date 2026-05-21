namespace Asset.Dto;

public sealed record AssetUnitDetailDto
{
    public AssetUnitDto Unit { get; init; } = new();
    public List<AssetUnitImageDto> Images { get; init; } = [];
    public List<AssetHistoryDto> Histories { get; init; } = [];
}
