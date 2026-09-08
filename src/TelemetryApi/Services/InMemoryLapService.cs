using TelemetryApi.Models;

namespace TelemetryApi.Services;

public class InMemoryLapService : ILapService{
    private readonly List<Lap> _laps = [];
    private readonly Lock _gate = new();
    private int _nextId = 1;

    public InMemoryLapService(){
        // Seeing data so API isn't empty on first run
        Add(new CreateLapRequest("HAM", "Silverstone", 1, 88.412, 312.4, "Soft"));
        Add(new CreateLapRequest("HAM", "Silverstone", 2, 87.905, 315.1, "Soft"));
        Add(new CreateLapRequest("RUS", "Silverstone", 1, 88.740, 310.8, "Medium"));
        Add(new CreateLapRequest("RUS", "Silverstone", 2, 88.201, 313.6, "Medium"));
        Add(new CreateLapRequest("HAM", "Monza", 1, 82.115, 341.2, "Hard"));
    }

    public IReadOnlyList<Lap> GetAll(string? driver = null, string? circuit = null){
        lock(_gate){
            return _laps
            .Where(l => driver is null ||
                        l.Driver.Equals(driver, StringComparison.OrdinalIgnoreCase))
            .Where(l => circuit is null ||
                        l.Circuit.Equals(circuit, StringComparison.OrdinalIgnoreCase))
            .OrderBy(l => l.Circuit)
            .ThenBy(l => l.Driver)
            .ThenBy(l => l.LapNumber)
            .ToList();
        }
    }

    public Lap? GetById(int id){
        lock (_gate){
            return _laps.FirstOrDefault(l => l.Id == id);
        }
    }

    public Lap Add(CreateLapRequest request){
        lock (_gate){
            var lap = new Lap(
                Id: _nextId++,
                Driver: request.Driver.ToUpperInvariant(),
                Circuit: request.Circuit,
                LapNumber: request.LapNumber,
                LapTimeSeconds: request.LapTimeSeconds,
                TopSpeedKph: request.TopSpeedKph,
                TyreCompound: request.TyreCompound);

            _laps.Add(lap);
            return lap;
        }
    }

    public bool Delete(int id){
        lock (_gate){
            var lap = _laps.FirstOrDefault(l => l.Id == id);
            return lap is not null && _laps.Remove(lap);
        }
    }

    public Lap? GetFastestLap(string? circuit = null){
        lock (_gate){
            return _laps
                .Where(l => circuit is null ||
                            l.Circuit.Equals(circuit, StringComparison.OrdinalIgnoreCase))
                .MinBy(l => l.LapTimeSeconds);
        }
    }

    public DriverStats? GetDriverStats(string driver){
        lock (_gate){
            var laps = _laps
                .Where(l => l.Driver.Equals(driver, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (laps.Count == 0) return null;

            return new DriverStats(
                Driver: laps[0].Driver,
                LapsCompleted: laps.Count,
                BestLapSeconds: laps.Min(l => l.LapTimeSeconds),
                AverageLapSeconds: Math.Round(laps.Average(l => l.LapTimeSeconds), 3),
                TopSpeedKph: laps.Max(l => l.TopSpeedKph));
        }
    }

}