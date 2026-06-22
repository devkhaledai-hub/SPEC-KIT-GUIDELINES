# Tasks: Task Management

**Input**: Design documents from `/specs/001-task-management/`

**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/openapi.yaml](./contracts/openapi.yaml), and [quickstart.md](./quickstart.md)

**Tests**: Tests are required by the constitution and the feature request. Write each story's contract and integration tests first; confirm they fail before the related implementation task begins.

**Organization**: Tasks are grouped by user story. Foundation provides the project, data access, authenticated request context, shared error envelope, and health probe needed by all stories.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the .NET 9 API, test project, reproducible local container stack, and safe configuration surface.

- [ ] T001 Create the .NET 9 solution and API project in `TaskManagement.sln` and `src/TaskManagement.Api/TaskManagement.Api.csproj`
- [ ] T002 [P] Create the xUnit integration, contract, and unit test project in `tests/TaskManagement.Api.Tests/TaskManagement.Api.Tests.csproj`
- [ ] T003 [P] Add ASP.NET Core, JWT bearer, EF Core 9, Npgsql, OpenAPI, xUnit, WebApplicationFactory, and PostgreSQL test dependencies in `src/TaskManagement.Api/TaskManagement.Api.csproj` and `tests/TaskManagement.Api.Tests/TaskManagement.Api.Tests.csproj`
- [ ] T004 [P] Define variable names only and ignore local secrets in `.env.example`, `.gitignore`, and `src/TaskManagement.Api/appsettings.json`
- [ ] T005 [P] Add the multi-stage API image and PostgreSQL 16 local development services with health checks in `Dockerfile` and `docker-compose.yml`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implement the infrastructure every protected API operation needs.

**⚠️ CRITICAL**: Complete this phase before beginning user-story endpoint work.

- [ ] T006 Create the `TaskStatus` enum and `TaskItem` aggregate with timestamps and optimistic version fields in `src/TaskManagement.Api/Domain/TaskStatus.cs` and `src/TaskManagement.Api/Domain/TaskItem.cs`
- [ ] T007 Create the EF Core DbContext and task mapping with team/status/assignee indexes in `src/TaskManagement.Api/Data/TaskManagementDbContext.cs` and `src/TaskManagement.Api/Data/Configurations/TaskItemConfiguration.cs`
- [ ] T008 Create and verify the initial task schema migration in `src/TaskManagement.Api/Data/Migrations/*_CreateTasks.cs`
- [ ] T009 [P] Define the shared API error DTO, error codes, and safe problem-details factory in `src/TaskManagement.Api/Contracts/ErrorResponse.cs` and `src/TaskManagement.Api/Endpoints/ApiErrorResults.cs`
- [ ] T010 Implement centralized validation, exception, and EF concurrency-to-error middleware in `src/TaskManagement.Api/Endpoints/ExceptionHandlingMiddleware.cs`
- [ ] T011 [P] Implement the current principal abstraction that reads opaque subject, team, and administrator claims in `src/TaskManagement.Api/Authorization/CurrentTeamPrincipal.cs`
- [ ] T012 Configure JWT bearer authentication to validate signature, issuer, audience, and expiry from environment-backed configuration in `src/TaskManagement.Api/Program.cs`
- [ ] T013 Implement reusable team-member, creator-or-admin, and creator-assignee-or-admin authorization handlers in `src/TaskManagement.Api/Authorization/TaskAuthorizationHandlers.cs`
- [ ] T014 Configure endpoint routing, global bearer authorization, OpenAPI bearer security, DbContext registration, and anonymous health routing in `src/TaskManagement.Api/Program.cs`
- [ ] T015 [P] Create a test host with a test-only authentication handler and PostgreSQL-backed database fixture without committed tokens or signing keys in `tests/TaskManagement.Api.Tests/Integration/TaskManagementWebApplicationFactory.cs` and `tests/TaskManagement.Api.Tests/Integration/PostgresFixture.cs`
- [ ] T016 [P] Add health contract tests for anonymous `GET /health` success and PostgreSQL-unavailable `503` behavior in `tests/TaskManagement.Api.Tests/Integration/HealthEndpointTests.cs`
- [ ] T017 Verify the foundation's error envelope, invalid/missing bearer-token `401`, and forbidden `403` contract behavior in `tests/TaskManagement.Api.Tests/Contract/FoundationContractTests.cs`

