# Repository Guidelines

## Project Structure & Module Organization
- `Application/Api` hosts the ASP.NET Core minimal API (entry point in `Program.cs`).
- `Modules/*` contains domain modules (`Assets`, `Auth`, `Notification`, `Request`) as class libraries.
- `Shared/Shared` holds shared building blocks (e.g., DDD helpers) used across modules.
- Build artifacts live under `bin/` and `obj/`; do not edit these files.

## Build, Test, and Development Commands
- `dotnet restore`  
  Restore NuGet packages for all referenced projects.
- `dotnet build Application/Api/Api.csproj`  
  Build the API host and its module dependencies.
- `dotnet run --project Application/Api/Api.csproj`  
  Run the API locally (currently exposes `GET /` returning "Hello World!").
- Configuration is in `Application/Api/appsettings.json` and `appsettings.Development.json`.

## Coding Style & Naming Conventions
- Language: C# (.NET 10). Projects enable `Nullable` and `ImplicitUsings`.
- Indentation: 4 spaces; braces on new lines per default C# style.
- Naming: PascalCase for types/methods/properties; camelCase for locals/parameters.
- File organization: one public type per file; file name matches the main type.
- Keep module-specific code inside its module folder; place cross-cutting code in `Shared/Shared`.

## Testing Guidelines
- No test projects are present yet. If you add tests, place them under a top-level `tests/`
  folder (e.g., `tests/Api.Tests`) and document the new `dotnet test <path>` command here.

## Commit & Pull Request Guidelines
- Commit messages in history are short and imperative (e.g., “Add initial project structure...”).
  Follow the same pattern: `<Verb> <object>`.
- PRs should include: a concise summary, relevant module paths touched, and how you verified
  changes (build/run commands). For API behavior changes, include a short request example.

## Security & Configuration Tips
- Do not commit secrets to `appsettings*.json`. Use environment variables or user secrets for
  local development credentials.
