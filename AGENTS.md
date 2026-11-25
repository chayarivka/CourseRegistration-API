# Project: Course Registration System – Backend

## Project context

This is a final academic project using ASP.NET Core Web API, .NET 8, EF Core and SQL Server.

The system manages courses, categories, users and registrations. The limited resource is the number of available seats in each course.

The API is deployed on Render and uses a cloud SQL Server database. Local development must remain supported.

## Mandatory academic requirements

1. REST endpoints with correct HTTP methods and status codes: 200, 201 with Location, 204, 400, 401, 403, 404 and 409. Swagger must document all endpoints.
2. Model Binding, Data Annotations, ModelState validation and business validation in the Service layer.
3. Four-layer architecture: Core, Data, Service and API. Preserve the required dependency direction. Core must not depend on EF Core.
4. Constructor Dependency Injection, interfaces in Core and appropriate service lifetimes.
5. EF Core Code First, DbContext, DbSets, at least two migrations, configuration-based connection strings and seed data.
6. CRUD, LINQ filtering, sorting, database-level pagination using Skip and Take, and AsNoTracking for read-only queries.
7. One-to-many and many-to-many relationships, Fluent API, efficient Include and ThenInclude where appropriate, and prevention of N+1 queries.
8. Separate request and response DTOs and AutoMapper Profiles. Do not expose entities directly through the API.
9. Asynchronous operations throughout the call chain with CancellationToken propagation.
10. Global JSON exception handling middleware, CorrelationId middleware and correct middleware order.
11. Secure configuration using appsettings, development configuration, User Secrets and environment variables.
12. JWT authentication, registration, login, claims, server-side token validation and actual role-based authorization for Admin and User.
13. xUnit and Moq tests for successful and rejected limited-resource operations.
14. Optimistic concurrency using a concurrency token on the limited resource. Handle DbUpdateConcurrencyException with HTTP 409. Business validation and saving must occur within the same transaction.
15. A concurrency test using two separate DbContexts that demonstrates a successful first save and a rejected second save.
16. NLog.Web.AspNetCore with nlog.config, request logging with CorrelationId, Error logging for exceptions and Warning logging for resource conflicts.
17. A clean Git repository and README explaining the system, limited resource, concurrency mechanism, local setup, required database and safe local demo accounts for both roles.
18. The system must run locally without manual database modifications.

## Existing business rules

- Admin manages courses.
- Regular users can register for courses and cancel registrations.
- Prevent duplicate active registrations.
- Prevent registration when a course is full or has started.
- Allow cancellation only before the course starts.
- Do not delete courses with active registrations.
- Do not reduce capacity below the number of active registrations.
- Do not change course start date or time when active registrations exist.
- Preserve canceled registration handling.
- Do not add payment functionality.
- Preserve existing concurrency protections and business logic.

## Development rules

- Inspect existing implementations before proposing changes.
- Never duplicate existing code or business logic.
- Reuse existing services, repositories, DTOs and helpers.
- Make minimal changes required for academic compliance or approved functionality.
- Do not add unnecessary features, packages or architectural complexity.
- Do not change database providers or upgrade .NET without explicit approval.
- Preserve local and cloud compatibility.
- Never expose, request, log or commit production secrets.
- Do not access or modify the production database.
- Use safe local or isolated test environments.
- Never remove existing functionality without approval.

## Workflow

1. Inspect the relevant code.
2. Explain findings and identify exact files.
3. Propose a minimal implementation plan in Hebrew.
4. Wait for explicit approval before implementing changes.
5. Build and run relevant tests after implementation.
6. Report modified files, test results and remaining limitations.
7. Never claim a test passed unless it was actually executed successfully.
8. Do not commit, push, merge or deploy without explicit approval.
9. Use a separate branch for approved code changes.
