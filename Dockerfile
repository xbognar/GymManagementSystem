# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/GymAPI/GymAPI.csproj src/GymAPI/
COPY src/GymDBAccess/GymDBAccess.csproj src/GymDBAccess/
RUN dotnet restore src/GymAPI/GymAPI.csproj

# Copy everything else and build
COPY src/GymAPI/ src/GymAPI/
COPY src/GymDBAccess/ src/GymDBAccess/
WORKDIR /app/src/GymAPI
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/src/GymAPI/out .
ENTRYPOINT ["dotnet", "GymAPI.dll"]
