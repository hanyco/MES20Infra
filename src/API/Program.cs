using API.Middlewares;

using Application;

using Domain;

using HanyCo.Infra;

var builder = WebApplication.CreateBuilder(args);

// Create IConfiguration
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Add services
builder.Services.With(services =>
{
    _ = services.AddLogging(loggingBuilder => _ = loggingBuilder.AddConsole());
    _ = services.AddExceptionHandler<GlobalExceptionHander>().AddProblemDetails();
    _ = services
        .AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DomainModule)))
        .AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(ApplicationModule)));

    _ = services.AddMesInfraServices(config);

    _ = services.AddControllers();
    _ = services.AddEndpointsApiExplorer();
    _ = services.AddSwaggerGen();
});

// Build application and add middleware
var app = builder.Build()
    .With(app =>
    {
        _ = app.UseMesInfraMiddleware();
        //_ = app.UseLoggerMiddleware();
        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
        }
        _ = app.UseAuthorization();
        _ = app.MapControllers();
    });

app.Run();