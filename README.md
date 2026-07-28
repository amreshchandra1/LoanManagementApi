# LoanManagementApi

## Project Overview
LoanManagementApi is an ASP.NET Core (.NET 8) Web API that provides RESTful endpoints to manage loans, borrowers, repayments and approvals. It implements a layered architecture with clear separation between API, business logic and data access and is suitable for deployment in containers or cloud platforms.

## Architecture Diagram
A simplified architecture:

Client (Web/Mobile)
  |
  v
ASP.NET Core Web API (Controllers / Minimal APIs)
  |
  v
Service Layer (Business Logic)
  |
  v
Repository Layer (EF Core)
  |
  v
Relational Database (SQL Server / PostgreSQL)

Supporting components: Authentication (JWT), Logging & Monitoring (Application Insights[once deployed to azure]),Serilog

Example ASCII:

Client --> API --> Services --> Repositories --> Database
                     |             |
                    

## Setup Instructions
Prerequisites
- .NET 8 SDK (https://dotnet.microsoft.com)
- Git
- SQL Server / PostgreSQL or Docker
- (Optional) Docker Desktop

Local setup
1. Clone the repository:
   - git clone https://github.com/amreshchandra1/LoanManagementApi.git
2. Change directory:
   - `cd LoanManagementApi`
3. Restore and build:
   - `dotnet restore`
   - `dotnet build`
4. Configure settings:
   - Copy `appsettings.Development.example.json` to `appsettings.Development.json` and update `ConnectionStrings:Default` and `Jwt` settings.
5. Apply EF Core migrations (if project uses EF Core):
   - `dotnet tool install --global dotnet-ef` (once)
   - `dotnet ef database update`
6. Run locally:
   - `dotnet run --project src/LoanManagementApi` (or use Visual Studio / Rider)

Environment variables (common):
- `ConnectionStrings__Default` - database connection
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` - JWT settings
- `ASPNETCORE_ENVIRONMENT` - environment name (Development/Production)

## Deployment Steps
Need to Add deployment steps for Azure App Service
Step 1: Initialize the Publish WizardOpen your project solution in Visual Studio.
Right-click your Web API project (e.g., LoanManagementApi) in the Solution Explorer and click Publish....On the target selection window, select Azure and click Next.
Choose Azure App Service (Windows) or Azure App Service (Linux) depending on your hosting plan configuration, then click Next.
Step 2: Connect Visual Studio to your Azure AccountIf prompted, sign in using the Azure Account credentials linked to your cloud subscriptions.
Select your active Subscription from the drop-down menu.Under App Service Instances, locate and select the name of your specific Azure Web App from the resource directory tree hierarchy.Click Finish.
Visual Studio will automatically configure the required background access tokens and connection endpoints.
Step 3: Set Production ParametersOn the summary dashboard that appears, click the pencil icon (Edit) next to the Configuration label.In the configuration popup, verify these target properties:Configuration: 
Change this from Debug to Release.Target Framework: Ensure this matches your project profile version (e.g., .NET 8.0).Deployment Mode: Select Framework-Dependent.
Target Runtime: Select Portable.
Click Save
Step 4: Run the DeploymentClick the large Publish button at the top of the Visual Studio screen.
Watch the compilation progress bar inside the Output Window at the bottom of the screen.
Once the deployment finishes successfully, Visual Studio will print a Publish Succeeded message and open your live API web URL in your browser.

## API Documentation
Base path: `/api`

Authentication
- `POST /api/auth/register`
  - Request: `{ "email": "user@example.com", "password": "P@ssw0rd" }`
  - Response: 201 Created, user summary
- `POST /api/auth/login`
  - Request: `{ "email": "user@example.com", "password": "P@ssw0rd" }`
  - Response: `{ "token": "<jwt>", "expires": "2026-..." }`

Loans
- `api/Loan/UserRegistation`
  - Use to register user with username ans password.Which will be used to generate token
- `GET [/api/Login/GenerateToken?usrname=amresh&password=password`]
  - Returns JWT Token
- `GET api/Loan/CreateLoanApplication`
  - Create Loan Application.Return Success or Fail
- `POST api/Loan/UpdateLoanStatus/DA3DA74A-C515-4DA2-A56F-517721F3DAF1/DocumentsVerified`
  - Update status of loan
- `POST api/Loan/LoanStatusTracking/DA3DA74A-C515-4DA2-A56F-517721F3DAF1`
  - Get Loan Status by LoanAppId
- `POST api/ViewLoanHistoryByUserName/amresh`
  - Get all Loan created by user.
- `POST api/Loan/LoanStatusTracking/DA3DA74A-C515-4DA2-A56F-517721F3DAF1`
  - Get Loan Status

Common responses
- 200 OK, 201 Created, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found, 500 Internal Server Error

Notes
- Endpoints expect and return JSON. Use `Authorization: Bearer <token>` for protected endpoints.
- Swagger/OpenAPI is recommended for interactive docs (`/swagger`).

## Assumptions
- The project uses a relational database (SQL Server).
- Authentication is JWT-based and stateless.
- Business logic is implemented in a service layer separate from controllers.
- EF Core is used for data access with migrations supported.

## Design Decisions
- Layered architecture (API -> Services -> Repositories) for separation of concerns and testability.
- DTOs and AutoMapper for mapping between domain models and API contracts.
- Async/await for I/O-bound operations to improve scalability.
- Dependency injection via built-in container for loose coupling.
- Logging via `Microsoft.Extensions.Logging` and structured logs.
- Configuration via `appsettings.{Environment}.json` and environment variables for secrets.

## Future Improvements
- Add comprehensive Swagger/OpenAPI.
- Implement role-based access control and claims-based authorization.
- Add end-to-end integration tests and contract tests.
- Implement encryption for sensitive data at rest and in transit.
- Support event-driven integration and message queue for long-running processes.


