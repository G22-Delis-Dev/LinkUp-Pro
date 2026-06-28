using LinkUpPro.Domain.Entities.Notification;
using LinkUpPro.Domain.Entities.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Configurations.Notification
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Domain.Entities.Notification.Notification>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Notification.Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Type)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(n => n.IsRead)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(n => n.RelatedEntityId)
                   .HasMaxLength(100);

            builder.Property(n => n.CreatedAt)
                   .IsRequired();

            // FK → User (recipient)
            builder.HasOne(n => n.User)
                   .WithMany()
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índice para consultas del destinatario
            builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });
        }
    }
}
