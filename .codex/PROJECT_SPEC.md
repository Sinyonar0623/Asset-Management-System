# Computer Engineering Asset Management System (CE-AMS) - Project Spec

## Goal
Build a web application for department asset management with a paperless, role-based workflow:
- Centralized asset data
- Request + approval workflow
- Asset status tracking + audit history
- Notification-ready architecture

## Users and Roles
- Student: create borrow requests
- Teacher (lab owner/staff): review student requests, assign specific asset unit(s), approve/reject at teacher step
- Admin (TA/staff): manage master data, support status updates
- Head of Department (HOD): final approver for every request

## Core Modules (Modular Monolith)
- Auth: signup/login/logout, JWT auth, roles
- Assets: asset master, unit-level statuses, history
- Request: borrow/repair/retire requests, approval flow, tracking
- Notification: email/alert worker and consumers
- Shared: CQRS contracts, behaviors (validation/logging), common utilities

## Approval Policy (Updated Requirement)
Every request must end with HOD final decision.

### Routing Rules
1. Student submits request.
2. Request goes to assigned lab teacher first.
3. Lab teacher must assign asset unit(s)/approved quantity, then approve or reject.
4. If teacher approves, request moves to HOD.
5. HOD performs final approve/reject.
6. If HOD rejects, request returns to lab teacher for reconciliation:
   - close as rejected, or
   - adjust assignment and resubmit to HOD.

### Special Case
- If requester is the same teacher responsible for the target lab, teacher step is skipped and request goes directly to HOD.

## Workflow State Model (Recommended)
Use request-level state + per-step tracking.

### Request Status (top-level)
- `PENDING`
- `IN_REVIEW_TEACHER`
- `IN_REVIEW_HOD`
- `HOD_REJECTED_RETURNED`
- `APPROVED`
- `REJECTED`
- `CANCELLED`
- `COMPLETED`

### Tracking Step Status
- `WAITING`
- `PENDING`
- `APPROVED`
- `REJECTED`
- `SKIPPED`
- `CANCELLED`

## Main Workflows
### Borrow (Student)
1. Student creates borrow request.
2. Teacher reviews and assigns specific asset unit(s).
3. Teacher approves/rejects.
4. If teacher approves, HOD does final approval.
5. If HOD approves, asset status changes to borrowed.
6. If HOD rejects, return to teacher for reconcile-and-resubmit or close rejected.

### Repair (Teacher)
1. Teacher submits repair request.
2. HOD final approves/rejects.
3. If approved, asset status moves to repair flow.

### Retire/Dispose (Teacher or Admin)
1. Submit retirement request.
2. HOD final approves/rejects.
3. If approved, asset status becomes retired and history is recorded.

## Architecture Decision: Saga vs State Machine
- Do not introduce Saga for core approval routing now.
- Implement approval logic as a state machine in `Request` aggregate + `RequestTracking` transitions with optimistic concurrency.
- Consider Saga/Process Manager later only for cross-module asynchronous flows requiring compensation (for example inventory reservation, timeout escalation, external notification guarantees).

## Non-Functional Requirements
- Web UI: Next.js
- Backend: .NET 10 + Carter minimal APIs
- DB: SQL Server (EF Core)
- Messaging: RabbitMQ + MassTransit
- Reliability: Outbox pattern for integration events
- Deploy: Docker Compose (single machine)
- Security: JWT auth, role-based authorization, rate limiting

## Constraints
- Single developer and limited timeline
- Keep architecture simple and maintainable
- Avoid over-engineering unless clear operational need
