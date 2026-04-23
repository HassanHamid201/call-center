# Tadawi Medical Group Portal – Prototype Build PRD

## Tasks

### Sprint 0: Foundation
- [ ] Setup monorepo structure with /backend, /frontend, /infra, /docs folders and root README
- [ ] Create docker-compose.yml with PostgreSQL 16, Redis 7, and adminer services
- [ ] Scaffold .NET 10 minimal API in /backend with Program.cs, health check < 50ms baseline
- [ ] Scaffold Vite + React 19 + TypeScript in /frontend with Tailwind CSS v4, ESLint, Prettier
- [ ] Write ADR-001 tech stack selection doc and coding standards doc in /docs/architecture

### Sprint 1: Database & Backend Core
- [ ] Design PostgreSQL schema for Branches, Doctors, Sectors, Specialties, Users with EF Core 10
- [ ] Create EF Core DbContext, entity configurations, and initial migration with seed data
- [ ] Implement CRUD API controllers for Branch, Doctor, Sector, Specialty with DTOs and validation
- [ ] Setup OpenAPI 3.1 + Swagger UI, XML docs, /health and /ready endpoints

### Sprint 2: Auth, Search & Cache
- [ ] Implement JWT authentication service with login/register endpoints and password hashing
- [ ] Build global search API with filtering (name, city, specialty, status) and pagination
- [ ] Configure Redis connection multiplexer, generic cache service, cache-aside pattern, rate limiting 100 req/min
- [ ] Implement RBAC middleware with role-based policies, claims-based access, endpoint authorization

### Sprint 3: Frontend Foundation
- [ ] Build layout shell with responsive header, sidebar, mobile drawer, breadcrumb, user menu, dark mode toggle
- [ ] Setup React Router v7 with route guards, lazy loading, Zustand stores for auth and branch context
- [ ] Create API client with Axios interceptors, TanStack Query v5, toast error handling, mock service worker
- [ ] Generate TypeScript types from OpenAPI spec and create useApi hooks

### Sprint 4: Frontend Views
- [ ] Build Branch Management views: list (table/card), detail with doctors, search/filter, create/edit form
- [ ] Build Doctor Directory views: list with contact actions, detail profile, search by name/specialty/branch/language, create/edit form
- [ ] Build Search & Filter page with global search bar, advanced filters, result cards
- [ ] Build Specialty/Sector views and User Dashboard with role-based widgets

### Sprint 5: Polish & QA
- [ ] Implement audit logging interceptor for entity changes, user actions, IP/UA capture, admin retrieval endpoint
- [ ] Add E2E tests covering login flow, branch CRUD, doctor search, and responsive layout
- [ ] Final integration test, bug fixes, and prototype demo preparation
