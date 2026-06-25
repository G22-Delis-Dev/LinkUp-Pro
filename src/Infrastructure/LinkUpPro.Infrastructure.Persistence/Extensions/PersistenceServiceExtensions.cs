using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using LinkUpPro.Domain.Interfaces.Repositories.Comment;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Notification;
using LinkUpPro.Domain.Interfaces.Repositories.Post;
using LinkUpPro.Domain.Interfaces.Repositories.Reaction;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Persistence.Context;
using LinkUpPro.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUpPro.Infrastructure.Persistence.Extensions;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── DbContext ─────────────────────────────────────────────────────
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.FullName)));

        // ── Repositorios — User ───────────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();

        // ── Repositorios — Post ───────────────────────────────────────────
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostImageRepository, PostImageRepository>();
        services.AddScoped<IPostVideoRepository, PostVideoRepository>();

        // ── Repositorios — Comment ────────────────────────────────────────
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ICommentReplyRepository, CommentReplyRepository>();

        // ── Repositorios — Reaction ───────────────────────────────────────
        services.AddScoped<IReactionRepository, ReactionRepository>();

        // ── Repositorios — Friendship ─────────────────────────────────────
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();

        // ── Repositorios — Notification ───────────────────────────────────
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // ── Repositorios — Battleship ─────────────────────────────────────
        services.AddScoped<IBattleshipGameRepository, BattleshipGameRepository>();
        services.AddScoped<IBattleshipBoardRepository, BattleshipBoardRepository>();
        services.AddScoped<IBattleshipShipRepository, BattleshipShipRepository>();
        services.AddScoped<IBattleshipAttackRepository, BattleshipAttackRepository>();

        return services;
    }
}