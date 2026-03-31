namespace Asset.Assets.Features.LaboratoryFeature.UpdateLaboratory;

public record UpdateLaboratoryCommand(Guid Id, LaboratoryDto Laboratory) : ICommand<UpdateLaboratoryResult>;
