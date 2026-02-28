namespace Request.Requests.Model;

public class RequestItem
{
    public Guid AssetId { get; private set; }
    public int QuantityRequested { get; private set; }
    public int? QuantityApproved { get; private set; }
    public string? Note { get; private set; }

    private RequestItem() {}

    private RequestItem(Guid assetId, int quantityRequested, string? note)
    {
        if (quantityRequested <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityRequested), "Quantity must be greater than zero.");
        }

        AssetId = assetId;
        QuantityRequested = quantityRequested;
        Note = note;
    }

    public static RequestItem Create(Guid assetId, int quantityRequested, string? note = null)
    {
        return new RequestItem(assetId, quantityRequested, note);
    }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        QuantityRequested += quantity;
    }

    public void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        QuantityRequested = quantity;
    }

    public void SetApprovalQuantity(int quantityApproved)
    {
        if (quantityApproved < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityApproved), "Approved quantity cannot be negative.");
        }

        QuantityApproved = quantityApproved;
    }

    public void UpdateNote(string? note)
    {
        Note = note;
    }
}
