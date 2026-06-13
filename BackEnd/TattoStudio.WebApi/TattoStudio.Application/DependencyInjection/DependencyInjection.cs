using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.Mappings;

namespace TattoStudio.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AppoinmentProfile), typeof(ArtistProfile), typeof(UserProfile)));

            return services;
        }
    }
}
