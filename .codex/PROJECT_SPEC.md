# Computer Engineering Asset Management System (CE-AMS) - Project Spec

Last synced with repository: 2026-04-07

## Goal
Build a paperless department asset management system with:
- Centralized asset data
- Request and approval workflows
- Asset status tracking and history
- Notification-ready architecture

## Stack (Current)
- Backend: .NET 10, Carter minimal APIs, MediatR
- Database: EF Core + PostgreSQL (`Npgsql`)
- Messaging: MassTransit + RabbitMQ
- Frontend: Next.js 16 + React 19 (`client`)
- Local infra: Docker Compose (`postgres` + `rabbitmq`)

## Architecture
- Modular monolith under `Server`:
  - `Application/Api` (composition root)
  - `Modules/Auth`
  - `Modules/Asset`
  - `Modules/Parameter`
  - `Modules/Request`
  - `Shared/Shared` and `Shared/Shared.Messaging`

## Current Implementation Snapshot

### Auth Module
- Implemented endpoints:
  - `POST /auth/signup/user`
  - `POST /auth/login`
  - `POST /auth/logout` (authorized)
- JWT auth configured in API host.
- Role seeds exist in Auth migrations:
  - `00 ADMIN`
  - `01 DEPTHEAD`
  - `02 LECTURER`
  - `03 STUDENT`

### Asset Module
- CRUD-style endpoints for:
  - `Laboratory`
  - `Asset`
  - `AssetUnit`
- Data model and migrations are present.

### Parameter Module
- CRUD-style endpoints for `Parameter` and query by group.
- Data model and migrations are present.

### Request Module
- Implemented CRUD-style endpoints for `Request`:
  - create, get(list), update, delete
- Aggregate model includes:
  - request status fields
  - approval step tracking (`RequestTracking`)
  - approval flow setup helper (`SetupApprovalFlow`)
- Approval decision endpoints/workflow orchestration are not completed yet.

### Notification/Integration
- RabbitMQ + MassTransit wiring is present.
- Outbox entities were recently removed from current module schemas.
- Full consumer-driven notification workflow is not completed.

## Security Status (Current vs Target)
- Current:
  - Authentication is configured.
  - Authorization is not enforced globally by fallback policy.
  - Only selected endpoints (notably `/auth/logout`) enforce authorization.
  - Rate limiting is not implemented yet.
  - Health check endpoint is not implemented yet.
- Target:
  - Enforce authorization policy by default for business endpoints.
  - Keep login/signup anonymous only where explicitly required.
  - Add global and login-specific rate limiting.

## Approval Workflow Requirement (Business Rule)
Every request must end with final HOD decision.

### Required Routing
1. Student submits request.
2. Assigned lab teacher reviews first and assigns unit(s)/approved quantity.
3. Teacher approves or rejects.
4. If teacher approves, request goes to HOD.
5. HOD gives final approve/reject.
6. If HOD rejects, request returns to teacher for reconciliation:
   - close as rejected, or
   - adjust and resubmit to HOD.

### Special Case
- If requester is the same teacher responsible for the target lab, teacher step can be skipped and request goes directly to HOD.

## Domain Status Codes In Code
- Request status:
  - `PENDING`, `APPROVED`, `REJECTED`, `CANCELLED`, `COMPLETED`
- Tracking status:
  - `WAITING`, `PENDING`, `APPROVED`, `REJECTED`, `SKIPPED`, `CANCELLED`
- Approver roles:
  - `TEACHER`, `HOD`

## Architecture Decision (Workflow)
- Keep approval logic in Request aggregate + step tracking state transitions.
- Do not introduce Saga/Process Manager unless cross-module async compensation is explicitly needed.

## Constraints
- Single-developer, limited timeline.
- Prefer minimal, maintainable changes.
- Avoid over-engineering unless there is clear operational benefit.
