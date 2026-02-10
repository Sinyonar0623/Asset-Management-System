# Computer Engineering Asset Management System (CE-AMS) – Project Spec

## Goal
Build a web application for managing department assets with a paperless, role-based workflow:
- Centralized asset data
- Request + approval workflow
- Asset status tracking + audit history
- Notifications (email)

## Users & Roles
- Student: create borrow requests
- Teacher (Lab staff): approve/reject borrow requests; create repair requests; may create procurement/withdraw requests if allowed
- Admin (TA/Staff): manage asset master data; update asset statuses
- Head of Department: approve repair-related and retirement/disposal-related requests

## Core Modules (Modular Monolith)
- Auth: login/logout, cookie auth, roles
- Assets: asset master, asset status, status history
- Request: borrow/repair/retire requests, approvals
- Notification: email notification worker/consumer
- Shared: CQRS contracts, behaviors (validation/logging), common utilities

## Main Workflows
### Borrow (Student)
1) Student submits borrow request (asset(s), time period, reason)
2) Teacher approves/rejects
3) If approved: asset status becomes Borrowed
4) On return: asset status becomes Available (Admin/Teacher action)

### Repair (Teacher)
1) Teacher submits repair request
2) Head approves/rejects
3) If approved: asset status becomes UnderRepair
4) After repair: Admin updates status to Available + record history

### Retire/Dispose (Teacher/Admin)
1) Submit retire request with reason
2) Head approves/rejects
3) If approved: asset status becomes Retired + record history

## Asset Statuses
- Available, Borrowed, UnderRepair, Retired

## Non-Functional Requirements
- Web UI: Next.js
- Backend: .NET 10 + Carter Minimal APIs
- DB: SQL Server (EF Core)
- Messaging: RabbitMQ + MassTransit
- Reliability: Outbox pattern (eventual consistency)
- Deploy: Docker Compose (single machine)
- Security: Cookie auth, role-based authorization, rate limiting for abuse prevention

## Constraints
- Single developer, limited timeline
- Keep architecture simple and maintainable
- Avoid over-engineering (no external IAM like Keycloak unless required)
