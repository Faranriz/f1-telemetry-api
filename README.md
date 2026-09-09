# F1 Telemetry API

[![CI](https://github.com/Faranriz/f1-telemetry-api/actions/workflows/ci.yml/badge.svg)](https://github.com/Faranriz/f1-telemetry-api/actions/workflows/ci.yml)

A REST API for recording and querying Formula 1 lap telemetry, built with ASP.NET Core
Minimal APIs on .NET 10, with an automated build-and-test pipeline running on every commit.

Companion to my autonomous vehicle pathfinding project, which processes raw telemetry
into lap data — this service exposes that data over HTTP.

## Features

- Record and retrieve lap telemetry (driver, circuit, lap time, top speed, tyre compound)
- Filter laps by driver and circuit
- Fastest and slowest lap, overall or per circuit
- Per-driver aggregate statistics (best lap, average lap, top speed)
- Request validation returning RFC 7807 problem details
- OpenAPI 3.1 specification with an interactive Scalar UI
- Health check endpoint

## Tech stack

| Concern | Choice |
|---|---|
| Runtime | .NET 10 (LTS) |
| Framework | ASP.NET Core Minimal APIs |
| API docs | Microsoft.AspNetCore.OpenApi + Scalar |
| Testing | xUnit, WebApplicationFactory |
| CI | GitHub Actions |
| Storage | In-memory (thread-safe), behind an interface |

## Running locally

```bash
git clone https://github.com/Faranriz/f1-telemetry-api.git
cd f1-telemetry-api
dotnet run --project src/TelemetryApi
```

Then open `https://localhost:7123/scalar/v1`.

## Running the tests

```bash
dotnet test
```

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/health` | Service health |
| GET | `/api/laps` | All laps (`?driver=`, `?circuit=`) |
| GET | `/api/laps/{id}` | Single lap |
| GET | `/api/laps/fastest` | Fastest lap (`?circuit=`) |
| GET | `/api/laps/slowest` | Slowest lap (`?circuit=`) |
| POST | `/api/laps` | Record a lap |
| DELETE | `/api/laps/{id}` | Delete a lap |
| GET | `/api/drivers/{driver}/stats` | Aggregate driver statistics |

## Project structure

```
├── .github/workflows/ci.yml   # Build, test and publish pipeline
├── src/TelemetryApi/          # API project
│   ├── Models/                # Domain records and DTOs
│   ├── Services/              # Business logic behind ILapService
│   └── Endpoints/             # Minimal API route definitions
└── tests/TelemetryApi.Tests/  # Unit and integration tests
```

## CI pipeline

Every push and pull request against `main` triggers a GitHub Actions workflow that:

1. Restores dependencies (with NuGet caching)
2. Verifies formatting with `dotnet format --verify-no-changes`
3. Builds in Release with warnings treated as errors
4. Runs the full unit and integration test suite
5. Uploads test results as an artifact
6. On merge to `main`, publishes a versioned application package tagged with the commit SHA

`main` is protected: pull requests cannot merge unless the pipeline passes.

## Design notes

Business logic lives behind `ILapService`, deliberately separated from the HTTP layer.
This keeps endpoints thin and makes the logic unit-testable in isolation, while
integration tests exercise the real routing, serialisation and status codes via
`WebApplicationFactory`.

Endpoints declare their responses via `Results<Ok<T>, NotFound>`, so the OpenAPI
document is generated from the type signatures rather than hand-maintained attributes.

<img width="1509" height="860" alt="Screenshot 2026-09-08 at 21 15 58" src="https://github.com/user-attachments/assets/8fd2d8fb-8809-408a-a35c-3bf7216a1193" />
<img width="1509" height="860" alt="Screenshot 2026-09-08 at 21 15 32" src="https://github.com/user-attachments/assets/bcf8520f-ed89-4e1a-aeee-806a4f4dcaf4" />
