using HanyCo.Infra.Security.Client.Providers;
using HanyCo.Infra.Security.DataSources;
using HanyCo.Infra.Security.Helpers;
using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Identity.Model;
using HanyCo.Infra.Security.Model;
using HanyCo.Infra.Security.Services;
using HanyCo.Infra.Services;

using Library.Logging;
using Library.Security.Authorization;
using Library.Security.Claims;
using Library.Validations;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HanyCo.Infra.Security;

public static class MesSecurityConfiguration
{
    //public static IServiceCollection AddMesInfraSecurityServices<TStartup>(this IServiceCollection services, ISecurityConfigOptions options)
    //{
    //    _ = options.Check(CheckBehavior.ThrowOnFail)
    //        .ArgumentNotNull()
    //        .NotNull(x => x.ConnectionString);

    // addLoggers(services, options.Logger); addIdentity(services); addAuthorization(services);
    // addAuthentication(services); addDbContextPool(services, options.ConnectionString);
    // addIdentityServices(services); addUserContext(services); addTools(services); _ = services.AddSingleton(options);

    // //MvcHelper.Initialize();

    // return services;

    // static void addIdentity(IServiceCollection services) => services
    // .AddIdentity<InfraIdentityUser, InfraIdentityRole>(options => { //options.Password = new() {
    // RequiredLength = 8 }; options.User.RequireUniqueEmail = true; })
    // .AddEntityFrameworkStores<InfraSecDbContext>() .AddDefaultTokenProviders() .AddErrorDescriber<PersianIdentityErrorDescriber>();

    // static void addAuthorization(IServiceCollection services) =>
    // services.AddAuthorization(options => { options.FallbackPolicy = new
    // AuthorizationPolicyBuilder().AddAuthenticationSchemes("Bearer") .RequireAuthenticatedUser()
    // .RequireClaim("scope", "read").Build(); _ =
    // options.AddPolicies(LibCrudPolicies.FullAccessPolicy,
    // LibCrudPolicies.AdminOrFullAccessPolicy) .AddCrudRequirementPolicies();

    // options.AddPolicy(InfraIdentityValues.PolicyCanViewSystemEntities, policy
    // => policy.RequireRole(InfraIdentityValues.RoleAdminValue, InfraIdentityValues.RoleSupervisor));
    // options.AddPolicy(InfraIdentityValues.PolicyCanCrudSystemEntities, policy
    // => policy.RequireRole(InfraIdentityValues.RoleAdminValue));
    // options.AddPolicy(InfraIdentityValues.PolicyIsAdmin, policy
    // => policy.RequireRole(InfraIdentityValues.RoleAdminValue)); });

    // static void addAuthentication(IServiceCollection services) { var defaultOptions = new
    // JwtOptions { Issuer = "MES", Audience = "MES Infra", SecretKey = "MES Infra JWT Secret Key",
    // ExpirationDate = DateTime.Now.AddDays(1), };

    // _ = services.Configure<JwtOptions>(options => { options.Issuer = defaultOptions.Issuer;
    // options.Audience = defaultOptions.Audience; options.SecretKey = defaultOptions.SecretKey;
    // options.ExpirationDate = defaultOptions.ExpirationDate; }); _ =
    // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    // options.Authority = "http://localhost:5000/openid"; options.RequireHttpsMetadata = false;
    // options.TokenValidationParameters = new() { ValidateIssuer = true, ValidateAudience = true,
    // ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer =
    // defaultOptions.Issuer, ValidAudience = defaultOptions.Audience, IssuerSigningKey =
    // JwtHelpers.GetIssuerSigningKey(defaultOptions.SecretKey), }; }); }

    // static void addDbContextPool(IServiceCollection services, string connectionString) =>
    // services.AddDbContextPool<InfraSecDbContext>(o => { _ =
    // o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); _ =
    // o.UseSqlServer(connectionString); _ = o.EnableSensitiveDataLogging(); });

    // static void addIdentityServices(IServiceCollection services) { _ = services
    // .AddScoped<System.Security.Claims.ClaimsIdentity>() .AddScoped<InfraUserManager,
    // InfraUserManager>() .AddScoped<InfraSignInManager, InfraSignInManager>()
    // .AddScoped<SignInManager<InfraIdentityUser>>(x => x.GetRequiredService<InfraSignInManager>());

    // _ = services .AddScoped<IAuthorizationHandler, DynamicRoleHandler>()
    // .AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>(); }

    // static void addUserContext(IServiceCollection services) => services.AddScoped<UserContext,
    // UserContext>() .AddScoped<IUserContext>(x => x.GetRequiredService<UserContext>());

    // static void addLoggers(IServiceCollection services, ILogger logger) =>
    // services.AddSingleton<Microsoft.Extensions.Logging.ILogger<UserManager<InfraIdentityUser>>>(new
    // WebLogger<UserManager<InfraIdentityUser>>(logger)).AddSingleton<Microsoft.Extensions.Logging.ILogger<RoleManager<InfraIdentityRole>>>(new
    // WebLogger<RoleManager<InfraIdentityRole>>(logger)).AddSingleton<Microsoft.Extensions.Logging.ILogger<SignInManager<InfraIdentityUser>>>(new WebLogger<SignInManager<InfraIdentityUser>>(logger));

