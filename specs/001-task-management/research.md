# Research: Task Management

## .NET API and PostgreSQL access

**Decision**: Use an ASP.NET Core 9 REST API with EF Core 9 and the Npgsql PostgreSQL provider. Keep persistence behind an application `DbContext` and apply schema changes through EF Core migrations.

**Rationale**: This is the requested stack and supports strongly typed mappings, migrations, PostgreSQL indexes, transactions, and integration testing against the real database provider.

**Alternatives considered**: Dapper/raw SQL was rejected because the feature needs several related write paths, validations, and migrations; SQLite was rejected because it would not validate PostgreSQL behavior.

## JWT bearer authentication and authorization

**Decision**: Validate bearer access tokens with `Microsoft.AspNetCore.Authentication.JwtBearer`; require valid signature, issuer, audience, and expiration. Resolve the existing user subject, team membership, and administrator role from the authenticated identity integration. The API does not mint access tokens.

**Rationale**: ASP.NET Core guidance requires APIs to fully validate access tokens and distinguishes `401` authentication failures from `403` authorization denials. Reusing existing claims and membership controls honors the feature specification and avoids a duplicate identity store. [Microsoft JWT bearer guidance](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-9.0)

**Alternatives considered**: A local username/password-to-JWT endpoint was rejected because it duplicates identity and is explicitly discouraged for production APIs. A custom opaque-token scheme was rejected because the requested bearer JWT integration already meets the need.

## Optimistic concurrency

**Decision**: Store a monotonically increasing `version` on each task. Read responses include it; task detail and status mutations require it. EF Core conditional updates increment it atomically. A mismatch returns `409 TASK_VERSION_CONFLICT`.

**Rationale**: The specification requires one valid concurrent status update and actionable feedback to the losing caller. An explicit version keeps this API contract portable and easy for clients to use.

**Alternatives considered**: Last-write-wins was rejected because it silently loses changes. PostgreSQL `xmin` was rejected because exposing a provider-internal system column creates an unnecessary external dependency.

## Health checks and local containers

**Decision**: Register ASP.NET Core health checks and expose anonymous `GET /health`; include PostgreSQL readiness in the dependency check. Run the API and PostgreSQL with Docker Compose and configure the API container health check against `/health`.

**Rationale**: ASP.NET Core directly supports health middleware and an endpoint at `/health`; its documentation also covers EF Core database probes and Docker health checks. [Microsoft health-check guidance](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-9.0)

**Alternatives considered**: A protected health endpoint was rejected because the constitution requires anonymous orchestration probes. An in-memory database was rejected because readiness needs to represent the PostgreSQL dependency.

## Identity identifier representation

**Decision**: Treat user and team identifiers as opaque strings at the API boundary and in task records. The existing identity/team system remains the authority for their format, existence, membership, and administrative role.

**Rationale**: The specification explicitly says those controls already exist but does not constrain identifier types. Opaque IDs prevent an incompatible assumption while preserving the team-scoping and authorization rules.

**Alternatives considered**: Assuming UUIDs was rejected because the existing provider might use a different stable subject format. Duplicating users, teams, or memberships locally was rejected as out of scope.
