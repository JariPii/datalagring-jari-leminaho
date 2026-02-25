# SkillFlow – Education Management Backend

## Overview

SkillFlow is a backend system built with C# and .NET for managing data storage for an education company. It handles courses, course instances, attendees, teachers, and enrollments using a relational database.

This project was developed as part of a Data Storage course assignment and focuses on backend architecture, database design, and domain modeling.

A separate Next.js frontend is used to interact with this API.

Frontend repository:  
https://github.com/JariPii/datalagring-frontend-jari-leminho

---

## Architecture

This project follows Clean Architecture and Domain-Driven Design (DDD).

### Layers

```
SkillFlow.Domain
SkillFlow.Application
SkillFlow.Infrastructure
SkillFlow.Presentation
SkillFlow.Tests
```

### Responsibilities

Domain  
Contains entities, Value Objects, and domain rules. Independent of frameworks.

Application  
Contains use cases, interfaces, and request validation using FluentValidation.

Infrastructure  
Handles database access using Entity Framework Core and SQL Server.

Presentation  
ASP.NET Core Minimal API exposing HTTP endpoints.

Tests  
Contains automated tests verifying system functionality.

### Dependency Flow

```
Presentation → Application → Domain
Infrastructure → Application → Domain
```

---

## Domain Modeling (Value Objects)

The Domain layer uses Value Objects to enforce validation and protect domain integrity.

Value Objects are:

- Immutable
- Created through factory methods
- Compared by value
- Responsible for enforcing domain invariants

Example: AttendeeName ensures valid names through normalization, length limits, and character validation.

This prevents invalid data from existing in the system.

---

## Validation

The system uses two levels of validation:

Application layer validation using FluentValidation  
Ensures incoming requests are valid before processing.

Domain layer validation using Value Objects  
Ensures domain rules are always enforced.

This approach ensures data integrity and separation of concerns.

---

## Database

The system uses SQL Server LocalDB with Entity Framework Core (Code First).

Connection string (Development):

```
Server=(localdb)\mssqllocaldb;Database=SkillFlowDb;Trusted_Connection=True;
```

Entity Framework Core manages:

- Database schema
- Relationships
- Migrations

---

## Migrations

To apply migrations:

```
dotnet ef database update --project SkillFlow.Infrastructure --startup-project SkillFlow.Presentation
```

To create a new migration:

```
dotnet ef migrations add MigrationName --project SkillFlow.Infrastructure --startup-project SkillFlow.Presentation
```

---

## API

The backend is implemented using ASP.NET Core Minimal API.

The API runs on:

```
https://localhost:7110
http://localhost:5031
```

The API handles:

- Courses
- Course instances
- Attendees
- Enrollments

The backend contains all business logic and database interaction.

---

## Running the Project

### Requirements

- .NET 8 SDK
- SQL Server LocalDB
- Git

---

### Clone repository

```
git clone https://github.com/JariPii/datalagring-jari-leminaho.git
cd datalagring-jari-leminaho
```

---

### Run migrations

```
dotnet ef database update --project SkillFlow.Infrastructure --startup-project SkillFlow.Presentation
```

---

### Run API

```
dotnet run --project SkillFlow.Presentation
```

Backend will start at:

```
https://localhost:7110
http://localhost:5031
```

---

## Running Tests

```
dotnet test
```

Tests verify core system behavior.

---

## Technologies Used

Backend:

- C#
- .NET
- ASP.NET Core Minimal API
- Entity Framework Core
- SQL Server LocalDB
- FluentValidation
- Clean Architecture
- Domain-Driven Design

Frontend:

- Next.js
- Tailwind CSS
- shadcn/ui

---

## What I Learned

- Clean Architecture and separation of concerns
- Domain-Driven Design with Value Objects
- Entity Framework Core and migrations
- SQL Server database design
- Building Minimal APIs
- Validating data at both application and domain level
- Structuring scalable backend systems

---

## Author

Jari Leminaho  
https://github.com/JariPii
