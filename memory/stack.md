---
name: Tech Stack
description: Technologies and key files in the project
type: project
---

- .NET 9 Minimal API backend (Program.cs, Endpoints/, Services/, Models/, Data/)
- React + Vite frontend (clientapp/)
- Python scraper (Scraper/fetchaliases.py) — syncs aliases from one.com, planned to be ported to C# BackgroundService
- SQL Server database (running in Docker as container "sqlserver" on db-network)

**Key files:**
- `Program.cs` — app entry point, JWT auth, CORS, rate limiting, EF Core
- `Endpoints/AuthEndpoints.cs` — login, register, forgot/reset password
- `Endpoints/AliasEndpoints.cs` — alias CRUD
- `Services/EmailService.cs` — SMTP email sending
- `Services/EmailDispatchService.cs` — BackgroundService that sends emails via Channel<EmailJob>
- `Data/EmailAliasDbContext.cs` — EF Core DbContext
