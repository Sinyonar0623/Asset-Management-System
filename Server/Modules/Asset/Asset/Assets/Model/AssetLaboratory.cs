using Shared.DDD;


namespace Asset.Assets.Model;

public class AssetLaboratory : Entity<long>
{
    public string LaboratoryName {get; private set;} = null!;
    public string RoomNo {get; private set;} = null!;
    public Guid TeacherId {get; private set;}
    public string Description {get; private set;} = null!;


    private AssetLaboratory() {}

    private AssetLaboratory (
        string laboratoryName,
        string roomNo,
        Guid teacherId,
        string description
    )
    {
        LaboratoryName = laboratoryName;
        RoomNo = roomNo;
        TeacherId = teacherId;
        Description = description;
    }

    public static AssetLaboratory Create(
        string laboratoryName,
        string roomNo,
        Guid teacherId,
        string description
    )
    {
        return new AssetLaboratory(
            laboratoryName,
            roomNo,
            teacherId,
            description
        );
    }
}
