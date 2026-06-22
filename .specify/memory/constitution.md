<!--
Sync Impact Report
- Version change: template/unversioned -> 1.0.0
- Modified principles: none (initial adoption)
- Added sections: Core Principles; Engineering Constraints; Delivery Workflow; Governance
- Removed sections: none
- Templates requiring updates:
  - ✅ updated: .specify/templates/plan-template.md
  - ✅ updated: .specify/templates/spec-template.md
  - ✅ updated: .specify/templates/tasks-template.md
  - ✅ reviewed, no change needed: .specify/templates/commands/ (directory absent)
  - ✅ reviewed, no change needed: AGENTS.md and runtime guidance (no README.md or docs/quickstart.md)
- Follow-up TODOs: none
-->
# my-project Constitution

## Core Principles

### I. Secure Configuration
Secrets, credentials, tokens, private keys, and connection strings MUST NOT be committed to
source code, configuration tracked by Git, test fixtures, logs, or documentation. Runtime
configuration containing sensitive values MUST be supplied through environment variables or an
approved secret-management system. Features that introduce configuration MUST document required
variable names without disclosing values. This prevents accidental disclosure and supports safe
environment-specific deployment.

### II. Feature-Branch Delivery
All implementation work MUST be performed on a named feature branch; direct work on a protected,
default, or release branch is prohibited. A feature branch MUST be created before code changes and
merged only after the applicable quality gates pass. This preserves reviewability, rollback safety,
and an auditable change history.

### III. Consistent API Errors
Every API endpoint MUST return errors using the project's shared error response schema, including a
stable machine-readable error code, a human-readable message, and request-relevant details that do
not expose sensitive information. Endpoints MUST use appropriate HTTP status codes and document
their failure responses. This gives clients a predictable integration contract.

### IV. Health and Observability
Every backend service MUST expose an unauthenticated `GET /health` endpoint that reports service
availability without leaking secrets or internal topology. Health checks MUST be covered by tests
and remain lightweight enough for orchestration probes. This makes service availability observable
and operational failures detectable.

### V. Verified Feature Behavior
Every feature MUST include automated tests for its successful behavior and its expected failure
behavior. Tests MUST exercise the relevant public contract or user-visible outcome, and regression
tests MUST accompany defect fixes where feasible. This protects both primary flows and error
handling from silent regressions.

### VI. Core-Flow Performance Protection
Changes MUST NOT significantly slow down established core user flows. Features that can affect
latency, throughput, resource consumption, or rendering responsiveness MUST define a relevant
baseline or budget and validate that the change remains within it. Any unavoidable regression
requires documented approval and a mitigation plan. This keeps incremental delivery from eroding
the product experience.

## Engineering Constraints

Implementation plans and API contracts MUST identify the configuration variables, error schema,
health-check behavior, tests, and performance impact that apply to the feature. Sensitive values
MUST be redacted in diagnostics. Shared API error handling MUST be implemented centrally where
practical so that all endpoints conform consistently. Backend health responses MUST be safe to call
from deployment and monitoring infrastructure.

## Delivery Workflow

Before implementation, confirm that the working branch is a feature branch and complete the
Constitution Check in the feature plan. Each feature's specification MUST define success and failure
acceptance scenarios. Tasks MUST include implementation and automated verification for each
applicable principle, including API failure contracts and `/health` for backend services. Before
merge, review the resulting tests and performance evidence against the defined baseline or budget.

## Governance

This constitution supersedes conflicting project guidance. Every feature plan, task list, review,
and merge decision MUST verify compliance with its applicable principles. Amendments require a
documented rationale, updates to dependent templates and guidance, and a semantic version bump:
MAJOR for removed or incompatible governance, MINOR for new or materially expanded requirements,
and PATCH for clarifications that do not change obligations. Compliance exceptions require explicit
documentation in the feature plan, approval by the responsible maintainer, and a time-bound
follow-up task.

**Version**: 1.0.0 | **Ratified**: 2026-06-22 | **Last Amended**: 2026-06-22
