using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RescuePC.Software.EntityFrameworkCore.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEntityFramework_ShouldRegister_IUnitOfWork()
    {
        var services = new ServiceCollection();
        services.AddEntityFramework<TestDbContext>("Data Source=:memory:");

        var provider = services.BuildServiceProvider();
        var unitOfWork = provider.GetService<IUnitOfWork>();

        Assert.NotNull(unitOfWork);
        Assert.IsAssignableFrom<TestDbContext>(unitOfWork);
    }

    [Fact]
    public void AddEntityFramework_ShouldRegister_DbContext()
    {
        var services = new ServiceCollection();
        services.AddEntityFramework<TestDbContext>("Data Source=:memory:");

        var provider = services.BuildServiceProvider();
        var context = provider.GetService<TestDbContext>();

        Assert.NotNull(context);
    }

    [Fact]
    public void AddEntityFramework_ShouldInvoke_RepositoriesAction()
    {
        var services = new ServiceCollection();
        var invoked = false;

        services.AddEntityFramework<TestDbContext>("Data Source=:memory:", repositories: _ => invoked = true);

        Assert.True(invoked);
    }

    [Fact]
    public void AddEntityFramework_WithInterceptors_ShouldNotThrow()
    {
        var services = new ServiceCollection();

        var exception = Record.Exception(() =>
            services.AddEntityFramework<TestDbContext>("Data Source=:memory:", interceptors: []));

        Assert.Null(exception);
    }

    private class TestDbContext : EfContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        protected override string DefaultSchema => "test";
    }
}
