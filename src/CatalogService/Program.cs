using CatalogService.CompiledModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<CatalogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("CatalogDb") ??
        throw new InvalidDataException("Missing connection string CatalogDb");

    options.UseNpgsql(connectionString)
           // Compiled model: this improves the startup time
           // https://learn.microsoft.com/ef/core/performance/advanced-performance-topics#compiled-models
           .UseModel(CatalogDbContextModel.Instance);

    // The need for speed, use with caution!
    // https://learn.microsoft.com/ef/core/performance/advanced-performance-topics#reducing-runtime-overhead
    options.EnableThreadSafetyChecks(enableChecks: false);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCatalogApi();

await app.Services.InitializeDatabaseAsync();

app.Run();
