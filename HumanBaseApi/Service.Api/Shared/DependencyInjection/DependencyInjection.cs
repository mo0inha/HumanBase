using Application.Shared;
using Application.Shared.Interfaces;
using FluentValidation;
using MediatR;

namespace Service.Api.Shared.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services) => services.AddValidators().AddMediatRConfiguration().AddInfrastructure().AddHandlers();

        private static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);
            services.AddSingleton<IValidationProvider, OptionalValidationProvider>();
            return services;
        }

        private static IServiceCollection AddMediatRConfiguration(this IServiceCollection services) => services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));

        private static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.BaseType is { IsGenericType: true } baseType && (baseType.GetGenericTypeDefinition() == typeof(BaseCommand<,,>) || baseType.GetGenericTypeDefinition() == typeof(BaseQuery<,,>))).ToList();

                foreach (var handlerType in handlerTypes)
                {
                    foreach (var @interface in handlerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                    {
                        Console.WriteLine($"Registering {@interface.FullName} as {handlerType.FullName}");
                        services.AddScoped(@interface, handlerType);
                    }
                }
            }
            return services;
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection services) => services.AddScoped<IRepository, Repository>();
    }

    public class OptionalValidationProvider(IServiceProvider serviceProvider) : IValidationProvider
    {
        public IValidator<T>? GetValidator<T>() => serviceProvider.GetService<IValidator<T>>();
    }
}