---
name: Deployment Setup
description: Production server, CI/CD, and environment configuration
type: project
---

**Production server:** antec (10.0.20.10)
**Production URL:** https://email.forstberg.net
**DNS/CDN:** Cloudflare → Caddy reverse proxy → Docker container

**CI/CD:**
- GitHub Actions self-hosted runner installed as `deploy` user at /home/deploy/actions-runner
- Runner registered per-repo (not org-level)
- Workflow: .github/workflows/deploy.yml — triggers on push to main + workflow_dispatch
- Runner service: actions.runner.niklasforstberg-MyEmailAliasesApi.antec

**Docker:**
- API container: myemailaliasesapi (port 127.0.0.1:8080:80)
- SQL Server container: sqlserver (on db-network)
- Docker network: db-network (external)

**Env file on server:** /home/deploy/.env.myemailaliasesapi
- Uses bash-compatible names (no colons): JWT_KEY, DB_CONNECTION_STRING, SMTP_USERNAME etc.
- deploy.sh reads via `set -a; source $ENV_FILE; set +a`

**Key env vars:**
- JWT_KEY, DB_CONNECTION_STRING, API_PORT=8080
- SMTP_USERNAME, SMTP_PASSWORD, SMTP_SERVER, SMTP_SSLPORT=465, SMTP_ENABLESSL=true
- OneCom vars (when BackgroundService is implemented)

**deploy.sh behavior:**
- Reads .env from /home/deploy/.env.myemailaliasesapi
- git fetch + reset --hard origin/main (redundant with runner checkout but harmless)
- docker compose down → build --no-cache → up -d
- Runs EF migrations via app startup (db.Database.Migrate() in Program.cs)
