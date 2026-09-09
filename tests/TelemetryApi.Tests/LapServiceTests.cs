using TelemetryApi.Models;
using TelemetryApi.Services;
using Xunit;

namespace TelemetryApi.Tests;

public class LapServiceTests
{

    // A helper so each test starts from a clean and known state.
    private static InMemoryLapService CreateService() => new();

    private static CreateLapRequest ALap(
        string driver = "HAM",
        string circuit = "Silverstone",
        int lapNumber = 1,
        double lapTime = 90.0,
        double topSpeed = 300.0,
        string tyre = "Soft")
        => new(driver, circuit, lapNumber, lapTime, topSpeed, tyre);

    [Fact]
    public void GetAll_returns_empty_when_no_laps_recorded()
    {
        // Arrange
        var service = CreateService();

        // Act
        var laps = service.GetAll();

        // Assert
        Assert.Empty(laps);
    }

    [Fact]
    public void Add_assigns_sequential_ids_starting_at_one()
    {
        var service = CreateService();

        var first = service.Add(ALap());
        var second = service.Add(ALap(lapNumber: 2));

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public void Add_normalises_driver_code_to_uppercase()
    {
        var service = CreateService();

        var lap = service.Add(ALap(driver: "ham"));

        Assert.Equal("HAM", lap.Driver);
    }

    [Fact]
    public void GetById_returns_null_for_unknown_id()
    {
        var service = CreateService();

        var lap = service.GetById(999);

        Assert.Null(lap);
    }

    [Fact]
    public void GetAll_filters_by_driver_case_insensitively()
    {
        var service = CreateService();
        service.Add(ALap(driver: "HAM"));
        service.Add(ALap(driver: "RUS"));

        var laps = service.GetAll(driver: "ham");

        Assert.Single(laps);
        Assert.Equal("HAM", laps[0].Driver);
    }

    [Fact]
    public void GetFastestLap_returns_the_lowest_lap_time()
    {
        var service = CreateService();
        service.Add(ALap(lapTime: 91.5));
        service.Add(ALap(lapTime: 88.2, lapNumber: 2));
        service.Add(ALap(lapTime: 90.1, lapNumber: 3));

        var fastest = service.GetFastestLap();

        Assert.NotNull(fastest);
        Assert.Equal(88.2, fastest.LapTimeSeconds);
    }

    [Fact]
    public void GetFastestLap_respects_the_circuit_filter()
    {
        var service = CreateService();
        service.Add(ALap(circuit: "Monza", lapTime: 82.0));
        service.Add(ALap(circuit: "Silverstone", lapTime: 88.0));

        var fastest = service.GetFastestLap(circuit: "Silverstone");

        Assert.NotNull(fastest);
        Assert.Equal("Silverstone", fastest.Circuit);
        Assert.Equal(88.0, fastest.LapTimeSeconds);
    }

    [Fact]
    public void GetFastestLap_returns_null_when_there_are_no_laps()
    {
        var service = CreateService();

        Assert.Null(service.GetFastestLap());
    }

    [Fact]
    public void GetDriverStats_computes_count_best_average_and_top_speed()
    {
        var service = CreateService();
        service.Add(ALap(driver: "HAM", lapTime: 90.0, topSpeed: 300.0));
        service.Add(ALap(driver: "HAM", lapTime: 88.0, topSpeed: 310.0, lapNumber: 2));
        service.Add(ALap(driver: "RUS", lapTime: 70.0, topSpeed: 350.0));

        var stats = service.GetDriverStats("HAM");

        Assert.NotNull(stats);
        Assert.Equal(2, stats.LapsCompleted);
        Assert.Equal(88.0, stats.BestLapSeconds);
        Assert.Equal(89.0, stats.AverageLapSeconds);
        Assert.Equal(310.0, stats.TopSpeedKph);
    }

    [Fact]
    public void GetDriverStats_returns_null_for_a_driver_with_no_laps()
    {
        var service = CreateService();
        service.Add(ALap(driver: "HAM"));

        Assert.Null(service.GetDriverStats("VER"));
    }

    [Fact]
    public void Delete_removes_the_lap_and_reports_success()
    {
        var service = CreateService();
        var lap = service.Add(ALap());

        var deleted = service.Delete(lap.Id);

        Assert.True(deleted);
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void Delete_returns_false_for_unknown_id()
    {
        var service = CreateService();

        Assert.False(service.Delete(999));
    }

    // Doing the same test multiple times; avoids copying and pasting the same test body
    [Theory]
    [InlineData("HAM", 2)]
    [InlineData("RUS", 1)]
    [InlineData("VER", 0)]
    public void GetAll_returns_the_expected_lap_count_per_driver(string driver, int expected)
    {
        var service = CreateService();
        service.Add(ALap(driver: "HAM"));
        service.Add(ALap(driver: "HAM", lapNumber: 2));
        service.Add(ALap(driver: "RUS"));

        var laps = service.GetAll(driver: driver);

        Assert.Equal(expected, laps.Count);
    }
}
