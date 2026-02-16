namespace Parameter.Dto;

public record ParameterDto
(
    string Group,
    string Value,
    string Description,
    bool Active
);