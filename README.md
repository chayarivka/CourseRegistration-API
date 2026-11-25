# Course Registration System – Backend API

## Project Overview

A course registration management system developed using ASP.NET Core Web API and SQL Server.

The system allows users to browse courses, search and filter courses, register for available courses, and manage their registrations. Administrators can manage the course catalog.

## Technologies

- C# and .NET 8
- ASP.NET Core Web API
- Entity Framework Core – Code First
- SQL Server
- AutoMapper
- JWT Authentication
- Swagger
- NLog
- xUnit and Moq
- Docker
- Git and GitHub
- Render and Somee

## System Architecture

The application follows a four-layer architecture:

- **Core** – Entities, DTOs, enums and repository and service interfaces.
- **Data** – Database context, repositories, entity configurations and migrations.
- **Service** – Business logic and AutoMapper profiles.
- **API** – Controllers, middleware, authentication and dependency injection configuration.

An additional Tests project contains automated tests.

## Database

The database contains four main entities:

- User
- Course
- Category
- Registration

The system implements one-to-many relationships and a many-to-many relationship between users and courses through the Registration entity.

Entity Framework Core migrations manage the database structure. Initial categories are populated using seed data.

## Main Features

### Regular Users

- User registration and login.
- Browse available courses.
- Search, filter, sort and paginate courses.
- Register for courses.
- View active registrations.
- Cancel registrations before a course begins.

### Administrators

- Create courses.
- Update courses.
- Delete courses according to business rules.
- Manage course capacity.

## REST API

The API implements REST principles, DTO validation, model binding and appropriate HTTP status codes:

- 200 – OK
- 201 – Created
- 204 – No Content
- 400 – Bad Request
- 401 – Unauthorized
- 403 – Forbidden
- 404 – Not Found
- 409 – Conflict

Swagger provides interactive API documentation.

## Authentication and Authorization

The system uses JWT authentication and role-based authorization.

Supported roles:

- Admin
- User

Passwords are stored as hashes. Sensitive configuration values are managed through User Secrets during local development and environment variables in production.

## Concurrency and Limited Resources

Course seats are limited resources.

The system prevents duplicate active registrations and registration beyond the maximum course capacity.

Registration and cancellation use a SQL Server transaction with `Serializable` isolation. The service checks the business rules and saves the registration and the change in occupied seats inside the same transaction.

EF Core uses the course's SQL Server `RowVersion` as an optimistic concurrency token. If a conflicting write changes the row version before a save, EF Core raises a concurrency exception and the API returns HTTP 409 Conflict.

## Error Handling and Logging

The application includes centralized exception handling middleware and Correlation ID middleware.

NLog is used for application logging, including request information, errors and concurrency warnings.

Sensitive information must not be included in application logs.

## Automated Testing

The project includes automated tests using xUnit and Moq.

The tests cover:

- Successful registration.
- Registration rejection.
- Limited resource availability.
- Concurrent registration attempts.
- Optimistic concurrency conflicts using separate database contexts.

## Local Setup

Run the commands from the root of this backend repository.

### 1. Build and prepare SQL Server

Install the .NET 8 SDK and start a local SQL Server instance. On Windows, SQL Server LocalDB can use the default connection in `CourseRegistration.API/appsettings.json`, which targets `(localdb)\MSSQLLocalDB` and the database `CourseRegistrationDB`. For another local SQL Server instance, set `ConnectionStrings:DefaultConnection` in the API project's User Secrets. Use a login with permission to create the local database and apply the migrations.

```sh
dotnet build CourseRegistration.API.sln
dotnet tool install --global dotnet-ef --version 8.0.33
```

If `dotnet-ef` is already installed globally, use `dotnet tool update --global dotnet-ef --version 8.0.33` if needed.

### 2. Configure local secrets and apply migrations

Replace each placeholder with a value for your local environment. Generate a JWT signing key of at least 32 bytes. Run the second command only when overriding the default LocalDB connection.

```sh
dotnet user-secrets set "Jwt:Key" "<locally-generated-signing-key>" --project CourseRegistration.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<local-SQL-Server-connection-string>" --project CourseRegistration.API
```

Apply the migrations using the API's Development configuration and local connection:

```sh
dotnet ef database update --project CourseRegistration.Data --startup-project CourseRegistration.API -- --environment Development
```

The migrations create the database schema and insert the initial categories. No manual SQL or table edits are required.

### 3. Configure the local Admin demo account

Choose a local password and replace the placeholder below. The email and display name are examples you can change.

```sh
dotnet user-secrets set "AdminSeed:Email" "admin.demo@example.test" --project CourseRegistration.API
dotnet user-secrets set "AdminSeed:Password" "<your-local-admin-password>" --project CourseRegistration.API
dotnet user-secrets set "AdminSeed:FullName" "Local Demo Admin" --project CourseRegistration.API
```

On startup, `AdminSeeder` creates the configured account with the `Admin` role if the local database has no administrator. If an administrator already exists, the seeder leaves it alone by default. Keep these settings in local User Secrets for the local database only.

### 4. Start the API and create the local User demo account

```sh
dotnet dev-certs https --trust
dotnet run --project CourseRegistration.API --launch-profile https
```

Open [local Swagger](https://localhost:7069/swagger/index.html). If your system does not support the SDK's HTTPS trust command, use `dotnet run --project CourseRegistration.API --launch-profile http` and open [Swagger on localhost over HTTP](http://localhost:5089/swagger/index.html).

Use `POST /api/auth/register` with your own local password of at least eight characters. For example, use `Local Demo User` for `fullName` and `user.demo@example.test` for `email`. The endpoint assigns the `User` role automatically.

Use `POST /api/auth/login` with each account's configured email and password. Check that the response reports the expected role, `Admin` or `User`. For protected operations, enter the returned JWT in Swagger's **Authorize** dialog.

## Cloud Deployment

The backend is deployed to Render using Docker.

The cloud database is hosted on Somee using SQL Server.

The frontend is deployed separately as a Render Static Site.

CORS is configured to support local development and the deployed frontend.

Production secrets are configured using environment variables.

Deployment has been completed. Full cloud functionality verification is pending network access approval.

## Project Links

Backend repository:

https://github.com/chayarivka/CourseRegistration-API

Frontend repository:

https://github.com/chayarivka/CourseRegistration-Client

Cloud API:

https://courseregistration-api.onrender.com

Cloud Swagger:

https://courseregistration-api.onrender.com/swagger/index.html

Cloud Client:

https://courseregistration-client.onrender.com
