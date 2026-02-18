using Shared.DDD;

namespace Asset.Assets.Model;

public class Laboratory : Aggregate<long>
{
    public string LaboratoryName { get; private set; } = null!;
    public string RoomNo { get; private set; } = null!;
    public Guid TeacherId { get; private set; }
    public string Description { get; private set; } = null!;

    private Laboratory() {}

    private Laboratory(
        string laboratoryName,
        string roomNo,
        Guid teacherId,
        string description)
    {
        LaboratoryName = laboratoryName;
        RoomNo = roomNo;
        TeacherId = teacherId;
        Description = description;
    }

    public static Laboratory Create(
        string laboratoryName,
        string roomNo,
        Guid teacherId,
        string description)
    {
        return new Laboratory(laboratoryName, roomNo, teacherId, description);
    }

    public void Update(
        string laboratoryName,
        string roomNo,
        Guid teacherId,
        string description)
    {
        LaboratoryName = laboratoryName;
        RoomNo = roomNo;
        TeacherId = teacherId;
        Description = description;
    }
}
