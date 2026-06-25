using LinkUpPro.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Configurations.Battleship
{
    public class BattleshipShipConfiguration : IEntityTypeConfiguration<BattleshipShip>
    {
        public void Configure(EntityTypeBuilder<BattleshipShip> builder)
        {
            builder.ToTable("BattleshipShips");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Size)
                   .IsRequired()
                   .HasConversion<int>(); // Guarda el valor numérico del enum

            builder.Property(s => s.Direction)
                   .HasConversion<string>()
                   .HasMaxLength(10)
                   .IsRequired();

            builder.Property(s => s.StartCoordinateY)
                   .IsRequired();

            builder.Property(s => s.StartCoordinateX)
                   .IsRequired();

            builder.Property(s => s.IsSunk)
                   .IsRequired()
                   .HasDefaultValue(false);

            // FK → Board
            builder.HasOne(s => s.Board)
                   .WithMany(b => b.Ships)
                   .HasForeignKey(s => s.BoardId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.BoardId);
            builder.HasIndex(s => new { s.BoardId, s.Size }); // Buscar barco por tamaño
        }
    }
}
