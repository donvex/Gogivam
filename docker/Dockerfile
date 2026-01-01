# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["Deliveries.sln", "."]

# Copy all project files for dependency restoration
COPY ["src/Deliveries.Api/Deliveries.Api.csproj", "src/Deliveries.Api/"]
COPY ["src/Deliveries.Application/Deliveries.Application.csproj", "src/Deliveries.Application/"]
COPY ["src/Deliveries.Domain/Deliveries.Domain.csproj", "src/Deliveries.Domain/"]
COPY ["src/Deliveries.Infrastructure/Deliveries.Infrastructure.csproj", "src/Deliveries.Infrastructure/"]

# Restore dependencies for the entire solution
RUN dotnet restore "Deliveries.sln"

# Copy the rest of the source code
COPY . .

# Build the entire solution
RUN dotnet build "Deliveries.sln" -c Release --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish "src/Deliveries.Api/Deliveries.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' --shell /bin/bash --home /app appuser

# Copy published application
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Add health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Deliveries.Api.dll"]