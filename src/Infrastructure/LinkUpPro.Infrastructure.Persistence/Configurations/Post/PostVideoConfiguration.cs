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
    public class PostVideoConfiguration : IEntityTypeConfiguration<PostVideo>
    {
        public void Configure(EntityTypeBuilder<PostVideo> builder)
        {
            builder.ToTable("PostVideos");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.VideoPath)
                   .IsRequired()
                   .HasMaxLength(500);

            // FK → Post
            builder.HasOne(v => v.Post)
                   .WithMany(p => p.Videos)
                   .HasForeignKey(v => v.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => v.PostId);
        }
    }
}
