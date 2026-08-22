using Microsoft.EntityFrameworkCore;

namespace RescuePC.Software.EntityFrameworkCore.Tests.Conventions;

public class TableNameConventionTests
{
    [Fact]
    public void TableName_ShouldMatchClassName()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TableNameConventionTest")
            .Options;

        using var context = new TestDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(SampleEntity));

        Assert.Equal(nameof(SampleEntity), entityType!.GetTableName());
    }

    [Fact]
    public void TableName_ShouldMatchClassName_ForMultipleEntities()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TableNameConventionMultiTest")
            .Options;

        using var context = new TestDbContext(options);

        Assert.Equal(nameof(SampleEntity), context.Model.FindEntityType(typeof(SampleEntity))!.GetTableName());
        Assert.Equal(nameof(AnotherEntity), context.Model.FindEntityType(typeof(AnotherEntity))!.GetTableName());
    }

    [Fact]
    public void TableName_ShouldNotBeOverridden_ForSharedClrTypeEntities()
    {
        var options = new DbContextOptionsBuilder<SharedTypeDbContext>()
            .UseInMemoryDatabase("TableNameConventionSharedTypeTest")
            .Options;

        using var context = new SharedTypeDbContext(options);
        var entityType = context.Model.FindEntityType("SharedEntityOne");

        Assert.NotNull(entityType);
        Assert.True(entityType.HasSharedClrType);
        Assert.Equal("SharedEntityOne", entityType.GetTableName());
    }

    private class SharedTypeDbContext : EfContext
    {
        public SharedTypeDbContext(DbContextOptions options) : base(options) { }

        protected override string DefaultSchema => "test";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("SharedEntityOne", b =>
            {
                b.Property<int>("Id");
                b.HasKey("Id");
            });
        }
    }

    private class TestDbContext : EfContext
    {
        public DbSet<SampleEntity> SampleEntities => Set<SampleEntity>();
        public DbSet<AnotherEntity> AnotherEntities => Set<AnotherEntity>();

        public TestDbContext(DbContextOptions options) : base(options) { }

        protected override string DefaultSchema => "test";
    }

    private class SampleEntity
    {
        public int Id { get; set; }
    }

    private class AnotherEntity
    {
        public int Id { get; set; }

    }
}
