
# Gym Management API Documentation

The Gym Management API is designed to manage gym memberships, members, and access control. The backend is developed using C# and .NET, with an MSSQL database. The project is designed to run locally on a client's PC, where Docker containers manage both the database and backend services. This setup connects with a WPF desktop application frontend, allowing you to manage the gym's members and clients efficiently. The API is secured using JWT (JSON Web Tokens) and is rigorously tested with unit and integration tests for services and controllers to ensure reliability and correctness. The containerized architecture using Docker and Docker Compose ensures easy deployment and consistent runtime environments.

# Technologies Used 

-   **Programming Language:** C#
-   **Framework:** .NET 8.0
-   **Database:** MSSQL
-   **ORM:** Entity Framework Core
-   **Authentication:** JWT (JSON Web Tokens)
-   **Containerization:** Docker
-   **API Documentation:** Swagger
-   **Testing:** xUnit, Moq, FluentAssertions

# Features 

-   **User Authentication:** Secure login with JWT tokens.
-   **Membership Management:** CRUD operations for members and
    memberships.
-   **Chip Management:** Managing gym access chips.
-   **Global Error Handling:** Centralized handling of exceptions.
-   **Integration Tests:** Ensuring the API endpoints function
    correctly.
-   **Unit Tests:** Testing the core logic and services.
-   **Automated Scripts:** Simplified start and stop scripts for running the application.

