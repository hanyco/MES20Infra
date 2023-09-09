//using HanyCo.Infra.Security.DataSources;
//using HanyCo.Infra.Security.Identity;
//using HanyCo.Infra.Security.Identity.Model;
//using HanyCo.Infra.Security.Model;
//using HanyCo.Infra.Security.Services;
//using Library.Logging;
//using Library.Security.Claims;
//using Library.Security.Authorization;
//using Library.Validations;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//namespace HanyCo.Infra.Security;

//public static class MesSecurityConfiguration
//{
//    public static IServiceCollection AddMesInfraSecurityServices<TStartup>(this IServiceCollection services, ISecutityConfigOptions options)
//    {
//        Check.MustBeArgumentNotNull(options);

//        addLoggers(services, options.Logger);
//        addIdentity(services);
//        addAuthorization(services);
//        addAuthentication(services);
//        addDbContextPool(services, options);
//        addServices(services);
//        addUserContext(services);
//        //MvcHelper.Initialize();

//        return services;

//        static void addIdentity(IServiceCollection services) =>
//            _ = services.AddIdentity<InfraIdentityUser, InfraIdentityRole>(options =>
//                {
//                    // موارد مربوط به تنظیمات آیدنتیتی، ایجاد قابل تغییر است. مثال:
//                    //options.Password = new() { RequiredLength = 8 };
//                    options.User.RequireUniqueEmail = true;
//                }).AddEntityFrameworkStores<InfraSecDbContext>()
//                  //.AddDefaultTokenProviders()
//                  .AddErrorDescriber<PersianIdentityErrorDescriber>();

//        static void addAuthorization(IServiceCollection services) =>
//            _ = services.AddAuthorization(options =>
//                        {
//                            options.FallbackPolicy = new AuthorizationPolicyBuilder().AddAuthenticationSchemes("Bearer")
//                                                                                     .RequireAuthenticatedUser()
//                                                                                     .RequireClaim("scope", "read").Build();
//                            options.AddPolicies(LibCrudPolicies.FullAccessPolicy, LibCrudPolicies.AdminOrFullAccessPolicy)
//                                   .AddCrudRequirementPolicies();
//                        });

//        static void addAuthentication(IServiceCollection services) =>
//            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                        .AddJwtBearer(o =>
//                        {
//                            o.Authority = "http://localhost:5000/openid";
//                            o.Audience = "embedded";
//                            o.RequireHttpsMetadata = false;
//                        });

//        static void addDbContextPool(IServiceCollection services, ISecutityConfigOptions options) =>
//            _ = services.AddDbContextPool<InfraSecDbContext>(o =>
//                {
//                    _ = o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
//                    _ = o.UseSqlServer(options.ConnectionString);
//                    _ = o.EnableSensitiveDataLogging();
//                });

//        static void addServices(IServiceCollection services)
//        {
//            _ = services.AddScoped<IInfraUserService, InfraIdentityService>()
//                        //.AddScoped<IInfraRoleService, InfraIdentityService>()
//                        //.AddScoped<IInfraClaimService, InfraIdentityService>()
//                        .AddScoped<ISecurityService, InfraIdentityService>()
//                        .AddScoped<System.Security.Claims.ClaimsIdentity>()
//                        ;
//            _ = services.AddScoped<IAuthorizationHandler, DynamicRoleHandler>()
//                        .AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>();
//        }
//        static void addUserContext(IServiceCollection services) =>
//            _ = services.AddScoped<IUserContext, UserContext>();

//        static void addLoggers(IServiceCollection services, ILogger logger) =>
//            _ = services.AddSingleton<Microsoft.Extensions.Logging.ILogger<UserManager<InfraIdentityUser>>>(new WebLogger<UserManager<InfraIdentityUser>>(logger))
//                .AddSingleton<Microsoft.Extensions.Logging.ILogger<RoleManager<InfraIdentityRole>>>(new WebLogger<RoleManager<InfraIdentityRole>>(logger))
//                .AddSingleton<Microsoft.Extensions.Logging.ILogger<SignInManager<InfraIdentityUser>>>(new WebLogger<Microsoft.AspNetCore.Identity.SignInManager<Security.Identity.InfraIdentityUser>>(logger));
//    }
//    public static IApplicationBuilder UseMesSecutityInfraMiddlewares(this IApplicationBuilder app)
//        => app.UseMiddleware<SecurityMiddleware>();
//}
