# Deployment Guide for email.forstberg.net

This guide explains how to deploy the MyEmailAliasesApi application to your Docker server with Caddy and Cloudflare Tunnel.

## Prerequisites

1. Docker and Docker Compose installed on your server
2. Caddy installed and configured on your server
3. Cloudflare Tunnel configured for `email.forstberg.net`
4. Cloudflare managing TLS certificates (automatic HTTPS)
5. Domain `email.forstberg.net` configured in Cloudflare

## Quick Start

1. **Clone/Copy the repository to your server**
   ```bash
   git clone <your-repo-url> /path/to/MyEmailAliasesApi
   cd /path/to/MyEmailAliasesApi
   ```

2. **Create environment file**
   ```bash
   cp env.example .env
   nano .env  # Edit with your actual values
   ```

3. **Configure Caddy** (see Caddy Configuration section)

4. **Deploy**
   ```bash
   chmod +x deploy.sh
   ./deploy.sh
   ```

## Environment Variables

Create a `.env` file in the project root with the following variables:

```bash
# Database Configuration - Point to your existing production database
# See Database Configuration section below for connection string options
DB_CONNECTION_STRING=Server=host.docker.internal;Database=EmailAliasesDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;Encrypt=False

# JWT Configuration
JWT_KEY=YourSuperSecretJWTKeyHere_MakeItLongAndRandom

# API Port (default: 8080)
# Change this if port 8080 is already in use by another Docker application
# Remember to update your Caddyfile to match this port
API_PORT=8080
```

**Important Security Notes:**
- Use a long, random string for `JWT_KEY` (at least 32 characters)
- Never commit the `.env` file to version control
- Use strong database credentials from your existing database setup

**Note:** JWT Issuer/Audience and CORS origins are configured in `docker-compose.yml` for `https://email.forstberg.net` (Cloudflare provides HTTPS).

## Caddy Configuration

The application runs on `localhost:8080` and needs to be proxied by Caddy. 

### Option 1: Add to Existing Caddyfile

If you already have a Caddyfile, add this configuration:

```caddy
email.forstberg.net {
    reverse_proxy localhost:8080 {
        header_up X-Real-IP {remote_host}
        header_up X-Forwarded-For {remote_host}
        header_up X-Forwarded-Proto {scheme}
        header_up X-Forwarded-Host {host}
        
        health_uri /health
        health_interval 30s
        health_timeout 5s
    }
    
    encode gzip zstd
}
```

### Option 2: Use Separate Caddyfile

See `Caddyfile.example` for a complete example configuration.

**Important:** Since Cloudflare Tunnel handles TLS termination, Caddy doesn't need to manage SSL certificates. The application receives HTTPS requests from Cloudflare.

### Reload Caddy Configuration

After updating your Caddyfile:

```bash
sudo caddy reload
# Or if using systemd:
sudo systemctl reload caddy
```

## Cloudflare Tunnel Setup

Ensure your Cloudflare Tunnel is configured to route `email.forstberg.net` to your server. The tunnel should:

1. Terminate TLS (Cloudflare handles certificates automatically)
2. Forward requests to Caddy (or directly to your server if Caddy is not in the path)
3. Preserve original request headers

Example Cloudflare Tunnel configuration (in Cloudflare dashboard or `config.yml`):

```yaml
ingress:
  - hostname: email.forstberg.net
    service: http://localhost:80  # Or wherever Caddy listens
```

## Deployment Steps

1. **Make deploy script executable**
   ```bash
   chmod +x deploy.sh
   ```

2. **Run deployment**
   ```bash
   ./deploy.sh
   ```

   This script will:
   - Pull latest code (if git repository)
   - Stop existing containers
   - Build new Docker images
   - Start all services (database, API)
   - Run database migrations

## Manual Deployment

If you prefer to deploy manually:

```bash
# Build and start services
docker-compose up -d --build

# Run migrations
docker-compose exec api dotnet ef database update

# View logs
docker-compose logs -f
```

## Service Management

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f db
```

### Stop Services
```bash
docker-compose down
```

### Restart Services
```bash
docker-compose restart
# Or restart specific service
docker-compose restart api
```

### Update Application
```bash
# Pull latest code
git pull

# Rebuild and restart
docker-compose up -d --build

# Run migrations if needed
docker-compose exec api dotnet ef database update

# Reload Caddy if configuration changed
sudo caddy reload
```

## Port Configuration

**Important:** Each Docker container needs a unique port on the host. Port 8080 is the default, but if you have other applications using port 8080, you must change it.

### Changing the Port

1. **Update `.env` file:**
   ```bash
   API_PORT=8081  # Or any other available port
   ```

2. **Update your Caddyfile** to match:
   ```caddy
   reverse_proxy localhost:8081  # Match the API_PORT value
   ```

3. **Reload Caddy:**
   ```bash
   sudo caddy reload
   ```

### Checking Port Availability

Before deploying, check if a port is available:
```bash
sudo netstat -tulpn | grep :8080  # Check if port 8080 is in use
# Or
sudo ss -tulpn | grep :8080
```

If the port is in use, choose a different port (e.g., 8081, 8082, etc.) and update both `.env` and your Caddyfile.

## Architecture

The deployment consists of one Docker container:

1. **api** - .NET 9.0 Web API with React frontend
   - Port: Configurable via `API_PORT` (default: 8080) on localhost
   - Exposed only to Caddy reverse proxy
   - Health check endpoint: `/health`
   - Handles forwarded headers from Cloudflare/Caddy

**External Services:**
- **Existing Database** - SQL Server running as a Docker container on the same server
- **Caddy** - Reverse proxy running on the host
- **Cloudflare Tunnel** - Handles TLS termination and routing

## Database Configuration

This deployment uses an **existing production database** running as a Docker container on the same server.

### Connection String Options

Update `DB_CONNECTION_STRING` in your `.env` file based on how your database is configured:

1. **Database on host network or localhost:**
   ```bash
   DB_CONNECTION_STRING=Server=localhost;Database=EmailAliasesDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;Encrypt=False
   ```

2. **Database on a Docker network (using container name):**
   ```bash
   DB_CONNECTION_STRING=Server=your-db-container-name;Database=EmailAliasesDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;Encrypt=False
   ```
   Then update `docker-compose.yml` to connect to that network:
   ```yaml
   networks:
     - your-existing-db-network
   ```

3. **Database on specific IP:**
   ```bash
   DB_CONNECTION_STRING=Server=192.168.1.100;Database=EmailAliasesDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;Encrypt=False
   ```

### Finding Your Database Connection Details

To find your existing database container:
```bash
# List all containers
docker ps -a

