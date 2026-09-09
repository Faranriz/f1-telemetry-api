using TelemetryApi.Services;
using TelemetryApi.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ILapService, InMemoryLapService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => 
    {
        options.Title = "F1 Telemetry API";
    });
    app.UseHttpsRedirection();
    SeedData.Populate(app.Services.GetRequiredService<ILapService>());
}

app.UseHttpsRedirection();


app.MapGet("/health", () => TypedResults.Ok(new { status = "healthy" }))
    .WithTags("System");

app.MapLapEndpoints();

app.Run();

// exposes auto-generated Program class so integration test project can reference it
public partial class Program { }