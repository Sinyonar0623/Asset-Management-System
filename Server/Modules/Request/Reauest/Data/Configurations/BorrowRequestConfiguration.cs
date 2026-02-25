using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reauest.Requests.Model;

namespace Reauest.Data.Configurations;

public class BorrowRequestConfiguration : IEntityTypeConfiguration<BorrowRequest>
{
    public void Configure(EntityTypeBuilder<BorrowRequest> builder)
    {
        builder.ToTable("BorrowRequests");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).UseIdentityColumn();

        builder.Property(b => b.RequesterId).IsRequired();
        builder.Property(b => b.RequesterName).HasMaxLength(200).IsRequired();
        builder.Property(b => b.AssetId).IsRequired();
        builder.Property(b => b.AssetName).HasMaxLength(200).IsRequired();
        builder.Property(b => b.AssetRealWorldId).HasMaxLength(100).IsRequired();
        builder.Property(b => b.BorrowDate).IsRequired();
        builder.Property(b => b.ReturnDate).IsRequired();
        builder.Property(b => b.Purpose).HasMaxLength(500).IsRequired();
        builder.Property(b => b.Status).HasMaxLength(50).IsRequired();
        builder.Property(b => b.ApprovalRemark).HasMaxLength(500).IsRequired(false);
        builder.Property(b => b.ApprovedBy).IsRequired(false);
        builder.Property(b => b.ApprovedAt).IsRequired(false);
        builder.Property(b => b.ActualReturnDate).IsRequired(false);

        builder.HasIndex(b => b.RequesterId);
        builder.HasIndex(b => b.AssetId);
        builder.HasIndex(b => b.Status);
    }
}
