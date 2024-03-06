using GameStore.Api.Dtos;

namespace GameStore.Api.EndPoints;

public static class GamesEndpoints
{
    private const string GetGameNameEndpoint = "GetGame";

    private static readonly List<GameDto> Games =
    [
        new GameDto(1, "Game1", "Action", 49.99m, new DateOnly(2022, 1, 1)),
        new GameDto(2, "Game2", "Adventure", 39.99m, new DateOnly(2022, 2, 15)),
        new GameDto(3, "Game3", "Puzzle", 19.99m, new DateOnly(2022, 3, 10)),
        new GameDto(4, "Game4", "Racing", 29.99m, new DateOnly(2022, 4, 20)),
        new GameDto(5, "Game5", "Simulation", 59.99m, new DateOnly(2022, 5, 5))
    ];

    public static WebApplication  MapGamesEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("games");
        
        // GET /games
        group.MapGet("/", () => Games);

        // GET/games/1
        group.MapGet("/{id}",
            (int id) =>
            {
                return Games.FirstOrDefault(g => g.Id == id)
                    is GameDto game
                    ? Results.Ok(game)
                    : Results.NotFound();
            }).WithName(GetGameNameEndpoint);

        // POST /games
        group.MapPost("/", (CreateGameDto gameDto) =>
        {
            var id = Games.Max(g => g.Id) + 1;
            var game = new GameDto(id, gameDto.Name, gameDto.Genre, gameDto.Price, gameDto.ReleaseDate);
            Games.Add(game);
            return Results.CreatedAtRoute(GetGameNameEndpoint, new { id = game.Id }, game);
        });

        // PUT /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = Games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            Games[index] = new GameDto(id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate);

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            var game = Games.FirstOrDefault(g => g.Id == id);

            if (game is null)
            {
                return Results.NotFound();
            }

            Games.Remove(game);
            return Results.NoContent();
        });

        return app;
    }
}