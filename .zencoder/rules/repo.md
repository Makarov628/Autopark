---
description: Repository Information Overview
alwaysApply: true
---

# Autopark Management System Information

## Summary

The Autopark Management System is a comprehensive vehicle fleet management application designed to help enterprises manage vehicles, drivers, trips, and track vehicle locations. It follows Clean Architecture principles and implements the CQRS pattern with MediatR for business logic organization.

## Structure

- **Domain**: Core business entities, value objects, and domain logic
- **UseCases**: Application logic using CQRS pattern (Commands and Queries)
- **Infrastructure**: Data access and external services integration
- **Web**: API controllers, Razor pages, and React SPA frontend
- **Tests**: xUnit test project for application testing
- **DataGenerator**: Tool for generating test data
- **TrackRealtimeGenerator**: Tool for simulating real-time vehicle tracking

## Language & Runtime

**Backend Language**: C# (.NET)
**Version**: .NET 8.0
**Architecture Pattern**: CQRS with MediatR
**Build System**: MSBuild
**Package Manager**: NuGet

**Frontend Language**: JavaScript/TypeScript (React)
**Version**: React 19.0.0
**Build System**: Vite
**Package Manager**: npm

## Dependencies

### Backend Dependencies

- Entity Framework Core (Database ORM)
- ASP.NET Core MVC
- JWT Bearer Authentication
- MediatR (CQRS implementation)
- Swashbuckle (Swagger/OpenAPI)

### Frontend Dependencies

- React 19.0.0
- React Router 6.28.0
- Zustand (State Management)
- Luxon (Date/Time)
- JWT Decode
- TailwindCSS

## Authentication & Authorization

**Authentication Method**: JWT tokens
**Authorization Roles**:

- Admin: Full system access
- Manager: Enterprise management and user oversight
- Driver: Vehicle and trip access

## API Endpoints

The system provides comprehensive REST API endpoints for:

- System management and configuration
- User authentication and management
- Vehicle tracking and management
- Driver management
- Enterprise administration
- Brand/model management
- Trip tracking and reporting

## Build & Installation

### Backend

```bash
dotnet restore
dotnet build
dotnet run --project Web/Autopark.Web.csproj
```

### Frontend

```bash
cd Web/client-app
npm install
npm run dev
```

## Docker

**Dockerfile**: Dockerfile (root directory)
**Compose**: docker-compose.yml
**Services**:

- autopark: ASP.NET Core application (.NET 8.0)
- mssql: SQL Server 2022 database

**Run Command**:

```bash
docker-compose up -d
```

## Testing

**Framework**: xUnit
**Test Location**: Tests/Autopark.Tests
**Configuration**: Autopark.Tests.csproj
**Run Command**:

```bash
dotnet test
```

## Database

**Provider**: Microsoft SQL Server
**ORM**: Entity Framework Core
**Configuration**: Infrastructure/Database/AutoparkDbContext.cs
**Migrations**: Infrastructure/Database/Migrations
