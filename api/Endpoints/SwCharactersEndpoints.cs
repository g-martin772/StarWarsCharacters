using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class SwCharactersEndpoints
{
    public static WebApplication MapSwCharactersEndpoints(this WebApplication app)
    {
        app.MapGet("/sw-characters", GetSwCharacters);
        app.MapGet("/sw-characters/{id:int}", GetSwCharacterById);
        app.MapPost("/sw-characters", AddSwCharacter);
        app.MapPut("/sw-characters/{id:int}", UpdateSwCharacter);
        app.MapDelete("/sw-characters", DeleteSwCharacters);
        app.MapDelete("/sw-characters/{id:int}", DeleteSwCharacters);
        return app;
    }
    
    private static IResult GetSwCharacters(
        [FromServices] SwDbContext dbContext,
        string? name, string? faction, string? homeWorld, string? species) =>
        Results.Json(dbContext.SwCharacters
            .Where(c => 
                (name == null || c.Name == name) &&
                (faction == null || c.Faction == faction) &&
                (homeWorld == null || c.Homeworld == homeWorld) &&
                (species == null || c.Species == species))
            .AsAsyncEnumerable());
    
    private static async Task<IResult> GetSwCharacterById(
        [FromServices] SwDbContext dbContext,
        int id)
    {
        var entry = await dbContext.SwCharacters.FindAsync(id);
        return entry is null ? Results.NotFound() : Results.Json(entry);
    }
    
    private static async Task<IResult> AddSwCharacter(
        [FromServices] SwDbContext dbContext,
        [FromBody] SwCharacter character)
    {
        if (await dbContext.SwCharacters.AnyAsync(c => c.Name == character.Name))
            return Results.Conflict($"Character with name {character.Name} already exists");
        
        if (string.IsNullOrWhiteSpace(character.Name))
            return Results.BadRequest("Name is required");
        
        if (string.IsNullOrWhiteSpace(character.Faction))
            return Results.BadRequest("Faction is required");
        
        if (string.IsNullOrWhiteSpace(character.Homeworld))
            return Results.BadRequest("Homeworld is required");
        
        if (string.IsNullOrWhiteSpace(character.Species))
            return Results.BadRequest("Species is required");
        
        var entity = new SwCharacterEntity
        {
            Name = character.Name,
            Faction = character.Faction,
            Homeworld = character.Homeworld,
            Species = character.Species
        };

        await dbContext.SwCharacters.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        
        return Results.Created($"/sw-characters/{entity.Id}", entity);
    }
    
    private static async Task<IResult> UpdateSwCharacter(
        [FromServices] SwDbContext dbContext,
        int id,
        [FromBody] SwCharacter character)
    {
        var entity = await dbContext.SwCharacters.FindAsync(id);
        if (entity is null)
            return Results.NotFound();
        
        if (await dbContext.SwCharacters.AnyAsync(c => c.Name == character.Name))
            return Results.Conflict($"Character with name {character.Name} already exists");
        
        if (!string.IsNullOrWhiteSpace(character.Name))
            entity.Name = character.Name;
        
        if (!string.IsNullOrWhiteSpace(character.Faction))
            entity.Faction = character.Faction;
        
        if (!string.IsNullOrWhiteSpace(character.Homeworld))
            entity.Homeworld = character.Homeworld;
        
        if (!string.IsNullOrWhiteSpace(character.Species))
            entity.Species = character.Species;
        
        await dbContext.SaveChangesAsync();
        
        return Results.Ok(entity);
    }
    
    private static async Task<IResult> DeleteSwCharacters(
        [FromServices] SwDbContext dbContext,
        int? id, string? name, string? faction, string? homeWorld, string? species) =>
        Results.Ok(await dbContext.SwCharacters
            .Where(c => 
                (id == null || c.Id == id) &&
                (name == null || c.Name == name) &&
                (faction == null || c.Faction == faction) &&
                (homeWorld == null || c.Homeworld == homeWorld) &&
                (species == null || c.Species == species))
            .ExecuteDeleteAsync());
}