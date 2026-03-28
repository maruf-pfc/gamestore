using System;
using GameStore.API.DTOs;

namespace GameStore.API.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpoint = "GetName";

    private static readonly List<GameDto> games = [
        new(1, "Elden Ring", "Action RPG", 59.99m, new DateOnly(2022, 2, 25)),
        new(2, "The Legend of Zelda: Breath of the Wild", "Action Adventure", 59.99m, new DateOnly(2017, 3, 3)),
        new(3, "Cyberpunk 2077", "Action RPG", 49.99m, new DateOnly(2020, 12, 10)),
        new(4, "Starfield", "Action RPG", 69.99m, new DateOnly(2023, 9, 6)),
        new(5, "Baldur's Gate 3", "RPG", 59.99m, new DateOnly(2023, 8, 3))
    ];

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var gameRoute = app.MapGroup("/games");

        app.MapGet("/", () => "Hello World!");

        gameRoute.MapGet("/", () => games);
        gameRoute.MapGet("/{id}", (int id) => games.Find(game => game.Id == id))
        .WithName(GetGameEndpoint);

        gameRoute.MapPost("/", (CreateGameDto newGame) => {
            if (string.IsNullOrEmpty(newGame.Name))
            {
                return Results.BadRequest("Name is required.");
            }

            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );
            games.Add(game);
            return Results.CreatedAtRoute(GetGameEndpoint, new { id = game.Id }, game);
        });

        gameRoute.MapPut("/{id}", (int id, UpdateGameDto updatedGame) => {
            var i = games.FindIndex(game => game.Id == id);
            games[i] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );
            return Results.NoContent();
        });

        gameRoute.MapDelete("/{id}", (int id) => {
            var i = games.FindIndex(game => game.Id == id);
            if (i == -1) {
                return Results.NotFound();
            }
            games.RemoveAt(i);
            return Results.NoContent();
        });
    }
}
