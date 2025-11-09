#!/bin/bash

# Pull latest changes with error checking
echo "Fetching latest changes..."
git fetch origin main
git reset --hard origin/main

# Stop and remove existing container
echo "Stopping and removing existing container..."
docker stop myemailaliasesapi || true
docker rm myemailaliasesapi || true

# Build new image
echo "Building new image..."
docker build -t myemailaliasesapi:latest .

# Run new container
echo "Starting new container..."
docker run -d \
  --name myemailaliasesapi \
  --restart unless-stopped \
  --env-file env.list \
  -p 8081:8080 \
  myemailaliasesapi:latest

# Clean up old images
echo "Cleaning up old images..."
docker image prune -f

echo "Deployment complete!" 