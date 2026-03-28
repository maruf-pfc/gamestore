using System.ComponentModel.DataAnnotations;

namespace GameStore.API.DTOs;

public record class UpdateGameDto(
    [Required][StringLength(100, MinimumLength = 1)]
    string Name,
    [StringLength(50)]
    string Genre,
    [Required][Range(1, double.MaxValue)]
    decimal Price,
    [Required]
    DateOnly ReleaseDate
);
