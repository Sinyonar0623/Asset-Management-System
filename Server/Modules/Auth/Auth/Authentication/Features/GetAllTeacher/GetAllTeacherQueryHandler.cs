using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.GetAllTeacher;

public class GetAllTeacherQueryHandler(
    IAuthService Service
) : IQueryHandler<GetAllTeacherQuery, GetAllTeacherResult>
{
    private readonly IAuthService _service = Service;
    public async Task<GetAllTeacherResult> Handle(GetAllTeacherQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _service.GetAllUserAsTeacher(cancellationToken);

        return new GetAllTeacherResult(teachers);
    }
}
