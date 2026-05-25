namespace Asset.Assets.Features.AssetFeature.AssignLaboratory;

public record AssignLaboratoryCommand(Guid AssetId, Guid LabId) : ICommand<AssignLaboratoryResult>;