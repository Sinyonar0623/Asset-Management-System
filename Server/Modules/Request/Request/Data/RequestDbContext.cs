namespace Request.Data;

public class RequestDbContext(DbContextOptions<RequestDbContext> options) : DbContext(options)
{
    public DbSet<Requests.Model.Request> Requests => Set<Requests.Model.Request>();
    public DbSet<RequestDetail> RequestDetail => Set<RequestDetail>();
    public DbSet<RequestItem> RequestItems => Set<RequestItem>();
    public DbSet<RequestTracking> RequestTrackings => Set<RequestTracking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("request");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new OutboxConfiguration(excludeFromMigrations: false));
        modelBuilder.ApplyAuditConventions();        

        base.OnModelCreating(modelBuilder);
    }
}