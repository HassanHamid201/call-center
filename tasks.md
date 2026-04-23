# Tasks Document – Tadawi Medical Group Portal
## Prototype Phase (Phase 1) – Solution Architect Breakdown

**Generated From:** BRD v1.0 – Tadawi Medical Group Internal Information Portal  
**Date:** April 23, 2026  
**Status:** Draft for SA Agent Review  
**Scope:** Prototype Phase (Weeks 1-6)

---

## Document Structure

1. [Task Dependency Graph](#1-task-dependency-graph)
2. [Team: Product Owner](#2-team-product-owner)
3. [Team: Solution Architecture](#3-team-solution-architecture)
4. [Team: Backend Development](#4-team-backend-development)
5. [Team: Frontend Development](#5-team-frontend-development)
6. [Team: Database Administration](#6-team-database-administration)
7. [Team: DevOps & Infrastructure](#7-team-devops--infrastructure)
8. [Team: Quality Assurance](#8-team-quality-assurance)
9. [Team: Security Engineering](#9-team-security-engineering)
10. [Team: Operations](#10-team-operations)
11. [Cross-Team Integration Points](#11-cross-team-integration-points)
12. [Sprint Timeline](#12-sprint-timeline)
13. [Risk & Dependency Matrix](#13-risk--dependency-matrix)

---

## 1. Task Dependency Graph

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SPRINT 0 (Week 1)                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐               │
│  │ PO-001: Backlog │  │ SA-001: Arch    │  │ DEV-001: CI/CD  │               │
│  │   Grooming      │  │   Foundation    │  │   Pipeline      │               │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘               │
│           │                    │                    │                        │
│           └────────────────────┼────────────────────┘                        │
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │ SA-002: Tech Stack Validation + DEV-002: Local Environment   │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                │                                             │
└────────────────────────────────┼─────────────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SPRINT 1 (Week 2)                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐               │
│  │ DBA-001: Schema │  │ BE-001: EF Core │  │ SEC-001: Threat │               │
│  │   Design        │  │   Setup         │  │   Modeling      │               │
│  └────────┬────────┘  └────────┬────────┘  └─────────────────┘               │
│           │                    │                                            │
│           └────────────────────┼────────────────────────────────────────────┘
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │ BE-002: Domain Models + DBA-002: Migration Scripts          │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                │                                             │
└────────────────────────────────┼─────────────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SPRINT 2 (Week 3)                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐               │
│  │ BE-003: Auth    │  │ BE-004: Search  │  │ BE-005: Redis   │               │
│  │   Service       │  │   API           │  │   Integration   │               │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘               │
│           │                    │                    │                        │
│           └────────────────────┼────────────────────┘                        │
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │ SEC-002: Auth Hardening + BE-006: RBAC Middleware           │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                │                                             │
└────────────────────────────────┼─────────────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SPRINT 3 (Week 4)                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐               │
│  │ FE-001: React   │  │ FE-002: API     │  │ FE-003: Layout  │               │
│  │   Scaffold      │  │   Client        │  │   Shell         │               │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘               │
│           │                    │                    │                        │
│           └────────────────────┼────────────────────┘                        │
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │ FE-004: Routing + State Management + SEC-003: FE Security │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                │                                             │
└────────────────────────────────┼─────────────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SPRINT 4 (Week 5)                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐               │
│  │ FE-005: Branch  │  │ FE-006: Doctor  │  │ FE-007: Search  │               │
│  │   Views         │  │   Views         │  │   & Filters     │               │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘               │
│           │                    │                    │                        │
│           └────────────────────┼────────────────────┘                        │
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │ FE-008: Specialty/Sector Views + FE-009: User Dashboard     │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                │                                             │
└────────────────────────────────┼─────────────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SPRINT 5 (Week 6)                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐               │
│  │ QA-001: E2E     │  │ QA-002: Perf    │  │ SEC-004: Pen    │               │
│  │   Testing       │  │   Testing       │  │   Test Scope    │               │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘               │
│           │                    │                    │                        │
│           └────────────────────┼────────────────────┘                        │
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │ QA-003: Bug Fix Verification + OP-001: Monitoring Setup     │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                │                                             │
└────────────────────────────────┼─────────────────────────────────────────────┘
                                 │
                                 ▼
                        ┌─────────────────┐
                        │  PROTOTYPE DEMO  │
                        │  Go/No-Go Gate   │
                        └─────────────────┘
```

---

## 2. Team: Product Owner

**Agent:** PO Agent  
**Responsibility:** Requirements refinement, backlog management, stakeholder communication, acceptance criteria

### Task PO-001: Backlog Grooming & Story Creation
- **Priority:** P0
- **Sprint:** 0
- **Dependencies:** None (kickoff task)
- **Blocked By:** None
- **Blocks:** All FE/BE development tasks
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] All BRD functional requirements converted to user stories
  - [ ] Acceptance criteria defined per story (Given/When/Then format)
  - [ ] Story points assigned using Fibonacci scale
  - [ ] Stories prioritized by business value and technical dependency
  - [ ] Definition of Ready (DoR) checklist created
- **Deliverables:**
  - `docs/backlog/user-stories.md` – All stories with AC
  - `docs/backlog/sprint-plan.md` – Sprint 0-5 plan
  - `docs/definition-of-ready.md` – DoR template

### Task PO-002: Stakeholder Communication Framework
- **Priority:** P0
- **Sprint:** 0
- **Dependencies:** PO-001
- **Blocked By:** None
- **Blocks:** Sprint review ceremonies
- **Estimated Effort:** 1 day
- **Acceptance Criteria:**
  - [ ] Stakeholder contact matrix established
  - [ ] Demo schedule defined (bi-weekly)
  - [ ] Feedback collection template created
  - [ ] Escalation path documented
- **Deliverables:**
  - `docs/stakeholder/communication-plan.md`
  - `docs/stakeholder/feedback-template.md`

### Task PO-003: Sprint Review & Demo Preparation
- **Priority:** P1
- **Sprint:** Per sprint (1-5)
- **Dependencies:** All team sprint deliverables
- **Blocked By:** Sprint completion
- **Blocks:** Phase 2 approval
- **Estimated Effort:** 1 day per sprint
- **Acceptance Criteria:**
  - [ ] Demo script prepared with user scenarios
  - [ ] Test data seeded for realistic demos
  - [ ] Stakeholder feedback documented
  - [ ] Go/no-go recommendation provided
- **Deliverables:**
  - `docs/demos/sprint-{N}-demo-script.md`
  - `docs/demos/sprint-{N}-feedback.md`

### Task PO-004: Phase 2 Scope Definition
- **Priority:** P2
- **Sprint:** 5
- **Dependencies:** PO-003 (Sprint 5 demo)
- **Blocked By:** Prototype approval
- **Blocks:** Phase 2 kickoff
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Phase 2 user stories drafted
  - [ ] UX Agent briefing document prepared
  - [ ] Technical debt items catalogued
  - [ ] Budget/resource estimates updated
- **Deliverables:**
  - `docs/phase2/scope-definition.md`
  - `docs/phase2/ux-agent-briefing.md`

---

## 3. Team: Solution Architecture

**Agent:** SA Agent  
**Responsibility:** Technical design, architecture decisions, integration patterns, team coordination

### Task SA-001: Architecture Foundation & Repository Structure
- **Priority:** P0
- **Sprint:** 0
- **Dependencies:** None
- **Blocked By:** None
- **Blocks:** SA-002, all development tasks
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Monorepo structure defined (`/backend`, `/frontend`, `/infra`, `/docs`)
  - [ ] Branching strategy documented (GitFlow vs trunk-based)
  - [ ] Coding standards documented (C# + TypeScript)
  - [ ] API contract conventions established
  - [ ] Architecture Decision Record (ADR) template created
- **Deliverables:**
  - `docs/architecture/repo-structure.md`
  - `docs/architecture/branching-strategy.md`
  - `docs/architecture/coding-standards.md`
  - `docs/architecture/adr-template.md`
  - `README.md` (root level)

### Task SA-002: Technology Stack Validation & Proof of Concept
- **Priority:** P0
- **Sprint:** 0
- **Dependencies:** SA-001
- **Blocked By:** None
- **Blocks:** All implementation tasks
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] .NET 10 Preview validated for production readiness
  - [ ] React 19 + TypeScript scaffold tested
  - [ ] PostgreSQL 16 + EF Core 10 compatibility verified
  - [ ] Redis 7 connection patterns validated
  - [ ] Docker Compose local stack functional
  - [ ] Performance baseline established (hello-world API < 50ms)
- **Deliverables:**
  - `docs/architecture/adr-001-tech-stack-selection.md`
  - `docs/architecture/poc-results.md`
  - `docker-compose.yml` (local dev stack)
  - `backend/src/Api/Program.cs` (minimal API scaffold)
  - `frontend/src/App.tsx` (React scaffold)

### Task SA-003: API Design & OpenAPI Specification
- **Priority:** P0
- **Sprint:** 1-2
- **Dependencies:** SA-002, DBA-001 (schema stable)
- **Blocked By:** DBA-001
- **Blocks:** BE-002, BE-003, BE-004
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] All API endpoints defined per BRD Section 5.6
  - [ ] Request/response DTOs specified
  - [ ] Error response standard documented (RFC 7807 Problem Details)
  - [ ] Pagination standard defined (cursor vs offset)
  - [ ] OpenAPI 3.1 YAML generated
  - [ ] FE-BE contract review completed
- **Deliverables:**
  - `docs/api/openapi.yaml`
  - `docs/api/endpoint-specifications.md`
  - `docs/api/error-handling.md`
  - `docs/api/pagination-standard.md`

### Task SA-004: Integration Architecture & Cross-Team Coordination
- **Priority:** P1
- **Sprint:** 2-5
- **Dependencies:** SA-003, BE-003 (auth stable)
- **Blocked By:** BE-003
- **Blocks:** FE-002 (API client generation)
- **Estimated Effort:** Ongoing (2 days per sprint)
- **Acceptance Criteria:**
  - [ ] Weekly architecture review meetings held
  - [ ] Cross-team blockers resolved within 24 hours
  - [ ] API contract changes communicated immediately
  - [ ] Integration test strategy defined
  - [ ] FE-BE mock server established for parallel development
- **Deliverables:**
  - `docs/architecture/integration-plan.md`
  - `docs/architecture/weekly-reviews.md`
  - `backend/tests/MockServer/` (mock API for FE development)

### Task SA-005: Performance & Scalability Planning
- **Priority:** P1
- **Sprint:** 3
- **Dependencies:** SA-002, BE-004 (search API)
- **Blocked By:** BE-004
- **Blocks:** DBA-003 (index optimization)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Load testing strategy defined (k6/Artillery)
  - [ ] Scaling thresholds documented (when to scale API vs DB)
  - [ ] Caching strategy validated against BRD Section 9.3
  - [ ] Database read replica strategy documented
  - [ ] CDN strategy for static assets defined
- **Deliverables:**
  - `docs/architecture/performance-plan.md`
  - `docs/architecture/scaling-policy.md`
  - `infra/k6/load-tests.js`

---

## 4. Team: Backend Development

**Agent:** BE Agent  
**Responsibility:** .NET 10 API development, business logic, EF Core, Redis integration

### Task BE-001: EF Core Setup & Database Context
- **Priority:** P0
- **Sprint:** 1
- **Dependencies:** DBA-001 (schema approved), SA-002 (stack validated)
- **Blocked By:** DBA-001, SA-002
- **Blocks:** BE-002, BE-003, BE-004, BE-005
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] DbContext configured with PostgreSQL provider
  - [ ] Entity configurations (Fluent API) for all tables
  - [ ] Connection string management (appsettings + env vars)
  - [ ] Migration strategy documented
  - [ ] Unit of work pattern implemented
- **Deliverables:**
  - `backend/src/Infrastructure/Data/ApplicationDbContext.cs`
  - `backend/src/Infrastructure/Data/Configurations/*.cs`
  - `backend/src/Api/appsettings.Development.json`
  - `docs/backend/ef-core-setup.md`

### Task BE-002: Domain Models & Basic CRUD APIs
- **Priority:** P0
- **Sprint:** 1-2
- **Dependencies:** BE-001, SA-003 (API spec)
- **Blocked By:** BE-001, SA-003
- **Blocks:** FE-002 (API client), QA-001
- **Estimated Effort:** 4 days
- **Acceptance Criteria:**
  - [ ] Domain entities implemented (Branch, Doctor, Specialty, Sector)
  - [ ] Repository pattern with generic base
  - [ ] CRUD endpoints for all entities per BRD Section 5.6
  - [ ] Input validation using DataAnnotations + .NET 10 `AddValidation()`
  - [ ] AutoMapper DTO mapping configured
  - [ ] Basic unit tests for controllers (> 80% coverage)
- **Deliverables:**
  - `backend/src/Domain/Entities/*.cs`
  - `backend/src/Application/DTOs/*.cs`
  - `backend/src/Api/Controllers/*.cs`
  - `backend/tests/Unit/Controllers/*.cs`

### Task BE-003: Authentication Service & JWT Implementation
- **Priority:** P0
- **Sprint:** 2
- **Dependencies:** BE-002 (users table exists), SEC-001 (threat model)
- **Blocked By:** BE-002, SEC-001
- **Blocks:** BE-006 (RBAC), FE-003 (login UI), SEC-002
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] ASP.NET Core Identity integration
  - [ ] JWT token generation (access + refresh)
  - [ ] Password hashing (bcrypt, cost factor 12)
  - [ ] Login/logout endpoints
  - [ ] Token refresh endpoint
  - [ ] Password reset flow (email token)
  - [ ] Session storage in Redis
- **Deliverables:**
  - `backend/src/Api/Controllers/AuthController.cs`
  - `backend/src/Infrastructure/Services/JwtService.cs`
  - `backend/src/Infrastructure/Services/PasswordService.cs`
  - `docs/backend/auth-implementation.md`

### Task BE-004: Search API & Advanced Filtering
- **Priority:** P0
- **Sprint:** 2
- **Dependencies:** BE-002, DBA-002 (indexes created)
- **Blocked By:** BE-002, DBA-002
- **Blocks:** FE-007 (search UI), SA-005
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] Full-text search across Branches, Doctors, Specialties
  - [ ] PostgreSQL `tsvector` search implementation
  - [ ] Multi-criteria filtering (specialty + branch + status + language)
  - [ ] Pagination with metadata
  - [ ] Sorting (relevance, name, experience)
  - [ ] Search result caching in Redis (10min TTL)
  - [ ] Autocomplete endpoint for search suggestions
- **Deliverables:**
  - `backend/src/Api/Controllers/SearchController.cs`
  - `backend/src/Application/Services/SearchService.cs`
  - `backend/tests/Unit/Services/SearchServiceTests.cs`

### Task BE-005: Redis Integration & Caching Layer
- **Priority:** P0
- **Sprint:** 2
- **Dependencies:** BE-002, SA-002 (Redis validated)
- **Blocked By:** BE-002
- **Blocks:** BE-004 (search cache), BE-006 (session store)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Redis connection multiplexer configured
  - [ ] Generic cache service implemented
  - [ ] Cache-aside pattern for entity lookups
  - [ ] Cache invalidation on entity mutations
  - [ ] Rate limiting middleware (100 req/min)
  - [ ] Distributed session storage
  - [ ] Health checks for Redis connectivity
- **Deliverables:**
  - `backend/src/Infrastructure/Services/CacheService.cs`
  - `backend/src/Api/Middleware/RateLimitingMiddleware.cs`
  - `backend/src/Api/Middleware/CacheInvalidationMiddleware.cs`
  - `docs/backend/redis-configuration.md`

### Task BE-006: RBAC Middleware & Authorization
- **Priority:** P1
- **Sprint:** 2-3
- **Dependencies:** BE-003 (auth service), SEC-002 (auth hardening)
- **Blocked By:** BE-003, SEC-002
- **Blocks:** FE-009 (role-based dashboard)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Role-based authorization policies
  - [ ] Claims-based access control
  - [ ] Endpoint-level authorization attributes
  - [ ] Admin-only endpoints protected
  - [ ] Row-level security preparation (future patient data)
- **Deliverables:**
  - `backend/src/Api/Authorization/Policies.cs`
  - `backend/src/Api/Authorization/Handlers/*.cs`
  - `backend/src/Api/Controllers/AdminController.cs`
  - `docs/backend/rbac-implementation.md`

### Task BE-007: Audit Logging Service
- **Priority:** P1
- **Sprint:** 3
- **Dependencies:** BE-002, BE-006 (RBAC)
- **Blocked By:** BE-002, BE-006
- **Blocks:** FE-010 (audit log UI - Phase 2)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Audit log interceptor for entity changes
  - [ ] User action logging (view, create, update, delete)
  - [ ] IP address and user agent capture
  - [ ] Old/new value diff storage (JSONB)
  - [ ] Async logging to prevent blocking
  - [ ] Admin endpoint for audit retrieval
- **Deliverables:**
  - `backend/src/Infrastructure/Services/AuditService.cs`
  - `backend/src/Infrastructure/Interceptors/AuditInterceptor.cs`
  - `backend/src/Api/Controllers/AuditController.cs`

### Task BE-008: API Documentation & Health Checks
- **Priority:** P1
- **Sprint:** 3-4
- **Dependencies:** BE-002 (all endpoints)
- **Blocked By:** BE-002
- **Blocks:** FE-002 (API client generation)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] OpenAPI 3.1 spec auto-generated from code
  - [ ] XML documentation for all public APIs
  - [ ] Swagger UI configured (dev environment)
  - [ ] Health check endpoints (/health, /ready)
  - [ ] Database connectivity health check
  - [ ] Redis connectivity health check
- **Deliverables:**
  - `backend/src/Api/Program.cs` (OpenAPI + Swagger)
  - `backend/src/Api/HealthChecks/*.cs`
  - `docs/api/swagger-setup.md`

---

## 5. Team: Frontend Development

**Agent:** FE Agent  
**Responsibility:** React 19 application, component development, API integration, state management

### Task FE-001: React Scaffold & Project Setup
- **Priority:** P0
- **Sprint:** 3
- **Dependencies:** SA-002 (stack validated), SA-001 (repo structure)
- **Blocked By:** SA-002
- **Blocks:** All FE tasks
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Vite + React 19 + TypeScript project scaffolded
  - [ ] ESLint + Prettier configuration
  - [ ] Tailwind CSS v4 installed and configured
  - [ ] Absolute path aliases configured (`@/components`, `@/hooks`)
  - [ ] Environment variable management (.env templates)
  - [ ] Build pipeline verified (dev + prod)
- **Deliverables:**
  - `frontend/vite.config.ts`
  - `frontend/tsconfig.json`
  - `frontend/tailwind.config.js`
  - `frontend/.eslintrc.js`
  - `frontend/package.json`

### Task FE-002: API Client & Type Generation
- **Priority:** P0
- **Sprint:** 3
- **Dependencies:** BE-008 (OpenAPI spec), SA-003 (API contract)
- **Blocked By:** BE-008
- **Blocks:** FE-005, FE-006, FE-007, FE-008, FE-009
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] OpenAPI generator configured (TypeScript fetch client)
  - [ ] Axios instance with interceptors (auth token, error handling)
  - [ ] TanStack Query v5 setup (default options, cache config)
  - [ ] API error handling standard (toast notifications)
  - [ ] Loading state management pattern
  - [ ] Mock service worker for offline development
- **Deliverables:**
  - `frontend/src/api/generated/` (auto-generated types)
  - `frontend/src/api/client.ts`
  - `frontend/src/hooks/useApi.ts`
  - `frontend/src/mocks/handlers.ts`

### Task FE-003: Layout Shell & Navigation
- **Priority:** P0
- **Sprint:** 3
- **Dependencies:** FE-001
- **Blocked By:** FE-001
- **Blocks:** FE-004, FE-005, FE-006, FE-007, FE-008, FE-009
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] App shell with header, sidebar, main content area
  - [ ] Responsive layout (mobile drawer, desktop sidebar)
  - [ ] Navigation menu with role-based items
  - [ ] Breadcrumb component
  - [ ] User menu (profile, logout)
  - [ ] Branch selector (contextual)
  - [ ] Dark mode toggle (preparation for Phase 2)
- **Deliverables:**
  - `frontend/src/components/Layout/AppShell.tsx`
  - `frontend/src/components/Layout/Sidebar.tsx`
  - `frontend/src/components/Layout/Header.tsx`
  - `frontend/src/components/Layout/Breadcrumb.tsx`

### Task FE-004: Routing & State Management
- **Priority:** P0
- **Sprint:** 3-4
- **Dependencies:** FE-003, FE-002 (API client)
- **Blocked By:** FE-003
- **Blocks:** FE-005, FE-006, FE-007, FE-008, FE-009
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] React Router v7 setup with route definitions
  - [ ] Route guards (auth required, role-based)
  - [ ] Lazy loading for code splitting
  - [ ] Zustand store for global state (auth, branch context)
  - [ ] URL state synchronization for filters
  - [ ] 404 and error boundary pages
- **Deliverables:**
  - `frontend/src/routes/index.tsx`
  - `frontend/src/routes/guards.tsx`
  - `frontend/src/store/authStore.ts`
  - `frontend/src/store/branchStore.ts`
  - `frontend/src/components/ErrorBoundary.tsx`

### Task FE-005: Branch Management Views
- **Priority:** P0
- **Sprint:** 4
- **Dependencies:** FE-004, FE-002, BE-002 (branch API)
- **Blocked By:** FE-004, FE-002, BE-002
- **Blocks:** QA-001
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] Branch list view (table + card toggle)
  - [ ] Branch detail view with associated doctors
  - [ ] Branch search and filter (city, status, services)
  - [ ] Branch create/edit form with validation
  - [ ] Branch status indicators
  - [ ] Map placeholder (geolocation display)
- **Deliverables:**
  - `frontend/src/pages/Branches/BranchList.tsx`
  - `frontend/src/pages/Branches/BranchDetail.tsx`
  - `frontend/src/pages/Branches/BranchForm.tsx`
  - `frontend/src/components/Branches/BranchCard.tsx`
  - `frontend/src/components/Branches/BranchFilters.tsx`

### Task FE-006: Doctor Directory Views
- **Priority:** P0
- **Sprint:** 4
- **Dependencies:** FE-004, FE-002, BE-002 (doctor API)
- **Blocked By:** FE-004, FE-002, BE-002
- **Blocks:** QA-001
- **Estimated Effort:** 4 days
- **Acceptance Criteria:**
  - [ ] Doctor list view with quick contact actions
  - [ ] Doctor detail view with full profile
  - [ ] Doctor search (name, specialty, branch, language)
  - [ ] Doctor create/edit form
  - [ ] Availability status display
  - [ ] "Copy Contact" button for call center use case
  - [ ] Printable contact card generation
- **Deliverables:**
  - `frontend/src/pages/Doctors/DoctorList.tsx`
  - `frontend/src/pages/Doctors/DoctorDetail.tsx`
  - `frontend/src/pages/Doctors/DoctorForm.tsx`
  - `frontend/src/components/Doctors/DoctorCard.tsx`
  - `frontend/src/components/Doctors/ContactCard.tsx`

### Task FE-007: Search & Discovery UI
- **Priority:** P0
- **Sprint:** 4-5
- **Dependencies:** FE-004, FE-002, BE-004 (search API)
- **Blocked By:** FE-004, FE-002, BE-004
- **Blocks:** QA-001
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] Global search bar (Cmd+K shortcut)
  - [ ] Search results page with relevance ranking
  - [ ] Advanced filters sidebar (multi-select, ranges)
  - [ ] Autocomplete dropdown with suggestions
  - [ ] Recent searches history
  - [ ] Saved favorites per user
  - [ ] Empty states and loading skeletons
- **Deliverables:**
  - `frontend/src/components/Search/GlobalSearch.tsx`
  - `frontend/src/components/Search/SearchResults.tsx`
  - `frontend/src/components/Search/SearchFilters.tsx`
  - `frontend/src/components/Search/Autocomplete.tsx`
  - `frontend/src/hooks/useSearch.ts`

### Task FE-008: Specialty & Sector Views
- **Priority:** P1
- **Sprint:** 4-5
- **Dependencies:** FE-004, FE-002, BE-002 (specialty API)
- **Blocked By:** FE-004, FE-002, BE-002
- **Blocks:** QA-001
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Specialty hierarchy tree view
  - [ ] Specialty detail with associated doctors
  - [ ] Sector classification display
  - [ ] Specialty create/edit form
  - [ ] Parent-child relationship management
- **Deliverables:**
  - `frontend/src/pages/Specialties/SpecialtyList.tsx`
  - `frontend/src/pages/Specialties/SpecialtyDetail.tsx`
  - `frontend/src/pages/Specialties/SpecialtyForm.tsx`
  - `frontend/src/components/Specialties/SpecialtyTree.tsx`

### Task FE-009: Role-Based Dashboard & User Management
- **Priority:** P1
- **Sprint:** 4-5
- **Dependencies:** FE-004, FE-002, BE-006 (RBAC), BE-003 (auth)
- **Blocked By:** FE-004, FE-002, BE-006, BE-003
- **Blocks:** QA-001
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] Role-based landing pages (6 personas)
  - [ ] Login page with form validation
  - [ ] Password reset flow UI
  - [ ] User profile page
  - [ ] Admin user management (list, edit, deactivate)
  - [ ] Session timeout warning
- **Deliverables:**
  - `frontend/src/pages/Dashboard/Dashboard.tsx`
  - `frontend/src/pages/Auth/Login.tsx`
  - `frontend/src/pages/Auth/ResetPassword.tsx`
  - `frontend/src/pages/Auth/Profile.tsx`
  - `frontend/src/pages/Admin/UserManagement.tsx`

### Task FE-010: Accessibility & Responsive Polish
- **Priority:** P1
- **Sprint:** 5
- **Dependencies:** All FE view tasks
- **Blocked By:** FE-005, FE-006, FE-007, FE-008, FE-009
- **Blocks:** QA-001
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] WCAG 2.1 AA compliance (semantic HTML, ARIA labels)
  - [ ] Keyboard navigation for all interactive elements
  - [ ] Screen reader testing with NVDA/VoiceOver
  - [ ] Color contrast validation (4.5:1 minimum)
  - [ ] Focus management and visible focus indicators
  - [ ] Reduced motion support (`prefers-reduced-motion`)
  - [ ] Mobile touch targets (44x44px minimum)
- **Deliverables:**
  - `frontend/src/components/Accessibility/SkipLink.tsx`
  - `frontend/src/components/Accessibility/LiveRegion.tsx`
  - `docs/frontend/accessibility-audit.md`

---

## 6. Team: Database Administration

**Agent:** DBA Agent  
**Responsibility:** PostgreSQL schema design, optimization, indexing, backup procedures

### Task DBA-001: Schema Design & Review
- **Priority:** P0
- **Sprint:** 1
- **Dependencies:** BRD Section 9.2 (schema defined)
- **Blocked By:** None
- **Blocks:** DBA-002, BE-001, BE-002, SA-003
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] Schema normalized to 3NF
  - [ ] Data types optimized (UUID PKs, JSONB for flexible data)
  - [ ] Foreign key constraints with CASCADE rules
  - [ ] Check constraints for enums (status, role)
  - [ ] Partitioning strategy for audit_logs (by month)
  - [ ] Schema review with SA and BE agents
- **Deliverables:**
  - `infra/database/schema.sql`
  - `docs/database/schema-design.md`
  - `docs/database/data-dictionary.md`

### Task DBA-002: Migration Scripts & Seeding
- **Priority:** P0
- **Sprint:** 1-2
- **Dependencies:** DBA-001
- **Blocked By:** DBA-001
- **Blocks:** BE-002 (entity mapping), QA-001 (test data)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] EF Core migrations generated
  - [ ] Seed data for development (10 branches, 50 doctors, 20 specialties)
  - [ ] Test data for QA (edge cases, large datasets)
  - [ ] Migration rollback procedures
  - [ ] Database versioning strategy
- **Deliverables:**
  - `backend/src/Infrastructure/Data/Migrations/`
  - `backend/src/Infrastructure/Data/Seeders/`
  - `docs/database/migration-guide.md`

### Task DBA-003: Index Optimization & Performance Tuning
- **Priority:** P1
- **Sprint:** 2-3
- **Dependencies:** DBA-002, BE-004 (search queries)
- **Blocked By:** DBA-002, BE-004
- **Blocks:** SA-005 (performance plan)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] B-tree indexes for foreign keys and frequent filters
  - [ ] GIN indexes for full-text search (`tsvector`)
  - [ ] Partial indexes for active records
  - [ ] Query execution plan review for top 10 queries
  - [ ] Index maintenance procedures (REINDEX schedule)
  - [ ] Slow query log configuration
- **Deliverables:**
  - `infra/database/indexes.sql`
  - `docs/database/index-strategy.md`
  - `docs/database/query-performance-report.md`

### Task DBA-004: Backup & Recovery Procedures
- **Priority:** P1
- **Sprint:** 3
- **Dependencies:** DBA-001
- **Blocked By:** DBA-001
- **Blocks:** OP-001 (monitoring setup)
- **Estimated Effort:** 1 day
- **Acceptance Criteria:**
  - [ ] Automated backup scripts (pg_dump + WAL archiving)
  - [ ] Backup schedule: Full daily, incremental every 6 hours
  - [ ] 30-day retention policy
  - [ ] Point-in-time recovery tested
  - [ ] Backup integrity verification
  - [ ] Disaster recovery runbook
- **Deliverables:**
  - `infra/database/backup-scripts/`
  - `docs/database/backup-recovery.md`
  - `docs/database/disaster-recovery-runbook.md`

---

## 7. Team: DevOps & Infrastructure

**Agent:** DevOps Agent  
**Responsibility:** CI/CD pipelines, Docker, Kubernetes, infrastructure as code

### Task DEV-001: CI/CD Pipeline Foundation
- **Priority:** P0
- **Sprint:** 0
- **Dependencies:** SA-001 (repo structure)
- **Blocked By:** SA-001
- **Blocks:** All build/test tasks
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] GitHub Actions workflows configured
  - [ ] Backend CI: build, test, SonarQube scan
  - [ ] Frontend CI: build, lint, test, bundle analysis
  - [ ] PR checks (status checks required)
  - [ ] Automated labeling and changelog generation
  - [ ] Artifact storage strategy
- **Deliverables:**
  - `.github/workflows/backend-ci.yml`
  - `.github/workflows/frontend-ci.yml`
  - `.github/workflows/pr-checks.yml`
  - `docs/devops/ci-cd-guide.md`

### Task DEV-002: Local Development Environment
- **Priority:** P0
- **Sprint:** 0
- **Dependencies:** SA-002 (stack validated)
- **Blocked By:** SA-002
- **Blocks:** All development tasks
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Docker Compose for full stack (API + DB + Redis + Frontend)
  - [ ] Hot reload for both backend and frontend
  - [ ] Volume mounts for local development
  - [ ] Environment variable templates (.env.example)
  - [ ] Database admin tools (pgAdmin, Redis Insight)
  - [ ] One-command startup (`make dev` or `npm run dev:all`)
- **Deliverables:**
  - `docker-compose.yml`
  - `docker-compose.override.yml`
  - `Makefile`
  - `.env.example`
  - `docs/devops/local-setup.md`

### Task DEV-003: Containerization & Image Optimization
- **Priority:** P1
- **Sprint:** 2-3
- **Dependencies:** DEV-002, BE-002 (API stable)
- **Blocked By:** DEV-002, BE-002
- **Blocks:** DEV-004 (K8s deployment)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Multi-stage Dockerfile for backend (build → runtime)
  - [ ] Multi-stage Dockerfile for frontend (build → nginx)
  - [ ] Image size optimization (< 200MB backend, < 50MB frontend)
  - [ ] Non-root user execution
  - [ ] Health check endpoints configured
  - [ ] Image scanning (Trivy) in CI pipeline
- **Deliverables:**
  - `backend/Dockerfile`
  - `frontend/Dockerfile`
  - `infra/docker/nginx.conf`
  - `docs/devops/container-guide.md`

### Task DEV-004: Kubernetes Manifests (Phase 2 Prep)
- **Priority:** P2
- **Sprint:** 5
- **Dependencies:** DEV-003, SA-005 (scaling plan)
- **Blocked By:** DEV-003, SA-005
- **Blocks:** OP-001 (monitoring)
- **Estimated Effort:** 3 days
- **Acceptance Criteria:**
  - [ ] Deployment manifests for API (replicas, resources, probes)
  - [ ] Service and Ingress configurations
  - [ ] ConfigMap and Secret management
  - [ ] Horizontal Pod Autoscaler (HPA) rules
  - [ ] PersistentVolumeClaims for PostgreSQL
  - [ ] Helm chart structure (optional but recommended)
- **Deliverables:**
  - `infra/k8s/api-deployment.yaml`
  - `infra/k8s/frontend-deployment.yaml`
  - `infra/k8s/postgres-statefulset.yaml`
  - `infra/k8s/redis-deployment.yaml`
  - `infra/k8s/ingress.yaml`
  - `docs/devops/kubernetes-guide.md`

---

## 8. Team: Quality Assurance

**Agent:** QA Agent  
**Responsibility:** Test strategy, automated testing, manual testing, performance testing

### Task QA-001: Test Strategy & Framework Setup
- **Priority:** P0
- **Sprint:** 1
- **Dependencies:** SA-001 (repo structure), DEV-001 (CI pipeline)
- **Blocked By:** SA-001, DEV-001
- **Blocks:** QA-002, QA-003
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Test pyramid defined (unit → integration → e2e)
  - [ ] Backend test framework: xUnit + Moq + FluentAssertions
  - [ ] Frontend test framework: Vitest + React Testing Library
  - [ ] E2E test framework: Playwright
  - [ ] Test data management strategy
  - [ ] Coverage thresholds configured (80% minimum)
- **Deliverables:**
  - `docs/qa/test-strategy.md`
  - `backend/tests/Directory.Build.props`
  - `frontend/vitest.config.ts`
  - `e2e/playwright.config.ts`

### Task QA-002: Automated Test Implementation
- **Priority:** P0
- **Sprint:** 3-5
- **Dependencies:** QA-001, BE-002 (API ready), FE-005 (views ready)
- **Blocked By:** QA-001, BE-002, FE-005
- **Blocks:** QA-003 (bug fix verification)
- **Estimated Effort:** Ongoing (3 days per sprint)
- **Acceptance Criteria:**
  - [ ] Backend unit tests: > 80% coverage for services/controllers
  - [ ] Backend integration tests: API endpoint validation
  - [ ] Frontend unit tests: Component rendering and interaction
  - [ ] Frontend integration tests: API mocking and state management
  - [ ] E2E tests: Critical user journeys (5 scenarios)
    - Login → Search Doctor → View Detail → Logout
    - Admin: Create Branch → Add Doctor → Verify Search
    - Call Center: Quick Contact Copy → Print Card
  - [ ] Visual regression tests (screenshot comparison)
- **Deliverables:**
  - `backend/tests/Unit/**/*.cs`
  - `backend/tests/Integration/**/*.cs`
  - `frontend/src/**/*.test.tsx`
  - `e2e/tests/critical-paths.spec.ts`
  - `e2e/tests/admin-workflows.spec.ts`

### Task QA-003: Performance & Load Testing
- **Priority:** P1
- **Sprint:** 5
- **Dependencies:** QA-002, SA-005 (performance plan)
- **Blocked By:** QA-002, SA-005
- **Blocks:** Phase 2 approval
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Load test: 150 concurrent users, 10-minute duration
  - [ ] API response time < 200ms (95th percentile)
  - [ ] Database connection pool stress test
  - [ ] Redis cache hit ratio > 90%
  - [ ] Frontend bundle load time < 2s (3G simulation)
  - [ ] Memory leak detection (heap snapshots)
- **Deliverables:**
  - `infra/k6/load-test.js`
  - `docs/qa/performance-test-report.md`
  - `docs/qa/load-test-results.md`

### Task QA-004: Bug Triage & Regression Suite
- **Priority:** P1
- **Sprint:** 5
- **Dependencies:** QA-002
- **Blocked By:** QA-002
- **Blocks:** Phase 2 approval
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Bug tracking process documented
  - [ ] Severity classification (Critical, High, Medium, Low)
  - [ ] Regression test suite automated
  - [ ] All P0/P1 bugs resolved or documented as known issues
  - [ ] Test execution report for prototype
- **Deliverables:**
  - `docs/qa/bug-triage-process.md`
  - `docs/qa/regression-suite.md`
  - `docs/qa/prototype-test-report.md`

---

## 9. Team: Security Engineering

**Agent:** SEC Agent  
**Responsibility:** Threat modeling, vulnerability assessment, compliance validation, hardening

### Task SEC-001: Threat Modeling (STRIDE)
- **Priority:** P0
- **Sprint:** 0
- **Dependencies:** BRD Section 11 (security requirements)
- **Blocked By:** None
- **Blocks:** SEC-002, BE-003 (auth design)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] STRIDE analysis completed for all system components
  - [ ] Threat catalog documented (Spoofing, Tampering, Repudiation, etc.)
  - [ ] Risk scoring (DREAD methodology)
  - [ ] Mitigation strategies defined per threat
  - [ ] Security requirements traceability matrix
- **Deliverables:**
  - `docs/security/threat-model.md`
  - `docs/security/stride-analysis.md`
  - `docs/security/risk-register.md`

### Task SEC-002: Authentication Hardening
- **Priority:** P0
- **Sprint:** 2
- **Dependencies:** SEC-001, BE-003 (auth service)
- **Blocked By:** SEC-001, BE-003
- **Blocks:** BE-006 (RBAC), FE-009 (login UI)
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] JWT security review (algorithm, expiration, refresh strategy)
  - [ ] Password policy enforcement (complexity, history)
  - [ ] Brute force protection (account lockout)
  - [ ] Secure session management
  - [ ] HTTPS enforcement (HSTS headers)
  - [ ] CORS policy configuration
- **Deliverables:**
  - `docs/security/auth-hardening.md`
  - `backend/src/Api/Security/SecurityHeaders.cs`
  - `backend/src/Api/Security/CorsPolicy.cs`

### Task SEC-003: Frontend Security Review
- **Priority:** P1
- **Sprint:** 4
- **Dependencies:** FE-003 (layout), FE-009 (auth UI)
- **Blocked By:** FE-003, FE-009
- **Blocks:** QA-001
- **Estimated Effort:** 1 day
- **Acceptance Criteria:**
  - [ ] XSS prevention (Content Security Policy)
  - [ ] CSRF token implementation
  - [ ] Secure storage review (no sensitive data in localStorage)
  - [ ] Dependency vulnerability scan (npm audit)
  - [ ] Input sanitization validation
- **Deliverables:**
  - `docs/security/frontend-security.md`
  - `frontend/Content-Security-Policy.md`
  - `frontend/.npmrc` (audit settings)

### Task SEC-004: Penetration Testing Scope
- **Priority:** P1
- **Sprint:** 5
- **Dependencies:** All implementation complete
- **Blocked By:** All BE/FE tasks
- **Blocks:** Phase 2 approval
- **Estimated Effort:** 2 days (scope definition, actual pen test Phase 2)
- **Acceptance Criteria:**
  - [ ] Penetration test plan documented
  - [ ] OWASP ZAP baseline scan executed
  - [ ] Dependency vulnerability scan (OWASP Dependency-Check)
  - [ ] Container image scan (Trivy)
  - [ ] Security scan results triaged
  - [ ] Critical vulnerabilities remediated or accepted with risk
- **Deliverables:**
  - `docs/security/pen-test-plan.md`
  - `docs/security/owasp-zap-report.md`
  - `docs/security/dependency-check-report.md`
  - `docs/security/container-scan-report.md`

---

## 10. Team: Operations

**Agent:** OP Agent  
**Responsibility:** Production monitoring, incident response, log aggregation, alerting

### Task OP-001: Monitoring & Observability Setup
- **Priority:** P1
- **Sprint:** 5
- **Dependencies:** DEV-004 (K8s manifests), DBA-004 (backup procedures)
- **Blocked By:** DEV-004, DBA-004
- **Blocks:** Phase 2 production readiness
- **Estimated Effort:** 2 days
- **Acceptance Criteria:**
  - [ ] Prometheus metrics collection configured
  - [ ] Grafana dashboards created (API, DB, Redis, Frontend)
  - [ ] Structured logging with Serilog (correlation IDs)
  - [ ] Log aggregation setup (ELK stack or cloud equivalent)
  - [ ] Alerting rules (PagerDuty/OpsGenie integration)
  - [ ] SLA monitoring (99.5% uptime tracking)
- **Deliverables:**
  - `infra/monitoring/prometheus-config.yaml`
  - `infra/monitoring/grafana-dashboards/`
  - `docs/operations/monitoring-setup.md`
  - `docs/operations/alerting-rules.md`

### Task OP-002: Incident Response Runbooks
- **Priority:** P2
- **Sprint:** 5
- **Dependencies:** OP-001
- **Blocked By:** OP-001
- **Blocks:** Phase 2 production readiness
- **Estimated Effort:** 1 day
- **Acceptance Criteria:**
  - [ ] Incident severity classification
  - [ ] Escalation matrix (on-call rotations)
  - [ ] Common incident runbooks (DB outage, API down, Redis failure)
  - [ ] Post-mortem template
  - [ ] Communication templates (status page updates)
- **Deliverables:**
  - `docs/operations/incident-response.md`
  - `docs/operations/runbooks/`
  - `docs/operations/post-mortem-template.md`

---

## 11. Cross-Team Integration Points

### Integration Matrix

| Integration Point | Teams Involved | Sprint | Trigger | Deliverable |
|-------------------|---------------|--------|---------|-------------|
| **API Contract Freeze** | SA + BE + FE | 2 | BE-002 complete | `docs/api/contract-v1.md` |
| **Auth Integration** | BE + FE + SEC | 3 | BE-003 + FE-009 ready | Working login flow |
| **Database Seeding** | DBA + BE + QA | 2 | DBA-002 complete | Dev + test datasets |
| **CI/CD Integration** | DevOps + All | 0-1 | DEV-001 complete | Automated builds |
| **Security Gate** | SEC + All | 5 | SEC-004 complete | Security clearance |
| **Performance Gate** | QA + SA + DBA | 5 | QA-003 complete | Performance baseline |
| **Prototype Demo** | PO + All | 5 | All sprint tasks | Stakeholder approval |

### Critical Path Dependencies

```
SA-001 (Repo) → SA-002 (Stack) → DBA-001 (Schema) → BE-001 (EF Core)
                                                          ↓
BE-002 (CRUD) → BE-003 (Auth) → BE-006 (RBAC) → FE-009 (Dashboard)
      ↓
BE-004 (Search) → FE-007 (Search UI)
      ↓
BE-005 (Redis) → FE-002 (API Client) → All FE Views
```

---

## 12. Sprint Timeline

### Sprint 0: Foundation (Week 1)

| Day | PO | SA | DevOps | SEC | Other |
|-----|----|----|--------|-----|-------|
| 1 | PO-001 Kickoff | SA-001 Repo | DEV-001 CI Setup | SEC-001 Start | – |
| 2 | PO-001 Continue | SA-001 Complete | DEV-001 Complete | SEC-001 Continue | – |
| 3 | PO-002 Comm Plan | SA-002 Stack POC | DEV-002 Local Env | SEC-001 Complete | – |
| 4 | PO-002 Complete | SA-002 Complete | DEV-002 Complete | – | – |
| 5 | Sprint Review | SA-003 Start | – | – | – |

**Sprint 0 Goal:** Development environment ready, architecture validated, team communication established.

### Sprint 1: Data Layer (Week 2)

| Day | DBA | BE | SA | QA |
|-----|-----|----|----|----|
| 1 | DBA-001 Start | – | SA-003 Continue | QA-001 Start |
| 2 | DBA-001 Continue | BE-001 Start | SA-003 Complete | QA-001 Continue |
| 3 | DBA-001 Complete | BE-001 Complete | – | QA-001 Complete |
| 4 | DBA-002 Start | BE-002 Start | – | – |
| 5 | DBA-002 Continue | BE-002 Continue | – | – |

**Sprint 1 Goal:** Database schema finalized, EF Core configured, basic CRUD APIs in progress.

### Sprint 2: Core Backend (Week 3)

| Day | BE | SEC | DBA | SA |
|-----|----|-----|----|----|
| 1 | BE-002 Complete | SEC-002 Start | DBA-002 Complete | SA-004 Start |
| 2 | BE-003 Start | SEC-002 Continue | DBA-003 Start | SA-004 Continue |
| 3 | BE-003 Continue | SEC-002 Continue | DBA-003 Continue | SA-004 Continue |
| 4 | BE-004 Start | SEC-002 Complete | DBA-003 Complete | SA-004 Continue |
| 5 | BE-004 Continue | – | – | SA-004 Continue |

**Sprint 2 Goal:** Authentication, search, and caching APIs functional. Security hardening in progress.

### Sprint 3: Frontend Scaffold (Week 4)

| Day | FE | BE | SA | SEC |
|-----|----|----|----|-----|
| 1 | FE-001 Start | BE-005 Start | SA-004 Continue | SEC-003 Start |
| 2 | FE-001 Complete | BE-005 Continue | SA-004 Complete | SEC-003 Continue |
| 3 | FE-002 Start | BE-005 Complete | SA-005 Start | SEC-003 Complete |
| 4 | FE-003 Start | BE-006 Start | SA-005 Continue | – |
| 5 | FE-003 Continue | BE-006 Continue | SA-005 Complete | – |

**Sprint 3 Goal:** React app scaffolded, API client generated, layout shell functional.

### Sprint 4: Frontend Features (Week 5)

| Day | FE | BE | QA |
|-----|----|----|----|
| 1 | FE-004 Start | BE-007 Start | QA-002 Start |
| 2 | FE-005 Start | BE-007 Continue | QA-002 Continue |
| 3 | FE-006 Start | BE-008 Start | QA-002 Continue |
| 4 | FE-007 Start | BE-008 Continue | QA-002 Continue |
| 5 | FE-008 Start | BE-008 Complete | QA-002 Continue |

**Sprint 4 Goal:** All core views implemented (branches, doctors, specialties, search).

### Sprint 5: Integration & Hardening (Week 6)

| Day | QA | SEC | FE | OP | All |
|-----|----|-----|----|----|-----|
| 1 | QA-002 Complete | SEC-004 Start | FE-009 Complete | OP-001 Start | – |
| 2 | QA-003 Start | SEC-004 Continue | FE-010 Start | OP-001 Continue | – |
| 3 | QA-003 Continue | SEC-004 Continue | FE-010 Complete | OP-001 Continue | – |
| 4 | QA-003 Complete | SEC-004 Complete | – | OP-001 Complete | – |
| 5 | QA-004 Start | – | – | OP-002 Start | Prototype Demo |

**Sprint 5 Goal:** All tests passing, security validated, monitoring configured, prototype demo ready.

---

## 13. Risk & Dependency Matrix

### High-Risk Dependencies

| Risk ID | Description | Mitigation | Contingency |
|---------|-------------|------------|-------------|
| **R-001** | .NET 10 Preview instability | Feature flags for preview APIs | Fallback to .NET 9 LTS |
| **R-002** | BE-004 (Search) delays impact FE-007 | Parallel mock API development | FE uses mock data until API ready |
| **R-003** | DBA-003 index optimization delays | Early query profiling | Add indexes post-launch if needed |
| **R-004** | SEC-002 auth hardening conflicts with BE-003 | Daily sync between SEC and BE | Buffer day in Sprint 2 |
| **R-005** | FE-007 search UI complexity | Component library reuse | Simplify to basic search if needed |

### Dependency Chain Analysis

```
CRITICAL PATH (longest chain = 18 days):
SA-001 → SA-002 → DBA-001 → BE-001 → BE-002 → BE-003 → BE-006 → FE-009

PARALLEL TRACKS:
Track A (Backend): DBA-001 → BE-001 → BE-002 → BE-004 → BE-005
Track B (Frontend): FE-001 → FE-002 → FE-003 → FE-004 → FE-005/FE-006/FE-007
Track C (Security): SEC-001 → SEC-002 → SEC-003 → SEC-004
Track D (DevOps): DEV-001 → DEV-002 → DEV-003 → DEV-004

BUFFER ALLOCATION:
- Sprint 2: 1 day buffer for auth integration
- Sprint 4: 1 day buffer for FE view completion
- Sprint 5: 2 days buffer for bug fixes before demo
```

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 0.1 | 2026-04-23 | SA Agent | Initial task breakdown from BRD v1.0 |
| 1.0 | 2026-04-23 | SA Agent | Approved for prototype execution |

---

## Next Actions

1. **SA Agent:** Review task granularity and adjust estimates with team leads
2. **PO Agent:** Confirm stakeholder availability for Sprint 5 demo
3. **DevOps Agent:** Initialize repositories and CI pipelines immediately
4. **SEC Agent:** Schedule STRIDE threat modeling session (Week 1, Day 1)
5. **All Agents:** Confirm resource availability and blockers for Sprint 0
