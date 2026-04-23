# Skill Registry & Specialist Catalog

The system's capability management — a curated collection of technology-specific specialist skills, each bundling deep expertise, best practices, and verification standards.

---

## How Specialist Skills Work

A specialist skill is loaded into an agent's prompt at execution time. Instead of a generic "implement this" instruction, the agent receives expert knowledge for its specific technology stack.

```
Task: "Create patient registration React components"
  │
  ├── Specialist: react
  │     └── Prompt includes: React patterns, TypeScript strict, accessibility, testing
  │
  ├── Task description + acceptance criteria
  │
  └── Memory context (paged-in relevant decisions)
```

### Skill Manifest Structure

Each specialist is defined by a manifest:

```yaml
skillId: "react-specialist"
name: "React Specialist"
version: "1.0.0"
description: "React, TypeScript, Next.js specialist for frontend development"

# What gets injected into the agent's prompt
expertisePrompt: |
  You are a React Specialist with deep expertise in:
  
  **Core**: React 16+, Hooks, React.memo, React.lazy
  **Meta Framework**: Next.js 12+, App Router, Server Components
  **State**: Redux Toolkit, Zustand, React Context
  **Testing**: Jest, React Testing Library, Cypress, Playwright
  
  Follow these patterns:
  - Use functional components with hooks (no class components)
  - Implement proper TypeScript types and interfaces
  - Use React.memo for performance-critical components
  - Implement error boundaries for fault isolation
  - Write tests using Jest + React Testing Library
  
  Security:
  - Never use dangerouslySetInnerHTML
  - Validate all user inputs
  - Use CSRF tokens for form submissions

technologies: ["React 16+", "TypeScript (strict)", "Next.js 12+", "Redux Toolkit", "Jest", "Cypress"]

patterns:
  - "Functional components with hooks"
  - "Container/Presentational pattern"
  - "Custom hooks for data fetching"
  - "Error boundaries for fault isolation"
  - "Code splitting with React.lazy"

standards:
  - "WCAG 2.1 AA accessibility"
  - "100% TypeScript strict coverage"
  - "> 80% test coverage"

# Auto-detection
detection:
  files: ["package.json"]
  dependencies: ["react", "react-dom", "next"]
  frameworks: ["React", "Next.js", "Remix"]

# Context requirements
contextProfile:
  baselineTokens: 2000
  optimalTokens: 6000
  pagingTriggers:
    - "similar component"
    - "existing pattern"
    - "codebase convention"

# Verification commands (override project defaults when this specialist is active)
verification:
  testCommand: "npm test"
  lintCommand: "npm run lint"
  buildCommand: "npm run build"
```

---

## Specialist Catalog

### Scrum Team Agent

#### Scrum Master Agent

| Property | Value |
|---|---|
| **Skill ID** | `scrum-master` |
| **Role** | Sprint planning, daily standup, handoff coordination, blocker escalation, retrospective |
| **Activation** | One per feature team; assigned by Orchestrator at team formation |
| **Best Practices** | [scrum-master.md](scrum-master.md) |
| **See also** | [Scrum Teams](../workflow/SCRUM-TEAMS.md), [Epic Sprints](../workflow/EPIC-SPRINTS.md) |

---

### Frontend Specialists

#### React Specialist

| Property | Value |
|---|---|
| **Skill ID** | `react-specialist` |
| **Technologies** | React 18+, TypeScript strict, Next.js App Router, TanStack Query, React Hook Form + Zod |
| **Patterns** | Functional components, hooks, compound components, error boundaries, code splitting |
| **Testing** | Jest, React Testing Library, Playwright |
| **Standards** | TypeScript strict mode, WCAG 2.1 AA, >80% test coverage |
| **Security** | XSS prevention, CSRF protection, JWT in-memory + httpOnly cookie, input validation |
| **Auto-detect** | `react`, `next`, `react-dom` in package.json |
| **Best Practices** | [react-specialist.md](react-specialist.md) |

#### Vue Specialist

| Property | Value |
|---|---|
| **Skill ID** | `vue-specialist` |
| **Technologies** | Vue 3+, TypeScript, Composition API, Pinia, Vue Router, Vitest, Playwright |
| **Patterns** | `<script setup>`, composables, reactive state, performance optimization |
| **Testing** | Vitest, Vue Test Utils, Playwright |
| **Standards** | TypeScript strict mode, >80% test coverage |
| **Auto-detect** | `vue` in package.json |
| **Best Practices** | [vue-specialist.md](vue-specialist.md) |

