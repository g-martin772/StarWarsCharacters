using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class SwDbContext(DbContextOptions<SwDbContext> options) : DbContext(options)
{
    public DbSet<SwCharacterEntity> SwCharacters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => 
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SwDbContext).Assembly);
}