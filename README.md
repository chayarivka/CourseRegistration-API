\# Course Registration System – Backend API



\## Project Overview



A course registration management system developed using ASP.NET Core Web API and SQL Server.



The system allows users to browse courses, search and filter courses, register for available courses, and manage their registrations. Administrators can manage the course catalog.



\## Technologies



\- C# and .NET 8

\- ASP.NET Core Web API

\- Entity Framework Core – Code First

\- SQL Server

\- AutoMapper

\- JWT Authentication

\- Swagger

\- NLog

\- xUnit and Moq

\- Docker

\- Git and GitHub

\- Render and Somee



\## System Architecture



The application follows a four-layer architecture:



\- \*\*Core\*\* – Entities, DTOs, enums and repository interfaces.

\- \*\*Data\*\* – Database context, repositories, entity configurations and migrations.

\- \*\*Service\*\* – Business logic, service interfaces and AutoMapper profiles.

\- \*\*API\*\* – Controllers, middleware, authentication and dependency injection configuration.



An additional Tests project contains automated tests.



\## Database



The database contains four main entities:



\- User

\- Course

\- Category

\- Registration



The system implements one-to-many relationships and a many-to-many relationship between users and courses through the Registration entity.



Entity Framework Core migrations manage the database structure. Initial categories are populated using seed data.



\## Main Features



\### Regular Users



\- User registration and login.

\- Browse available courses.

\- Search, filter, sort and paginate courses.

\- Register for courses.

\- View active registrations.

\- Cancel registrations before a course begins.



\### Administrators



\- Create courses.

\- Update courses.

\- Delete courses according to business rules.

\- Manage course capacity.



\## REST API



The API implements REST principles, DTO validation, model binding and appropriate HTTP status codes:



\- 200 – OK

\- 201 – Created

\- 204 – No Content

\- 400 – Bad Request

\- 401 – Unauthorized

\- 403 – Forbidden

\- 404 – Not Found

\- 409 – Conflict



Swagger provides interactive API documentation.



\## Authentication and Authorization



The system uses JWT authentication and role-based authorization.



Supported roles:



\- Admin

\- User



Passwords are stored as hashes. Sensitive configuration values are managed through User Secrets during local development and environment variables in production.



\## Concurrency and Limited Resources



Course seats are limited resources.



The system prevents duplicate active registrations and registration beyond the maximum course capacity.



Registration operations use database transactions with Serializable isolation.



Optimistic concurrency is implemented using the RowVersion property.



Concurrency conflicts are handled and returned as HTTP 409 Conflict responses.



\## Error Handling and Logging



The application includes centralized exception handling middleware and Correlation ID middleware.



NLog is used for application logging, including request information, errors and concurrency warnings.



Sensitive information must not be included in application logs.



\## Automated Testing



The project includes automated tests using xUnit and Moq.



The tests cover:



\- Successful registration.

\- Registration rejection.

\- Limited resource availability.

\- Concurrent registration attempts.

\- Optimistic concurrency conflicts using separate database contexts.



Local test result: 11 tests passed.



\## Local Setup



\### Requirements



\- .NET 8 SDK

\- Visual Studio

\- SQL Server or SQL Server LocalDB



\### Installation



1\. Clone the backend repository.

2\. Open the solution in Visual Studio.

3\. Restore NuGet packages.

4\. Configure the database connection string.

5\. Configure the JWT signing key using User Secrets.

6\. Apply Entity Framework Core migrations.

7\. Run the API project.



Local Swagger URL:



https://localhost:7069/swagger/index.html



\## Cloud Deployment



The backend is deployed to Render using Docker.



The cloud database is hosted on Somee using SQL Server.



The frontend is deployed separately as a Render Static Site.



CORS is configured to support local development and the deployed frontend.



Production secrets are configured using environment variables.



Deployment has been completed. Full cloud functionality verification is pending network access approval.



\## Project Links



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



