# Tadawi Medical Group - Call Center Management System

A modern, full-stack monorepo for managing Tadawi Medical Group's call center operations including branch management, doctor directory, and patient inquiries.

## Project Structure

```
.
├── backend/          # .NET 10 Minimal API with EF Core 10
├── frontend/         # React 19 + TypeScript + Vite + Tailwind CSS v4
├── infra/            # Infrastructure as code (Docker, Kubernetes configs)
├── docs/             # Architecture, API documentation, and development guides
└── README.md         # This file
```

## Tech Stack

### Backend
- **.NET 10 Preview** with Minimal APIs
- **Entity Framework Core 10** for data access
- **PostgreSQL 16** as the primary database
- **Redis 7** for caching and session storage
- **JWT Bearer** authentication with role-based claims
- **OpenAPI 3.1** with Swagger UI

### Frontend
- **React 19** with TypeScript
- **Vite** for fast development and building
- **Tailwind CSS v3** for styling
- **TanStack Query v5** for server state management
- **Zustand** for client state management
- **Lucide React** for icons

## Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 10 SDK (optional, for local backend development)
- Node.js 20+ (optional, for local frontend development)

### Run with Docker Compose

```bash
# Start infrastructure services
 docker compose up -d postgres redis adminer

# Start backend (from backend/ directory)
cd backend/src/Api
dotnet run

# Start frontend (from frontend/ directory)
cd frontend
npm install
npm run dev
```

### Access Points
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Adminer (DB UI)**: http://localhost:8080

## Default Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@tadawi.med | (seeded hash - use registration endpoint) |

## API Overview

| Endpoint | Description |
|----------|-------------|
| `POST /api/auth/login` | Authenticate and receive JWT |
| `GET /api/auth/me` | Current user profile |
| `GET /api/branches` | List branches |
| `GET /api/doctors` | List doctors |
| `GET /api/search` | Global search |
| `GET /health` | Health check |

## Development

### Backend
```bash
cd backend/src/Api
dotnet restore
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```

## License
Internal - Tadawi Medical Group
