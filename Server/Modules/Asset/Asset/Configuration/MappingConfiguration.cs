using Asset.Assets.Model;
using Asset.Assets.ValueObject;
using Asset.Dto;
using Mapster;
using AssetModel = Asset.Assets.Model.Asset;

namespace Asset.Configuration;

public static class MappingConfiguration
{
    public static void ConfigurationMappings()
    {
        TypeAdapterConfig<LaboratoryDto, Laboratory>
            .NewConfig()
            .ConstructUsing(src => Laboratory.Create(
                src.LaboratoryName,
                src.RoomNo,
                src.TeacherId,
                src.Description
            ));

        TypeAdapterConfig<AssetDto, AssetModel>
            .NewConfig()
            .ConstructUsing(src => AssetModel.Create(
                src.Name,
                src.Description,
                src.Category
            ));

        TypeAdapterConfig<AssetHistoryDto, AssetHistory>
            .NewConfig()
            .ConstructUsing(src => AssetHistory.Create(
                src.ActionType,
                src.Remark,
                src.PerformedBy,
                src.PerformedAt,
                src.FromAvailabilityStatus,
                src.ToAvailabilityStatus,
                src.FromOperationalStatus,
                src.ToOperationalStatus,
                src.FromResponsibleUserId,
                src.ToResponsibleUserId,
                src.ApprovedBy,
                src.ApprovedAt,
                src.ReferenceNo,
                src.RequestId
            ));

        TypeAdapterConfig<AssetUnitConditionDto, AssetUnitCondition>
            .NewConfig()
            .ConstructUsing(src => AssetUnitCondition.Create(
                src.Reason,
                src.EffectiveFrom,
                src.IsEffectiveFromUnknown,
                src.EffectiveTo,
                src.IsEffectiveToUnknown
            ));

        TypeAdapterConfig<AssetUnitDto, AssetUnit>
            .NewConfig()
            .ConstructUsing(src => AssetUnit.Create(
                src.AssetTag,
                src.SerialNo,
                src.Name,
                src.Brand,
                src.AvailabilityStatus,
                src.OperationalStatus,
                src.Remark,
                src.ResponsibleUserId
            ));

        TypeAdapterConfig<AssetModel, AssetDto>
            .NewConfig();

        TypeAdapterConfig<AssetUnit, AssetUnitDto>
            .NewConfig()
            .Map(dest => dest.AssetId, src => src.Asset != null ? src.Asset.Id : (Guid?)null);
    }
}
