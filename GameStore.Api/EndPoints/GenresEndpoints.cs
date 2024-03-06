using GameStore.Api.Data;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GenresEndpoints
{
    public static WebApplication MapGeneresEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("/genres");
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            return await dbContext.Genres.Select(genre => genre.ToDto()).AsNoTracking().ToListAsync();
        });
        return app;
    }
}