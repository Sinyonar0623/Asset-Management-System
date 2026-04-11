# Codex Instructions (Repository Rules)

Last synced with repository: 2026-04-07

## Tech And Layout
- Backend: .NET 10 (`net10.0`) modular monolith in `Server`.
- API style: Carter minimal APIs (`ICarterModule`) + MediatR.
- Database: EF Core + PostgreSQL (`Npgsql`), using `ConnectionStrings:Database`.
- Messaging: MassTransit + RabbitMQ.
- Frontend: Next.js app in `client` (not `ClientApp`).
- Main composition root: `Server/Application/Api/Program.cs`.
- Modules: `Server/Modules/{Auth,Asset,Parameter,Request}` with shared libraries in `Server/Shared`.

## Coding Rules
- Prefer Carter modules; do not introduce MVC controllers unless requested.
- Keep route style consistent with existing endpoints (`/auth/*`, `/Asset`, `/AssetUnit`, `/Laboratory`, `/Parameter`, `/Request`).
- Keep changes scoped and minimal; preserve module boundaries and folder conventions.
- Do not change DB provider, messaging stack, or architecture patterns unless explicitly requested.
- Add or modify tests only when requested.

## Security Reality (Current Code)
- JWT authentication is configured globally.
- Authorization is not enforced by default for all endpoints yet.
- `/auth/logout` currently uses `.RequireAuthorization()`.
- Global/login rate limiting is not implemented yet.
- Health check endpoint is not implemented yet.

## Request Workflow Rules
- Business requirement remains: every request ends with final HOD decision.
- Domain currently uses request-level status codes:
  - `PENDING`, `APPROVED`, `REJECTED`, `CANCELLED`, `COMPLETED`
- Tracking step status codes:
  - `WAITING`, `PENDING`, `APPROVED`, `REJECTED`, `SKIPPED`, `CANCELLED`
- Tracking approver roles:
  - `TEACHER`, `HOD`
- Keep approval logic in `Request` aggregate + tracking transitions.
- Do not introduce Saga unless cross-module async compensation is explicitly required.

## Output Rules
- Explain changes file-by-file.
- Do not change dependencies unless necessary.
