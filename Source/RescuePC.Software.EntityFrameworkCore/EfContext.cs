using Microsoft.EntityFrameworkCore;
using RescuePC.Software.EntityFrameworkCore;
using RescuePC.Software.EntityFrameworkCore.Conventions;

public abstract class EfContext : DbContext, IUnitOfWork
{
    protected abstract string DefaultSchema { get; }

    protected EfContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
        configurationBuilder.Conventions.Add(_ => new TableNameConvention());
    }
}