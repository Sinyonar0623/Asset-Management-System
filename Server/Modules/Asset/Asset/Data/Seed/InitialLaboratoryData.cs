using Asset.Assets.Model;

namespace Asset.Data.Seed;

public static class InitialLaboratoryData
{
    public static IReadOnlyList<Laboratory> Laboratories { get; } =
    [
        CreateLaboratory(
            Guid.Parse("00000000-0000-0000-0000-000000000101"),
            "CPE Programming Lab 1",
            "G-601",
            Guid.Parse("00000000-0000-0000-0000-000000001001"),
            "Primary lab for programming courses"),
        CreateLaboratory(
            Guid.Parse("00000000-0000-0000-0000-000000000102"),
            "CPE Programming Lab 2",
            "G-602",
            Guid.Parse("00000000-0000-0000-0000-000000001002"),
            "Advanced programming and web development"),
        CreateLaboratory(
            Guid.Parse("00000000-0000-0000-0000-000000000103"),
            "CPE Network Lab",
            "G-603",
            Guid.Parse("00000000-0000-0000-0000-000000001003"),
            "Networking, routing, and server configuration"),
        CreateLaboratory(
            Guid.Parse("00000000-0000-0000-0000-000000000104"),
            "CPE Embedded Systems Lab",
            "G-604",
            Guid.Parse("00000000-0000-0000-0000-000000001004"),
            "Microcontroller and IoT experiments"),
        CreateLaboratory(
            Guid.Parse("00000000-0000-0000-0000-000000000105"),
            "CPE Hardware Lab",
            "G-605",
            Guid.Parse("00000000-0000-0000-0000-000000001005"),
            "Digital logic and circuit practice"),
        CreateLaboratory(
            Guid.Parse("00000000-0000-0000-0000-000000000106"),
            "CPE Computer Architecture Lab",
            "G-606",
            Guid.Parse("00000000-0000-0000-0000-000000001006"),
            "CPU and low-level system study"),
        CreateLaboratory(
            Guid.Parse("00000000-0000-0000-0000-000000000107"),
            "CPE Senior Project Lab",
            "G-607",
            Guid.Parse("00000000-0000-0000-0000-000000001007"),
            "Workspace for final year projects")
    ];

    private static Laboratory CreateLaboratory(
        Guid id,
        string laboratoryName,
        string roomNo,
        Guid teacherId,
        string description)
    {
        var laboratory = Laboratory.Create(laboratoryName, roomNo, teacherId, description);
        laboratory.Id = id;
        laboratory.CreateBy = "SYSTEM";
        return laboratory;
    }
}
