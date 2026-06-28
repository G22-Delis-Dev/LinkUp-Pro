using FluentValidation;
using LinkUpPro.Application.Interfaces.Identity;
using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Application.Interfaces.User;
using LinkUpPro.Application.Services.Identity;
using LinkUpPro.Application.Services.Post;
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

        return services;
    }
}
