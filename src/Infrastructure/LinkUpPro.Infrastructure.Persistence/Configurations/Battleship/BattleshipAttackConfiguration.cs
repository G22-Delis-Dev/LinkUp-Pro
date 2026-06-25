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
    public class BattleshipAttackConfiguration : IEntityTypeConfiguration<BattleshipAttack>
    {
        public void Configure(EntityTypeBuilder<BattleshipAttack> builder)
        {
            builder.ToTable("BattleshipAttacks");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.CoordinateY)
                   .IsRequired();

            builder.Property(a => a.CoordinateX)
                   .IsRequired();

            builder.Property(a => a.IsHit)
                   .IsRequired();

            builder.Property(a => a.CreatedAt)
                   .IsRequired();

            // FK → Board
            builder.HasOne(a => a.Board)
                   .WithMany(b => b.ReceivedAttacks)
                   .HasForeignKey(a => a.BoardId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(a => a.BoardId);
        }
    }
}
