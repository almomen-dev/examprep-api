# Examprep API

A REST API for a Q&A exam-prep system built with ASP.NET Core 8.

## Overview

This is a backend API that lets users register, log in, create questions, and search through them. It uses JWT tokens for auth, EF Core for database access, and follows a clean layered structure.

## Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core with SQL Server
- JWT authentication (access + refresh tokens)
- BCrypt for password hashing
- Serilog for logging
- Swagger for API documentation

## Project Structure

The solution has four main projects:

- Examprep.API: controllers, middleware, and app startup
- Examprep.Application: DTOs, services, and repository interfaces
- Examprep.Domain: entity classes
- Examprep.Infrastructure: EF Core DbContext and repository implementations

## Main Features

- User registration and login
- JWT-based authentication with refresh tokens
- Role-based authorization (Admin and User)
- Full CRUD for questions
- Search, filter, sort, and pagination
- Rate limiting and CORS
- API versioning (v1)
- Global exception handling
- Response caching

## Endpoints

Auth:
- POST /api/v1/auth/register
- POST /api/v1/auth/login
- POST /api/v1/auth/refresh

Questions:
- GET /api/v1/questions
- GET /api/v1/questions/{id}
- GET /api/v1/questions/search?search=
- GET /api/v1/questions/paged?page=1&pageSize=10
- POST /api/v1/questions (requires login)
- PUT /api/v1/questions/{id} (requires login)
- DELETE /api/v1/questions/{id} (admin only)

## How to Run

1. Clone the repo
2. Update the connection string in appsettings.json
3. Add the JWT key via user secrets
4. Run dotnet ef database update
5. Run dotnet run --project Examprep.API
6. Open Swagger at https://localhost:7189/swagger

## Testing

1. Register a user with POST /api/v1/auth/register
2. Log in with POST /api/v1/auth/login
3. Copy the access token
4. Click Authorize in Swagger and paste "Bearer TOKEN"
5. Test the protected endpoints

## Notes

Passwords are hashed with BCrypt. The JWT secret key is stored in user secrets locally and should be set as an environment variable in production.

## Author

Al Momen - github.com/almomen-dev
