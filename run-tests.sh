#!/bin/bash

# Build the Docker image for the application.
docker build -t companyapi ./CompanyAPI/

# Start the containers defined in docker-compose.yml.
docker-compose up -d

# Run the unit tests.
dotnet test ./Company.UnitTests/Company.UnitTests.csproj

# Run the integration tests.
dotnet test ./Company.IntegrationTests/Company.IntegrationTests.csproj

# Stop and remove the containers.
docker-compose down