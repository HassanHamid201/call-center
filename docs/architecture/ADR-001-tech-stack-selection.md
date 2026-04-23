# ADR-001: Technology Stack Selection

## Status
Accepted

## Context
Tadawi Medical Group needs a centralized internal information portal for employees. The system must support rapid prototype development while maintaining a path to production scalability.

## Decision

### Backend
- **.NET 10 Preview** with Minimal APIs - latest LTS track, excellent performance, native OpenAPI support
- **EF Core 10** with PostgreSQL 16 - mature ORM, strong PostgreSQL support, JSONB capabilities
- **Redis 7** - caching layer, session storage, rate limiting
- **JWT Bearer Authentication** - stateless auth suitable for SPA frontend

### Frontend
- **React 19** - latest stable with concurrent features
- **TypeScript** - type safety across the full stack
- **Vite** - fast dev server and optimized builds
- **Tailwind CSS v3** - utility-first styling for rapid UI development
- **TanStack Query v5** - server state management with caching
- **Zustand** - lightweight client state management

### Infrastructure
- **Docker Compose** - local development environment
- **Git** - source control with feature branch workflow

## Consequences
- .NET 10 Preview requires monitoring for breaking changes before production
- React 19 + TypeScript provides excellent developer experience
- PostgreSQL JSONB enables flexible audit logging
- Redis simplifies caching and rate limiting implementation

## Alternatives Considered
- Node.js/NestJS: Less performant for CPU-bound operations
- Next.js: SSR not required for internal portal; adds complexity
- SQL Server: Higher licensing costs; PostgreSQL preferred for JSON features
