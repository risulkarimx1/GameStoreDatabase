using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Entities;

public class Game
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int GenreId { get; set; }

    [Required]
    public Genre? Genre { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public DateOnly ReleaseDate { get; set; }
}