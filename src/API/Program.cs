using API.Middlewares;

using Application;

using Domain;

using HanyCo.Infra;

using Library.Coding;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

builder.Services.With(services =>
{
    _ = services.AddLogging(loggingBuilder =>
     {
         _ = loggingBuilder.AddConsole();
     });
    _ = services.AddExceptionHandler<GlobalExceptionHander>().AddProblemDetails();
    _ = services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DomainModule)));
    _ = services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(ApplicatioModule)));
    //services.AddMesInfraServices()
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.With(app =>
{
    _ = app.UseMesInfraMiddleware();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();