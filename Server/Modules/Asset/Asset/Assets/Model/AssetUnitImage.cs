using Shared.DDD;

namespace Asset.Assets.Model;

public class AssetUnitImage : Entity<Guid>
{
    public string ImageUrl { get; private set; } = null!;
    public string? FileName { get; private set; }
    public string? ContentType { get; private set; }
    public long? FileSizeBytes { get; private set; }
    public bool IsPrimary { get; private set; }

    public AssetUnit AssetUnit { get; private set; } = default!;

    private AssetUnitImage() {}

    private AssetUnitImage(
        string imageUrl,
        string? fileName,
        string? contentType,
        long? fileSizeBytes,
        bool isPrimary)
    {
        ImageUrl = imageUrl;
        FileName = fileName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
        IsPrimary = isPrimary;
    }

    public static AssetUnitImage Create(
        string imageUrl,
        string? fileName = null,
        string? contentType = null,
        long? fileSizeBytes = null,
        bool isPrimary = false)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ArgumentException("Image url is required.", nameof(imageUrl));
        }

        return new AssetUnitImage(
            imageUrl.Trim(),
            fileName,
            contentType,
            fileSizeBytes,
            isPrimary);
    }

    public void MarkAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void Update(
        string imageUrl,
        string? fileName = null,
        string? contentType = null,
        long? fileSizeBytes = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ArgumentException("Image url is required.", nameof(imageUrl));
        }

        ImageUrl = imageUrl.Trim();
        FileName = fileName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
    }
}