#### .NET MVC Specialist

| Property | Value |
|---|---|
| **Skill ID** | `net-mvc-specialist` |
| **Technologies** | ASP.NET Core MVC, C# 12+, Razor Tag Helpers, FluentValidation |
| **Patterns** | Thin controllers, ViewModels, Tag Helpers, server-side rendering |
| **Auto-detect** | `.csproj` with MVC references |
| **Best Practices** | [net-mvc-specialist.md](net-mvc-specialist.md) |

---

### Backend Specialists

#### .NET Core Specialist

| Property | Value |
|---|---|
| **Skill ID** | `dotnet-specialist` |
| **Technologies** | .NET 8/9/10, C# 12+, ASP.NET Core, EF Core, MediatR, FluentValidation |
| **Patterns** | Clean Architecture, CQRS/MediatR, Repository pattern, DI, async/await |
| **Testing** | xUnit, Moq, FluentAssertions, WebApplicationFactory |
| **Standards** | SOLID principles, async/await best practices, >80% coverage |
| **Auto-detect** | `.csproj` / `.sln` files |
| **Best Practices** | [dotnet-specialist.md](dotnet-specialist.md) |

#### Node.js Specialist

| Property | Value |
|---|---|
| **Skill ID** | `nodejs-specialist` |
| **Technologies** | Node.js 20+, NestJS, TypeScript, Prisma, class-validator, Jest, Supertest |
| **Patterns** | NestJS module architecture, Controller→Service→Repository, ValidationPipe |
| **Testing** | Jest, Supertest |
| **Auto-detect** | `express`, `nestjs`, `@nestjs/core` in package.json |
| **Best Practices** | [nodejs-specialist.md](nodejs-specialist.md) |

---

### Database Specialists

#### PostgreSQL Specialist

| Property | Value |
|---|---|
| **Skill ID** | `postgresql-specialist` |
| **Technologies** | PostgreSQL 15+, Prisma / EF Core, Testcontainers |
| **Patterns** | UUID PKs, TIMESTAMPTZ, RLS, expand-contract migrations, indexing strategy |
| **Auto-detect** | `prisma/schema.prisma`, `pg` dependency, Docker Compose with postgres service |
| **Best Practices** | [postgresql-specialist.md](postgresql-specialist.md) |

#### SQL Server Specialist

| Property | Value |
|---|---|
| **Skill ID** | `sqlserver-specialist` |
| **Technologies** | SQL Server 2022+, T-SQL, EF Core SQL Server provider, Testcontainers |
| **Patterns** | NEWSEQUENTIALID(), NVARCHAR, DATETIMEOFFSET, TDE, RLS, RCSI |
| **Auto-detect** | `.csproj` with EF SQL Server provider |
| **Best Practices** | [sqlserver-specialist.md](sqlserver-specialist.md) |

---

### Architecture Specialists

#### System Architect

| Property | Value |
|---|---|
| **Skill ID** | `system-architect` |
| **Technologies** | Microservices, DDD, CQRS, event-driven, resilience patterns, OpenAPI |
| **Patterns** | Bounded contexts, Saga, Outbox, circuit breaker, ADRs |
| **Activation** | Explicit assignment or "architecture", "system design", "service boundaries" in task |
| **Best Practices** | [system-architect.md](system-architect.md) |

#### Data Architect

| Property | Value |
|---|---|
| **Skill ID** | `data-architect` |
| **Technologies** | ERD, data modeling, data governance, ETL, polyglot persistence |
| **Patterns** | 3NF/denormalization trade-offs, expand-contract migration, master data management |
| **Activation** | Explicit assignment or "data model", "ERD", "data migration", "data governance" in task |
| **Best Practices** | [data-architect.md](data-architect.md) |

---

### Security Specialists

#### App Security Specialist

| Property | Value |
|---|---|
| **Skill ID** | `app-security` |
| **Technologies** | OWASP Top 10, JWT/OAuth 2.0, CSP, SAST/DAST tools |
| **Patterns** | Defense in depth, parameterized queries, security headers, dependency auditing |
| **Always active** | Included in every verification pipeline |
| **Best Practices** | [app-security.md](app-security.md) |

