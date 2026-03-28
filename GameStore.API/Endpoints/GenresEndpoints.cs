using GameStore.API.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Endpoints;

public static class GenresEndpoints
{
    public static void MapGenresEndpoints(this WebApplication app)
    {
        app.MapGet("/genres", async (GameStoreContext context) =>
        {
            return await context.Genres.Select(g => new { g.Id, g.Name }).AsNoTracking().ToListAsync();
        });
    }
}
