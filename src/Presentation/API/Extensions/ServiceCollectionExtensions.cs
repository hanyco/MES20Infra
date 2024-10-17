using API.Services.Identity;
using Application.DTOs.Settings;
using Application.Identity.Services;
using Application.Infrastructure.Identity;
using Application.Interfaces.Shared;
using Domain.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json;
using System.Text;
using Application.Interfaces;
using Library.Results;
using Application.Infrastructure.Identity.Services;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
        return services;
    }

    public static IServiceCollection AddEssentials(this IServiceCollection services)
    {
        services.RegisterSwagger();
        return services;
    }

    private static void RegisterSwagger(this IServiceCollection services)
    {
        //services.AddSwaggerGen(c =>
        //{
        //    //TODO - Lowercase Swagger Documents
        //    //c.DocumentFilter<LowercaseDocumentFilter>();
        //    //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f
        //    c.IncludeXmlComments(string.Format(@"{0}\GsTechCoreV1.Api.xml", System.AppDomain.CurrentDomain.BaseDirectory));
        //    c.SwaggerDoc("v1", new OpenApiInfo
        //    {
        //        Version = "v1",
        //        Title = "GsTechCoreV1",
        //        License = new OpenApiLicense()
        //        {
        //            Name = "MIT License",
        //            Url = new Uri("https://opensource.org/licenses/MIT")
        //        }
        //    });
        //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //    {
        //        Name = "Authorization",
        //        In = ParameterLocation.Header,
        //        Type = SecuritySchemeType.ApiKey,
        //        Scheme = "Bearer",
        //        BearerFormat = "JWT",
        //        Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
        //    });
        //    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        //        {
        //            {
        //                new OpenApiSecurityScheme
        //                {
        //                    Reference = new OpenApiReference
        //                    {
        //                        Type = ReferenceType.SecurityScheme,
        //                        Id = "Bearer",
        //                    },
        //                    Scheme = "Bearer",
        //                    Name = "Bearer",
        //                    In = ParameterLocation.Header,
        //                }, new List<string>()
        //            },
        //        });
        //});
    }

    public static IServiceCollection AddContextInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"), op => op.CommandTimeout(120)));
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 4;
        }).AddEntityFrameworkStores<IdentityContext>().AddDefaultUI().AddDefaultTokenProviders();

        #region Services

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ISecurityService, SecurityService>();

        #endregion Services

        // Configuration for JWT authentication and handling
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
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]!))
                };
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context => context.Response.WriteErrorAsync(StatusCodes.Status401Unauthorized, "Authentication failed"),
                    OnChallenge = context =>
                        !context.Response.HasStarted
                            ? context.Response.WriteErrorAsync(StatusCodes.Status401Unauthorized, "You are not authorized")
                            : Task.CompletedTask,
                    OnForbidden = context => context.Response.WriteErrorAsync(StatusCodes.Status403Forbidden, "You are not authorized to access this resource")
                };
            });

        // Old way
        //services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
        //services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //})
        //    .AddJwtBearer(o =>
        //    {
        //        o.RequireHttpsMetadata = false;
        //        o.SaveToken = false;
        //        o.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ClockSkew = TimeSpan.Zero,
        //            ValidIssuer = configuration["JWTSettings:Issuer"],
        //            ValidAudience = configuration["JWTSettings:Audience"],
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
        //        };
        //        o.Events = new JwtBearerEvents()
        //        {
        //            OnAuthenticationFailed = c =>
        //            {
        //                c.NoResult();
        //                c.Response.StatusCode = 500;
        //                c.Response.ContentType = "text/plain";
        //                return c.Response.WriteAsync(c.Exception.ToString());
        //            },
        //            OnChallenge = context =>
        //            {
        //                context.HandleResponse();
        //                context.Response.StatusCode = 401;
        //                context.Response.ContentType = "application/json";
        //                var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized"));
        //                return context.Response.WriteAsync(result);
        //            },
        //            OnForbidden = context =>
        //            {
        //                context.Response.StatusCode = 403;
        //                context.Response.ContentType = "application/json";
        //                var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource"));
        //                return context.Response.WriteAsync(result);
        //            },
        //        };
        //    });
        return services;
    }
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