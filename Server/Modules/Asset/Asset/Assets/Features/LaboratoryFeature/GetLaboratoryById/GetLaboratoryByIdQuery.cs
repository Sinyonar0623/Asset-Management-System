namespace Asset.Assets.Features.LaboratoryFeature.GetLaboratoryById;

public record GetLaboratoryByIdQuery(Guid Id) : IQuery<GetLaboratoryByIdResult>;
