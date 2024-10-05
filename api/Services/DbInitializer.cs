using System.Diagnostics;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class DbInitializer(
    IWebHostEnvironment env, 
    IServiceProvider serviceProvider, 
    ILogger<DbInitializer> logger
    ) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource m_ActivitySource = new(ActivitySourceName);
    private SwDbContext m_DbContext = null!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        m_DbContext = scope.ServiceProvider.GetRequiredService<SwDbContext>();
        await InitializeDatabaseAsync(cancellationToken);
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        using var activity = m_ActivitySource.StartActivity(ActivityKind.Client);

        var sw = Stopwatch.StartNew();

        var strategy = m_DbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(m_DbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        if (await m_DbContext.SwCharacters.CountAsync(cancellationToken) == 0)
        {
             m_DbContext.SwCharacters.AddRange(
                new SwCharacterEntity { Name = "Luke Skywalker", Faction = "Rebel Alliance", Homeworld = "Tatooine", Species = "Human" },
                new SwCharacterEntity { Name = "Darth Vader", Faction = "Galactic Empire", Homeworld = "Tatooine", Species = "Human" },
                new SwCharacterEntity { Name = "Leia Organa", Faction = "Rebel Alliance", Homeworld = "Alderaan", Species = "Human" },
                new SwCharacterEntity { Name = "Obi-Wan Kenobi", Faction = "Jedi Order", Homeworld = "Stewjon", Species = "Human" },
                new SwCharacterEntity { Name = "Yoda", Faction = "Jedi Order", Homeworld = "Dagobah", Species = "Yoda's species" },
                new SwCharacterEntity { Name = "R2-D2", Faction = "Rebel Alliance", Homeworld = "Naboo", Species = "Astromech droid" },
                new SwCharacterEntity { Name = "C-3PO", Faction = "Rebel Alliance", Homeworld = "Tatooine", Species = "Protocol droid" },
                new SwCharacterEntity { Name = "Chewbacca", Faction = "Rebel Alliance", Homeworld = "Kashyyyk", Species = "Wookie" },
                new SwCharacterEntity { Name = "Han Solo", Faction = "Rebel Alliance", Homeworld = "Corellia", Species = "Human" },
                new SwCharacterEntity { Name = "Boba Fett", Faction = "Galactic Empire", Homeworld = "Kamino", Species = "Human" },
                new SwCharacterEntity { Name = "Darth Maul", Faction = "Sith", Homeworld = "Dathomir", Species = "Zabrak" },
                new SwCharacterEntity { Name = "Emperor Palpatine", Faction = "Galactic Empire", Homeworld = "Naboo", Species = "Human" },
                new SwCharacterEntity { Name = "Jabba the Hutt", Faction = "Hutt Clan", Homeworld = "Nal Hutta", Species = "Hutt" },
                new SwCharacterEntity { Name = "Lando Calrissian", Faction = "Rebel Alliance", Homeworld = "Socorro", Species = "Human" },
                new SwCharacterEntity { Name = "Padmé Amidala", Faction = "Galactic Republic", Homeworld = "Naboo", Species = "Human" },
                new SwCharacterEntity { Name = "Qui-Gon Jinn", Faction = "Jedi Order", Homeworld = "Coruscant", Species = "Human" },
                new SwCharacterEntity { Name = "Mace Windu", Faction = "Jedi Order", Homeworld = "Haruun Kal", Species = "Human" },
                new SwCharacterEntity { Name = "Count Dooku", Faction = "Sith", Homeworld = "Serenno", Species = "Human" },
                new SwCharacterEntity { Name = "General Grievous", Faction = "Separatists", Homeworld = "Kalee", Species = "Kaleesh" },
                new SwCharacterEntity { Name = "Ahsoka Tano", Faction = "Jedi Order", Homeworld = "Shili", Species = "Togruta" }
            );
        }
       
        if (env.IsDevelopment())
        {
            
        }

        await m_DbContext.SaveChangesAsync(cancellationToken);
    }
}