namespace Asset.Assets.Features.LaboratoryFeature.AssignTeacher;

public record AssignTeacherCommand(Guid LaboratoryId, Guid TeacherId) : ICommand<AssignTeacherResult>;
