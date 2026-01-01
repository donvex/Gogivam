# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy just the API project
COPY ["src/Deliveries.Api/Deliveries.Api.csproj", "src/Deliveries.Api/"]

# Restore dependencies
RUN dotnet restore "src/Deliveries.Api/Deliveries.Api.csproj"

# Copy the rest of the code
COPY . .

# Build the API project
RUN dotnet build "src/Deliveries.Api/Deliveries.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/Deliveries.Api/Deliveries.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Deliveries.Api.dll"]