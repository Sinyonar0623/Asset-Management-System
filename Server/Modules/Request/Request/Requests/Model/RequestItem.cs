namespace Request.Requests.Model;

public class RequestItem
{
    public Guid AssetId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private RequestItem() {}

    private RequestItem(Guid assetId)
    {
        if (assetId == Guid.Empty)
        {
            throw new ArgumentException("AssetId is required.", nameof(assetId));
        }

        AssetId = assetId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static RequestItem Create(Guid assetId)
    {
        return new RequestItem(assetId);
    }

    public void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