**Checkpoint**: Foundation supports safe authenticated requests, consistent errors, migrations, and health checks.

---

## Phase 3: User Story 1 - Create and organize a task (Priority: P1) 🎯 MVP

**Goal**: A team member creates a task in Backlog, views it, updates permitted details, and moves it through valid statuses.

**Independent Test**: Create a task with a same-team assignee, retrieve it, and change it from Backlog to In Progress; confirm its assignee, status, details, and incremented version. Validate omitted title, foreign-team assignee, stale version, and unauthorized mutations fail without persisting changes.

### Tests for User Story 1 (REQUIRED)

- [ ] T018 [P] [US1] Write create, read, and detail-update OpenAPI contract tests for `POST`/`GET`/`PATCH /api/tasks` in `tests/TaskManagement.Api.Tests/Contract/TaskCommandsContractTests.cs`
- [ ] T019 [P] [US1] Write status-change and stale-version `409 TASK_VERSION_CONFLICT` contract tests in `tests/TaskManagement.Api.Tests/Contract/TaskStatusContractTests.cs`
- [ ] T020 [P] [US1] Write successful create-without-due-date and assignee status-change integration tests in `tests/TaskManagement.Api.Tests/Integration/TaskLifecycleTests.cs`
- [ ] T021 [P] [US1] Write invalid title, invalid assignee, creator/admin detail authorization, creator/assignee/admin status authorization, and concurrent update integration tests in `tests/TaskManagement.Api.Tests/Integration/TaskAuthorizationAndValidationTests.cs`

### Implementation for User Story 1

- [ ] T022 [P] [US1] Create task command and response DTOs matching the documented contract in `src/TaskManagement.Api/Contracts/Tasks/CreateTaskRequest.cs`, `src/TaskManagement.Api/Contracts/Tasks/UpdateTaskRequest.cs`, and `src/TaskManagement.Api/Contracts/Tasks/TaskResponse.cs`
- [ ] T023 [P] [US1] Implement same-team assignee validation and task create/detail-update/status business rules in `src/TaskManagement.Api/Domain/TaskCommandService.cs`
- [ ] T024 [US1] Implement atomic version-checked task updates and conflict mapping in `src/TaskManagement.Api/Data/TaskRepository.cs`
- [ ] T025 [US1] Map create, get-detail, detail-update, and status-update endpoints with their documented response codes in `src/TaskManagement.Api/Endpoints/TaskCommandEndpoints.cs`
- [ ] T026 [US1] Run the User Story 1 contract and integration suites and correct deviations in `tests/TaskManagement.Api.Tests/Contract/TaskCommandsContractTests.cs` and `tests/TaskManagement.Api.Tests/Integration/TaskLifecycleTests.cs`

**Checkpoint**: User Story 1 is independently usable and validates FR-001 through FR-006, including concurrency behavior.

---

## Phase 4: User Story 2 - Find assigned and status-specific work (Priority: P2)

**Goal**: A team member filters their team's tasks by status, assignee, or both and receives a clear empty result when nothing matches.

**Independent Test**: Seed multiple teams, statuses, and assignees; each individual and combined filter returns only same-team records matching every requested filter, while a no-match request returns `200`, empty `items`, and the active filters.

### Tests for User Story 2 (REQUIRED)

- [ ] T027 [P] [US2] Write task-list OpenAPI contract tests for status, assignee, combined, invalid-status, and empty-result queries in `tests/TaskManagement.Api.Tests/Contract/TaskQueryContractTests.cs`
- [ ] T028 [P] [US2] Write team-isolation and filter-intersection integration tests in `tests/TaskManagement.Api.Tests/Integration/TaskFilteringTests.cs`

### Implementation for User Story 2

