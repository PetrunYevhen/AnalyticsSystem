# AnalyticSystem

Multi-tenant SaaS analytics platform for e-commerce businesses.

## Features

- Customer analytics: LTV, CAC, retention rate, churn rate
- Order management and CSV import
- Marketing campaign tracking and expense analysis
- Unit economics dashboard (AOV, ARPU, purchase frequency)
- Role-based access control (Admin / Analyst)
- API key authentication for server-to-server integrations
- Redis caching with tenant-scoped cache invalidation

## Tech stack

**Backend:** ASP.NET Core 8, EF Core, Dapper, MediatR, FluentValidation, Autofac, PostgreSQL, Redis
**Frontend:** React 19, Vite, TanStack Query, shadcn/ui, Recharts, Tailwind CSS v4

## Getting started

### Prerequisites
- .NET 8 SDK
- Node.js 20+
- PostgreSQL 15+
- Redis

### Backend

```bash
cd backend
dotnet restore
dotnet build
```

Run migrations:
```bash
dotnet ef database update \
  --project src/Analytics/Analytics.Infrastructure \
  --startup-project src/Analytics/Analytics.API
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

### Docker

```bash
cp .env.example .env  # fill in secrets
docker compose up --build
```

See `.env.example` for required environment variables.
