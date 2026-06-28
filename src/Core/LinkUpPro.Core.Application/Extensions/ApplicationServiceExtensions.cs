using FluentValidation;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.Interfaces.Comment;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.Interfaces.Identity;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Application.Interfaces.Reaction;
using LinkUpPro.Application.Interfaces.User;
using LinkUpPro.Application.Services.Battleship;
using LinkUpPro.Application.Services.Comment;
using LinkUpPro.Application.Services.Friendship;
using LinkUpPro.Application.Services.Identity;
using LinkUpPro.Application.Services.Notification;
using LinkUpPro.Application.Services.Post;
using LinkUpPro.Application.Services.Reaction;
using LinkUpPro.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LinkUpPro.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // ── AutoMapper & FluentValidation ─────────────────────────────
        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        // ── Identity Services ─────────────────────────────────────────
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<IAccountActivationService, AccountActivationService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();
        services.AddScoped<ISessionService, SessionService>();

        // ── User Services ─────────────────────────────────────────────
        services.AddScoped<IUserService, UserService>();

        // ── Post Services ─────────────────────────────────────────────
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostQueryService, PostQueryService>();
        services.AddScoped<IPostPrivacyService, PostPrivacyService>();

        // ── Comment & Reaction Services ───────────────────────────────
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ICommentReplyService, CommentReplyService>();
        services.AddScoped<IReactionService, ReactionService>();

        // ── Notification Services ─────────────────────────────────────
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationDispatchService, NotificationDispatchService>();

        // ── Friendship Services ───────────────────────────────────────
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IMutualFriendService, MutualFriendService>();
        services.AddScoped<IFriendRequestService, FriendRequestService>();
        services.AddScoped<IFriendRequestQueryService, FriendRequestQueryService>();

        // ── Battleship Services ───────────────────────────────────────
        services.AddScoped<IBattleshipGameService, BattleshipGameService>();
        services.AddScoped<IBattleshipSetupService, BattleshipSetupService>();
        services.AddScoped<IBattleshipAttackService, BattleshipAttackService>();
        services.AddScoped<IBattleshipHistoryService, BattleshipHistoryService>();
        // IBattleshipHubService se registrará en la capa de Presentación con SignalR

        return services;
    }
}
