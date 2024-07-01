
# Gym Management API Documentation

The Gym Management API is designed to manage gym memberships, members,
and access control. The backend is developed using C# and .NET, with an
MSSQL database. The project is containerized using Docker and Docker
Compose for easy deployment.

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

-   **Rate Limiting:** Protecting the API from abuse.

-   **Integration Tests:** Ensuring the API endpoints function
    correctly.

-   **Unit Tests:** Testing the core logic and services.

# Getting Started 

## Prerequisites 

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

-   [Docker](https://www.docker.com/)

-   [Microsoft SQL
    Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

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

# Installation 

1.  **Clone the repository:**

                    git clone https://github.com/yourusername/gym-management-api.git
                    cd gym-management-api

2.  **Build and run the Docker containers:**

                    docker-compose up -d

3.  **Apply migrations:**

    The migrations will be applied automatically when the application
    starts.

# Usage 

1.  **Starting the API:**

    The API will be available at `http://localhost:80` after running
    `docker-compose up`.

2.  **Accessing Swagger:**

    Swagger documentation will be available at
    `http://localhost:80/swagger`.

3.  **Endpoints:**

    -   **Authentication:**

        -   `POST /api/auth/login`

        -   `POST /api/auth/refresh`

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

    -   **Chips:**

        -   `GET /api/chips`

        -   `GET /api/chips/{id}`

        -   `POST /api/chips`

        -   `PUT /api/chips/{id}`

        -   `DELETE /api/chips/{id}`

# Testing

## Run Unit Tests: 

            dotnet test tests/GymDBAccess.Tests/
            dotnet test tests/GymAPI.Tests/

## Integration Tests: 

Integration tests are included in the `GymAPI.Tests` project and can be
run with the same `dotnet test` command.

