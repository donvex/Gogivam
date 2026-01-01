# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Deliveries.sln", "."]
COPY ["src/Deliveries.Api/Deliveries.Api.csproj", "src/Deliveries.Api/"]
COPY ["src/Deliveries.Application/Deliveries.Application.csproj", "src/Deliveries.Application/"]
COPY ["src/Deliveries.Domain/Deliveries.Domain.csproj", "src/Deliveries.Domain/"]
COPY ["src/Deliveries.Infrastructure/Deliveries.Infrastructure.csproj", "src/Deliveries.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Deliveries.sln"

# Copy the rest of the code
COPY . .

# Build the application
RUN dotnet build "Deliveries.sln" -c Release -o /app/build

# Publish stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /src
COPY --from=build /src .
RUN dotnet publish "src/Deliveries.Api/Deliveries.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000 5001
ENTRYPOINT ["dotnet", "Deliveries.Api.dll"]