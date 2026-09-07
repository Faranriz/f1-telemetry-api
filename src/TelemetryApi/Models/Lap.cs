namespace TelemetryApi.Models;

/// <summary>A single completed lap of telemetry</summary>
public record Lap(
    int Id,
    string Driver,
    string Circuit,
    int LapNumber,
    double LapTimeSeconds,
    double TopSpeedKph,
    string TyreCompound);


