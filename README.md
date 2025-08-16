# CleanArchJwt (.NET 8)

A minimal Clean Architecture starter for a JWT-authenticated Web API.

## Projects
- `Domain` – entities
- `Application` – interfaces, DTOs, validators
- `Infrastructure` – EF Core (SQLite), JWT token generation, auth service
- `WebApi` – API host, Serilog, Swagger, CORS, ProblemDetails

## Features
- JWT access tokens (15 minutes) + refresh tokens
- Role-based auth (`Admin`, `User`)
- Endpoints: Register, Login, Forgot Password, Refresh Token, Get Current User (`/api/users/me`)
- EF Core + SQLite (default)
- Serilog to console and file (`logs/log.txt`)
- FluentValidation for DTOs
- Global exception handling -> RFC 7807 ProblemDetails
- Swagger/OpenAPI
- CORS configured for Angular (http://localhost:4200)

## Quickstart

1. Install EF Core tools:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. Add an initial migration:
   ```bash
   dotnet ef migrations add InitialCreate -p src/CleanArchJwt.Infrastructure -s src/CleanArchJwt.WebApi
   ```

3. Update the database:
   ```bash
   dotnet ef database update -p src/CleanArchJwt.Infrastructure -s src/CleanArchJwt.WebApi
   ```

4. Run the API:
   ```bash
   dotnet run --project src/CleanArchJwt.WebApi
   ```

5. Open Swagger:
   https://localhost:5001/swagger

> **Note**: Change `Jwt:Secret` in `src/CleanArchJwt.WebApi/appsettings.json` before production use.
