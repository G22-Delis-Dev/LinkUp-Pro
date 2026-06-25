using LinkUpPro.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Configurations.User
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable("UserTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Token)
                   .IsRequired()
                   .HasMaxLength(512);

            builder.Property(t => t.Type)
                   .IsRequired()
                   .HasConversion<string>()  // Guarda enum como string
                   .HasMaxLength(50);

            builder.Property(t => t.ExpirationDate)
                   .IsRequired();

            builder.Property(t => t.IsUsed)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(t => t.CreatedAt)
                   .IsRequired();

            // FK → User
            builder.HasOne(t => t.User)
                   .WithMany()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Índice en Token para búsqueda rápida al validar
            builder.HasIndex(t => t.Token);

            // Índice compuesto para buscar tokens vigentes de un usuario
            builder.HasIndex(t => new { t.UserId, t.Type, t.IsUsed });
        }
    }
}
