namespace Asset.Assets.Features.LaboratoryFeature.CreateLaboratory;

public record CreateLaboratoryCommand(LaboratoryDto Laboratory) : ICommand<CreateLaboratoryResult>;
