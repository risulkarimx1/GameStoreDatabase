using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GamesEndpoints
{
    private const string GetGameNameEndpoint = "GetGame";
    
    public static WebApplication  MapGamesEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
        
        // GET /games
        GetAllGames(group);
        
        // GET /games/1
        GetGameById(group);

        // POST /games
        AddNewGame(group);

        // PUT /games/1
        UpdateExistingGame(group);
        
        // DELETE /games/1
        DeleteAnExistingGame(group);

        return app;
    }

    private static void DeleteAnExistingGame(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
        {
            var game = dbContext.Games.Find(id);
            if (game is null)
            {
                return Results.NotFound();
            }

            dbContext.Games.Remove(game);
            dbContext.SaveChanges();
            return Results.NoContent();
        });
    }

    private static void UpdateExistingGame(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var game = dbContext.Games.Find(id);
            if (game is null)
            {
                return Results.NotFound();
            }

            game.Name = updatedGame.Name;
            game.GenreId = updatedGame.GenreId;
            game.Genre = dbContext.Genres.Find(updatedGame.GenreId);
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;
            dbContext.SaveChanges();
            
            return Results.NoContent();
        }).WithParameterValidation();
    }

    private static void AddNewGame(RouteGroupBuilder group)
    {
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            
            // creating a Game Entity from the CreateGameDto
            var game = new Game() 
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Genre = dbContext.Genres.Find(newGame.GenreId),
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };

            dbContext.Games.Add(game);
            dbContext.SaveChanges();
            
            var gameDto = new GameDto(game.Id, game.Name, game.Genre?.Name!, game.Price, game.ReleaseDate);
            return Results.CreatedAtRoute(GetGameNameEndpoint, new { id = game.Id }, gameDto);
        }).WithParameterValidation();
    }

    private static void GetGameById(RouteGroupBuilder group)
    {
        group.MapGet("/{id}",
            (int id, GameStoreContext dbContext) =>
            {
                var game = dbContext.Games.Find(id);
                return game is not null
                    ? Results.Ok(new GameDto(game.Id, game.Name, game.Genre!.Name!, game.Price, game.ReleaseDate))
                    : Results.NotFound();
            }).WithName(GetGameNameEndpoint);
    }

    private static void GetAllGames(RouteGroupBuilder group)
    {
        group.MapGet("/", (GameStoreContext dbContext) =>
        {
            var gameDtos = 
                dbContext.Games.Select(g => new GameDto(g.Id, g.Name, g.Genre!.Name!, g.Price, g.ReleaseDate));
            return gameDtos;
        }).WithName(GetGameNameEndpoint);
    }
}