#!/bin/bash

# Docker setup script for Deliveries API

set -e

echo "🚀 Starting Deliveries API Docker Setup..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Navigate to docker directory
cd "$(dirname "$0")"

# Create .env file if it doesn't exist
if [ ! -f .env ]; then
    echo "📝 Creating .env file from template..."
    cp .env.example .env
    echo "✅ .env file created. You can modify it if needed."
fi

# Create logs directory if it doesn't exist
if [ ! -d logs ]; then
    echo "📁 Creating logs directory..."
    mkdir -p logs
fi

# Build and start services
echo "🏗️ Building and starting services..."
docker-compose up --build -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 10

# Check services status
echo "🔍 Checking services status..."
docker-compose ps

# Show service URLs
echo ""
echo "✅ Services are running!"
echo "🌐 API: http://localhost:5000"
echo "🗄️ pgAdmin: http://localhost:8080 (admin@admin.com / admin)"
echo "🐘 PostgreSQL: localhost:5432 (admin / admin)"
echo ""
echo "📋 Useful commands:"
echo "  View logs: docker-compose logs -f"
echo "  Stop services: docker-compose down"
echo "  Restart API: docker-compose restart deliveries-api"
echo "  Access DB: docker-compose exec deliveries-postgres psql -U admin -d deliveries_db"

# Check if API is responding
echo "🏥 Checking API health..."
sleep 5
if curl -f http://localhost:5000/health > /dev/null 2>&1; then
    echo "✅ API is healthy!"
else
    echo "⚠️ API might still be starting up. Check logs with: docker-compose logs deliveries-api"
fi