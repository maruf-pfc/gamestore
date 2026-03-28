using GameStore.API.Data;
using GameStore.API.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Services.AddValidation();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connStr = Environment.GetEnvironmentVariable("DATABASE_URL") ?? builder.Configuration.GetConnectionString("GameStoreContext");

builder.Services.AddDbContext<GameStoreContext>(options =>
    options.UseSqlServer(connStr));

var app = builder.Build();

app.UseCors();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
    context.Database.Migrate();
}

app.Run();