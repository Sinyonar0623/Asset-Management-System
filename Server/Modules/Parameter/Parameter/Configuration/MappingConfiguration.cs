using Mapster;
using Parameter.Dto;

namespace Parameter.Configuration;

public static class MappingConfiguration
{
    public static void ConfigurationMappings()
    {
        TypeAdapterConfig<ParameterDto, Parameters.Model.Parameter>
            .NewConfig()
            .ConstructUsing(src => Parameters.Model.Parameter.Create(
                src.Group,
                src.Value,
                src.Description
            ));
    }
}