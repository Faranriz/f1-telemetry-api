namespace TelemetryApi.Models;

public record DriverStats(
    string Driver,
    int LapsCompleted,
    double BestLapSeconds,
    double AverageLapSeconds,
    double TopSpeedKph);