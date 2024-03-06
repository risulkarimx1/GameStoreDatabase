using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Mapping;
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

    private static void DeleteAnExistingGame(IEndpointRouteBuilder group)
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

    private static void UpdateExistingGame(IEndpointRouteBuilder group)
    {
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = dbContext.Games.Find(id);
            
            if (existingGame is null)
            {
                return Results.NotFound();
            }
            
            dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
            
            dbContext.SaveChanges();
            
            return Results.NoContent();
        }).WithParameterValidation();
    }

    private static void AddNewGame(IEndpointRouteBuilder group)
    {
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            var game = newGame.ToEntity();
            game.Genre = dbContext.Genres.Find(newGame.GenreId);

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            var gameDto = game.ToDto();// new GameDto(game.Id, game.Name, game.Genre?.Name!, game.Price, game.ReleaseDate);
            return Results.CreatedAtRoute(GetGameNameEndpoint, new { id = game.Id }, gameDto);
        }).WithParameterValidation();
    }

    private static void GetGameById(IEndpointRouteBuilder group)
    {
        group.MapGet("/{id}",
            (int id, GameStoreContext dbContext) =>
            {
                var game = dbContext.Games.Find(id);
                return game is not null
                    ? Results.Ok(game.ToDetailsDto())
                    : Results.NotFound();
            });
    }

    private static void GetAllGames(IEndpointRouteBuilder group)
    {
        group.MapGet("/", (GameStoreContext dbContext) =>
        {
            var gameDtos = dbContext.Games.Include(g => g.Genre).Select(g => g.ToSummaryDto()).AsNoTracking();
            return gameDtos;
        }).WithName(GetGameNameEndpoint);
    }
}