- [ ] T029 [P] [US2] Create task-list filter and response DTOs that retain active filters in `src/TaskManagement.Api/Contracts/Tasks/ListTasksRequest.cs` and `src/TaskManagement.Api/Contracts/Tasks/TaskListResponse.cs`
- [ ] T030 [US2] Implement authorized team-scoped status/assignee query composition in `src/TaskManagement.Api/Domain/TaskQueryService.cs`
- [ ] T031 [US2] Implement indexed task-list endpoint and empty-state response in `src/TaskManagement.Api/Endpoints/TaskQueryEndpoints.cs`
- [ ] T032 [US2] Run the User Story 2 contract and integration suites and correct deviations in `tests/TaskManagement.Api.Tests/Contract/TaskQueryContractTests.cs` and `tests/TaskManagement.Api.Tests/Integration/TaskFilteringTests.cs`

**Checkpoint**: User Stories 1 and 2 work independently, with team isolation and all documented filter combinations.

---

## Phase 5: User Story 3 - Discuss task work (Priority: P3)

**Goal**: Every authenticated member of the task's team can add and view a non-empty task comment.

**Independent Test**: A team member posts a comment to an existing team task and another member retrieves it with its author and UTC posting time; empty text and a cross-team attempt fail without a persisted comment.

### Tests for User Story 3 (REQUIRED)

- [ ] T033 [P] [US3] Write comment-create OpenAPI contract tests for success, empty body, unauthenticated, forbidden, and missing-task responses in `tests/TaskManagement.Api.Tests/Contract/CommentContractTests.cs`
- [ ] T034 [P] [US3] Write comment author/time visibility and team-membership integration tests in `tests/TaskManagement.Api.Tests/Integration/TaskCommentTests.cs`

### Implementation for User Story 3

- [ ] T035 [P] [US3] Create the comment aggregate and EF Core cascade-delete configuration in `src/TaskManagement.Api/Domain/TaskComment.cs` and `src/TaskManagement.Api/Data/Configurations/TaskCommentConfiguration.cs`
- [ ] T036 [US3] Create and verify the comment-table cascade-delete migration in `src/TaskManagement.Api/Data/Migrations/*_CreateTaskComments.cs`
- [ ] T037 [P] [US3] Create comment command and response DTOs in `src/TaskManagement.Api/Contracts/Comments/CreateCommentRequest.cs` and `src/TaskManagement.Api/Contracts/Comments/CommentResponse.cs`
- [ ] T038 [US3] Implement team-authorized comment creation and chronological detail loading in `src/TaskManagement.Api/Domain/TaskCommentService.cs`
- [ ] T039 [US3] Map the comment-create endpoint and include comments in task detail responses in `src/TaskManagement.Api/Endpoints/TaskCommentEndpoints.cs` and `src/TaskManagement.Api/Endpoints/TaskCommandEndpoints.cs`
- [ ] T040 [US3] Run the User Story 3 contract and integration suites and correct deviations in `tests/TaskManagement.Api.Tests/Contract/CommentContractTests.cs` and `tests/TaskManagement.Api.Tests/Integration/TaskCommentTests.cs`

**Checkpoint**: User Stories 1–3 are independently functional, including team-scoped discussions.

---

## Phase 6: User Story 4 - Remove an authorized task (Priority: P3)

**Goal**: Creators delete their own tasks, administrators delete any team task, and all other members are denied; deletion permanently removes comments.

**Independent Test**: Delete a task with comments as its creator and as an administrator, then confirm task and comments cannot be retrieved or filtered. Attempt deletion as another non-administrator and confirm `403` with unchanged data.

### Tests for User Story 4 (REQUIRED)

- [ ] T041 [P] [US4] Write delete-task OpenAPI contract tests for `204`, `401`, `403`, and `404` responses in `tests/TaskManagement.Api.Tests/Contract/TaskDeletionContractTests.cs`
- [ ] T042 [P] [US4] Write creator/admin deletion, unauthorized preservation, cascade deletion, and filter exclusion integration tests in `tests/TaskManagement.Api.Tests/Integration/TaskDeletionTests.cs`

### Implementation for User Story 4

- [ ] T043 [US4] Implement creator-or-administrator task deletion with a transaction-safe not-found/forbidden distinction in `src/TaskManagement.Api/Domain/TaskDeletionService.cs`
- [ ] T044 [US4] Map permanent task deletion and shared error responses in `src/TaskManagement.Api/Endpoints/TaskDeletionEndpoints.cs`
- [ ] T045 [US4] Run the User Story 4 contract and integration suites and correct deviations in `tests/TaskManagement.Api.Tests/Contract/TaskDeletionContractTests.cs` and `tests/TaskManagement.Api.Tests/Integration/TaskDeletionTests.cs`

