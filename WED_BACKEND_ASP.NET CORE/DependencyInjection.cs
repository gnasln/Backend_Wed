using FluentEmail.Core;
using Wed.Infrastructure.Data;
using WED_BACKEND_ASP.Services;
using Wed.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using WED_BACKEND_ASP.Helper.Services;
using Wed.Domain.Entities;
using WED_BACKEND_ASP.Helper;
using Microsoft.AspNetCore.Identity;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;
using IEmailSender = WED_BACKEND_ASP.Helper.Services.IEmailSender;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddScoped<IUser, CurrentUser>();
        services.AddHttpContextAccessor();
        
        services.AddControllers();
        services.AddMemoryCache(); // Thêm dòng này để sử dụng MemoryCache
        services.AddSingleton<OTPService>();
        
        // Register the BackgroundTaskQueue service
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        //vo hieu hoa
        services.AddScoped<SignInManager<ApplicationUser>, CustomSignInManager>();
        
        //email
        services.AddScoped<IEmailSender, EmailSender>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .Build())
            .AddPolicy("OpenIddict.Server.AspNetCore", policy =>
            {
                policy.AuthenticationSchemes.Add(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy("admin", policy =>
            {
                policy.AuthenticationSchemes = [OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme];
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Administrator");
            })
            .AddPolicy("owner", policy =>
            {
                policy.AuthenticationSchemes = [OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme];
                policy.RequireAuthenticatedUser();
                policy.RequireRole("FacilityOwner");
            });

        return services;
    }

    public static void AddFluentEmail(this IServiceCollection services, ConfigurationManager configuration)
    {
        var emailSettings = configuration.GetSection("EmailSettings");
        var defaultFromEmail = emailSettings["FromAddress"];
        var host = emailSettings["Host"];
        var port = emailSettings.GetValue<int>("Port");
        
        services.AddFluentEmail(defaultFromEmail)
            .AddRazorRenderer()
            .AddSmtpSender(host, port, defaultFromEmail, emailSettings["Password"]);
    }
}
