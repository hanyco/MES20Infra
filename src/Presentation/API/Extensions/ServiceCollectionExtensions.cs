using API.Services.Identity;

using Application;
using Application.Features.Identity;
using Application.Infrastructure.Persistence;
using Application.Interfaces.Permissions.Repositories;
using Application.Interfaces.Shared;
using Application.Interfaces.Shared.Security;
using Application.Settings;

using Domain.Identity;

using Library.Data.SqlServer;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;

using System.Text;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContextInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseServices(configuration);

        _ = services.AddTransient<IIdentityService, IdentityService>()
            .AddTransient<ISecurityService, SecurityService>()
            .AddJwtAuthentication(configuration);

        _ = services
            .AddHttpContextAccessor();
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
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
                            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
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

    private static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddScoped(_ => new Sql(configuration.GetConnectionString("ApplicationConnectionString").NotNull(() => "Connection String not found.")));

        _ = services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnectionString").NotNull(() => "Identity Connection String not found."), op => op.CommandTimeout(120)));
        _ = services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 4;
        })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection AddEssentials(this IServiceCollection services)
    {
        services.RegisterSwagger();
        return services;
    }

    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services
            .AddApplicationServices(configuration)
            .AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
        return services;
    }

    private static void RegisterSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(c =>
        {
            // TODO - Lowercase Swagger Documents
            //c.DocumentFilter<LowercaseDocumentFilter>();
            // Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f
            //c.IncludeXmlComments(string.Format(@"{0}\Api.xml", System.AppDomain.CurrentDomain.BaseDirectory));
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