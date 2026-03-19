namespace Asset.Assets.Features.LaboratoryFeature.DeleteLaboratory;

public record DeleteLaboratoryCommand(Guid Id) : ICommand<DeleteLaboratoryResult>;
