using LinkUpPro.Domain.Entities.Reaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Configurations.Reaction
{
    public class ReactionConfiguration : IEntityTypeConfiguration<Domain.Entities.Reaction.Reaction>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Reaction.Reaction> builder)
        {
            builder.ToTable("Reactions");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Type)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            // FK → Post
            builder.HasOne(r => r.Post)
                   .WithMany(p => p.Reactions)
                   .HasForeignKey(r => r.PostId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → User
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Garantiza una sola reacción por usuario por post a nivel DB
            builder.HasIndex(r => new { r.PostId, r.UserId })
                   .IsUnique();
        }
    }
}
