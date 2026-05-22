# Repository Guidelines

## Project Structure & Module Organization
The repository is split into `client/` and `Server/`. `client/` is a Next.js 16 app using the App Router; UI lives in `app/`, shared components in `components/`, helpers in `lib/`, and static assets in `public/`. `Server/` is a .NET 10 modular monolith: `Application/Api/` is the host, `Modules/` contains the `Auth`, `Asset`, `Parameter`, and `Request` domains, and `Shared/` holds reusable CQRS, DDD, data, and messaging code. Utility scripts live in `scripts/`, including EF migration helpers.

## Build, Test, and Development Commands
- `docker compose up -d`: start PostgreSQL on `localhost:5433` and RabbitMQ on `localhost:5672`.
- `dotnet restore Server.sln`: restore backend dependencies.
- `dotnet build Server.sln`: build all server projects.
- `dotnet run --project Server/Application/Api/Api.csproj`: run the API locally.
- `powershell -ExecutionPolicy Bypass -File scripts/ef-migrations.ps1 -Action update -Context all -NoBuild`: apply all EF Core migrations.
- `cd client; npm install`: install frontend dependencies.
- `cd client; npm run dev`: start the Next.js dev server.
- `cd client; npm run build` and `npm run lint`: verify production build and ESLint rules.

## Coding Style & Naming Conventions
Use 4 spaces for C# and match the existing TypeScript style in `client/`. Keep one public C# type per file. Use `PascalCase` for C# types, handlers, DTOs, and feature folders; use `camelCase` for local variables and function parameters. In the client, React components and context files use `PascalCase`, hooks use `use-*.ts` or `use*.ts`, and route files follow Next.js conventions like `page.tsx` and `layout.tsx`.

## Testing Guidelines
There are no committed test projects yet. Until backend and frontend test suites are added, treat `dotnet build Server.sln`, `cd client && npm run build`, and `cd client && npm run lint` as the minimum verification set. When adding tests, place them in a dedicated test project or frontend test folder and document the command in this file.

## Commit & Pull Request Guidelines
Recent history follows short, imperative summaries, often Conventional Commit style such as `feat(auth): require RoleCode...`. Prefer `type(scope): summary` when practical. Keep PRs focused, describe behavior changes, list verification commands, link the issue if one exists, and include screenshots for `client/` UI changes. Do not overwrite unrelated local changes already present in the worktree.

## Security & Configuration Tips
Never commit secrets in `.env` or `appsettings*.json`. Copy from `.env.example`, use Docker for local services, and store backend secrets with `dotnet user-secrets` for `Server/Application/Api/Api.csproj`.
