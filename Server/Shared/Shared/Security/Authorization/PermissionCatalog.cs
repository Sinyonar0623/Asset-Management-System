namespace Shared.Security.Authorization;

public static class PermissionCatalog
{
    public const string AssetRead = "ASSET_READ";
    public const string AssetWrite = "ASSET_WRITE";
    public const string RequestRead = "REQUEST_READ";
    public const string RequestWrite = "REQUEST_WRITE";

    public static readonly string[] All =
    [
        AssetRead,
        AssetWrite,
        RequestRead,
        RequestWrite
    ];
}
