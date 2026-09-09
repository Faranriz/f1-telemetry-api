using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TelemetryApi.Models;
using Xunit;

namespace TelemetryApi.Tests;

public class LapEndpointsTests : IClassFixture<WebApplicationFactory<Program>>{
    private readonly WebApplicationFactory<Program> _factory;

    public LapEndpointsTests(WebApplicationFactory<Program> factory){
        _factory = factory;
    }

    [Fact]
    public async Task Health_endpoint_returns_200(){
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_laps_returns_200_and_json(){
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/laps");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json",
            response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_lap_by_unknown_id_returns_404(){
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/laps/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_lap_with_non_numeric_id_returns_404_from_route_constraint(){
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/laps/not-a-number");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_valid_lap_returns_201_with_location_header(){
        var client = _factory.CreateClient();
        var request = new CreateLapRequest("NOR", "Spa", 1, 104.221, 322.5, "Medium");

        var response = await client.PostAsJsonAsync("/api/laps", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<Lap>();
        Assert.NotNull(created);
        Assert.Equal("NOR", created.Driver);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task Post_then_get_returns_the_same_lap(){
        var client = _factory.CreateClient();
        var request = new CreateLapRequest("PIA", "Suzuka", 3, 95.004, 318.0, "Hard");

        var postResponse = await client.PostAsJsonAsync("/api/laps", request);
        var created = await postResponse.Content.ReadFromJsonAsync<Lap>();

        var getResponse = await client.GetAsync($"/api/laps/{created!.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<Lap>();

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal("PIA", fetched.Driver);
        Assert.Equal("Suzuka", fetched.Circuit);
    }

    [Theory]
    [InlineData("", "Monza", 1, 90.0, 300.0)]       // empty driver
    [InlineData("HAM", "", 1, 90.0, 300.0)]          // empty circuit
    [InlineData("HAM", "Monza", 0, 90.0, 300.0)]     // lap number not positive
    [InlineData("HAM", "Monza", 1, 0.0, 300.0)]      // lap time not positive
    [InlineData("HAM", "Monza", 1, 90.0, 900.0)]     // top speed out of range
    public async Task Post_invalid_lap_returns_400(
        string driver, string circuit, int lapNumber, double lapTime, double topSpeed){

        var client = _factory.CreateClient();
        var request = new CreateLapRequest(driver, circuit, lapNumber, lapTime, topSpeed, "Soft");

        var response = await client.PostAsJsonAsync("/api/laps", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_unknown_lap_returns_404(){
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/laps/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Driver_stats_for_unknown_driver_returns_404(){
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/drivers/ZZZ/stats");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}