#### Compliance Specialist

| Property | Value |
|---|---|
| **Skill ID** | `compliance-specialist` |
| **Technologies** | PDPL (Saudi Arabia), ISO 27001, CBAHI, HIPAA |
| **Patterns** | Audit trails, consent management, data minimization, breach notification |
| **Activated by** | Explicit assignment or healthcare/government/finance project detection |
| **Best Practices** | [compliance-specialist.md](compliance-specialist.md) |

---

### QA Specialists

#### Automation QA Specialist

| Property | Value |
|---|---|
| **Skill ID** | `automation-qa` |
| **Technologies** | Jest, Vitest, xUnit, Playwright, Testcontainers, k6 |
| **Patterns** | Test pyramid, AAA, Page Object Model, CI coverage gates, flaky test prevention |
| **Always active** | Included in every verification pipeline |
| **Best Practices** | [automation-qa.md](automation-qa.md) |

#### Performance QA Specialist

| Property | Value |
|---|---|
| **Skill ID** | `performance-qa` |
| **Technologies** | k6, Lighthouse CI, EXPLAIN ANALYZE, clinic.js, dotnet-trace |
| **Patterns** | Smoke/load/stress/soak test types, SLA targets, N+1 detection, caching strategy |
| **Activated by** | Explicit assignment or "performance", "SLA", "load test" in task description |
| **Best Practices** | [performance-qa.md](performance-qa.md) |

---

## Auto-Detection Rules

The system detects the project stack and activates relevant specialists automatically:

| Detection Signal | Specialists |
|---|---|
| `package.json` with `react` | `react-specialist` |
| `package.json` with `vue` | `vue-specialist` |
| `package.json` with `next` | `react-specialist` |
| `package.json` with `express` or `@nestjs/core` | `nodejs-specialist` |
| `.csproj` or `.sln` files | `dotnet-specialist` |
| `prisma/schema.prisma` | `postgresql-specialist` |
| Any project | `app-security`, `automation-qa` |

Explicit `specialist` assignment in a task definition overrides auto-detection.

---

## Skill Lifecycle

```
1. Registration
   Skill manifest loaded from skills/ directory or bundled defaults
       │
       ▼
2. Discovery
   Task arrives → auto-detect or read explicit specialist field
       │
       ▼
3. Configuration
   Skill expertise injected into agent prompt
   Verification commands loaded (or project defaults used)
   Paging triggers activated
       │
       ▼
4. Execution
   Agent operates with specialist knowledge
   Follows specialist patterns and standards
       │
       ▼
5. Verification
   Specialist's verification commands run
   Specialist review checklist applied
       │
       ▼
6. Deactivation
   Skill caches cleared, metrics recorded
```

---

## Adding a New Specialist

Create a YAML manifest in the `skills/` directory:

```yaml
# skills/rust-specialist.yaml
skillId: "rust-specialist"
name: "Rust Specialist"
version: "1.0.0"
description: "Rust systems programming specialist"

expertisePrompt: |
  You are a Rust Specialist with deep expertise in:
  - Ownership, borrowing, and lifetimes
  - Async Rust (tokio, async-std)
  - Error handling with Result and Option
  - Trait design and generics
  - Unsafe Rust only when absolutely necessary

technologies: ["Rust", "Cargo", "Tokio", "Serde"]
patterns: ["Result-based error handling", "Builder pattern", "Newtype pattern"]
standards: ["cargo clippy passes", "cargo test passes", "no unsafe unless justified"]

detection:
  files: ["Cargo.toml"]

contextProfile:
  baselineTokens: 2000
  optimalTokens: 6000
  pagingTriggers: ["similar module", "existing pattern"]

verification:
  testCommand: "cargo test"
  lintCommand: "cargo clippy -- -D warnings"
  buildCommand: "cargo build --release"
```

---

## Next Steps

- [Architecture](../architecture/OVERVIEW.md) — How specialists fit in the system
- [Workflow](../workflow/README.md) — How specialists are assigned and loaded
- [Scrum Teams](../workflow/SCRUM-TEAMS.md) — How specialists form teams and run sprints
- [Epic Sprints](../workflow/EPIC-SPRINTS.md) — Epic → Feature → Sprint → Task hierarchy
- [Interfaces](../interfaces/README.md) — The SkillManifest interface definition
