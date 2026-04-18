namespace Asset.Assets.Model;

public static class AssetUnitStatuses
{
    public static class Availability
    {
        public const string PendingActivation = "PENDING_ACTIVATION";
        public const string Available = "AVAILABLE";
        public const string Reserved = "RESERVED";
        public const string InUse = "IN_USE";
        public const string Unavailable = "UNAVAILABLE";

        public static readonly HashSet<string> All =
        [
            PendingActivation,
            Available,
            Reserved,
            InUse,
            Unavailable
        ];
    }

    public static class Operational
    {
        public const string Ready = "READY";
        public const string Maintenance = "MAINTENANCE";
        public const string Damaged = "DAMAGED";
        public const string Lost = "LOST";
        public const string Retired = "RETIRED";

        public static readonly HashSet<string> All =
        [
            Ready,
            Maintenance,
            Damaged,
            Lost,
            Retired
        ];
    }

    public static bool IsValidAvailability(string status)
    {
        return Availability.All.Contains(status);
    }

    public static bool IsValidOperational(string status)
    {
        return Operational.All.Contains(status);
    }

    public static bool IsReadyForAssignAsset(string availabilityStatus, string operationalStatus)
    {
        return operationalStatus == Operational.Ready
               && (availabilityStatus == Availability.Available
                   || availabilityStatus == Availability.PendingActivation);
    }

    public static bool IsPendingActivationReady(string availabilityStatus, string operationalStatus)
    {
        return availabilityStatus == Availability.PendingActivation
               && operationalStatus == Operational.Ready;
    }
}
