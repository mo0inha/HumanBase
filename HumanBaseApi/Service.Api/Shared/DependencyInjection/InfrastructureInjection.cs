using Application.Shared;
using Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;

namespace Services.Api.Shared.DependencyInjection;

public static class InfrastructureInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Scan(selector => selector.FromAssemblies(AssemblyReference.Assembly).AddClasses(false).AsMatchingInterface().WithScopedLifetime());

        services.AddDbContext<BaseContext>(options =>
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Database connectionString not established");
            }

            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
