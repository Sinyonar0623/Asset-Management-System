using Shared.CQRS;

namespace Auth.Authentication.Features.GetAllTeacher;

public record GetAllTeacherQuery() : IQuery<GetAllTeacherResult>;