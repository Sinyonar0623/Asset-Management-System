namespace request.Data.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request.Requests.Model.Request>
{
    public void Configure(EntityTypeBuilder<Request.Requests.Model.Request> builder)
    {
        builder.ToTable("Requests");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.RequestNo).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.RequestNo).IsUnique();

        builder.Property(x => x.RequestType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(2000).IsRequired();

        builder.Property(x => x.SubmittedOn).IsRequired();
        builder.Property(x => x.FinalizedOn).IsRequired(false);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(x => new { x.Status, x.NextApproverId });
        builder.HasIndex(x => x.RequesterId);

        builder.HasOne(x => x.Detail)
            .WithOne(x => x.Request)
            .HasForeignKey<RequestDetail>("RequestId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RequestDetail_RequestId");

        builder.OwnsMany(x => x.Items, item =>
        {
            item.ToTable("RequestItems");
            item.WithOwner().HasForeignKey("RequestId");

            item.Property<long>("Id").UseIdentityColumn();
            item.HasKey("Id");

            item.Property(x => x.AssetId).IsRequired();
            item.Property(x => x.QuantityRequested).IsRequired();
            item.Property(x => x.QuantityApproved).IsRequired(false);
            item.Property(x => x.Note).HasMaxLength(1000).IsRequired(false);

            item.HasIndex("RequestId", nameof(RequestItem.AssetId)).IsUnique();
            item.HasIndex(nameof(RequestItem.AssetId));
        });

        builder.OwnsMany(x => x.Trackings, tracking =>
        {
            tracking.ToTable("RequestTrackings");
            tracking.WithOwner().HasForeignKey("RequestId");

            tracking.Property<long>("Id").UseIdentityColumn();
            tracking.HasKey("Id");

            tracking.Property(x => x.StepNo).IsRequired();
            tracking.Property(x => x.RequiredRoleCode).HasMaxLength(20).IsRequired();
            tracking.Property(x => x.Status).HasMaxLength(20).IsRequired();
            tracking.Property(x => x.Comment).HasMaxLength(2000).IsRequired(false);
            tracking.Property(x => x.IsCurrent).IsRequired();

            tracking.HasIndex("RequestId", nameof(RequestTracking.StepNo)).IsUnique();
            tracking.HasIndex("RequestId")
                .HasFilter("[IsCurrent] = 1")
                .IsUnique();
            tracking.HasIndex(nameof(RequestTracking.AssignedApproverId), nameof(RequestTracking.Status));
        });

        builder.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Trackings).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
