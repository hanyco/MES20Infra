#nullable disable

using HanyCo.Infra.Security.Client.Providers;
using HanyCo.Infra.Security.DataSources;
using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Identity.Model;
using HanyCo.Infra.Security.Model;

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
    public static IServiceCollection AddMesInfraSecurityServices<TStartup>(this IServiceCollection services, ISecurityConfigOptions options)
    {
        _ = options.Check(CheckBehavior.ThrowOnFail)
            .ArgumentNotNull()
            .NotNull(x => x.ConnectionString);

        addLoggers(services, options.Logger);
        addIdentity(services);
        addAuthorization(services);
        addAuthentication(services);
        addDbContextPool(services, options.ConnectionString);
        addServices(services);
        addUserContext(services);
        addTools(services);

        //MvcHelper.Initialize();

        return services;

        static void addIdentity(IServiceCollection services) =>
            services
            .AddIdentity<InfraIdentityUser, InfraIdentityRole>(options =>
            {
                //options.Password = new() { RequiredLength = 8 };
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<InfraSecDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<PersianIdentityErrorDescriber>();

        static void addAuthorization(IServiceCollection services) =>
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .RequireClaim("scope", "read").Build();
                _ = options.AddPolicies(LibCrudPolicies.FullAccessPolicy, LibCrudPolicies.AdminOrFullAccessPolicy)
                    .AddCrudRequirementPolicies();

                options.AddPolicy("CanViewList", policy => policy.RequireClaim("Administrators"));
                options.AddPolicy("CanViewDetail", policy => policy.RequireClaim("Administrators"));
                options.AddPolicy("CanCreate", policy => policy.RequireClaim("Administrators"));
                options.AddPolicy("CanUpdate", policy => policy.RequireClaim("Administrators"));
                options.AddPolicy("CanDelete", policy => policy.RequireClaim("Administrators"));
            });

        static void addAuthentication(IServiceCollection services) =>
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.Authority = "http://localhost:5000/openid";
                o.Audience = "embedded";
                o.RequireHttpsMetadata = false;
            });

        static void addDbContextPool(IServiceCollection services, string connectionString) =>
            services.AddDbContextPool<InfraSecDbContext>(o =>
            {
                _ = o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                _ = o.UseSqlServer(connectionString);
                _ = o.EnableSensitiveDataLogging();
            });

        static void addServices(IServiceCollection services)
        {
            _ = services
                //.AddScoped<IInfraUserService, InfraIdentityService>()
                //.AddScoped<IInfraRoleService, InfraIdentityService>()
                //.AddScoped<IInfraClaimService, InfraIdentityService>()
                //.AddScoped<ISecurityService, InfraIdentityService>()
                .AddScoped<System.Security.Claims.ClaimsIdentity>()
                ;
            _ = services
                .AddScoped<IAuthorizationHandler, DynamicRoleHandler>()
                .AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>();
        }
        static void addUserContext(IServiceCollection services) =>
            services.AddScoped<IUserContext, UserContext>();

        static void addLoggers(IServiceCollection services, ILogger logger) =>
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger<UserManager<InfraIdentityUser>>>(new WebLogger<UserManager<InfraIdentityUser>>(logger)).AddSingleton<Microsoft.Extensions.Logging.ILogger<RoleManager<InfraIdentityRole>>>(new WebLogger<RoleManager<InfraIdentityRole>>(logger)).AddSingleton<Microsoft.Extensions.Logging.ILogger<SignInManager<InfraIdentityUser>>>(new WebLogger<SignInManager<InfraIdentityUser>>(logger));

        static void addTools(IServiceCollection services) => services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
    }

    public static IApplicationBuilder UseMesSecurityInfraMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<SecurityMiddleware>();
}