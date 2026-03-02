using Mapster;
using Auth.Dto;
using Auth.Authentication.Model;
using Auth.Authentication.Features.SignUp;

namespace Auth.Configuration;

public static class MappingConfiguration
{
    public static void ConfigurationMappings()
    {
        TypeAdapterConfig<UsernameDto, UserName>
            .NewConfig()
            .ConstructUsing(src => UserName.Create(
                src.Username,
                src.Email
            ));

        TypeAdapterConfig<SignUpRequest, UsernameDto>
            .NewConfig()
            .ConstructUsing(src => new UsernameDto(
                src.Username,
                src.Email,
                src.Password,
                src.RoleCode
            ));
    }
}