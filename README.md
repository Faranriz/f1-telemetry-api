[![CI](https://github.com/Faranriz/f1-telemetry-api/actions/workflows/ci.yml/badge.svg)](https://github.com/Faranriz/f1-telemetry-api/actions/workflows/ci.yml)

# F1 Telemetry API

A REST API for recording and querying Formula 1 lap telemetry, built with ASP.NET Core
Minimal APIs on .NET 10.

Companion to my [autonomous vehicle pathfinding project](https://github.com/Faranriz/autonomous-vehicle-pathfinding), which processes raw
telemetry into lap data — this service exposes the data over HTTP.

## Features

- Record and retrieve lap telemetry (driver, circuit, lap time, top speed, tyre compound)
- Filter laps by driver and circuit
- Compute fastest lap overall or per circuit
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
| Storage | In-memory (thread-safe), behind an interface |

## Running locally

```bash
git clone https://github.com/Faranriz/f1-telemetry-api.git
cd f1-telemetry-api
dotnet run --project src/TelemetryApi
```

Then open `http://localhost:5010/scalar/v1`.

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/health` | Service health |
| GET | `/api/laps` | All laps (`?driver=`, `?circuit=`) |
| GET | `/api/laps/{id}` | Single lap |
| GET | `/api/laps/fastest` | Fastest lap (`?circuit=`) |
| POST | `/api/laps` | Record a lap |
| DELETE | `/api/laps/{id}` | Delete a lap |
| GET | `/api/drivers/{driver}/stats` | Aggregate driver statistics |

## Design notes

Business logic lives in `ILapService`, deliberately separated from the HTTP layer.
This keeps the endpoints thin and makes the logic testable in isolation — the storage implementation can be swapped (in-memory, SQLite, Postgres) without touching routing.

Endpoints declare their responses via `Results<Ok<T>, NotFound>`, so the OpenAPI
document is generated from the type signatures rather than hand-maintained attributes.

<img width="1509" height="860" alt="Screenshot 2026-09-08 at 21 15 58" src="https://github.com/user-attachments/assets/8fd2d8fb-8809-408a-a35c-3bf7216a1193" />
<img width="1509" height="860" alt="Screenshot 2026-09-08 at 21 15 32" src="https://github.com/user-attachments/assets/bcf8520f-ed89-4e1a-aeee-806a4f4dcaf4" />
