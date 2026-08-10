using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace RescuePC.Software.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static void AddEntityFramework<TContext>(this IServiceCollection services,
        string connectionString,
        Type[]? interceptors = null,
        Action<IServiceCollection>? repositories = null)
        where TContext : EfContext
    {
        services.AddDbContext<TContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);

            if (interceptors is not null)
            {
                foreach (var interceptorType in interceptors)
                {
                    options.AddInterceptors((IInterceptor)sp.GetRequiredService(interceptorType));
                }
            }
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TContext>());

        repositories?.Invoke(services);
    }
}