**Checkpoint**: All specified user stories are independently functional and destructive operations meet the authorization and cascade-delete requirements.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Verify the complete service against operational, security, contract, and performance requirements.

- [ ] T046 [P] Add unit tests for task validation, role decisions, version conflict mapping, and no-recurring-task enforcement in `tests/TaskManagement.Api.Tests/Unit/TaskCommandServiceTests.cs` and `tests/TaskManagement.Api.Tests/Unit/TaskAuthorizationHandlerTests.cs`
- [ ] T047 [P] Add a Docker Compose smoke test for startup ordering, health probes, and migration application in `tests/TaskManagement.Api.Tests/Integration/DockerComposeSmokeTests.cs`
- [ ] T048 [P] Add repeatable p95 filter and status-change budget tests using representative seeded data in `tests/TaskManagement.Api.Tests/Integration/TaskPerformanceTests.cs`
- [ ] T049 Verify query plans use the documented team/status/assignee indexes and remediate regressions in `src/TaskManagement.Api/Data/Configurations/TaskItemConfiguration.cs`
- [ ] T050 Verify every operation conforms to `specs/001-task-management/contracts/openapi.yaml` and update implementation-only OpenAPI metadata in `src/TaskManagement.Api/Program.cs`
- [ ] T051 Verify no secrets, connection strings, tokens, private keys, or sensitive diagnostics are tracked or logged in `.gitignore`, `.env.example`, `docker-compose.yml`, and `src/TaskManagement.Api/Program.cs`
- [ ] T052 Execute the documented end-to-end and performance validation steps and record the results in `specs/001-task-management/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: starts immediately.
- **Foundational (Phase 2)**: depends on setup and blocks endpoint work.
- **US1 (Phase 3)**: depends on Phase 2; establishes the MVP task command/detail capability.
- **US2 (Phase 4)**: depends on Phase 2 and can be developed in parallel with US1; it uses the common task schema, not US1 endpoints.
- **US3 (Phase 5)**: depends on Phase 2 and task-detail support; adding comments is independently testable with seeded tasks.
- **US4 (Phase 6)**: depends on Phase 2 and task/comment persistence; deletion is independently testable with seeded tasks.
- **Polish (Phase 7)**: depends on all desired stories.

### User Story Completion Graph

```text
Setup → Foundation → US1 (MVP) → Polish
                   ├→ US2 → Polish
                   ├→ US3 → US4 → Polish
                   └→ US4 → Polish
```

### Parallel Opportunities

- Setup tasks T002–T005 can proceed in parallel after T001 establishes the solution.
- Foundational tasks T009, T011, T015, and T016 are file-isolated after their listed prerequisites.
- After Phase 2, test-first work for US1–US4 can be assigned independently where seeded fixtures are available.
- Within each story, tasks carrying `[P]` modify distinct files and can proceed concurrently.

## Parallel Examples

### User Story 1

```text
T018 Contract command tests
T019 Contract status tests
T020 Lifecycle integration tests
T021 Authorization and validation integration tests
```

### User Story 2

```text
T027 Filter contract tests
T028 Filter integration tests
T029 Query DTOs
```

### User Story 3

```text
T033 Comment contract tests
T034 Comment integration tests
T035 Comment model/configuration
T037 Comment DTOs
```

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational work.
2. Complete US1 through T026.
3. Run its contract and integration tests against PostgreSQL.
4. Demo create, detail edit, assignee status update, and stale-write handling before expanding scope.

### Incremental Delivery

1. Add US2 filtering after the MVP to make existing task data usable at scale.
2. Add US3 comments and then US4 deletion/cascade behavior.
3. Complete Phase 7 before merge, including `/health`, configuration, contract, and p95 performance evidence.

## Format Validation

All 52 tasks use the required checkbox, sequential task ID, optional `[P]` marker, required user-story label for story tasks, and at least one exact file path.
