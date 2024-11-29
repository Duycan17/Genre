using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Entities;

namespace WebApplication1.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpoints = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        // Get all games from the database
        group.MapGet("/", async (GameStoreContext dbContext, IMapper mapper) =>
        {
            var games = await dbContext.Games
                .Include(game => game.Genre) // If Genre is a related entity, make sure to include it
                .ToListAsync();
            return Results.Ok(games.Select(game => mapper.Map<GameDto>(game)));
        });

        // Get a single game by ID
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext, IMapper mapper) =>
        {
            var game = await dbContext.Games
                .Include(game => game.Genre)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game is null)
                return Results.NotFound();

            return Results.Ok(mapper.Map<GameDto>(game));
        })
        .WithName(GetGameEndpoints);

        // Create a new game
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext, IMapper mapper) =>
        {
            // Map CreateGameDto to Game (Entity)
            var game = mapper.Map<Game>(newGame);

            // Manually assign Genre based on GenreId
            game.Genre = await dbContext.Genres.FindAsync(newGame.GenreId);

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            // Map Game (Entity) to GameDto
            var gameDto = mapper.Map<GameDto>(game);

            return Results.CreatedAtRoute(GetGameEndpoints, new { id = game.Id }, gameDto);
        });

        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext, IMapper mapper) =>
        {
            var game = await dbContext.Games
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game is null)
            {
                return Results.NotFound();
            }
            game.Name = updatedGame.Name;
            game.Genre = await dbContext.Genres.FindAsync(updatedGame.GenreId); // Assuming you update by GenreId
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;
            await dbContext.SaveChangesAsync();
            var gameDto = mapper.Map<GameDto>(game);
            return Results.Ok(gameDto);
        });
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            if (game is null)
            {
                return Results.NotFound();
            }

            dbContext.Games.Remove(game);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        return group;
    }
}
