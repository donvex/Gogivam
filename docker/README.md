# Docker Setup for Deliveries API

This directory contains Docker configuration files for running the Deliveries API with PostgreSQL database.

## Quick Start

```bash
# Navigate to docker directory
cd docker

# Run the setup script (creates .env, builds, and starts services)
./start.sh
```

## Services

### 🚀 Deliveries API
- **URL**: http://localhost:5000
- **Health Check**: http://localhost:5000/health
- **Swagger**: http://localhost:5000/swagger (if enabled)

### 🐘 PostgreSQL Database
- **Host**: localhost
- **Port**: 5432
- **Database**: deliveries_db
- **Username**: admin
- **Password**: admin

### 🛠️ pgAdmin (Optional)
- **URL**: http://localhost:8080
- **Email**: admin@admin.com
- **Password**: admin

## Manual Commands

### Development
```bash
# Start all services
docker-compose up -d

# Start with build
docker-compose up --build -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Restart API only
docker-compose restart deliveries-api
```

### Production
```bash
# Start with production configuration
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Stop production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml down
```

### Database Operations
```bash
# Access PostgreSQL CLI
docker-compose exec deliveries-postgres psql -U admin -d deliveries_db

# View database logs
docker-compose logs deliveries-postgres

# Backup database
docker-compose exec deliveries-postgres pg_dump -U admin deliveries_db > backup.sql

# Restore database
docker-compose exec -T deliveries-postgres psql -U admin -d deliveries_db < backup.sql
```

## Configuration

### Environment Variables (.env)
```bash
# PostgreSQL
POSTGRES_PASSWORD=admin
POSTGRES_USER=admin
POSTGRES_DB=deliveries_db

# API
ASPNETCORE_ENVIRONMENT=Development
API_PORT=5000

# pgAdmin
PGADMIN_EMAIL=admin@admin.com
PGADMIN_PASSWORD=admin
```

### Connection String
The API automatically uses this connection string:
```
Host=deliveries-postgres;Database=deliveries_db;Username=admin;Password=admin;
```

## Volumes

- **postgres_data**: PostgreSQL data persistence
- **pgadmin_data**: pgAdmin configuration persistence
- **./logs**: API logs directory

## Health Checks

All services include health checks:
- **API**: HTTP GET /health every 30s
- **PostgreSQL**: pg_isready every 10s
- **Dependencies**: API waits for healthy PostgreSQL

## Profiles

### Default Profile
Starts API and PostgreSQL only.

### Tools Profile
Includes pgAdmin for database management:
```bash
docker-compose --profile tools up -d
```

## Troubleshooting

### API Won't Start
```bash
# Check logs
docker-compose logs deliveries-api

# Check if PostgreSQL is ready
docker-compose logs deliveries-postgres
```

### Database Connection Issues
```bash
# Test PostgreSQL connection
docker-compose exec deliveries-postgres pg_isready -U admin

# Check network
docker-compose exec deliveries-api ping deliveries-postgres
```

### Performance Issues
```bash
# Check resource usage
docker stats

# View container processes
docker-compose top
```

## File Structure
```
docker/
├── Dockerfile                 # Multi-stage .NET API build
├── docker-compose.yml         # Main services configuration
├── docker-compose.prod.yml    # Production overrides
├── .env.example              # Environment variables template
├── start.sh                  # Quick setup script
├── postgres/
│   └── init/
│       └── 01-init-db.sql    # Database initialization
└── logs/                     # API logs (created automatically)
```

## Security Notes

### Development
- Default credentials are used for convenience
- All ports are exposed for testing
- Detailed logging is enabled

### Production
- Change default passwords
- Use environment variables for secrets
- Consider using secrets management
- Restrict network access
- Enable only necessary ports

## Monitoring

### Health Endpoints
- API Health: http://localhost:5000/health
- Database: Included in API health check

### Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f deliveries-api

# Since specific time
docker-compose logs --since 30m deliveries-api
```