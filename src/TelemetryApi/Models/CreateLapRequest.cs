namespace TelemetryApi.Models;

/// <summary>Incoming payload for recording a new lap. Worth noting: no Id - the server assigns it</summary>
public record CreateLapRequest(
    string Driver
    string Circuit,
    int LapNumber,
    double LapTimeSeconds,
    double TopSpeedKph,
    string TyreCompound);