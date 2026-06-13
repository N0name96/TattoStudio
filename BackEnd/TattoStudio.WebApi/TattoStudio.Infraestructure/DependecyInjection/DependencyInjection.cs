using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.Interfaces;
using TattoStudio.Infraestructure.Persistence;
using TattoStudio.Infraestructure.Repositories.Appoinments;
using TattoStudio.Infraestructure.Repositories.Artists;
using TattoStudio.Infraestructure.Repositories.Users;
using TattoStudio.Infraestructure.Services;

namespace TattoStudio.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAppoinmentRepository, AppoinmentRepository>();
            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
