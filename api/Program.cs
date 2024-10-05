using Api.Data;
using Api.Endpoints;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var assemblyName = typeof(Program).Assembly.FullName;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<SwDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("local");
    options.UseSqlite(connectionString, options => {
        options.MigrationsAssembly(assemblyName);
        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    });
});

builder.Services.AddSingleton<DbInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DbInitializer>());

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapSwCharactersEndpoints();

await app.RunAsync("http://*:7145");

public partial class Program { }