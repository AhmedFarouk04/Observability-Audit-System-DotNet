# Architecture Decisions

## ADR-001: CQRS with MediatR
Decision: Use MediatR for CQRS.
Reason: Keeps read/write workflows isolated and supports reusable pipeline behaviors.

## ADR-002: Serilog for Structured Logging
Decision: Use Serilog instead of plain ILogger sinks.
Reason: Better structured output, enrichers, and multiple sink support.

## ADR-003: Prometheus for Metrics
Decision: Use `prometheus-net` with `/metrics` scrape endpoint.
Reason: Standard observability ecosystem and simple Grafana integration.

## ADR-004: PostgreSQL
Decision: Use PostgreSQL via EF Core provider.
Reason: Reliable OSS DB with strong indexing and JSON support.

## ADR-005: Health Probes Separation
Decision: Distinct liveness and readiness checks.
Reason: Better orchestration behavior in Kubernetes/containerized deployments.
