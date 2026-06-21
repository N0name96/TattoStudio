using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TattoStudio.Api.Middleware;
using TattoStudio.Infrastructure.Settings;

namespace TattoStudio.Api;

/// <summary>
/// Registro de dependencias de la capa API: Swagger, autenticación JWT, políticas de autorización
/// y manejador global de excepciones.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios propios de la capa API en el contenedor de DI.
    /// </summary>
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Botón "Authorize" en Swagger UI con soporte Bearer JWT
            var scheme = new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Introduce el token JWT obtenido en /api/auth/login"
            };

            options.AddSecurityDefinition("Bearer", scheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                          ?? new JwtSettings();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuer   = true,
                    ValidIssuer      = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience    = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew        = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("Staff",     policy => policy.RequireClaim("role", "0", "1"))
            .AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "0"));

        return services;
    }
}
