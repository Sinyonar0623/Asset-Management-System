namespace Request.Requests.Model;

public static class RequestTypeCodes
{
    public const string Allocate = "ALLOCATE";
    public const string Borrow = "BORROW";
    public const string Repair = "REPAIR";
    public const string Retire = "RETIRE";
}

public static class RequestStatusCodes
{
    public const string Pending = "PENDING";
    public const string Approved = "APPROVED";
    public const string Rejected = "REJECTED";
    public const string Cancelled = "CANCELLED";
    public const string Completed = "COMPLETED";
}

public static class TrackingStatusCodes
{
    public const string Waiting = "WAITING";
    public const string Pending = "PENDING";
    public const string Approved = "APPROVED";
    public const string Rejected = "REJECTED";
    public const string Skipped = "SKIPPED";
    public const string Cancelled = "CANCELLED";
}

public static class RequestDecisionCodes
{
    public const string Approve = "APPROVE";
    public const string Reject = "REJECT";
}
