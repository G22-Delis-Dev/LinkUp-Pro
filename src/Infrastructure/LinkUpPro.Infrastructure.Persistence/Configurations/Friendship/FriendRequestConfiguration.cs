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
    public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {
            builder.ToTable("FriendRequests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            // FK → Sender
            builder.HasOne(r => r.Sender)
                   .WithMany()
                   .HasForeignKey(r => r.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → Receiver
            builder.HasOne(r => r.Receiver)
                   .WithMany()
                   .HasForeignKey(r => r.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices para consultas frecuentes
            builder.HasIndex(r => new { r.ReceiverId, r.Status });
            builder.HasIndex(r => new { r.SenderId, r.Status });
            builder.HasIndex(r => new { r.SenderId, r.ReceiverId, r.Status });
        }
    }
}
