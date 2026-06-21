using TattoStudio.Api;
using TattoStudio.Api.Controllers;
using TattoStudio.Application;
using TattoStudio.Infrastructure;
using TattoStudio.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthController();
app.MapClientsController();
app.MapArtistsController();
app.MapAppointmentsController();
app.MapConsentsController();

app.Run();

public partial class Program { }
