# Data Model: Task Management

## External identity integration

`UserId` and `TeamId` are opaque, non-empty string identifiers. The existing identity system validates JWTs and supplies the current user, current team membership, and `Administrator` authorization. This module does not create or alter users, teams, memberships, or roles.

## Task

| Field | Type | Rules |
|---|---|---|
| `id` | UUID | Primary key, server-generated. |
| `team_id` | text | Required; derived from authorized team context; immutable. |
| `title` | text | Required after trimming; whitespace-only is invalid. |
| `description` | text nullable | Optional. |
| `due_date` | date nullable | Optional; past dates are valid and represent overdue work. |
| `creator_id` | text | Required; authenticated creator; immutable. |
| `assignee_id` | text | Required; must be a current member of `team_id` at create/detail-update time. |
| `status` | enum/text | Required: exactly `BACKLOG`, `IN_PROGRESS`, `IN_REVIEW`, or `DONE`; initial value is `BACKLOG`. |
| `created_at` | timestamptz | Server-generated, UTC, immutable. |
| `updated_at` | timestamptz | Server-generated, UTC. |
| `version` | bigint | Required optimistic-concurrency token; increments on each task mutation. |

Indexes: `(team_id, status)`, `(team_id, assignee_id)`, and `(team_id, status, assignee_id)` support authorized list queries. The primary key supports details and comment joins.

## Comment

| Field | Type | Rules |
|---|---|---|
| `id` | UUID | Primary key, server-generated. |
| `task_id` | UUID | Required foreign key to `Task`; `ON DELETE CASCADE`. |
| `author_id` | text | Required authenticated member of the task's team. |
| `body` | text | Required after trimming; whitespace-only is invalid. |
| `created_at` | timestamptz | Server-generated, UTC, immutable. |

Index: `(task_id, created_at)` supports chronological task discussion retrieval.

## Relationships and lifecycle

- A task belongs to one team, has one creator, and has one current assignee.
- A task has zero or more comments.
- Deleting a task permanently deletes its comments in the same database transaction through the foreign-key cascade.
- Allowed status values are fixed; any valid status may follow any other valid status. No recurring-task or archived state exists.
- Detail edits and status edits use the supplied `version`. A mismatch causes no persisted change and returns a refresh/retry error.
