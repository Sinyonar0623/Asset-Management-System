using Auth.Dto;

namespace Auth.Authentication.Features.GetAllTeacher;

public record GetAllTeacherResult(List<TeacherDto> Teachers);