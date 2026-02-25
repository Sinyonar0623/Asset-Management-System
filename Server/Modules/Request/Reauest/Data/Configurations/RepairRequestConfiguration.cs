using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reauest.Requests.Model;

namespace Reauest.Data.Configurations;

public class RepairRequestConfiguration : IEntityTypeConfiguration<RepairRequest>
{
    public void Configure(EntityTypeBuilder<RepairRequest> builder)
    {
        builder.ToTable("RepairRequests");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn();

        builder.Property(r => r.RequesterId).IsRequired();
        builder.Property(r => r.RequesterName).HasMaxLength(200).IsRequired();
        builder.Property(r => r.AssetId).IsRequired();
        builder.Property(r => r.AssetName).HasMaxLength(200).IsRequired();
        builder.Property(r => r.AssetRealWorldId).HasMaxLength(100).IsRequired();
        builder.Property(r => r.ProblemDescription).HasMaxLength(1000).IsRequired();
        builder.Property(r => r.Status).HasMaxLength(50).IsRequired();
        builder.Property(r => r.ApprovalRemark).HasMaxLength(500).IsRequired(false);
        builder.Property(r => r.ApprovedBy).IsRequired(false);
        builder.Property(r => r.ApprovedAt).IsRequired(false);
        builder.Property(r => r.CompletedAt).IsRequired(false);
        builder.Property(r => r.TechnicianNote).HasMaxLength(1000).IsRequired(false);

        builder.HasIndex(r => r.RequesterId);
        builder.HasIndex(r => r.AssetId);
        builder.HasIndex(r => r.Status);
    }
}