# Inspect database container network
docker inspect <db-container-name> | grep -A 20 Networks

# Check database container IP
docker inspect <db-container-name> | grep IPAddress
```

## Network Flow

```
Internet → Cloudflare Tunnel (HTTPS/TLS) → Caddy (HTTP) → api:8080 (HTTP) → existing-db:1433
```

The application receives HTTPS requests (via forwarded headers) but runs on HTTP internally.

## Troubleshooting

### Check container status
```bash
docker-compose ps
```

### Check if ports are in use
```bash
# Check API port (default 8080, or whatever you set in API_PORT)
sudo netstat -tulpn | grep :8080
# Or use ss command
sudo ss -tulpn | grep :8080
```

### Database connection issues
- Verify `DB_CONNECTION_STRING` in `.env` points to your existing database
- Check database server name/IP matches your existing database container
- Verify database credentials are correct
- Test connection from host: `docker exec -it myemailaliasesapi curl http://localhost:80/health`
- Check API logs for connection errors: `docker-compose logs api | grep -i "connection\|database\|sql"`
- Ensure your existing database container is running: `docker ps | grep <your-db-container>`

### Caddy configuration issues
- Verify Caddyfile syntax: `sudo caddy validate --config /path/to/Caddyfile`
- Check Caddy logs: `sudo journalctl -u caddy -f` or `sudo tail -f /var/log/caddy/access.log`
- Test Caddy configuration: `sudo caddy reload --dry-run`

### Cloudflare Tunnel issues
- Verify tunnel is running: Check Cloudflare dashboard or `cloudflared tunnel list`
- Check tunnel logs for connection issues
- Verify DNS is pointing to Cloudflare (not directly to your server IP)

### CORS issues
- Verify `ALLOWED_ORIGINS` in docker-compose.yml includes `https://email.forstberg.net`
- Check browser console for CORS errors
- Verify Caddy is forwarding headers correctly (X-Forwarded-Proto should be `https`)

### Application not accessible
1. Check API container is running: `docker-compose ps api`
2. Check API logs: `docker-compose logs api`
3. Test API directly: `curl http://localhost:8080/health`
4. Test through Caddy: `curl http://localhost:80/health` (if Caddy listens on port 80)
5. Verify Cloudflare Tunnel is routing correctly
6. Check DNS: `nslookup email.forstberg.net` (should show Cloudflare IPs)

### Forwarded headers issues
- The application uses `UseForwardedHeaders()` to trust Cloudflare/Caddy proxies
- Verify X-Forwarded-Proto header is set to `https` by Cloudflare
- Check application logs for any forwarded header warnings

## Backup

Since you're using an existing database container, use your existing backup procedures. Example commands:

### Database Backup
```bash
# Connect to your existing database container and create backup
docker exec <your-db-container-name> /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "$DB_SA_PASSWORD" \
  -Q "BACKUP DATABASE EmailAliasesDb TO DISK='/var/opt/mssql/backup/EmailAliasesDb.bak'"

# Copy backup from container
docker cp <your-db-container-name>:/var/opt/mssql/backup/EmailAliasesDb.bak ./backup/
```

### Restore Database
```bash
# Copy backup to container
docker cp ./backup/EmailAliasesDb.bak <your-db-container-name>:/var/opt/mssql/backup/

# Restore
docker exec <your-db-container-name> /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "$DB_SA_PASSWORD" \
  -Q "RESTORE DATABASE EmailAliasesDb FROM DISK='/var/opt/mssql/backup/EmailAliasesDb.bak' WITH REPLACE"
```

## Security Considerations

1. **Firewall**: Only expose necessary ports. Database port (1433) should not be exposed externally.
2. **TLS**: Cloudflare handles TLS termination automatically. No SSL certificates needed on the server.
3. **Passwords**: Use strong, unique passwords for database and JWT key.
4. **Updates**: Regularly update Docker images and application dependencies.
5. **Backups**: Set up regular database backups.
6. **Monitoring**: Consider setting up monitoring and alerting for your services.
7. **Cloudflare Security**: Enable Cloudflare security features (WAF, DDoS protection, etc.) in the Cloudflare dashboard.

## Support

For issues or questions:
- Check application logs: `docker-compose logs -f api`
- Check Caddy logs: `sudo journalctl -u caddy -f`
- Check Cloudflare Tunnel status in Cloudflare dashboard
- Review this deployment guide
- Check application README.md
