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
    public class BattleshipBoardConfiguration : IEntityTypeConfiguration<BattleshipBoard>
    {
        public void Configure(EntityTypeBuilder<BattleshipBoard> builder)
        {
            builder.ToTable("BattleshipBoards");

            builder.HasKey(b => b.Id);

            // FK → Game
            builder.HasOne(b => b.Game)
                   .WithMany(g => g.Boards)
                   .HasForeignKey(b => b.GameId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → Player
            builder.HasOne(b => b.Player)
                   .WithMany()
                   .HasForeignKey(b => b.PlayerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Ships)
                   .WithOne(s => s.Board)
                   .HasForeignKey(s => s.BoardId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Un solo tablero por jugador por partida
            builder.HasIndex(b => new { b.GameId, b.PlayerId })
                   .IsUnique();
        }
    }
}
