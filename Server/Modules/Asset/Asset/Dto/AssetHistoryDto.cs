namespace Asset.Dto;

public record AssetHistoryDto(
    string Purpose,
    string Remark,
    Guid ApproveBy,
    DateTime ApproveAt
);
