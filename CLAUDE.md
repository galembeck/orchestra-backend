# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build
dotnet build
dotnet build -c Release

# Run the API
dotnet run --project API.Public

# EF Core migrations (run from solution root)
dotnet ef migrations add <MigrationName> --project Repository --startup-project API.Public
dotnet ef database update --project Repository --startup-project API.Public
```

No test projects exist yet.

## Architecture

Four-project layered solution (`orchestra-backend.slnx`):

```
API.Public          → ASP.NET Core 10 Web API (controllers, DTOs, middleware, filters, validators)
Domain              → Entities, service interfaces, repository interfaces, exceptions, constants
Repository          → EF Core DbContext + repository implementations (SQL Server)
IoC                 → Dependency injection wiring (NativeInjector.cs), Serilog configuration
```

**Request flow**: Controller → Service (Domain) → Repository (EF Core) → SQL Server

**Key patterns**:
- Repository pattern with generic base (`BaseRepository<T>`)
- Soft deletes via `DeletedAt` on all entities (inherit from `BaseEntity`)
- Audit fields: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`
- DTOs are separate from domain entities; suffixed with `DTO`
- Interfaces prefixed with `I`, registered in `IoC/NativeInjector.cs`
- Custom exceptions: `BusinessException`, `PersistenceException` — caught by `ExceptionMiddleware`

## Authentication

JWT + refresh token flow:
- Tokens stored in HttpOnly Secure cookies
- `AuthAttribute` filter enforces authentication and role checks via `ProfileType` enum (`CLIENT`, `ADMIN`)
- 3-step password recovery with OTP
- `UserSecurityInfo` captures IP, MAC, browser info on each auth event

## Configuration

Settings are bound to the `Settings` record in `Domain/Utils/Constants/Settings.cs`. In development, all config lives in `API.Public/appsettings.Development.json`:

- `ConnectionStrings:ORCHESTRA_DEV` — SQL Server connection
- `Authentication:*` — token expiration, recovery settings
- `JwtSettings:*` — secret, issuer, audience
- `IpRateLimiting:*` — per-endpoint rate limits (endpoint-specific rules already configured)
- `EmailServiceSettings:*` — Resend API token and sender

## Key Libraries

| Purpose | Library |
|---|---|
| ORM | Entity Framework Core 10 (SQL Server) |
| Auth | `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt` |
| Password hashing | `BCrypt.Net-Next` |
| Validation | `FluentValidation` (validators in `API.Public/Validators/`) |
| Background jobs | `Hangfire` + `Hangfire.SqlServer` (dashboard at `/hangfire`) |
| Email | `Resend` |
| Payments | Mercado Pago SDK |
| Rate limiting | `AspNetCoreRateLimit` (IP-based) |
| Security headers | `OwaspHeaders.Core` |
| Logging | `Serilog` → Console + MSSqlServer sinks |
| Real-time | SignalR (`/hubs/notifications`) |
| API docs | Scalar (`/scalar/v1`) |

## Service Registration

All services, repositories, and infrastructure are registered in `IoC/NativeInjector.cs`. When adding a new service or repository, register it there. API.Public extensions (`API.Public/Extensions/`) call into IoC for configuration modules (Database, JWT, Hangfire, RateLimit, etc.).

## CORS

Allowed origins in development: `http://localhost:5173`, `http://localhost:5174`.

## Nullable Reference Types

The `.editorconfig` suppresses CS8600, CS8602, CS8603, CS8604, CS8618, CS8625 — nullable warnings are intentionally silenced project-wide.
