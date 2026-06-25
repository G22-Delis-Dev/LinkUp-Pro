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
    public class PostImageConfiguration : IEntityTypeConfiguration<PostImage>
    {
        public void Configure(EntityTypeBuilder<PostImage> builder)
        {
            builder.ToTable("PostImages");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ImagePath)
                   .IsRequired()
                   .HasMaxLength(500);

            // FK → Post
            builder.HasOne(i => i.Post)
                   .WithMany(p => p.Images)
                   .HasForeignKey(i => i.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.PostId);
        }
    }
}
