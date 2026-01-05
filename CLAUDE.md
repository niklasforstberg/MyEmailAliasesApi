# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Email Aliases API - An application to manage email aliases fetched from one.com hosting service. Users can search and browse their email aliases and their forwarding addresses.

## Development Commands

### .NET API (root directory)
```bash
dotnet run                    # Start the API server
dotnet build                  # Build the project
dotnet ef migrations add <name>  # Create a new migration
dotnet ef database update     # Apply migrations
```

### React Client (clientapp/)
```bash
cd clientapp
npm install                   # Install dependencies
npm run dev                   # Start dev server (port 5173, proxies to API)
npm run build                 # Build for production (outputs to ../wwwroot)
```

### Python Scraper (Scraper/)
```bash
cd Scraper
python -m venv venv           # Create virtual environment
source venv/bin/activate      # Activate venv (macOS/Linux)
pip install -r requirements.txt
python fetchaliases.py        # Fetch aliases from one.com (requires login)
```

## Architecture

### Backend (.NET 8 Minimal API)
- **Program.cs**: Application entry point, configures JWT auth, CORS, Swagger, and EF Core
- **Endpoints/**: Minimal API endpoint definitions
  - `AliasEndpoints.cs`: Email alias CRUD operations (requires auth)
  - `AuthEndpoints.cs`: Authentication (login, register, JWT token generation)
- **Data/EmailAliasDbContext.cs**: Entity Framework DbContext with Users, EmailAliases, EmailForwardings
- **Models/**: Domain entities and DTOs

### Frontend (React + Vite)
- **clientapp/src/context/AuthContext.jsx**: JWT authentication state management
- **clientapp/src/services/api.js**: API client with auth headers
- **clientapp/src/components/**: Login and AliasesList UI components

### Data Sync (Python Scraper)
- **Scraper/fetchaliases.py**: Logs into one.com, fetches email aliases via their internal API, and syncs to SQL Server database

### Database (SQL Server)
Three tables: Users, EmailAliases, EmailForwardings with relationships:
- User → EmailAliases (one-to-many)
- EmailAlias → EmailForwardings (one-to-many)

## Environment Variables

Required for .NET API:
```
smtp:username = myemailalias@forstberg.com
smtp:sslport = 2525
smtp:server = mail-eu.smtp2go.com
smtp:password = <your_secret_password>
smtp:enablessl = false
Jwt:Key=<your_secret_key>
Jwt:Issuer=http://localhost
Jwt:Audience=http://localhost
ConnectionStrings:DefaultConnection=Server=<ip>;Database=<name>;User Id=sa;Password=<pwd>;TrustServerCertificate=True
ALLOWED_ORIGINS=http://localhost:5173,http://127.0.0.1:5173
```

Required for Scraper (.env in Scraper/):
```
ONE_COM_USERNAME=<your_username>
ONE_COM_DOMAIN=<your_domain>
USER_ID=<user_id_in_db>
DB_SERVER=<ip>
DB_NAME=<name>
DB_USER=sa
DB_PASSWORD=<pwd>
```

## Development Workflow

1. Start API: `dotnet run` (runs on port 5000 or 5267)
2. Start React dev server: `cd clientapp && npm run dev` (port 5173)
3. React dev server proxies `/api` requests to the .NET backend

For production: build React app (`npm run build`), .NET serves static files from `wwwroot/`.
