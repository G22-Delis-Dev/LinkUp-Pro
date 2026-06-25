using LinkUpPro.Domain.Entities.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Configurations.Comment
{
    public class CommentReplyConfiguration : IEntityTypeConfiguration<CommentReply>
    {
        public void Configure(EntityTypeBuilder<CommentReply> builder)
        {
            builder.ToTable("CommentReplies");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Content)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            // FK → Comment (comentario raíz)
            builder.HasOne(r => r.Comment)
                   .WithMany(c => c.Replies)
                   .HasForeignKey(r => r.CommentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → User
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.CommentId);
        }
    }
}
