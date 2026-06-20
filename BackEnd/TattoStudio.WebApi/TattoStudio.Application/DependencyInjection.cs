using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TattoStudio.Application;

/// <summary>
/// Registro de dependencias de la capa Application: MediatR y AutoMapper.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra MediatR (handlers CQRS) y AutoMapper (perfiles de mapeo) en el contenedor DI.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}
