# Asset-Management-System

Modular .NET 10 solution with a minimal API host, feature modules, and shared CQRS/DDD building blocks.

## Solution Structure

```text
Asset-Management-System/
|-- Server.sln
|-- README.md
|-- Server/
|   |-- Application/
|   |   |-- Api/
|   |       |-- Api.csproj
|   |       |-- Program.cs
|   |       |-- appsettings.json
|   |       |-- appsettings.Development.json
|   |       |-- Properties/
|   |           |-- launchSettings.json
|   |-- Modules/
|   |   |-- Assets/
|   |   |   |-- Assets.csproj
|   |   |   |-- Class1.cs
|   |   |-- Auth/
|   |   |   |-- Auth.csproj
|   |   |   |-- Auth.cs
|   |   |   |-- GlobalUsing.cs
|   |   |   |-- Data/ (empty placeholder)
|   |   |-- Notification/
|   |   |   |-- Notification.csproj
|   |   |   |-- Class1.cs
|   |   |-- Request/
|   |       |-- Request.csproj
|   |       |-- Class1.cs
|   |-- Shared/
|       |-- Shared/
|           |-- Shared.csproj
|           |-- CQRS/
|           |   |-- ICommand.cs
|           |   |-- ICommandHandler.cs
|           |   |-- IQuery.cs
|           |   |-- IQueryHandler.cs
|           |-- DDD/
|           |   |-- Aggregate.cs
|           |   |-- Entity.cs
|           |   |-- IAggregate.cs
|           |   |-- IEntity.cs
|           |   |-- IDomainEvent.cs
|           |-- Data/
|           |   |-- Extensions/
|           |       |-- MigrationExtension.cs
|           |-- Extensions/
|               |-- CarterExtensions.cs
|               |-- MediatRExtensions.cs
```

## Architecture Notes

- `Application/Api` is the minimal API host. `Program.cs` wires Carter and MediatR using the `Auth` module assembly.
- `Modules/*` are domain-focused class libraries: `Assets`, `Auth`, `Notification`, `Request`.
- `Shared/Shared` contains cross-cutting building blocks: CQRS interfaces, DDD base types, EF Core migration helper, and DI extensions for Carter/MediatR.

## Build and Run

```powershell
dotnet restore
dotnet build Server/Application/Api/Api.csproj
dotnet run --project Server/Application/Api/Api.csproj
```

## Configuration

- `Server/Application/Api/appsettings.json`
- `Server/Application/Api/appsettings.Development.json`
