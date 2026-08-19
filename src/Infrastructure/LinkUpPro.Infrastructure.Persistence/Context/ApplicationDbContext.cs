using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Entities.Comment;
using LinkUpPro.Domain.Entities.Friendship;
using LinkUpPro.Domain.Entities.Notification;
using LinkUpPro.Domain.Entities.Post;
using LinkUpPro.Domain.Entities.Reaction;
using LinkUpPro.Domain.Entities.User;
using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Infrastructure.Persistence.Context;

// Contexto principal de la aplicación. Hereda de IdentityDbContext compartiendo conexión,
// pero las tablas de Identity viven en su esquema propio ("Identity").
public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // ── Domain DbSets ───────────────────────────────────────────────
    public DbSet<User> UsersDomain => Set<User>();
    new public DbSet<UserToken> UserTokens => Set<UserToken>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostImage> PostImages => Set<PostImage>();
    public DbSet<PostVideo> PostVideos => Set<PostVideo>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<CommentReply> CommentReplies => Set<CommentReply>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<BattleshipGame> BattleshipGames => Set<BattleshipGame>();
    public DbSet<BattleshipBoard> BattleshipBoards => Set<BattleshipBoard>();
    public DbSet<BattleshipShip> BattleshipShips => Set<BattleshipShip>();
    public DbSet<BattleshipAttack> BattleshipAttacks => Set<BattleshipAttack>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Esquema "Identity" para las tablas de Identity (siguiendo guía del profe)
        modelBuilder.Entity<AppUser>().ToTable("Users", "Identity");
        modelBuilder.Entity<AppRole>().ToTable("Roles", "Identity");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "Identity");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "Identity");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "Identity");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "Identity");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "Identity");

        // Aplicar todas las EntityTypeConfigurations del assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    // Override de SaveChangesAsync para auditoría automática de CreatedAt/LastModifiedAt
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}