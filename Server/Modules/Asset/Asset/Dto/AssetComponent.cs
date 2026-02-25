namespace Asset.Dto;

public record AssetComponentDto(
    long Id,
    string RealWorldId,
    string Brand,
    string Name,
    string SerialNo,
    string Description,
    string Type,
    string Remark
);
