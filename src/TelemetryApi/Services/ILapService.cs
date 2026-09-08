using TelemetryApi.Models;

namespace TelemetryApi.Services;

public interface ILapService{
    IReadOnlyList<Lap> GetAll(string? driver = null, string? circuit = null);
    Lap? GetById(int id);
    Lap Add(CreateLapRequest request);
    bool Delete(int id);
    Lap? GetFastestLap(string? circuit = null);
    DriverStats? GetDriverStats(string driver);
}