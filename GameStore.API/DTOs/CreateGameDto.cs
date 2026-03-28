namespace GameStore.API.DTOs;

public record class CreateGameDto(
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);