# Examprep

An exam preparation platform built with ASP.NET Core. The project includes a REST API and an ASP.NET Core MVC web application for authentication, question management, and admin operations.

## Features

* User registration and login
* JWT access and refresh token authentication
* Role-based authorization (Admin/User)
* Question CRUD
* Question search, filtering, sorting, and pagination
* ASP.NET Core MVC web interface
* API versioning
* Rate limiting
* CORS
* Response caching
* Global exception handling
* Serilog logging
* Unit testing

## Architecture

The solution is separated into multiple projects:

```text
Examprep
│
├── Examprep.Web
│   └── ASP.NET Core MVC frontend
│
├── Examprep.API
│   └── REST API
│
├── Examprep.Application
│   └── DTOs, services, repository interfaces
│
├── Examprep.Domain
│   └── Domain models
│
├── Examprep.Infrastructure
│   └── EF Core, DbContext, repositories
│
└── Examprep.Tests
    └── Unit tests
```

The MVC application communicates with the API through HTTP. The API handles business logic and database operations.

## Tech Stack

**Backend**

* C#
* ASP.NET Core 8
* Entity Framework Core
* SQL Server
* JWT
* BCrypt
* Serilog

**Frontend**

* ASP.NET Core MVC
* Razor Views
* Bootstrap
* HTML/CSS
* JavaScript

**Testing**

* xUnit
* Moq
* FluentAssertions

## Authentication

The application uses JWT authentication with access and refresh tokens.

```text
MVC
 ↓
API
 ↓
AuthService
 ↓
User Repository
 ↓
SQL Server
```

Passwords are hashed using BCrypt.

Admin and User roles are supported.

## API Endpoints

### Authentication

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
```

### Questions

```text
GET    /api/v1/questions
GET    /api/v1/questions/{id}
GET    /api/v1/questions/search?search=
GET    /api/v1/questions/paged?page=1&pageSize=10

POST   /api/v1/questions
PUT    /api/v1/questions/{id}
DELETE /api/v1/questions/{id}
```

Protected endpoints require authentication. Question deletion requires the Admin role.

## Running Locally

Clone the repository:

```bash
git clone https://github.com/almomen-dev/Examprep.git
cd Examprep
```

Configure the SQL Server connection string and JWT key using local configuration/User Secrets.

Apply migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run --project Examprep.API
```

Run the MVC application:

```bash
dotnet run --project Examprep.Web
```

Swagger is available when the API is running.

## Deployment

The API is deployed on MonsterASP with a hosted SQL Server database.

Production secrets are kept outside the source code.

## Author

**Al Momen**

ASP.NET Core Developer

GitHub: https://github.com/almomen-dev
