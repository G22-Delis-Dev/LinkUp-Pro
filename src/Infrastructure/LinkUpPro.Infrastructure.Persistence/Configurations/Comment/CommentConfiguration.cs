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
    public class CommentConfiguration : IEntityTypeConfiguration<Domain.Entities.Comment.Comment>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Comment.Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.CreatedAt)
                   .IsRequired();

            // FK → Post
            builder.HasOne(c => c.Post)
                   .WithMany(p => p.Comments)
                   .HasForeignKey(c => c.PostId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → User (autor)
            builder.HasOne(c => c.User)
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Nav: replies
            builder.HasMany(c => c.Replies)
                   .WithOne(r => r.Comment)
                   .HasForeignKey(r => r.CommentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.PostId);
            builder.HasIndex(c => new { c.PostId, c.CreatedAt });
        }
    }
}
