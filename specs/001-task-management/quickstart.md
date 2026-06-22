# Quickstart: Task Management API Validation

## Prerequisites

- .NET SDK 9
- Docker Engine with Docker Compose v2
- A JWT issuer/test authority that issues an access token containing the existing subject, team-membership, and administrator claims expected by the service

## Configure and run

1. Copy `.env.example` to `.env` and set non-secret local values. Supply `ConnectionStrings__TaskManagement`, `Jwt__Authority`, and `Jwt__Audience` through environment variables or an untracked local secret file. Do not put signing keys or connection-string passwords in Git.
2. Start the stack: `docker compose up --build`.
3. Wait until `GET http://localhost:8080/health` returns `200`; it requires no token and must not return database credentials or topology.
4. Run automated validation: `dotnet test`.

## End-to-end checks

Use the documented requests in [contracts/openapi.yaml](./contracts/openapi.yaml) and identities for at least two members in the same team plus one administrator.

1. Create a task as a team member, then confirm it is `BACKLOG`, has the selected assignee, and includes a version.
2. Change its status as the assignee; submit the original version a second time and confirm `409 TASK_VERSION_CONFLICT` with no lost update.
3. Update task details as creator and administrator; verify a non-creator/non-admin receives `403 TASK_DETAIL_UPDATE_FORBIDDEN`.
4. Filter by status, assignee, and both. Verify every returned task belongs to the caller's team and matches all selected filters. Verify an empty result is `200` with an empty collection and active filters.
5. Add a non-empty comment as a team member; verify its author and UTC posting time. Submit whitespace-only text and verify `400 COMMENT_BODY_REQUIRED`.
6. Delete as creator and as administrator; verify the task and comments are permanently unavailable. Verify a different non-administrator receives `403 TASK_DELETE_FORBIDDEN` and leaves data unchanged.

## Performance validation

Seed representative tasks across statuses and assignees for one team, run repeated authorized list-filter and status-update requests, and record p95 client-observed time. Both scenarios must be at or below 2 seconds; investigate index/query-plan regressions before merge.
