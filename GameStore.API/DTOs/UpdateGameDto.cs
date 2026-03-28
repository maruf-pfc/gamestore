namespace GameStore.API.DTOs;

public record class UpdateGameDto
(
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);
