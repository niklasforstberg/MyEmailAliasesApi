---
name: Security Audit Status
description: OWASP audit findings and fix status
type: project
---

Full OWASP audit performed 2026-03-15. 18 findings total.

**Fixed (in code, not yet all deployed):**
- F-01 ✅ Connection string logged to stdout — removed Console.WriteLine
- F-02 ✅ Swagger in production — gated behind IsDevelopment()
- F-03 ✅ JWT error details in 401 — removed OnChallenge handler
- F-04 ✅ No rate limiting — added sliding window 10 req/min per IP on /api/auth
- F-06 ✅ Open registration — RequireAuthorization("AdminOnly") added
- F-08 ✅ Reset token plaintext in DB — now stores SHA-256 hash
- F-10 ✅ Container runs as root — USER app added to Dockerfile
- F-12 ✅ DateTime.Now → DateTime.UtcNow in JWT generation
- F-17 ✅ JWT logged to browser console — console.log calls removed

**Open GitHub Issues:**
- #3 [High] SMTP SSL disabled — fixed in .env on server (port 465, enablessl=true)
- #4 [High] Forwarded headers trusted from all sources
- #5 ✅ [High] Timing oracle on forgot-password — fixed with Channel<EmailJob> + EmailDispatchService
- #6 [Medium] No input validation on auth DTOs — NEXT TO FIX
- #7 [Medium] No HTTP security headers
- #8 [Medium] React renders emails without validation
- #9 [Medium] Scraper cache unprotected — becomes irrelevant when ported to C# BackgroundService
- #10 [Low] Self-hosted runner no isolation

**Planned: Port Python scraper to C# BackgroundService**
Plan exists (from Backend Architect agent). Requires TOTP secret from one.com 2FA setup.
Uses: Otp.NET, HtmlAgilityPack packages.
Config keys needed: OneCom__Username, OneCom__Password, OneCom__TotpSecret, OneCom__Domain, OneCom__SyncInterval
