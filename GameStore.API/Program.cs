using GameStore.API.DTOs; // Importing the namespace for DTOs (Data Transfer Objects)

var builder = WebApplication.CreateBuilder(args); // Create a new WebApplication builder
var app = builder.Build(); // Build the application

// Constant for the endpoint name used in CreatedAtRoute
const string GetGameEndpoint = "GetName";

// In-memory list of games (mock database)
List<GameDto> games = [
    new(1, "Elden Ring", "Action RPG", 59.99m, new DateOnly(2022, 2, 25)),
    new(2, "The Legend of Zelda: Breath of the Wild", "Action Adventure", 59.99m, new DateOnly(2017, 3, 3)),
    new(3, "Cyberpunk 2077", "Action RPG", 49.99m, new DateOnly(2020, 12, 10)),
    new(4, "Starfield", "Action RPG", 69.99m, new DateOnly(2023, 9, 6)),
    new(5, "Baldur's Gate 3", "RPG", 59.99m, new DateOnly(2023, 8, 3))
];

// Define a simple GET endpoint for the root URL
app.MapGet("/", () => "Hello World!");

// GET endpoint to retrieve all games
app.MapGet("/games", () => games);

// GET endpoint to retrieve a specific game by its ID
app.MapGet("/games/{id}", (int id) =>
{
    var game = games.Find(game => game.Id == id);
    return game is not null ? Results.Ok(game) : Results.NotFound();
}).WithName(GetGameEndpoint); 
   
// Assign a name to the route for use in CreatedAtRoute

// POST endpoint to add a new game
app.MapPost("/games", (CreateGameDto newGame) => {
    // Create a new GameDto object from the incoming data
    GameDto game = new(
        games.Count + 1, // Generate a new ID based on the current count
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
    );

    games.Add(game); // Add the new game to the list

    // Return a 201 Created response with the location of the new resource
    return Results.CreatedAtRoute(GetGameEndpoint, new { id = game.Id }, game);
});

// PUT endpoint to update an existing game by its ID
app.MapPut("/games/{id}", (int id, UpdateGameDto updatedGame) =>
{
    // Find the index of the game to update
    var i = games.FindIndex(game => game.Id == id);

    if(i == -1)
    {
        return Results.NotFound(); // Return 404 Not Found if the game doesn't exist
    }

    // Replace the existing game with the updated data
    games[i] = new GameDto(
        id, // Keep the same ID
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleaseDate
    );

    return Results.NoContent(); // Return a 204 No Content response
});

// DELETE endpoint to remove a game by its ID
app.MapDelete("/games/{id}", (int id) =>
{
    // Find the index of the game to delete
    var i = games.FindIndex(game => game.Id == id);

    if (i == -1)
    {
        return Results.NotFound(); // Return 404 Not Found if the game doesn't exist
    }

    games.RemoveAt(i); // Remove the game from the list

    return Results.NoContent(); // Return a 204 No Content response
});

// Run the application
app.Run();