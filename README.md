# Job Application Tracker

A full-stack web application for tracking job applications throughout the entire hiring process — from the first response to offer acceptance or rejection.

---

## What problem does it solve?

When you're actively job hunting, applications pile up quickly across different companies, channels, and stages. Spreadsheets become messy and hard to maintain. This app gives you a single, structured place to:

- See the current status of every application at a glance
- Track the full hiring pipeline (screening → interviews → offer)
- Store contacts, salary expectations, cover letters and vacancy descriptions
- Know when you last communicated and with whom
- Attach vacancy files (PDF/TXT) for offline reference

---

## Features

### Applications
- ✅ Create, edit, delete job applications
- ✅ Track company, contact channel, contact person, salary range
- ✅ Configurable hiring pipeline (stages as chips)
- ✅ Current stage badge on the list
- ✅ Applied date, first/last contact dates
- ✅ "Applied with" field (Resume, CV + Cover Letter, LinkedIn, etc.)
- ✅ Links to vacancy and application pages
- ✅ Vacancy file attachment (PDF / TXT, up to 10 MB)
- ✅ Cover letter and notes with **Markdown** support and live preview

### List & Navigation
- ✅ Sortable, paginated table
- ✅ Full-text search across company, stage, contact
- ✅ Configurable column visibility (saved in localStorage)
- ✅ Responsive layout — works on mobile and desktop

### Auth & Account
- ✅ Email + password registration with email verification
- ✅ Google OAuth sign-in
- ✅ JWT authentication
- ✅ Password reset via email link
- ✅ Change email with confirmation link
- ✅ Change password in settings

### UX
- ✅ Light / Dark theme toggle (persisted)
- ✅ Interface language: **English**, **Українська**, **Русский**
- ✅ Localized paginator, stages, and all UI strings
- ✅ Automatic DB backups on every deploy

---

## Tech Stack

### Backend
| | |
|---|---|
| Runtime | .NET 10, ASP.NET Core Web API |
| Architecture | Vertical Slice (feature folders, no MediatR) |
| ORM | EF Core + PostgreSQL (Npgsql) |
| Auth | JWT + Google OAuth (Google.Apis.Auth) |
| Passwords | BCrypt.Net-Next |
| Validation | FluentValidation |
| Email | Resend API |
| Tests | xUnit + Moq, SQLite in-memory |

### Frontend
| | |
|---|---|
| Framework | Angular 19 (standalone components, signals) |
| UI | Angular Material 21 (M3 theming) |
| Themes | Light (Azure + Blue) / Dark (Magenta + Violet) |
| Markdown | ngx-markdown |
| Auth | JWT in localStorage |

### Infrastructure
| | |
|---|---|
| Containerization | Docker + Docker Compose |
| Web server | nginx (frontend + reverse proxy) |
| CI/CD | GitHub Actions (build → test → push → deploy) |
| Registry | GitHub Container Registry (ghcr.io) |
| DNS / CDN | Cloudflare |

---

## Running Locally

