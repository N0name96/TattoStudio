using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Reqnroll.BoDi;
using TattoStudio.Infrastructure.Persistence;

namespace TattoStudio.AcceptanceTests.Support;

[Binding]
/// <summary>
/// Hooks de Reqnroll: levanta un <see cref="WebApplicationFactory{Program}"/> con base de datos
/// InMemory aislada por escenario y registra el <see cref="HttpClient"/> en el contenedor BoDi.
/// </summary>
public class Hooks
{
    private readonly IObjectContainer _container;
    private WebApplicationFactory<Program>? _factory;

    public Hooks(IObjectContainer container) => _container = container;

    [BeforeScenario]
    public void BeforeScenario()
    {
        // Cada escenario obtiene su propia base de datos InMemory aislada.
        var dbName = Guid.NewGuid().ToString();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Eliminar el DbContext real (PostgreSQL / Supabase)
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<TattoStudioDbContext>));
                    if (descriptor is not null)
                        services.Remove(descriptor);

                    // Sustituir por InMemory para los tests
                    services.AddDbContext<TattoStudioDbContext>(options =>
                        options.UseInMemoryDatabase(dbName));
                });
            });

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _container.RegisterInstanceAs(client);
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        _container.Resolve<HttpClient>().Dispose();
        if (_factory is not null)
            await _factory.DisposeAsync();
    }
}
