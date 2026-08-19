using LinkUpPro.Domain.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Configurations.Post
{
    public class PostConfiguration : IEntityTypeConfiguration<Domain.Entities.Post.Post>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Post.Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Content)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(p => p.Privacy)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(p => p.ContentType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(p => p.AllowComments)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.LastModifiedAt);

            // FK → User (autor)
            builder.HasOne(p => p.User)
                   .WithMany(u => u.Posts)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Nav props
            builder.HasMany(p => p.Images)
                   .WithOne(i => i.Post)
                   .HasForeignKey(i => i.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Videos)
                   .WithOne(v => v.Post)
                   .HasForeignKey(v => v.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => new { p.UserId, p.CreatedAt });
        }
    }
}
