using LinkUpPro.Domain.Entities.Friendship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Configurations.Friendship
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Domain.Entities.Friendship.Friendship>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Friendship.Friendship> builder)
        {
            builder.ToTable("Friendships");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(f => f.CreatedAt)
                   .IsRequired();

            // FK → User A
            builder.HasOne(f => f.User)
                   .WithMany()
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → User B
            builder.HasOne(f => f.Friend)
                   .WithMany()
                   .HasForeignKey(f => f.FriendId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Una sola relación por pareja (sin importar orden A-B o B-A)
            // La app garantiza que siempre UserId < FriendId al crear
            builder.HasIndex(f => new { f.UserId, f.FriendId })
                   .IsUnique();
        }
    }
}
