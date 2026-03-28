using GameStore.API.Data;
using GameStore.API.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

// Database Configuration and services would go here
var connStr = "Server=localhost;Database=GameStoreDb;User Id=sa;Password=your_password;";
builder.Services.AddDbContext<GameStoreContext>(options =>
    options.UseSqlServer(connStr));

var app = builder.Build();

app.MapGamesEndpoints();

app.Run();