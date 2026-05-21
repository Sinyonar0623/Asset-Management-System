using Auth.Dto;

namespace Auth.Authentication.Features.GetAllTeacher;

public record GetAllTeacherResponse(List<TeacherDto> Teachers);