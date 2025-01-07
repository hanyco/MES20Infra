using System.Text;

using API.Extensions;
using API.Services.Identity;

using Application;
using Application.Features.Identity;
using Application.Infrastructure.Persistence;
using Application.Interfaces.Shared;
using Application.Interfaces.Shared.Security;
using Application.Settings;

using Library.Data.SqlServer;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;

namespace API.Extensions;

[Obsolete("Extension methods class. Do NOT use, directly.", true)]
public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddAddControllers(this IServiceCollection services)
    {
        _ = services
            .AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        _ = services
            .AddEndpointsApiExplorer();
        return services;
    }

    public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddScoped(_ => new Sql(configuration.GetConnectionString("ApplicationConnectionString").NotNull(() => "ConnectionString not found.")));

        _ = services
            .AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityConnectionString").NotNull(() => "Identity ConnectionString not found."),
                    op => op.CommandTimeout(120))
                .LogTo(Console.WriteLine, LogLevel.Information));
        _ = services
            .AddIdentity<AspNetUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        _ = services
            .Configure<JWTSettings>(configuration.GetSection("JWTSettings"))
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]!))
                };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Ignore token validation for [AllowAnonymous] endpoints
                        var endpoint = context.HttpContext.GetEndpoint();
                        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                        {
                            return Task.CompletedTask;
                        }

                        // Otherwise, proceed with token validation
                        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context => context.Response.WriteErrorAsync(StatusCodes.Status401Unauthorized, "Authentication failed"),
                    OnChallenge = context =>
                        !context.Response.HasStarted
                            ? context.Response.WriteErrorAsync(StatusCodes.Status401Unauthorized, "You are not authorized")
                            : Task.CompletedTask,
                    OnForbidden = context => context.Response.WriteErrorAsync(StatusCodes.Status403Forbidden, "You are not authorized to access this resource")
                };
            });
        return services;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services) =>
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly))
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DomainModule).Assembly))
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationModule).Assembly));

    public static IServiceCollection AddSecurity(this IServiceCollection services, ConfigurationManager configuration) =>
        services
            .AddTransient<IIdentityService, IdentityService>()
            .AddTransient<ISecurityService, SecurityService>()
            .AddJwtAuthentication(configuration);

    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddApplicationServices(configuration)
            .AddTransient<ILoggedInUser, LoggedInUser>();

    public static IServiceCollection AddSwagger(this IServiceCollection services) =>
        services
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MES API",
                    Version = "v1",
                    Description = "API Documentation for MES System",
                    Contact = new OpenApiContact
                    {
                        Name = "Support Team",
                        Email = "support@hanyco.com"
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
}

internal static class HttpResponseExtensions
{
    public static Task WriteErrorAsync(this HttpResponse response, int statusCode, string message)
    {
        if (!response.HasStarted)
        {
            response.StatusCode = statusCode;
        }

        response.ContentType = "application/json";
        return response.WriteAsync(JsonConvert.SerializeObject(Result.Fail(message)));
    }
}