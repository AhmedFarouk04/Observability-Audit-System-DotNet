# Observability & Audit System Architecture

## Overview
The solution follows Clean Architecture with strict layering:

- `Api`: transport layer (HTTP endpoints, middleware, health, swagger)
- `Application`: use-cases (CQRS handlers, validators, behaviors, DTOs)
- `Domain`: core business model (entities, value objects, domain events, repository contracts)
- `Infrastructure`: database, logging, metrics implementation, repository implementations
- `SharedKernel`: reusable primitives and base abstractions
- `Bootstrapper`: dependency wiring and startup composition

## Request Flow
1. Request enters API middleware pipeline (correlation id, exception handling, request logging)
2. Controller sends command/query via MediatR
3. Pipeline behaviors run (logging, validation, metrics)
4. Handler executes business logic through domain repository interfaces
5. Infrastructure repositories persist/read data via EF Core + PostgreSQL
6. Response returns with consistent shape and correlation id

## Cross-Cutting Concerns
- Structured logging via Serilog in JSON format
- Correlation propagation via `X-Correlation-ID`
- Prometheus-compatible metrics endpoint at `/metrics`
- Health endpoints (`/health`, `/health/live`, `/health/ready`, `/health/detail`)
- Unified exception handling middleware

## Data Storage
- `audit_logs` table for audit events
- `metric_snapshots` table for captured metrics summaries
- Indexes optimized for user, time, action, and correlation-based investigations
