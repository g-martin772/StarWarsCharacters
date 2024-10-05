using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data;

public record SwCharacter
{
    public required string Name { get; set; }
    public required string Faction { get; set; }
    public required string  Homeworld { get; set; }
    public required string Species { get; set; }
}

public record SwCharacterEntity : SwCharacter
{
    public int Id { get; init; }
}

public class SwCharacterEntityConfiguration : IEntityTypeConfiguration<SwCharacterEntity>
{
    public void Configure(EntityTypeBuilder<SwCharacterEntity> builder)
    {
        builder.ToTable("SwCharacters");
        
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Faction).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Homeworld).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Species).IsRequired().HasMaxLength(50);
    }
}