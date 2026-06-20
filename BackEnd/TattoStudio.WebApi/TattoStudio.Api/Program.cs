using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TattoStudio.Api.Controllers;
using TattoStudio.Api.Middleware;
using TattoStudio.Application;
using TattoStudio.Infrastructure;
using TattoStudio.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TattoStudioDbContext>();
    if (db.Database.IsRelational())
    {
        var creator = db.Database.GetService<IRelationalDatabaseCreator>() as RelationalDatabaseCreator;
        try
        {
            await creator!.CreateTablesAsync();
        }
        catch (DbException ex) when (ex.SqlState == "42P07")
        {
            // Las tablas ya existen, no es necesario crearlas
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapAuthController();
app.MapClientsController();
app.MapArtistsController();
app.MapAppointmentsController();

app.Run();

public partial class Program { }
