using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

const string GetGameNameEndpoint = "GetGame";

var games = new List<GameDto>
{
    new GameDto(1, "Game1", "Action", 49.99m, new DateOnly(2022, 1, 1)),
    new GameDto(2, "Game2", "Adventure", 39.99m, new DateOnly(2022, 2, 15)),
    new GameDto(3, "Game3", "Puzzle", 19.99m, new DateOnly(2022, 3, 10)),
    new GameDto(4, "Game4", "Racing", 29.99m, new DateOnly(2022, 4, 20)),
    new GameDto(5, "Game5", "Simulation", 59.99m, new DateOnly(2022, 5, 5))
};

// GET /games
app.MapGet("/games", () => games);
// GET/games/1
app.MapGet("/games/{id}",
    (int id) =>
    {
        return games.FirstOrDefault(g => g.Id == id)
            is GameDto game
            ? Results.Ok(game)
            : Results.NotFound();
    }).WithName(GetGameNameEndpoint);

// POST /games
app.MapPost("/games", (CreateGameDto gameDto) =>
{
    var id = games.Max(g => g.Id) + 1;
    var game = new GameDto(id, gameDto.Name, gameDto.Genre, gameDto.Price, gameDto.ReleaseDate);
    games.Add(game);
    return Results.CreatedAtRoute(GetGameNameEndpoint, new { id = game.Id }, game);
});

// PUT /games/1
app.MapPut("/games/{id}", (int id, UpdateGameDto updatedGame) =>
{
    var index = games.FindIndex(game => game.Id == id);
    
    if (index == -1)
    {
        return Results.NotFound();
    }
    
    games[index] = new GameDto(id, 
        updatedGame.Name, 
        updatedGame.Genre, 
        updatedGame.Price, 
        updatedGame.ReleaseDate);

    return Results.NoContent();
});

// DELETE /games/1
app.MapDelete("/games/{id}", (int id) =>
{
    var game = games.FirstOrDefault(g => g.Id == id);
    
    if (game is null)
    {
        return Results.NotFound();
    }
    
    games.Remove(game);
    return Results.NoContent();
});

app.Run();