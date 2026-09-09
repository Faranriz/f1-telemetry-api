using TelemetryApi.Models;

namespace TelemetryApi.Services;

public static class SeedData{

    public static void Populate(ILapService service){
        
        service.Add(new CreateLapRequest("HAM", "Silverstone", 1, 88.412, 312.4, "Soft"));
        service.Add(new CreateLapRequest("HAM", "Silverstone", 2, 87.905, 315.1, "Soft"));
        service.Add(new CreateLapRequest("RUS", "Silverstone", 1, 88.740, 310.8, "Medium"));
        service.Add(new CreateLapRequest("RUS", "Silverstone", 2, 88.201, 313.6, "Medium"));
        service.Add(new CreateLapRequest("HAM", "Monza", 1, 82.115, 341.2, "Hard"));
    }
}