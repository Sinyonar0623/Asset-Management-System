# Codex Instructions (Repository Rules)

## Tech
- .NET 10, Carter Minimal APIs
- MediatR CQRS + FluentValidation (pipeline behaviors)
- EF Core + SQL Server
- RabbitMQ + MassTransit
- Modular Monolith: Modules/* + Shared/*

## Coding Rules
- Do not introduce MVC Controllers unless asked; prefer Carter modules.
- Keep changes minimal; avoid rewriting project structure.
- Every public endpoint must be authorized by default; allow anonymous only for /auth/login and health.
- Add rate limiting policies: global and login-specific.
- Follow existing folder/module conventions.
- Add/modify tests only if asked.

## Output Rules
- Provide file-by-file changes and explain what was changed.
- Do not change dependencies unless necessary.