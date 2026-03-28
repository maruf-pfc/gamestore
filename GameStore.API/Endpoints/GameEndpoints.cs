using System;
using GameStore.API.Data;
using GameStore.API.DTOs;
using GameStore.API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpoint = "GetName";

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var gameRoute = app.MapGroup("/games");

        gameRoute.MapGet("/", async (GameStoreContext context) =>
            await context.Games.Include(g => g.Genre)
                               .Select(game => game.ToDto())
                               .AsNoTracking()
                               .ToListAsync()
        );

        gameRoute.MapGet("/{id}", async Task<IResult> (int id, GameStoreContext context) =>
        {
            var game = await context.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game.ToDto());
        }).WithName(GetGameEndpoint);

        gameRoute.MapPost("/", async Task<IResult> (CreateGameDto newGame, GameStoreContext context) =>
        {
            var game = newGame.ToEntity();
            context.Games.Add(game);
            await context.SaveChangesAsync();

            var createdGame = await context.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == game.Id);

            return Results.CreatedAtRoute(GetGameEndpoint, new { id = game.Id }, createdGame!.ToDto());
        });

        gameRoute.MapPut("/{id}", async Task<IResult> (int id, UpdateGameDto updatedGame, GameStoreContext context) =>
        {
            var existingGame = await context.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;

            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        gameRoute.MapDelete("/{id}", async Task<IResult> (int id, GameStoreContext context) =>
        {
            await context.Games.Where(game => game.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        });
    }
}
