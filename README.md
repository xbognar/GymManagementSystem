
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
-   **Testing:** xUnit, Moq

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
            ├── src/
            │   ├── GymAPI/
            │   │   ├── Controllers/
            │   │   │   ├── AuthController.cs
            │   │   │   ├── ChipsController.cs
            │   │   │   ├── MembersController.cs
            │   │   │   └── MembershipsController.cs
            │   │   ├── Program.cs
            │   │   ├── GymAPI.csproj
            │   │   └── ...
            │   ├── GymDBAccess/
            │   │   ├── DataAccess/
            │   │   │   └── ApplicationDbContext.cs
            │   │   ├── DTOs/
            │   │   │   ├── ActiveChipDTO.cs
            │   │   │   ├── ActiveMembershipDTO.cs
            │   │   │   └── ...
            │   │   ├── Models/
            │   │   │   ├── Chip.cs
            │   │   │   ├── Member.cs
            │   │   │   └── Membership.cs
            │   │   ├── Services/
            │   │   │   ├── ChipService.cs
            │   │   │   ├── MemberService.cs
            │   │   │   ├── MembershipService.cs
            │   │   │   └── Interfaces/
            │   │   │       ├── IChipService.cs
            │   │   │       ├── IMemberService.cs
            │   │   │       └── IMembershipService.cs
            │   │   └── GymDBAccess.csproj
            ├── tests/
            │   ├── GymAPI.Tests/
            │   │   ├── Controllers/
            │   │   │   ├── ChipsControllerTests.cs
            │   │   │   ├── MembersControllerTests.cs
            │   │   │   └── MembershipsControllerTests.cs
            │   │   └── GymAPI.Tests.csproj
            │   ├── GymDBAccess.Tests/
            │   │   ├── ChipServiceTests.cs
            │   │   ├── MemberServiceTests.cs
            │   │   └── MembershipServiceTests.cs
            │   │   └── GymDBAccess.Tests.csproj
            ├── Dockerfile
            ├── docker-compose.yml
            └── README.md
            ├── StartBE.bat
            └── StopBE.bat  

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

# Testing

### Integration and Unit Tests: 

Run the following commands to execute the tests:

            dotnet test tests/GymDBAccess.Tests/
            dotnet test tests/GymAPI.Tests/

- **Mocking:** The tests use Moq for mocking dependencies, ensuring that the controllers and services are tested in isolation.
- **Endpoint Testing:** Each endpoint is tested to confirm it handles both valid and invalid inputs correctly, verifying authentication, authorization, and business logic.

