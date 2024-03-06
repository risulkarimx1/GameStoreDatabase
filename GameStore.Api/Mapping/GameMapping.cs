using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GameMapping
{
    public static Game ToEntity(this CreateGameDto createGameDto)
    {
        return new Game
        {
            Name = createGameDto.Name,
            GenreId = createGameDto.GenreId,
            Price = createGameDto.Price,
            ReleaseDate = createGameDto.ReleaseDate
        };
    }
    
    public static Game ToEntity(this UpdateGameDto updateGameDto, int id)
    {
        return new Game
        {
            Id = id,
            Name = updateGameDto.Name,
            GenreId = updateGameDto.GenreId,
            Price = updateGameDto.Price,
            ReleaseDate = updateGameDto.ReleaseDate
        };
    }
    
    public static GameDto ToDto(this Game game)
    {
        return new GameDto(game.Id, game.Name, game.Genre?.Name ?? string.Empty, game.Price, game.ReleaseDate);
    }
    
    public static GameDetailsDto ToDetailsDto(this Game game)
    {
        return new GameDetailsDto(game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate);
    }
    
    public static GameSummaryDto ToSummaryDto(this Game game)
    {
        return new GameSummaryDto(game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate);
    }
}