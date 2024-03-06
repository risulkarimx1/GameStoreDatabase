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
        
        GetAllGames(group);
        
        GetGameById(group);
        
        AddNewGame(group);
        
        UpdateExistingGame(group);
        
        DeleteAnExistingGame(group);

        return app;
    }

    private static void DeleteAnExistingGame(IEndpointRouteBuilder group)
    {
        // DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
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
        // PUT /games/1
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            
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
        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            var game = newGame.ToEntity();
            game.Genre = await dbContext.Genres.FindAsync(newGame.GenreId);

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            var gameDto = game.ToDto();// new GameDto(game.Id, game.Name, game.Genre?.Name!, game.Price, game.ReleaseDate);
            return Results.CreatedAtRoute(GetGameNameEndpoint, new { id = game.Id }, gameDto);
        }).WithParameterValidation();
    }

    private static void GetGameById(IEndpointRouteBuilder group)
    {
        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
            {
                var game = await dbContext.Games.FindAsync(id);
                return game is not null
                    ? Results.Ok(game.ToDetailsDto())
                    : Results.NotFound();
            });
    }

    private static void GetAllGames(IEndpointRouteBuilder group)
    {
        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            var gameDtos = 
                await dbContext.Games
                    .Include(g => g.Genre)
                    .Select(g => g.ToSummaryDto())
                    .AsNoTracking()
                    .AsNoTracking()
                    .ToListAsync();
            return gameDtos;
        }).WithName(GetGameNameEndpoint);
    }
}