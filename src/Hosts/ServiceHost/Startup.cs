using System;
using System.IO;

using Autofac;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Web;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ServiceHost
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            _ = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .ConfigureMesInfra(env.WebRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json");
            //.AddJsonFile("hosting.json", optional: false)
            //.AddJsonFile($"hosting.{env.EnvironmentName}.json")
            //.AddJsonFile("launchSettings.json", optional: false)
            //.AddJsonFile($"launchSettings.{env.EnvironmentName}.json");
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //! This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.Configure<KestrelServerOptions>(this.Configuration.GetSection("Kestrel"));
            _ = services.AddHttpContextAccessor();
            _ = services.AddControllers(options => options.EnableEndpointRouting = false);
            _ = services.AddMvcCore();

            _ = services.AddMapper();
            _ = services.AddMesInfraServices<Startup>(this.Configuration);

            _ = services.AddDbContextPool<InfraWriteDbContext>(options =>
               {
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                   _ = options.UseSqlServer(this.Configuration.GetConnectionString("InfraWriteDbContext"));
                   _ = options.EnableSensitiveDataLogging();
               })
               .AddDbContextPool<InfraReadDbContext>(options =>
               {
                   _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                   _ = options.UseSqlServer(this.Configuration.GetConnectionString("InfraReadDbContext"));
                   _ = options.EnableSensitiveDataLogging();
               });

            _ = services.AddSwaggerGen(c =>
                  {
                      c.SwaggerDoc("v1", new OpenApiInfo { Title = "HanyCo Infrastructure Services", Version = "v1" });
                      var filePath = Path.Combine(AppContext.BaseDirectory, "HanyCo.InfraServiceHost.xml");
                      c.IncludeXmlComments(filePath);
                  });

        }

        //! This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = app.UseMesInfraMiddleware();

            //! Swagger must be enabled for now. Later, will be disabled.
            //x if (env.IsDevelopment())
            {
                //! Handled by Infrastructure
                //x app.UseDeveloperExceptionPage();
                _ = app.UseSwagger()
                   .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HanyCo MES Infra Web API V1"));
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseRouting();

            _ = app.UseAuthorization().UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
              {
                  _ = env.IsDevelopment()
                      ? endpoints.MapGet("/", async context => await context.Response.WriteAsync(@"
<h1><center>Welcome to HanyCo Manufacturing Execution System 2.0.<center></h1>
<h2><center><a href=""/swagger/index.html"">API Help Page</center><h2>
"))
                      : endpoints.MapControllers();

                  _ = endpoints.MapControllerRoute(
                          "default",
                          "{controller=Home}/{action=Index}/{id?}");
              });
        }

        public void ConfigureContainer(ContainerBuilder builder) { }
    }
}