# Prerequisites 

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Docker](https://www.docker.com/)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop)
-   [Microsoft SQL
    Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## UML Diagram

Below is the UML diagram showing the relationships between the classes:

![Model Relations](https://github.com/xbognar/GymManagementSystem/blob/master/docs/TableRelations.png)

# Project Structure 

            GymManagementSystem/
            ├── .env
            ├── docker-compose.yml
            ├── Dockerfile
            ├── GymManagementSystem.sln
            ├── README.md
            ├── StartBE.bat
            ├── StopBE.bat
            ├── docs/
            │   └── TableRelations.png
            ├── src/
            │   ├── GymAPI/
            │   │   ├── appsettings.Development.json
            │   │   ├── appsettings.json
            │   │   ├── GymAPI.csproj
            │   │   ├── GymAPI.http
            │   │   ├── Program.cs
            │   │   ├── Controllers/
            │   │   │   ├── AuthController.cs
            │   │   │   ├── ChipsController.cs
            │   │   │   ├── MembersController.cs
            │   │   │   └── MembershipsController.cs
            │   │   └── Properties/
            │   │       └── launchSettings.json
            │   ├── GymDBAccess/
            │   │   ├── GymDBAccess.csproj
            │   │   ├── DataAccess/
            │   │   │   └── ApplicationDbContext.cs
            │   │   ├── DTOs/
            │   │   │   ├── ActiveChipDTO.cs
            │   │   │   ├── ActiveMembershipDTO.cs
            │   │   │   ├── InactiveChipDTO.cs
            │   │   │   ├── InactiveMembershipDTO.cs
            │   │   │   ├── UserMembershipsDTO.cs
            │   │   │   ├── CreateChipDTO.cs
            │   │   │   ├── CreateMembershipDTO.cs
            │   │   │   ├── UpdateChipDTO.cs
            │   │   │   └── UpdateMembershipDTO.cs
            │   │   ├── Models/
            │   │   │   ├── Chip.cs
            │   │   │   ├── ChipUpdateRequest.cs
            │   │   │   ├── LoginModel.cs
            │   │   │   ├── Member.cs
            │   │   │   └── Membership.cs
            │   │   ├── Services/
            │   │   │   ├── ChipService.cs
            │   │   │   ├── JwtService.cs
            │   │   │   ├── MemberService.cs
            │   │   │   ├── MembershipService.cs
            │   │   │   └── Interfaces/
            │   │   │       ├── IChipService.cs
            │   │   │       ├── IJwtService.cs
            │   │   │       ├── IMemberService.cs
            │   │   │       └── IMembershipService.cs
            │   │   ├── Migrations/
            │   │   │   ├── <migration files>
            │   │   │   └── ApplicationDbContextModelSnapshot.cs
            ├── tests/
            │   ├── IntegrationTests/
            │   │   ├── IntegrationTests.csproj
            │   │   ├── Controllers/
            │   │   │   ├── AuthControllerIntegrationTests.cs
            │   │   │   ├── ChipsControllerIntegrationTests.cs
            │   │   │   ├── MembersControllerIntegrationTests.cs
            │   │   │   └── MembershipsControllerIntegrationTests.cs
            │   │   ├── Dependencies/
            │   │   │   ├── IntegrationTestFixture.cs
            │   │   │   ├── SeedDataHelper.cs
            │   │   │   └── TestUtilities.cs
            │   ├── UnitTests/
            │   │   ├── UnitTests.csproj
            │   │   ├── Controllers/
            │   │   │   ├── AuthControllerTests.cs
            │   │   │   ├── ChipsControllerTests.cs
            │   │   │   ├── MembersControllerTests.cs
            │   │   │   └── MembershipsControllerTests.cs
            │   │   ├── Services/
            │   │   │   ├── ChipServiceTests.cs
            │   │   │   ├── JwtServiceTests.cs
            │   │   │   ├── MemberServiceTests.cs
            │   │   │   └── MembershipServiceTests.cs


# Script Details

- **StartBE.bat:** This script starts Docker Desktop (if not already running), navigates to the project directory, and starts the Docker containers using Docker Compose. It allows the user to run the application with a single click.
- **StopBE.bat:** This script stops all running Docker containers related to the application and stops Docker Desktop. It provides a clean and easy way to shut down the application.

# Installation 

1.  **Clone the repository:**

                    git clone https://github.com/xbognar/GymManagementSystem.git
                    cd GymManagementSystem

2. **Create a `.env` file in the root directory:**

   ```bash
    ASPNETCORE_ENVIRONMENT=Production
    CONNECTION_STRING=Server=db;Database=GymDatabase;User Id=sa;Password=YourStrong@Passw0rd; 
    JWT_KEY=your_very_secure_jwt_key
    LOGIN_USERNAME=your_auth_username
    LOGIN_PASSWORD=your_auth_password
    SA_PASSWORD=your_sa_password
   ```

3. **Run the application using the START script:**

   Simply double-click the `START.bat` file. This will automatically start Docker Desktop (if not running), build and run the containers, and set up the application environment.

4. **Apply migrations:**

   Migrations will be applied automatically when the application starts.

# Usage 

1.  **Starting the API:**

    The API will be available at `http://localhost:80` after running the `START.bat` script.

2. **Stopping the API:**

   To stop the API and Docker containers, double-click the `STOP.bat` file. This will shut down the containers and stop Docker Desktop.

3.  **Accessing Swagger:**

    Swagger documentation will be available at
    `http://localhost:80/swagger`.

4.  **Endpoints:**

    -   **Authentication:**

        -   `POST /api/auth/login`

    -   **Members:**

        -   `GET /api/members`

        -   `GET /api/members/{id}`

        -   `POST /api/members`

        -   `PUT /api/members/{id}`

        -   `DELETE /api/members/{id}`

    -   **Memberships:**

        -   `GET /api/memberships`

        -   `GET /api/memberships/{id}`

        -   `POST /api/memberships`

        -   `PUT /api/memberships/{id}`

        -   `DELETE /api/memberships/{id}`
     
        -   `GET /api/memberships/active`
     
        -   `GET /api/memberships/inactive`
     
        -   `GET /api/memberships/user/{memberId}/memberships`

    -   **Chips:**

        -   `GET /api/chips`

        -   `GET /api/chips/{id}`

        -   `POST /api/chips`

        -   `PUT /api/chips/{id}`

        -   `DELETE /api/chips/{id}`

## Testing

The Gym Management API utilizes a robust testing strategy with **UnitTests** and **IntegrationTests** to ensure functionality and reliability.

### Unit Tests

- **Scope**:
  - **Services**: Test individual services (e.g., `ChipService`, `MembershipService`) by mocking `DbContext` to verify business logic and data interactions without a real database.
  - **Controllers**: Test controllers (e.g., `ChipsController`, `MembersController`) by mocking service interfaces to ensure correct HTTP responses and input validation.

- **Tools**:
  - [xUnit](https://xunit.net/)
  - [Moq](https://github.com/moq/moq4)
  - [FluentAssertions](https://fluentassertions.com/)

### Integration Tests

- **Scope**:
  - **Integration tests**: Validate the complete ASP.NET Core pipeline using a real `HttpClient`.
  - **In-Memory DB**: Use EF Core’s In-Memory provider for fast and deterministic testing.
  - **Authentication**: Generate and utilize real JWT tokens to verify that secured endpoints enforce authentication correctly.

- **Tools**:
  - [xUnit](https://xunit.net/)
  - [Moq](https://github.com/moq/moq4)
  - [FluentAssertions](https://fluentassertions.com/)
  - [Microsoft.AspNetCore.Mvc.Testing](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing/)

### Running Tests

Execute tests from the solution root using the following commands:

```bash
# Run only the Unit Tests
dotnet test tests/UnitTests/UnitTests.csproj

# Run only the Integration Tests
dotnet test tests/IntegrationTests/IntegrationTests.csproj

# Run all tests
dotnet test
```

By combining Unit Tests for isolated logic verification with Integration Tests for comprehensive end-to-end validation, the Gym Management API ensures a high level of quality and reliability.
