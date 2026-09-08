using TelemetryApi.Endpoints;
using TelemetryApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ILapService, InMemoryLapService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/health", () => TypedResults.Ok(new { status = "healthy" }))
    .WithTags("System");

app.MapLapEndpoints();

app.Run();