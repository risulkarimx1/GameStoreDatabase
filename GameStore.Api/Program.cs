using GameStore.Api.EndPoints;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app = app.MapGamesEndPoints();

app.Run();