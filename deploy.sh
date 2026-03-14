#!/bin/bash

set -e

echo "=========================================="
echo "Deploying MyEmailAliasesApi to Docker"
echo "=========================================="

# Load environment variables from a fixed path outside the checkout
ENV_FILE="${DEPLOY_ENV_FILE:-/home/deploy/.env.myemailaliasesapi}"
if [ ! -f "$ENV_FILE" ]; then
    echo "ERROR: .env file not found at $ENV_FILE"
    echo "Create it with the required variables (see env.example for reference)."
    exit 1
fi
export $(grep -v '^#' "$ENV_FILE" | xargs)

# Pull latest changes (if in a git repository)
if [ -d .git ]; then
    echo "Fetching latest changes..."
    git fetch origin main 2>/dev/null || git fetch origin master 2>/dev/null || echo "No git remote found, skipping fetch"
    git reset --hard origin/main 2>/dev/null || git reset --hard origin/master 2>/dev/null || echo "No git branch found, skipping reset"
fi

# Stop and remove existing containers
echo "Stopping existing containers..."
docker compose down || true

# Build and start containers
echo "Building and starting containers..."
docker compose build --no-cache
docker compose up -d

# Wait for database to be ready
echo "Waiting for database to be ready..."
sleep 10

# Run database migrations
echo "Running database migrations..."
docker compose exec -T api dotnet ef database update || echo "Migrations may have failed - check logs"

# Show container status
echo ""
echo "=========================================="
echo "Deployment complete!"
echo "=========================================="
echo ""
echo "Container status:"
docker compose ps
echo ""
echo "To view logs: docker compose logs -f"
echo "To stop: docker compose down"
echo ""