### Prerequisites
- [Docker](https://docs.docker.com/get-docker/) + Docker Compose
- Git

### Steps

**1. Clone the repository**
```bash
git clone https://github.com/sanioooook/HiringProcess.git
cd HiringProcess
```

**2. Create environment file**
```bash
cp .env.example .env
```

Edit `.env` — at minimum set a strong password and JWT secret:
```env
POSTGRES_PASSWORD=your_strong_password
JWT_SECRET=at_least_32_characters_long_secret
RESEND_API_KEY=                        # leave empty to disable emails
RESEND_FROM_EMAIL=noreply@example.com
APP_FRONTEND_URL=http://localhost
```

**3. Build and start**
```bash
docker compose up --build
```

**4. Open in browser**
```
http://localhost
```

The API is also available at `http://localhost:5000` for direct access.

> **Note:** On first launch, EF Core migrations run automatically and the database schema is created.

---

## Deploy from GitHub Packages (no build required)

The easiest way to run the app — pull pre-built images directly from GitHub Container Registry without cloning the repository or installing any development tools.

**1. Create a working directory and `.env` file**
```bash
mkdir hiringprocess && cd hiringprocess

cat > .env << 'EOF'
POSTGRES_PASSWORD=your_strong_password
JWT_SECRET=at_least_32_characters_long_secret
GOOGLE_CLIENT_ID=                        # optional
RESEND_API_KEY=                          # optional
RESEND_FROM_EMAIL=noreply@yourdomain.com
APP_FRONTEND_URL=https://yourdomain.com
EOF
```

**2. Create `docker-compose.yml`**
```yaml
services:

  postgres:
    image: postgres:17-alpine
    container_name: hp_postgres
    restart: unless-stopped
    environment:
      POSTGRES_DB: hiringprocess
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  backend:
    image: ghcr.io/sanioooook/hiringprocess-backend:latest
    container_name: hp_backend
    restart: unless-stopped
    depends_on:
      postgres:
        condition: service_healthy
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=hiringprocess;Username=postgres;Password=${POSTGRES_PASSWORD}"
      Jwt__Secret: ${JWT_SECRET}
      Jwt__Issuer: hiringprocess-api
      Jwt__Audience: hiringprocess-frontend
      Jwt__ExpireMinutes: 60
      Authentication__Google__ClientId: ${GOOGLE_CLIENT_ID:-}
      FileStorage__BasePath: /app/uploads
      Cors__AllowedOrigins__0: ${APP_FRONTEND_URL}
      Resend__ApiKey: ${RESEND_API_KEY:-}
      Resend__FromEmail: ${RESEND_FROM_EMAIL:-noreply@example.com}
      App__FrontendUrl: ${APP_FRONTEND_URL}
    volumes:
      - uploads_data:/app/uploads

  frontend:
    image: ghcr.io/sanioooook/hiringprocess-frontend:latest
    container_name: hp_frontend
    restart: unless-stopped
    depends_on:
      - backend
    ports:
      - "80:80"

volumes:
  postgres_data:
  uploads_data:
```

**3. Pull and start**
```bash
docker compose pull
docker compose up -d
```

**To update to the latest version:**
```bash
docker compose pull && docker compose up -d && docker image prune -f
```

---

## Self-Hosting (VPS)

### Prerequisites
- VPS with Ubuntu 22.04+ and at least 1 GB RAM
- Docker + Docker Compose installed
- A domain pointed to your server's IP (optional but recommended)

### Steps

**1. Clone the repo on your server**
```bash
git clone https://github.com/sanioooook/HiringProcess.git ~/app
cd ~/app
```

**2. Configure environment**
```bash
cp .env.example .env
nano .env
```

```env
POSTGRES_PASSWORD=very_strong_password
JWT_SECRET=32_characters_minimum_secret_key
GOOGLE_CLIENT_ID=xxx.apps.googleusercontent.com   # optional
RESEND_API_KEY=re_xxxxxxxxxxxxxxxxxxxx             # optional
RESEND_FROM_EMAIL=noreply@yourdomain.com
APP_FRONTEND_URL=https://yourdomain.com
```

**3. Start**
```bash
docker compose up -d
```

**4. Cloudflare (optional, recommended)**

Add an `A` record pointing to your server IP with **Proxy enabled**, then set SSL/TLS mode to **Flexible**. This gives you free HTTPS without configuring certificates on the server.

### CI/CD Auto-Deploy (GitHub Actions)

On every push to `main`, the pipeline automatically:
1. Runs backend unit tests (40 tests)
2. Builds the Angular frontend
3. Builds and pushes Docker images to GitHub Container Registry
4. Creates a database backup on the server
5. Pulls new images and restarts containers

To enable auto-deploy, add these secrets to your GitHub repository (**Settings → Secrets → Actions**):

| Secret | Description |
|---|---|
| `DEPLOY_HOST` | Your server IP address |
| `DEPLOY_SSH_KEY` | Private SSH key for server access |
| `DEPLOY_SSH_PORT` | SSH port (e.g. `22`) |

---

## Project Structure

```
/backend
  /src/HiringProcess.Api/
    /Common/            — Result<T>, PagedResult, extensions
    /Infrastructure/    — AppDbContext, FileStorage, Migrations
    /Features/
      /Auth/            — Register, Login, Google OAuth handlers
      /HiringProcesses/ — CRUD handlers + controller
  /tests/HiringProcess.Tests/

/frontend
  /src/app/
    /core/auth/         — AuthService, JwtInterceptor, AuthGuard
    /core/api/          — API service, models
    /core/i18n/         — TranslationService, translations (en/ru/uk)
    /features/auth/     — Login, Register, Reset password pages
    /features/hiring-processes/ — List, FormDialog, ColumnSelector
    /features/settings/ — Settings page
    /shared/            — ConfirmDialog

/.github/workflows/ci.yml
/docker-compose.yml
/.env.example
```

---

## Environment Variables Reference

| Variable | Required | Description |
|---|---|---|
| `POSTGRES_PASSWORD` | ✅ | PostgreSQL password |
| `JWT_SECRET` | ✅ | JWT signing key (min 32 chars) |
| `APP_FRONTEND_URL` | ✅ | Frontend URL for CORS and email links |
| `GOOGLE_CLIENT_ID` | ➖ | Enables Google OAuth sign-in |
| `RESEND_API_KEY` | ➖ | Enables transactional emails (Resend.com) |
| `RESEND_FROM_EMAIL` | ➖ | Sender address for outgoing emails |
