using Shared.DDD;

namespace Asset.Assets.Model;

public class AssetUnitImage : Entity<Guid>
{
    public string ImageUrl { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? FileName { get; private set; }
    public string? ContentType { get; private set; }
    public long? FileSizeBytes { get; private set; }
    public bool IsPrimary { get; private set; }

    public AssetUnit AssetUnit { get; private set; } = default!;

    private AssetUnitImage() {}

    private AssetUnitImage(
        string imageUrl,
        string? description,
        string? fileName,
        string? contentType,
        long? fileSizeBytes,
        bool isPrimary)
    {
        Id = Guid.NewGuid();
        ImageUrl = imageUrl;
        Description = description;
        FileName = fileName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
        IsPrimary = isPrimary;
    }

    public static AssetUnitImage Create(
        string imageUrl,
        string? description = null,
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
            NormalizeDescription(description),
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
        string? description = null,
        string? fileName = null,
        string? contentType = null,
        long? fileSizeBytes = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ArgumentException("Image url is required.", nameof(imageUrl));
        }

        ImageUrl = imageUrl.Trim();
        Description = NormalizeDescription(description);
        FileName = fileName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var normalizedDescription = description.Trim();
        if (normalizedDescription.Length > 1000)
        {
            throw new ArgumentException("Image description must not exceed 1000 characters.", nameof(description));
        }

        return normalizedDescription;
    }
}
