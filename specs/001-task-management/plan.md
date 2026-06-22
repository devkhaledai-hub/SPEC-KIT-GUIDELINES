# Implementation Plan: Task Management

**Branch**: `001-task-management` | **Date**: 2026-06-22 | **Spec**: [spec.md](./spec.md)

## Summary

Deliver a team-scoped REST API for creating, updating, filtering, discussing, and permanently deleting tasks. The service will be an ASP.NET Core 9 API backed by PostgreSQL through EF Core, authenticated with validated JWT bearer access tokens, and runnable locally with Docker Compose. It integrates with the existing identity, team-membership, and administrator-role claims rather than creating a new identity system.

## Technical Context

**Language/Version**: C# / .NET 9

**Primary Dependencies**: ASP.NET Core Web API, `Microsoft.AspNetCore.Authentication.JwtBearer`, EF Core 9, `Npgsql.EntityFrameworkCore.PostgreSQL`, OpenAPI support

**Storage**: PostgreSQL 16; EF Core migrations; task and comment tables only. Team membership and role information remain owned by the existing identity system.

**Testing**: xUnit, `Microsoft.AspNetCore.Mvc.Testing`, EF Core/Npgsql integration tests against PostgreSQL in Docker Compose, and `dotnet test`

**Target Platform**: Linux containers; Docker Compose local development

**Project Type**: Web service / REST API

**Performance Goals**: p95 task filtering, including status and assignee filters, completes within 2 seconds under normal supported usage; p95 successful status changes complete within 2 seconds.

**Constraints**: JWT bearer validation must validate signature, issuer, audience, and expiry. Secrets and connection strings are environment variables only. All API failures use the shared error envelope. `GET /health` is anonymous and reports liveness/readiness without sensitive details. Optimistic concurrency prevents silent loss of concurrent task changes.

**Scale/Scope**: One task-management API module for team-scoped tasks, comments, and filters. No recurring tasks, attachments, subtasks, notifications, restoration, or new identity/team-management endpoints.

## Constitution Check

*GATE: Passed before Phase 0 research; re-checked after Phase 1 design.*

- [x] Work is on named feature branch `001-task-management`, not a default, protected, or release branch.
- [x] Required configuration is supplied by environment variables; contracts and Compose use variable names only.
- [x] API changes define a shared error schema, HTTP status codes, and failure contracts in [contracts/openapi.yaml](./contracts/openapi.yaml).
- [x] The API exposes and tests anonymous `GET /health`; Docker Compose uses it for the API health check.
- [x] The test plan covers successful and failure behavior for every requirement.
- [x] The plan defines p95 2-second budgets and load/integration validation for filtering and status changes.
- [x] No exceptions are required.

## Project Structure

```text
src/
└── TaskManagement.Api/
    ├── Contracts/          # request/response DTOs and shared error model
    ├── Data/               # DbContext, EF configurations, migrations
    ├── Domain/              # Task, Comment, TaskStatus
    ├── Endpoints/           # task, comment, and health route mappings
    ├── Authorization/       # membership and task-action policies
    └── Program.cs

tests/
└── TaskManagement.Api.Tests/
    ├── Contract/
    ├── Integration/
    └── Unit/

docker-compose.yml
Dockerfile
```

**Structure Decision**: A single ASP.NET Core API keeps the task module deployable with one data store and one externally documented contract. The existing identity/team system is consumed through JWT claims and its membership abstraction; it is not duplicated here.

## Implementation Approach

1. Bootstrap the API, Npgsql EF Core context, migrations, Dockerfile, and Compose services. Configure `ConnectionStrings__TaskManagement`, JWT authority/audience configuration, and named non-secret defaults in `.env.example`.
2. Map JWT subject, team, and role claims to an authenticated request principal. Add policies for team membership, creator-or-admin detail edits/deletes, and creator-assignee-or-admin status changes. Return `401` for invalid/missing credentials and `403` for authenticated users without the required access.
3. Implement `Task` and `Comment` persistence with team isolation and indexes described in [data-model.md](./data-model.md). Use the task version supplied by the client for updates; a stale version produces `409 TASK_VERSION_CONFLICT` and does not overwrite data.
4. Implement documented task list, detail, create, detail-update, status-update, delete, and comment endpoints. Enforce validation and use the shared error response for all failures.
5. Add health registration and an anonymous `GET /health` endpoint. Compose waits for PostgreSQL readiness, runs API health checks, and never embeds production secrets.
6. Add unit, contract, integration, authorization, concurrency, cascade-delete, and performance tests. Validate p95 filter and status-change operations against a representative seeded team data set.

## Complexity Tracking

No constitution violations or complexity exceptions.
