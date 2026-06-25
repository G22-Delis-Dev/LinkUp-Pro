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
    public class BattleshipGameConfiguration : IEntityTypeConfiguration<BattleshipGame>
    {
        public void Configure(EntityTypeBuilder<BattleshipGame> builder)
        {
            builder.ToTable("BattleshipGames");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(g => g.Result)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(g => g.CreatedAt)
                   .IsRequired();

            builder.Property(g => g.LastModifiedAt);
            builder.Property(g => g.WinnerId);

            // CurrentTurnPlayerId — nullable (null durante setup)
            builder.Property(g => g.CurrentTurnPlayerId)
                   .IsRequired();

            // FK → Player 1
            builder.HasOne(g => g.Player1)
                   .WithMany()
                   .HasForeignKey(g => g.Player1Id)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → Player 2
            builder.HasOne(g => g.Player2)
                   .WithMany()
                   .HasForeignKey(g => g.Player2Id)
                   .OnDelete(DeleteBehavior.Restrict);

            // Nav props
            builder.HasMany(g => g.Boards)
                   .WithOne(b => b.Game)
                   .HasForeignKey(b => b.GameId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(g => g.Player1Id);
            builder.HasIndex(g => g.Player2Id);
            builder.HasIndex(g => new { g.Player1Id, g.Player2Id, g.Status });
        }
    }
}
