using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.Interfaces;
using TattoStudio.Infraestructure.Persistence;
using TattoStudio.Infraestructure.Repositories.Appoinments;

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

            return services;
        }
    }
}
