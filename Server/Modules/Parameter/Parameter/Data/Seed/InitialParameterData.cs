using ParameterEntity = Parameter.Parameters.Model.Parameter;

namespace Parameter.Data.Seed;

public static class InitialParameterData
{
    public static IReadOnlyList<ParameterEntity> Parameters { get; } =
    [
        Create("REQUEST_TYPE", "BORROW", "Borrow request"),
        Create("REQUEST_TYPE", "REPAIR", "Repair request"),
        Create("REQUEST_TYPE", "RETIRE", "Retire request"),

        Create("REQUEST_STATUS", "PENDING", "Request is waiting for approval"),
        Create("REQUEST_STATUS", "APPROVED", "Request has been approved"),
        Create("REQUEST_STATUS", "REJECTED", "Request has been rejected"),
        Create("REQUEST_STATUS", "CANCELLED", "Request has been cancelled"),
        Create("REQUEST_STATUS", "COMPLETED", "Request has been completed"),

        Create("TRACKING_STATUS", "WAITING", "Approval step is waiting"),
        Create("TRACKING_STATUS", "PENDING", "Approval step is active"),
        Create("TRACKING_STATUS", "APPROVED", "Approval step has been approved"),
        Create("TRACKING_STATUS", "REJECTED", "Approval step has been rejected"),
        Create("TRACKING_STATUS", "SKIPPED", "Approval step has been skipped"),
        Create("TRACKING_STATUS", "CANCELLED", "Approval step has been cancelled"),

        Create("REQUEST_DECISION", "APPROVE", "Approve the current approval step"),
        Create("REQUEST_DECISION", "REJECT", "Reject the current approval step"),

        Create("ASSET_AVAILABILITY_STATUS", "PENDING_ACTIVATION", "Asset is pending activation"),
        Create("ASSET_AVAILABILITY_STATUS", "AVAILABLE", "Asset is available"),
        Create("ASSET_AVAILABILITY_STATUS", "RESERVED", "Asset is reserved"),
        Create("ASSET_AVAILABILITY_STATUS", "IN_USE", "Asset is in use"),
        Create("ASSET_AVAILABILITY_STATUS", "UNAVAILABLE", "Asset is unavailable"),
        Create("A001", "PENDING_ACTIVATION", "Pending activation"),
        Create("A001", "AVAILABLE", "Available"),
        Create("A001", "RESERVED", "Reserved"),
        Create("A001", "IN_USE", "In use"),
        Create("A001", "UNAVAILABLE", "Unavailable"),

        Create("ASSET_OPERATIONAL_STATUS", "READY", "Asset is ready for use"),
        Create("ASSET_OPERATIONAL_STATUS", "MAINTENANCE", "Asset is under maintenance"),
        Create("ASSET_OPERATIONAL_STATUS", "DAMAGED", "Asset is damaged"),
        Create("ASSET_OPERATIONAL_STATUS", "LOST", "Asset is lost"),
        Create("ASSET_OPERATIONAL_STATUS", "RETIRED", "Asset is retired"),
        Create("A002", "READY", "Ready"),
        Create("A002", "MAINTENANCE", "Maintenance"),
        Create("A002", "DAMAGED", "Damaged"),
        Create("A002", "LOST", "Lost"),
        Create("A002", "RETIRED", "Retired"),

        Create("ROLE_CODE", "ADMIN", "System administrator"),
        Create("ROLE_CODE", "HOD", "Department head"),
        Create("ROLE_CODE", "TEACHER", "Lecturer"),
        Create("ROLE_CODE", "STUDENT", "Student"),

        Create("H001", "01", "Home"),
        Create("H001", "02", "Task"),
        Create("H001", "03", "Inventory"),
        Create("H001", "04", "Report"),
        Create("H001", "05", "Setting")
    ];

    private static ParameterEntity Create(string group, string value, string description)
    {
        var parameter = ParameterEntity.Create(group, value, description);
        parameter.CreateBy = "SYSTEM";
        return parameter;
    }
}