    //    static void addTools(IServiceCollection services) =>
    //        services.AddScoped<CustomAuthenticationStateProvider>()
    //                .AddScoped<AuthenticationStateProvider>(x => x.GetRequiredService<CustomAuthenticationStateProvider>())
    //                .AddScoped<ISecurityService, SecurityService>()
    //                ;
    //}
    public static IServiceCollection AddMesInfraSecurityServices<TStartup>(this IServiceCollection services, ISecurityConfigOptions options)
    {
        _ = options.Check(CheckBehavior.ThrowOnFail)
            .ArgumentNotNull()
            .NotNull(x => x.ConnectionString);

        addLoggers(services, options!.Logger);
        addIdentity(services);
        addAuthorization(services);
        addAuthentication(services);
        addDbContextPool(services, options.ConnectionString);
        addIdentityServices(services);
        addUserContext(services);
        addTools(services);
        _ = services.AddSingleton(options);

        return services;

        static void addIdentity(IServiceCollection services) =>
            services
            .AddIdentity<InfraIdentityUser, InfraIdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<InfraSecDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<PersianIdentityErrorDescriber>();

        static void addAuthorization(IServiceCollection services) =>
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()//.AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .RequireClaim("scope", "read").Build();
                _ = options.AddPolicies(LibCrudPolicies.FullAccessPolicy, LibCrudPolicies.AdminOrFullAccessPolicy)
                    .AddCrudRequirementPolicies();

                options.AddPolicy(InfraIdentityValues.PolicyCanViewSystemEntities, policy
                    => policy.RequireRole(InfraIdentityValues.RoleAdminValue, InfraIdentityValues.RoleSupervisor));
                options.AddPolicy(InfraIdentityValues.PolicyCanCrudSystemEntities, policy
                    => policy.RequireRole(InfraIdentityValues.RoleAdminValue));
                options.AddPolicy(InfraIdentityValues.PolicyIsAdmin, policy
                    => policy.RequireRole(InfraIdentityValues.RoleAdminValue));
            });

        static void addAuthentication(IServiceCollection services)
        {
            var defaultOptions = new JwtOptions
            {
                Issuer = "MES",
                Audience = "MES Infra",
                SecretKey = "MES Infra JWT Secret Key",
                ExpirationDate = DateTime.Now.AddDays(1),
            };

            _ = services.Configure<JwtOptions>(options =>
            {
                options.Issuer = defaultOptions.Issuer;
                options.Audience = defaultOptions.Audience;
                options.SecretKey = defaultOptions.SecretKey;
                options.ExpirationDate = defaultOptions.ExpirationDate;
            });
            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:5000/openid";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = defaultOptions.Issuer,
                    ValidAudience = defaultOptions.Audience,
                    IssuerSigningKey = JwtHelpers.GetIssuerSigningKey(defaultOptions.SecretKey),
                };
            });
        }

        static void addDbContextPool(IServiceCollection services, string connectionString) =>
            services.AddDbContextPool<InfraSecDbContext>(o =>
            {
                _ = o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                _ = o.UseSqlServer(connectionString);
                _ = o.EnableSensitiveDataLogging();
            });

        static void addIdentityServices(IServiceCollection services)
        {
            _ = services
                .AddSingleton<InfraUserManager>()
                .AddSingleton<InfraSignInManager>()
                .AddSingleton<SignInManager<InfraIdentityUser>>(x => x.GetRequiredService<InfraSignInManager>());

            _ = services
                .AddScoped<IAuthorizationHandler, DynamicRoleHandler>()
                .AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>();
        }

        static void addUserContext(IServiceCollection services) =>
            services.AddScoped<UserContext>()
                .AddScoped<IUserContext>(x => x.GetRequiredService<UserContext>());

        static void addLoggers(IServiceCollection services, ILogger logger) =>
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger<UserManager<InfraIdentityUser>>>(new WebLogger<UserManager<InfraIdentityUser>>(logger))
                .AddSingleton<Microsoft.Extensions.Logging.ILogger<RoleManager<InfraIdentityRole>>>(new WebLogger<RoleManager<InfraIdentityRole>>(logger))
                .AddSingleton<Microsoft.Extensions.Logging.ILogger<SignInManager<InfraIdentityUser>>>(new WebLogger<SignInManager<InfraIdentityUser>>(logger));

        static void addTools(IServiceCollection services) =>
            services.AddScoped<CustomAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(x => x.GetRequiredService<CustomAuthenticationStateProvider>())
                .AddScoped<ISecurityService, SecurityService>()
                    ;
    }

    public static IApplicationBuilder UseMesSecurityInfraMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<SecurityMiddleware>();
}