using GameStore.Api.Entities;

namespace GameStore.Api.Dtos;

public record GameDto(int Id, string Name, string Genre, decimal Price, DateOnly ReleaseDate)
{
}