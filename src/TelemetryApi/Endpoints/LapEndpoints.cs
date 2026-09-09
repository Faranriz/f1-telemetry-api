using Microsoft.AspNetCore.Http.HttpResults;

using TelemetryApi.Models;
using TelemetryApi.Services;

namespace TelemetryApi.Endpoints;

public static class LapEndpoints
{

    public static IEndpointRouteBuilder MapLapEndpoints(this IEndpointRouteBuilder app)
    {

        var group = app.MapGroup("/api/laps")
                        .WithTags("Laps");

        group.MapGet("/", (ILapService service, string? driver, string? circuit) =>
                TypedResults.Ok(service.GetAll(driver, circuit)))
            .WithName("GetLaps")
            .WithSummary("List laps, optionally filtered by driver and/or circuit.");

        group.MapGet("/fastest", Results<Ok<Lap>, NotFound> (ILapService service, string? circuit) =>
        {
            var lap = service.GetFastestLap(circuit);
            return lap is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(lap);
        })
            .WithName("GetFastestLap")
            .WithSummary("Return the fastest recorded lap, optionally within one circuit.");

        group.MapGet("/{id:int}", Results<Ok<Lap>, NotFound> (ILapService service, int id) =>
            {
                var lap = service.GetById(id);
                return lap is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(lap);
            })
            .WithName("GetLapById");

        group.MapPost("/", Results<Created<Lap>, ValidationProblem> (
                ILapService service, CreateLapRequest request) =>
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                {
                    return TypedResults.ValidationProblem(errors);
                }

                var lap = service.Add(request);
                return TypedResults.Created($"/api/laps/{lap.Id}", lap);
            })
            .WithName("CreateLap")
            .WithSummary("Record a new lap.");

        group.MapDelete("/{id:int}", Results<NoContent, NotFound> (ILapService service, int id) =>
                service.Delete(id)
                    ? TypedResults.NoContent()
                    : TypedResults.NotFound())
                .WithName("DeleteLap");

        app.MapGet("/api/drivers/{driver}/stats",
                Results<Ok<DriverStats>, NotFound> (ILapService service, string driver) =>
            {
                var stats = service.GetDriverStats(driver);
                return stats is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(stats);
            })
            .WithTags("Drivers")
            .WithName("GetDriverStats");

        return app;
    }

    private static Dictionary<string, string[]> Validate(CreateLapRequest r)
    {

        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(r.Driver))
            errors[nameof(r.Driver)] = ["Driver is required."];

        if (string.IsNullOrWhiteSpace(r.Circuit))
            errors[nameof(r.Circuit)] = ["Circuit is required."];

        if (r.LapNumber <= 0)
            errors[nameof(r.LapNumber)] = ["Lap number must be greater than zero."];

        if (r.LapTimeSeconds <= 0)
            errors[nameof(r.LapTimeSeconds)] = ["Lap time must be greater than zero."];

        if (r.TopSpeedKph is <= 0 or > 400)
            errors[nameof(r.TopSpeedKph)] = ["Top speed must be between 0 and 400 kph."];

        return errors;
    }

}