# ObservabilityAuditSystem

Production-oriented Observability and Audit API built with ASP.NET Core 8 and Clean Architecture.

## Tech Stack
- ASP.NET Core 8
- Entity Framework Core 8
- SQL Server
- MediatR + FluentValidation
- Serilog (structured JSON logs)
- Prometheus metrics (`/metrics`)
- Built-in ASP.NET Core rate limiting
- JWT Bearer authentication (development placeholder flow)
- Health Checks (`/health`, `/health/live`, `/health/ready`, `/health/detail`)
- Swagger/OpenAPI
- Grafana dashboard provisioning
- xUnit + FluentAssertions + Testcontainers

## Solution Structure
- `src/Api` HTTP layer (controllers, middleware, health, swagger, auth)
- `src/Application` CQRS handlers, validators, DTOs, behaviors
- `src/Domain` entities, value objects, domain events, interfaces
- `src/Infrastructure` EF Core, repositories, logging, monitoring services
- `src/SharedKernel` base abstractions and shared primitives
- `src/Bootstrapper` dependency injection and startup composition
- `tests` unit/integration tests

## Run Locally
1. Restore/build:
```bash
dotnet restore ObservabilityAuditSystem.sln
dotnet build ObservabilityAuditSystem.sln
```

2. Run API:
```bash
dotnet run --project src/Api/Api.csproj
```

3. Open Swagger:
- `http://localhost:5000/swagger` (or the port shown in launch profile)

## JWT Placeholder Flow
Generate token:
```bash
curl -X POST http://localhost:5000/api/v1/auth/token \
  -H "Content-Type: application/json" \
  -d "{\"userId\":\"admin-1\",\"email\":\"admin@test.local\",\"role\":\"admin\"}"
```

Use token in protected endpoint (purge):
```bash
curl -X DELETE "http://localhost:5000/api/v1/audit-logs/purge?olderThan=2026-01-01T00:00:00Z" \
  -H "Authorization: Bearer <token>"
```

## Docker (API + SQL Server + Prometheus + Grafana)
From project root:
```bash
docker compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml up --build
```

- API: `http://localhost:5000`
- SQL Server: `localhost:1433` (`sa/Your_strong_Passw0rd!`)
- Prometheus: `http://localhost:9090`
- Grafana: `http://localhost:3000` (`admin/admin`)

Grafana dashboard is pre-provisioned from:
- `docker/grafana/dashboards/observability-audit-dashboard.json`

## Database Migrations
Create migration:
```bash
dotnet ef migrations add <MigrationName> --project src/Infrastructure/Infrastructure.csproj --startup-project src/Api/Api.csproj --output-dir Persistence/Migrations
```

Update DB:
```bash
dotnet ef database update --project src/Infrastructure/Infrastructure.csproj --startup-project src/Api/Api.csproj
```

## Key Endpoints
### Auth
- `POST /api/v1/auth/token`

### Audit
- `GET /api/v1/audit-logs`
- `GET /api/v1/audit-logs/{id}`
- `GET /api/v1/audit-logs/user/{userId}`
- `GET /api/v1/audit-logs/export`
- `DELETE /api/v1/audit-logs/purge?olderThan=<utc-date>` (requires `admin` role)

### Health
- `GET /health`
- `GET /health/live`
- `GET /health/ready`
- `GET /health/detail`

### Metrics
- `GET /metrics`
- `GET /api/v1/metrics/summary`
- `GET /api/v1/metrics/errors`
- `GET /api/v1/metrics/latency`

## Tests
```bash
dotnet test ObservabilityAuditSystem.sln
```
