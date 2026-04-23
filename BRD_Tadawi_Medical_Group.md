# Business Requirements Document (BRD)
## Tadawi Medical Group – Internal Information Portal

**Document Version:** 1.0 (Prototype Phase)  
**Date:** April 23, 2026  
**Status:** Approved for Prototype Development  
**Classification:** Internal – Tadawi Medical Group

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Business Context & Objectives](#2-business-context--objectives)
3. [Scope Definition](#3-scope-definition)
4. [Target Users & Personas](#4-target-users--personas)
5. [Functional Requirements](#5-functional-requirements)
6. [Non-Functional Requirements](#6-non-functional-requirements)
7. [Technical Architecture](#7-technical-architecture)
8. [Multi-Agent Development Team Structure](#8-multi-agent-development-team-structure)
9. [Data Model & Entity Relationships](#9-data-model--entity-relationships)
10. [UI/UX Strategy](#10-uiux-strategy)
11. [Security & Compliance](#11-security--compliance)
12. [Development Phases](#12-development-phases)
13. [Risk Assessment](#13-risk-assessment)
14. [Success Metrics](#14-success-metrics)
15. [Appendices](#15-appendices)

---

## 1. Executive Summary

Tadawi Medical Group requires a centralized web application to provide employees (marketing teams, call center agents, receptionists, and administrative staff) with instant access to critical organizational information. The application serves as the single source of truth for branch locations, doctor directories, medical sectors, specializations, and related operational data.

**Strategic Goal:** Reduce information retrieval time by 80% and eliminate data silos across departments.

**Delivery Approach:** Two-phase delivery:
- **Phase 1 (Prototype):** Functional wireframe with core data display, minimal styling, rapid validation (6 weeks)
- **Phase 2 (Production):** Full UI/UX design by specialized UX Agent, enhanced interactions, branding compliance (8 weeks)

---

## 2. Business Context & Objectives

### 2.1 Current Pain Points
- Scattered spreadsheets and printed directories
- Call center agents cannot verify doctor availability quickly
- Marketing teams lack real-time branch service data
- Receptionists work with outdated information
- No centralized system for cross-departmental consistency

### 2.2 Business Objectives

| Objective | Success Metric | Priority |
|-----------|---------------|----------|
| Centralize medical group information | Single portal for all staff | P0 |
| Real-time data accuracy | Data freshness < 5 minutes | P0 |
| Multi-role access patterns | 5+ user role types | P1 |
| Mobile accessibility | Responsive for tablets/phones | P1 |
| Reduce training time | < 30 minutes to proficiency | P2 |
| SSO integration | Active Directory compatible | P2 |

---

## 3. Scope Definition

### 3.1 Prototype Phase Scope (In Scope)

- Core entity management: Branches, Doctors, Sectors, Specialties
- Basic CRUD operations via REST API
- Simple React frontend with functional navigation
- Role-based access control (RBAC) foundation
- PostgreSQL data persistence with Redis caching layer
- Search and filter capabilities
- Basic responsive layout (utility-first CSS)

### 3.2 Prototype Phase Scope (Out of Scope)

- Custom UI/UX design (deferred to Phase 2)
- Advanced analytics dashboards
- Patient-facing portal
- Appointment scheduling integration
- Multi-language support (Arabic/English toggle)
- Advanced audit logging UI
- Real-time notifications (WebSockets/SSE)

### 3.3 Full Production Scope (Phase 2)

- Complete UI/UX redesign by UX Agent
- Advanced search (fuzzy matching, phonetic)
- HMS integration
- Analytics dashboard
- Mobile application
- Offline mode support

---

## 4. Target Users & Personas

### 4.1 Primary Users

| Persona | Role | Primary Needs | Access Pattern |
|---------|------|--------------|----------------|
| **Sarah** | Call Center Agent | Quick doctor lookup by specialty/branch | High volume, desktop + headset |
| **Ahmed** | Marketing Coordinator | Branch services, doctor credentials | Research-heavy, content export |
| **Fatima** | Receptionist | Branch doctor schedules, room assignments | Front desk tablet, interruption-driven |
| **Dr. Khalid** | Department Head | Specialty management, onboarding workflows | Administrative, report generation |
| **Omar** | IT Administrator | User management, system configuration | Backend operations, monitoring |

### 4.2 User Volume Estimates
- **Concurrent users (peak):** 150
- **Total registered users:** 500+
- **Geographic distribution:** KSA/UAE branches

---

## 5. Functional Requirements

### 5.1 Module 1: Branch Management

- **FR-BR-01:** Display branches with name, address, contact, operating hours, services
- **FR-BR-02:** Branch detail with associated doctors, departments, facilities
- **FR-BR-03:** Search by location (city/region), name, or service type
- **FR-BR-04:** Geolocation data storage (future map integration)
- **FR-BR-05:** Status indicators (operational, maintenance, new opening)

### 5.2 Module 2: Doctor Directory

- **FR-DR-01:** Doctor profiles (name, credentials, specialties, languages, experience)
- **FR-DR-02:** Doctor-branch association (primary + visiting branches)
- **FR-DR-03:** Availability status (active, on leave, retired, suspended)
- **FR-DR-04:** Search by name (partial match), specialty, branch, language
- **FR-DR-05:** Quick contact card generation (printable/digital)

### 5.3 Module 3: Medical Sectors & Specialties

- **FR-SE-01:** Hierarchical specialty taxonomy
- **FR-SE-02:** Sector classification (Medical, Surgical, Diagnostic, Administrative, Support)
- **FR-SE-03:** Specialty-doctor relationship mapping
- **FR-SE-04:** Service descriptions per specialty

### 5.4 Module 4: Search & Discovery

- **FR-SR-01:** Global search across all entities
- **FR-SR-02:** Advanced filters (multi-select, range sliders)
- **FR-SR-03:** Search result relevance ranking
- **FR-SR-04:** Recent searches and saved favorites
- **FR-SR-05:** Autocomplete/suggestions (Redis-powered)

### 5.5 Module 5: User Management & Access Control

- **FR-UM-01:** Role-based dashboard (different landing pages per persona)
- **FR-UM-02:** JWT authentication (SSO-ready architecture)
- **FR-UM-03:** Password reset and account recovery
- **FR-UM-04:** Session management with timeout
- **FR-UM-05:** Basic audit trail (who viewed what, when)

### 5.6 API Endpoints

| Category | Operations | Description |
|----------|-----------|-------------|
| `/api/branches` | GET, POST, PUT, DELETE | Branch CRUD |
| `/api/doctors` | GET, POST, PUT, DELETE | Doctor CRUD |
| `/api/specialties` | GET, POST, PUT, DELETE | Specialty CRUD |
| `/api/sectors` | GET, POST, PUT, DELETE | Sector CRUD |
| `/api/search` | GET | Unified search |
| `/api/auth` | POST, DELETE | Login/logout |
| `/api/users` | GET, PUT | User profiles |
| `/api/audit` | GET | Audit logs (admin) |

---

## 6. Non-Functional Requirements

### 6.1 Performance
- **NFR-PF-01:** Page load < 2 seconds (95th percentile)
- **NFR-PF-02:** API response < 200ms cached, < 500ms database
- **NFR-PF-03:** Support 150 concurrent users with < 10% degradation
- **NFR-PF-04:** Search results < 300ms

### 6.2 Availability
- **NFR-AV-01:** 99.5% uptime during business hours (8 AM – 8 PM)
- **NFR-AV-02:** Zero-downtime deployments (blue-green)
- **NFR-AV-03:** Automated backups every 6 hours, 30-day retention

### 6.3 Scalability
- **NFR-SC-01:** Horizontal scaling for API layer
- **NFR-SC-02:** Database read replicas for reporting
- **NFR-SC-03:** Redis cluster for session/cache distribution

### 6.4 Security
- **NFR-SE-01:** HTTPS/TLS 1.3 enforcement
- **NFR-SE-02:** OWASP Top 10 compliance
- **NFR-SE-03:** bcrypt password hashing (cost factor 12)
- **NFR-SE-04:** Rate limiting: 100 req/min, 10 login attempts/5min
- **NFR-SE-05:** JWT expiration: 8h access, 7d refresh
- **NFR-SE-06:** Row-level security for sensitive doctor data

### 6.5 Maintainability
- **NFR-MA-01:** 80% minimum test coverage
- **NFR-MA-02:** OpenAPI 3.1 documentation (native .NET 10)
- **NFR-MA-03:** Structured logging (Serilog) with correlation IDs
- **NFR-MA-04:** Containerized deployment (Docker + Kubernetes)

---

## 7. Technical Architecture

### 7.1 Technology Stack

| Layer | Technology | Justification |
|-------|-----------|---------------|
| **Frontend** | React 19 + TypeScript | Component reusability, strong typing |
| **State Management** | TanStack Query v5 | Server state sync, caching, refetching |
| **UI Components** | Headless UI / Radix UI | Accessible, unstyled primitives |
| **Styling** | Tailwind CSS | Rapid prototyping, design-agnostic |
| **Backend** | .NET 10 ASP.NET Core | Native OpenAPI 3.1, minimal APIs, SSE |
| **ORM** | Entity Framework Core 10 | PostgreSQL optimized, migrations |
| **Database** | PostgreSQL 16 | ACID, JSONB, full-text search |
| **Cache** | Redis 7 | Session, search cache, rate limiting |
| **Auth** | ASP.NET Core Identity + JWT | Native passkey/FIDO2 support |
| **Containers** | Docker + Compose | Dev/prod parity |
| **Proxy** | YARP | Load balancing, API gateway |

### 7.2 Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                      CLIENT LAYER                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   React SPA  │  │   Mobile     │  │   Admin      │      │
│  │   (Staff)    │  │   (Future)   │  │   Dashboard  │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
└─────────┼─────────────────┼─────────────────┼─────────────┘
          │                 │                 │
          └─────────────────┴─────────────────┘
                            │
                    ┌───────▼───────┐
                    │   CDN / WAF   │
                    └───────┬───────┘
                            │
┌───────────────────────────▼───────────────────────────────┐
│                   API GATEWAY LAYER                         │
│              YARP Reverse Proxy / Load Balancer             │
└───────────────────────────┬───────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
┌───────▼───────┐  ┌───────▼───────┐  ┌───────▼───────┐
│   API Pod 1   │  │   API Pod 2   │  │   API Pod N   │
│  .NET 10      │  │  .NET 10      │  │  .NET 10      │
│  Minimal APIs │  │  Minimal APIs │  │  Minimal APIs │
└───────┬───────┘  └───────┬───────┘  └───────┬───────┘
        │                   │                   │
        └───────────────────┼───────────────────┘
                            │
              ┌─────────────▼─────────────┐
              │      REDIS CLUSTER        │
              │  ┌─────────┐ ┌─────────┐  │
              │  │  Cache  │ │ Session │  │
              │  │  Node 1 │ │  Store  │  │
              │  └─────────┘ └─────────┘  │
              └─────────────┬─────────────┘
                            │
              ┌─────────────▼─────────────┐
              │    POSTGRESQL CLUSTER     │
              │  ┌─────────┐ ┌─────────┐  │
              │  │ Primary │ │  Read   │  │
              │  │  Node   │ │ Replica │  │
              │  └─────────┘ └─────────┘  │
              └───────────────────────────┘
```

### 7.3 .NET 10 Specific Features

- **Minimal API Validation:** Native `AddValidation()` with DataAnnotations
- **OpenAPI 3.1:** Native YAML/JSON generation with XML docs
- **Server-Sent Events:** `TypedResults.ServerSentEvents` for future real-time
- **JSON Patch:** `System.Text.Json`-based (160x faster than Newtonsoft)
- **Passkey/FIDO2:** Native ASP.NET Core Identity support
- **Persistent State:** `[PersistentState]` for future Blazor admin dashboard

---

## 8. Multi-Agent Development Team Structure

### 8.1 Agent Roles

| Agent | Role | Responsibilities | Deliverables |
|-------|------|-----------------|------------|
| **PO Agent** | Product Owner | Requirements, backlog, stakeholder communication | User stories, sprint plans, release notes |
| **SA Agent** | Solution Architect | Technical design, architecture, tech selection | Architecture docs, ADRs, specifications |
| **FE Agent** | Frontend Developer | React development, components, state management | React components, hooks, unit tests |
| **BE Agent** | Backend Developer | .NET 10 API, database, business logic | Controllers/services, EF migrations, OpenAPI |
| **DBA Agent** | Database Administrator | Schema design, optimization, backups | Schema scripts, migration plans |
| **DevOps Agent** | DevOps Engineer | CI/CD, IaC, container orchestration | GitHub Actions, Dockerfiles, K8s manifests |
| **QA Agent** | Quality Assurance | Test strategy, automated/manual testing | Test plans, Playwright tests, load scripts |
| **SEC Agent** | Security Engineer | Threat modeling, pen testing, compliance | Security audits, OWASP scans, hardening |
| **UX Agent** | UI/UX Designer | User research, wireframes, design system | Figma designs, design tokens, accessibility |
| **OP Agent** | Operations Engineer | Monitoring, incident response, capacity | Runbooks, dashboards, alerting rules |

### 8.2 Agent Interaction Model

```
┌─────────────────────────────────────────────────────────────┐
│                    GOVERNANCE LAYER                          │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐      │
│  │   PO Agent  │◄──►│   SA Agent  │◄──►│   SEC Agent │      │
│  │  (Backlog)  │    │ (Technical) │    │  (Compliance)│     │
│  └──────┬──────┘    └──────┬──────┘    └──────┬──────┘      │
└─────────┼──────────────────┼──────────────────┼────────────┘
          │                  │                  │
          ▼                  ▼                  ▼
┌─────────────────────────────────────────────────────────────┐
│                   DEVELOPMENT LAYER                          │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐      │
│  │   FE Agent  │◄──►│   BE Agent  │◄──►│   DBA Agent │      │
│  │   (React)   │    │  (.NET 10)  │    │ (PostgreSQL)│      │
│  └──────┬──────┘    └──────┬──────┘    └──────┬──────┘      │
│         │                  │                  │              │
│         └──────────────────┼──────────────────┘              │
│                            │                                │
│                   ┌─────────▼─────────┐                      │
│                   │    UX Agent       │                      │
│                   │  (Design System)  │                      │
│                   │   [PHASE 2 ONLY]  │                      │
│                   └───────────────────┘                      │
└─────────────────────────────────────────────────────────────┘
          │                  │                  │
          ▼                  ▼                  ▼
┌─────────────────────────────────────────────────────────────┐
│                   OPERATIONS LAYER                           │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐      │
│  │ DevOps Agent│◄──►│   QA Agent  │◄──►│   OP Agent  │      │
│  │   (CI/CD)   │    │  (Testing)  │    │ (Monitoring)│      │
│  └─────────────┘    └─────────────┘    └─────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

### 8.3 Communication Protocols

- **Daily Standups:** Async updates via structured markdown in shared repository
- **Sprint Reviews:** PO Agent consolidates outputs for stakeholder demos
- **Architecture Decisions:** ADRs reviewed by SA, SEC, and PO agents
- **Code Reviews:** Cross-agent PR reviews
- **Incident Response:** OP escalates to SEC (security) or SA (architectural)

---

## 9. Data Model & Entity Relationships

### 9.1 Entity Diagram

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│    Branch       │       │     Doctor      │       │   Specialty     │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ PK id (UUID)    │◄──────┤ PK id (UUID)    │◄──────┤ PK id (UUID)    │
│ name (string)   │  1:M  │ full_name       │  M:M  │ name (string)   │
│ address         │       │ email           │       │ category        │
│ city            │       │ phone           │       │ description     │
│ phone           │       │ years_exp       │       │ parent_id (FK)  │──┐
│ email           │       │ status          │       │ sector_id (FK)  │  │
│ operating_hours │       │ photo_url       │       └─────────────────┘  │
│ status          │       │ bio             │              │               │
│ created_at      │       │ created_at      │              │ 1:M           │
│ updated_at      │       │ updated_at      │              ▼               │
└─────────────────┘       └─────────────────┘       ┌─────────────────┐   │
         │                      │                      │    Sector       │   │
         │                      │                      ├─────────────────┤   │
         │                      │                      │ PK id (UUID)    │   │
         │                      │                      │ name (string)   │   │
         │                      │                      │ type            │   │
         │                      │                      │ description     │   │
         │                      │                      └─────────────────┘   │
         │                      │                                              │
         │              ┌───────┴───────┐                                    │
         │              │ Doctor_Specialty│                                    │
         │              │ (Junction)    │                                    │
         │              ├───────────────┤                                    │
         │              │ doctor_id (FK)│                                    │
         │              │ specialty_id  │                                    │
         │              │ is_primary    │                                    │
         └──────────────┤ branch_id (FK)│                                    │
                        └───────────────┘                                    │
                                                                             │
                        ┌─────────────────┐       ┌─────────────────┐      │
                        │  Doctor_Branch  │       │     User        │      │
                        │  (Junction)     │       ├─────────────────┤      │
                        ├─────────────────┤       │ PK id (UUID)    │      │
                        │ doctor_id (FK)  │       │ username        │      │
                        │ branch_id (FK)  │       │ email           │      │
                        │ schedule        │       │ role (enum)     │      │
                        │ is_primary      │       │ branch_id (FK)  │──────┘
                        └─────────────────┘       │ password_hash   │
                                                  │ last_login      │
                                                  │ status          │
                                                  │ created_at      │
                                                  └─────────────────┘
```

### 9.2 PostgreSQL Schema

```sql
-- Core Entities
CREATE TABLE sectors (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    type VARCHAR(50) CHECK (type IN ('Medical', 'Surgical', 'Diagnostic', 'Administrative', 'Support')),
    description TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE specialties (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    category VARCHAR(50),
    description TEXT,
    parent_id UUID REFERENCES specialties(id),
    sector_id UUID NOT NULL REFERENCES sectors(id),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE branches (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(150) NOT NULL,
    address TEXT NOT NULL,
    city VARCHAR(100) NOT NULL,
    region VARCHAR(100),
    phone VARCHAR(20),
    email VARCHAR(100),
    operating_hours JSONB,
    services TEXT[],
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'maintenance', 'closed', 'opening_soon')),
    geo_location POINT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE doctors (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(150) NOT NULL,
    email VARCHAR(100),
    phone VARCHAR(20),
    years_experience INTEGER,
    languages VARCHAR(50)[],
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'on_leave', 'retired', 'suspended')),
    photo_url VARCHAR(255),
    bio TEXT,
    credentials JSONB,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Junction Tables
CREATE TABLE doctor_specialties (
    doctor_id UUID REFERENCES doctors(id) ON DELETE CASCADE,
    specialty_id UUID REFERENCES specialties(id) ON DELETE CASCADE,
    is_primary BOOLEAN DEFAULT false,
    PRIMARY KEY (doctor_id, specialty_id)
);

CREATE TABLE doctor_branches (
    doctor_id UUID REFERENCES doctors(id) ON DELETE CASCADE,
    branch_id UUID REFERENCES branches(id) ON DELETE CASCADE,
    schedule JSONB,
    is_primary BOOLEAN DEFAULT false,
    PRIMARY KEY (doctor_id, branch_id)
);

-- User Management
CREATE TYPE user_role AS ENUM ('admin', 'call_center', 'receptionist', 'marketing', 'department_head', 'viewer');

CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(150),
    role user_role NOT NULL DEFAULT 'viewer',
    branch_id UUID REFERENCES branches(id),
    is_active BOOLEAN DEFAULT true,
    last_login TIMESTAMPTZ,
    password_changed_at TIMESTAMPTZ,
    failed_login_attempts INTEGER DEFAULT 0,
    locked_until TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Audit Trail
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id),
    action VARCHAR(50) NOT NULL,
    entity_type VARCHAR(50),
    entity_id UUID,
    old_values JSONB,
    new_values JSONB,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_branches_city ON branches(city);
CREATE INDEX idx_branches_status ON branches(status);
CREATE INDEX idx_doctors_name ON doctors USING gin(to_tsvector('english', full_name));
CREATE INDEX idx_doctors_status ON doctors(status);
CREATE INDEX idx_specialties_sector ON specialties(sector_id);
CREATE INDEX idx_specialties_parent ON specialties(parent_id);
CREATE INDEX idx_audit_user ON audit_logs(user_id, created_at);
CREATE INDEX idx_audit_entity ON audit_logs(entity_type, entity_id, created_at);
```

### 9.3 Redis Cache Strategy

| Cache Key | TTL | Type | Invalidation |
|-----------|-----|------|-------------|
| `branch:{id}` | 1h | Hash | Branch update/delete |
| `branches:list` | 15m | Sorted Set | Any branch mutation |
| `doctor:{id}` | 30m | Hash | Doctor update/delete |
| `doctors:by_specialty:{id}` | 20m | Set | Junction change |
| `search:{query_hash}` | 10m | JSON | Background refresh |
| `session:{jwt_id}` | 8h | Hash | Logout/token refresh |
| `rate_limit:{ip}` | 1m | Counter | Auto-expire |
| `favorites:{user_id}` | 24h | Set | User action |

---

## 10. UI/UX Strategy

### 10.1 Prototype Principles
- **Functional over Beautiful:** Focus on IA and flows
- **Accessibility Baseline:** WCAG 2.1 AA (semantic HTML, keyboard nav, ARIA)
- **Mobile-First:** Responsive for tablets (primary receptionist device)
- **Design-Agnostic:** Neutral grays, basic spacing for future brand compatibility

### 10.2 Page Structure

```
App Shell
├── Header
│   ├── Logo (placeholder)
│   ├── Global Search Bar
│   ├── User Menu (notifications, profile, logout)
│   └── Branch Selector (contextual)
│
├── Sidebar Navigation
│   ├── Dashboard (role-based)
│   ├── Branches
│   ├── Doctors
│   ├── Specialties
│   ├── Sectors
│   └── Admin (conditional)
│
└── Main Content Area
    ├── List Views (table/card toggle)
    │   ├── Filters (sidebar drawer on mobile)
    │   ├── Sort controls
    │   └── Pagination / Infinite scroll
    │
    ├── Detail Views
    │   ├── Breadcrumb navigation
    │   ├── Action buttons (edit, delete, print)
    │   ├── Tabbed content sections
    │   └── Related entities sidebar
    │
    └── Forms
        ├── Inline validation
        ├── Auto-save drafts
        └── Confirmation modals
```

### 10.3 Key Interactions

| Interaction | Behavior | UX Agent Notes |
|-------------|----------|----------------|
| Global Search | Cmd+K, fuzzy matching, recent searches | Refine search UX pattern |
| Doctor Lookup | Quick card with "Copy Contact" | Call center primary use case |
| Branch Switcher | Persistent context; affects filters | Marketing multi-branch comparison |
| Data Tables | Sortable, column toggle, CSV export | Receptionist print-friendly views |
| Mobile Nav | Bottom sheet filters, swipe gestures | Tablet-optimized for front desk |

### 10.4 UX Agent Handoff (Phase 2)

**Prototype delivers:**
- Validated information architecture
- Proven user flows per persona
- Component inventory with usage contexts
- Accessibility baseline test results
- Performance budget targets

**UX Agent delivers:**
- Tadawi brand design system
- High-fidelity mockups
- Micro-interaction specifications
- Arabic RTL layout
- Illustration/iconography guidelines

---

## 11. Security & Compliance

### 11.1 Authentication
- JWT: Access (8h), Refresh (7d), sliding expiration
- RBAC: 6 roles with granular permissions
- Future: .NET 10 native Passkey/WebAuthn
- Sessions: Redis-backed distributed

### 11.2 Data Protection

| Data Category | Classification | Protection |
|---------------|---------------|------------|
| Doctor personal contact | Internal | RBAC + audit |
| Patient data (future) | Restricted | Phase 2 scope |
| User passwords | Confidential | bcrypt cost 12 |
| System logs | Internal | Encrypted at rest |
| API keys | Confidential | Vault integration (Phase 2) |

### 11.3 Compliance
- **GDPR/CCPA Ready:** Export/deletion in schema (Phase 2 activation)
- **Healthcare:** HIPAA-aligned audit logging
- **KSA Data Residency:** Saudi Arabia deployment option

### 11.4 Security Agent Responsibilities
- **Sprint 0:** STRIDE threat modeling
- **Per Sprint:** OWASP Dependency-Check
- **Pre-Release:** Penetration testing scope
- **Ongoing:** Security champion PR reviews

---

## 12. Development Phases

### 12.1 Phase 1: Prototype (Weeks 1-6)

| Sprint | Week | Focus | Deliverables | Lead |
|--------|------|-------|-------------|------|
| Sprint 0 | 1 | Setup & Architecture | Repo, Docker, CI/CD, ADRs | SA + DevOps |
| Sprint 1 | 2 | Database & API Foundation | Schema, migrations, basic CRUD | BE + DBA |
| Sprint 2 | 3 | Core Backend | Search API, Redis, auth, RBAC | BE + SEC |
| Sprint 3 | 4 | Frontend Scaffold | React setup, routing, layout | FE + SA |
| Sprint 4 | 5 | Frontend Features | List/detail views, search UI | FE |
| Sprint 5 | 6 | Integration & QA | E2E testing, perf baseline, security | QA + All |

**Milestone:** Stakeholder demo + go/no-go for Phase 2

### 12.2 Phase 2: Production (Weeks 7-14)

| Sprint | Week | Focus | Deliverables | Lead |
|--------|------|-------|-------------|------|
| Sprint 6 | 7 | UX Design | Figma, design tokens, component spec | UX |
| Sprint 7 | 8 | UI Implementation | Themed components, RTL | FE + UX |
| Sprint 8 | 9 | Advanced Features | Analytics, export, bulk ops | FE + BE |
| Sprint 9 | 10 | Security Hardening | Pen test remediation, audit UI | SEC + BE |
| Sprint 10 | 11 | DevOps & Monitoring | K8s, Grafana, alerting | DevOps + OP |
| Sprint 11 | 12 | Performance | Load testing, query optimization | DBA + DevOps |
| Sprint 12 | 13 | UAT & Training | UAT, docs, training materials | PO + QA |
| Sprint 13 | 14 | Launch | Final scan, migration, go-live | All |

### 12.3 Phase 3: Enhancement (Post-Launch)
- Real-time notifications (SSE)
- Mobile application (React Native)
- Advanced analytics
- HMS integration
- AI-powered search

---

## 13. Risk Assessment

| Risk | Probability | Impact | Mitigation | Owner |
|------|-------------|--------|------------|-------|
| UX delays in Phase 2 | Medium | High | Parallel design in Sprint 4 | PO |
| DB performance at scale | Low | High | Read replicas, query optimization | DBA |
| Security vulnerabilities | Medium | Critical | SEC embedded from Sprint 0 | SEC |
| Stakeholder scope creep | High | Medium | Strict Phase 1 lock, change board | PO |
| Agent communication gaps | Medium | Medium | ADR process, async standups | SA |
| .NET 10 preview stability | Low | Medium | LTS fallback path, feature flags | SA |
| Redis cluster complexity | Low | Medium | Single instance start, cluster Phase 2 | DevOps |

---

## 14. Success Metrics

### 14.1 Technical

| Metric | Target | Tool |
|--------|--------|------|
| API response (p95) | < 200ms | Prometheus + Grafana |
| Bundle size | < 200KB initial | Webpack Analyzer |
| Test coverage | > 80% | SonarQube |
| Lighthouse score | > 90 | Lighthouse CI |
| Security critical issues | 0 | SonarQube + ZAP |

### 14.2 Business

| Metric | Baseline | Target | Measurement |
|--------|----------|--------|-------------|
| Info lookup time | 5+ min | < 30s | User timing |
| Data accuracy complaints | 10+/week | < 1/week | Support tickets |
| System adoption | N/A | > 90% in 30d | Login analytics |
| Cross-dept consistency | Low | High | Quarterly audit |
| Training time | 2+ days | < 30 min | HR feedback |

---

## 15. Appendices

### Appendix A: Frontend Dependencies

```json
{
  "react": "^19.0.0",
  "react-dom": "^19.0.0",
  "react-router-dom": "^7.0.0",
  "@tanstack/react-query": "^5.0.0",
  "axios": "^1.7.0",
  "tailwindcss": "^4.0.0",
  "@headlessui/react": "^2.0.0",
  "lucide-react": "^0.400.0",
  "zustand": "^5.0.0"
}
```

### Appendix B: Backend Dependencies

```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
<PackageReference Include="StackExchange.Redis" Version="2.8.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
<PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
```

### Appendix C: API Contract Example

**GET /api/doctors?specialty=cardiology&branch=riyadh-main&status=active**

```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "fullName": "Dr. Ahmed Al-Rashid",
      "specialties": [
        {
          "id": "550e8400-e29b-41d4-a716-446655440001",
          "name": "Interventional Cardiology",
          "isPrimary": true,
          "sector": "Medical"
        }
      ],
      "branches": [
        {
          "id": "550e8400-e29b-41d4-a716-446655440002",
          "name": "Tadawi Riyadh Main",
          "isPrimary": true,
          "schedule": {
            "sunday": { "start": "09:00", "end": "17:00", "room": "C-201" },
            "monday": { "start": "09:00", "end": "17:00", "room": "C-201" }
          }
        }
      ],
      "languages": ["Arabic", "English"],
      "yearsExperience": 15,
      "status": "active",
      "contact": {
        "email": "a.alrashid@tadawi.med",
        "phone": "+966 50 123 4567"
      }
    }
  ],
  "meta": {
    "total": 45,
    "page": 1,
    "pageSize": 20,
    "filters": {
      "specialty": "cardiology",
      "branch": "riyadh-main",
      "status": "active"
    }
  }
}
```

### Appendix D: Multi-Agent Workflow

```
STAKEHOLDER REQUEST
        │
        ▼
┌───────────────┐
│   PO Agent    │───► Backlog refinement
│  (Analyze)    │───► Acceptance criteria
└───────┬───────┘
        │
        ▼
┌───────────────┐
│   SA Agent    │───► Architecture design
│  (Design)     │───► Tech stack validation
└───────┬───────┘
        │
        ├──────────────────┬──────────────────┐
        ▼                  ▼                  ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│   FE Agent    │  │   BE Agent    │  │   DBA Agent   │
│  (Develop)    │  │  (Develop)    │  │  (Schema)     │
└───────┬───────┘  └───────┬───────┘  └───────┬───────┘
        │                  │                  │
        └──────────────────┼──────────────────┘
                           ▼
                   ┌───────────────┐
                   │   QA Agent    │───► Automated testing
                   │   (Test)      │───► Manual validation
                   └───────┬───────┘
                           │
                           ▼
                   ┌───────────────┐
                   │   SEC Agent   │───► Security scan
                   │  (Validate)   │───► Compliance check
                   └───────┬───────┘
                           │
                           ▼
                   ┌───────────────┐
                   │  DevOps Agent │───► CI/CD execution
                   │   (Deploy)    │───► Environment provisioning
                   └───────┬───────┘
                           │
                           ▼
                   ┌───────────────┐
                   │   OP Agent    │───► Monitoring setup
                   │  (Monitor)    │───► Alert configuration
                   └───────┬───────┘
                           │
                           ▼
                   ┌───────────────┐
                   │   PO Agent    │───► Stakeholder review
                   │  (Review)     │───► Sprint retrospective
                   └───────┬───────┘
                           │
                    [ACCEPTED?]
                       /                         YES      NO
                     /                              ▼            ▼
              ┌────────┐    ┌────────┐
              │ DEPLOY │    │ REFINE │
              │  PROD  │    │ BACKLOG│
              └────────┘    └────────┘
```

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 0.1 | 2026-04-23 | SA Agent | Initial draft |
| 0.2 | 2026-04-23 | PO Agent | Added personas and metrics |
| 1.0 | 2026-04-23 | All Agents | Approved for prototype |

---

## Next Steps

1. **Stakeholder Approval:** Present to Tadawi leadership
2. **Agent Assignment:** Confirm availability and specialization
3. **Sprint 0 Kickoff:** Initialize repos, environments, channels
4. **UX Agent Briefing:** Provide Phase 1 timeline for parallel brand research
