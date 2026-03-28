using GameStore.API.Data;
using GameStore.API.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Services.AddValidation();

var connStr = Environment.GetEnvironmentVariable("DATABASE_URL") ?? builder.Configuration.GetConnectionString("GameStoreContext");

builder.Services.AddDbContext<GameStoreContext>(options =>
    options.UseSqlServer(connStr));

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
    context.Database.Migrate();
}

app.Run();