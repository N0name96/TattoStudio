using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.Behaviors;

namespace TattoStudio.Application;

/// <summary>
/// Registro de dependencias de la capa Application: MediatR, AutoMapper y FluentValidation.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra MediatR con pipeline de validación, AutoMapper y todos los validadores del ensamblado.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
