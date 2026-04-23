# Architecture Overview

The Multi-Agent Orchestration System coordinates technology-specialist agents through a task-driven loop with enforced verification, persistent memory, and strict resource management.

---

## System Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                            CLIENT LAYER                         │
│                     (Web Dashboard / API / CLI)                 │
└──────────────────────────┬──────────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────────┐
│                ORCHESTRATOR CORE (Product Owner)                │
│                                                                 │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────────────┐     │
│  │  Epic Loader  │  │   CONTEXT    │  │  Task Scheduler   │     │
│  │  (multi-fmt)  │  │   BUDGETER   │  │  (parallel groups)│     │
│  └──────────────┘  └──────────────┘  └───────────────────┘     │
│                                                                 │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────────────┐     │
│  │   Memory     │  │   Overflow   │  │   Paging          │     │
│  │   Manager    │  │   Handler    │  │   Controller      │     │
│  └──────────────┘  └──────────────┘  └───────────────────┘     │
│                                                                 │
│  ┌──────────────┐  ┌──────────────────────────────────────┐    │
│  │   Planning   │  │        VERIFICATION ENGINE            │    │
│  │   Engine     │  │  acceptance criteria · test/lint/build│    │
│  └──────────────┘  └──────────────────────────────────────┘    │
│                                                                 │
└──────┬──────────────────────────────────────────────────────────┘
       │  spawns one team per feature (parallel)
┌──────▼──────────────────────────────────────────────────────────┐
│                      SCRUM TEAM POOL                            │
│                                                                 │
│  Team A (Feature: Auth)    Team B (Feature: Reg)    Team C ...  │
│  ┌────────────────────┐   ┌────────────────────┐               │
│  │ Scrum Master Agent │   │ Scrum Master Agent │               │
│  │ .NET Core Spec     │   │ React Specialist   │               │
│  │ React Specialist   │   │ .NET Core Spec     │               │
│  │ PostgreSQL Spec    │   │ App Security       │               │
│  │ Automation QA      │   │ Automation QA      │               │
│  └────────────────────┘   └────────────────────┘               │
│   Feature-complete sprint   Feature-complete sprint             │
│                                                                 │
└──────┬──────────────────────────────────┬───────────────────────┘
       │                                  │
┌──────▼──────┐                    ┌──────▼──────┐
│   SKILL     │                    │  EXECUTION  │
│   REGISTRY  │◄───────────────────│   ENGINE    │
│  (specialist│    page requests   │  (sandbox / │
│   catalog)  │                    │   worktree) │
└──────┬──────┘                    └──────┬──────┘
       │                                  │
┌──────▼──────────────────────────────────▼──────────────────────┐
│                        AGENT WORKERS                            │
│                                                                 │
│  Specialists (loaded as skills per task):                       │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │
│  │ React    │ │ .NET Core│ │ Postgre  │ │ Security │ ...      │
│  │ + Pager  │ │ + Pager  │ │ SQL      │ │ + Pager  │          │
│  └──────────┘ └──────────┘ │ + Pager  │ └──────────┘          │
│                            └──────────┘                        │
│  Review Agents:                                                 │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐                       │
│  │ Testing  │ │   Code   │ │Compliance│                       │
│  │ + Pager  │ │ Review   │ │ + Pager  │                       │
│  └──────────┘ │ + Pager  │ └──────────┘                       │
│               └──────────┘                                      │
└─────────────────────────────────────────────────────────────────┘
```

---

## Layer Responsibilities

### Client Layer

The external interface for interacting with the system:

- **CLI** — Submit tasks, manage projects, view progress
- **Web Dashboard** — Real-time monitoring of budgets, memory, agent activity, and verification results
- **REST/GraphQL API** — Programmatic task submission and result retrieval (future scope)

All client types talk to the Orchestrator Core through the same interface.

### Orchestrator Core

The central coordinator. See [Component Definitions](COMPONENTS.md) for full details.

| Component | Purpose |
|---|---|
| **Epic Loader** | Loads epics, features, and tasks from multiple formats; parses the Epic → Feature → Task hierarchy and acceptance criteria at all levels |
| **Planning Engine** | Interprets task intent, scores complexity, decomposes work, selects execution strategy |
| **Context Budgeter** | Allocates and enforces token budgets per team and per agent |
| **Task Scheduler** | Queues work, manages parallel groups within a team, dispatches to agents |
| **Memory Manager** | Maintains the three-tier memory model |
| **Overflow Handler** | Manages context compression and emergency protocols |
| **Paging Controller** | Coordinates deep retrieval requests from agents |
| **Verification Engine** | Enforces acceptance criteria, runs test/lint/build commands, coordinates specialist review |

### Scrum Team Pool

A dynamic layer between the Orchestrator and Agent Workers. The Orchestrator spawns one Scrum Team per feature, each with its own Scrum Master Agent and a set of specialist agents.

| Component | Purpose |
|---|---|
| **Team Pool Manager** | Spawns and monitors Scrum Teams; enforces `max_parallel_features`; recycles teams when sprints complete |
| **Scrum Master Agent** | Per-team coordinator: sprint planning, standup, handoff packages, blocker escalation, retrospective |

See [Scrum Teams](../workflow/SCRUM-TEAMS.md) and [Epic & Sprint Hierarchy](../workflow/EPIC-SPRINTS.md) for full specifications.

### Skill Registry

The system's specialist catalog. Each skill bundles:

- **Technology expertise** — Deep knowledge of a specific stack (React, .NET, PostgreSQL, etc.)
- **Best practices** — Patterns, anti-patterns, and standards for that technology
- **Verification checklist** — What "done right" looks like for that specialist
- **Auto-detection rules** — How to recognize when this specialist should be activated

See [Skill Registry](../skills/README.md) for the full specialist catalog.

### Execution Engine

Runs agent workers in isolated environments:

- **Sandbox mode** — Symlinks read-only dependencies, copies source files for fast isolation
- **Worktree mode** — Full git worktree isolation when agents need git history access
- Provides each agent with its configuration, budget, specialist skill, and paging toolkit
- Monitors execution for budget compliance
- Reports results, verification outcomes, and memory updates back to the orchestrator

### Agent Workers

Two categories of agents, all equipped with paging tools:

**Specialist Agents** (task execution):

| Agent | Domain | Expertise | Best Practices |
|---|---|---|---|
| Scrum Master Agent | Coordination | Sprint planning, standup, handoff, retrospective | [scrum-master.md](../skills/scrum-master.md) |
| React Specialist | Frontend | React, TypeScript, Next.js, Redux, accessibility | [react-specialist.md](../skills/react-specialist.md) |
| Vue Specialist | Frontend | Vue 3, Pinia, composition API | [vue-specialist.md](../skills/vue-specialist.md) |
| .NET MVC Specialist | Frontend | .NET MVC, Razor, server-side rendering | [net-mvc-specialist.md](../skills/net-mvc-specialist.md) |
| .NET Core Specialist | Backend | ASP.NET Core, EF Core, C#, microservices | [dotnet-specialist.md](../skills/dotnet-specialist.md) |
| Node.js Specialist | Backend | Express, NestJS, TypeScript | [nodejs-specialist.md](../skills/nodejs-specialist.md) |
| PostgreSQL Specialist | Database | Schema design, query optimization, migrations | [postgresql-specialist.md](../skills/postgresql-specialist.md) |
| SQL Server Specialist | Database | T-SQL, stored procedures, enterprise features | [sqlserver-specialist.md](../skills/sqlserver-specialist.md) |
| System Architect | Architecture | Design patterns, microservices, distributed systems | [system-architect.md](../skills/system-architect.md) |
| Data Architect | Architecture | Data modeling, migration strategies, governance | [data-architect.md](../skills/data-architect.md) |
| App Security Specialist | Security | OWASP, secure coding, vulnerability assessment | [app-security.md](../skills/app-security.md) |
| Automation QA Specialist | QA | Test frameworks, CI/CD testing, coverage | [automation-qa.md](../skills/automation-qa.md) |
| Performance QA Specialist | QA | Load testing, profiling, optimization | [performance-qa.md](../skills/performance-qa.md) |
| Compliance Specialist | Compliance | PDPL, ISO 27001, CBAHI, HIPAA | [compliance-specialist.md](../skills/compliance-specialist.md) |

**Review Agents** (quality gates):

| Agent | Purpose |
|---|---|
| Testing Agent | Creates and runs validation suites |
| Code Review Agent | Reviews against specialist standards and project conventions |
| Compliance Agent | Checks regulatory requirements (PDPL, ISO 27001, HIPAA, etc.) |

---

## Data Flow Summary

```
User submits epic (or feature list) via CLI, API, or dashboard
       │
       ▼
Epic Loader parses format, extracts features + tasks + acceptance criteria
       │
       ▼
Orchestrator (Product Owner) prioritizes features, allocates per-team budgets
       │
       ▼  (one branch per feature, all parallel up to max_parallel_features)
       ├── Feature A → spawn Scrum Team A
       ├── Feature B → spawn Scrum Team B
       └── Feature C → spawn Scrum Team C
              │
              ▼  (within each team)
       Scrum Master: Sprint Planning → parallel batches → standup → retrospective
              │
              ▼  (within each batch)
       Planning Engine scores complexity, assigns specialist
              │
              ▼
       Context Budgeter allocates tokens per agent
              │
              ▼
       Skill Registry loads specialist expertise
              │
              ▼
       Execution Engine runs Specialist Agent (with paging)
              │  └─ Agent follows: implement → write tests → run tests → run lint → commit
              ▼
       Verification Engine checks acceptance criteria
              │  └─ Runs test/lint/build commands independently
              │  └─ Specialist review agent evaluates quality
              ▼
              ├── PASS → Store artifacts, update memory, next batch/task
              ├── FAIL → Retry with delta-prompt (max 3)
              └── BLOCKER → Scrum Master escalates (team → Orchestrator → human)
```

For the detailed phase-by-phase flow, see [Data Flow](DATA_FLOW.md).

---

## Key Design Decisions

### Why Technology-Specific Specialist Agents?

A single generic "Implementation Agent" produces generic output. Specialists know:
- **React** — functional components, hooks, TypeScript strict mode, accessibility
- **.NET** — layered architecture, DI patterns, EF Core best practices
- **PostgreSQL** — indexing strategies, query optimization, migration safety

Specialist expertise is loaded as a skill — the same agent framework runs all specialists. Adding a new technology means adding a new skill manifest, not rebuilding the agent.

### Why Independent Verification?

Agents are told to write tests and run them, but they can mark tasks complete without actually doing it. The Verification Engine runs independently:

1. **Automated checks** — `npm test`, `npm run lint`, `npm run build` must all pass
2. **Acceptance criteria** — Each criterion in the task is checked (automated where possible, reviewed where subjective)
3. **Specialist review** — The relevant specialist reviews the output against its standards

A task is not complete until verification passes. This prevents the most common failure mode: superficially correct output that fails in practice.

### Why Multiple Task Formats?

Projects already have task lists in different forms:
- **Markdown** — Simple checklists in PRD files
- **YAML/JSON** — Structured tasks with parallel groups, dependencies, and acceptance criteria
- **GitHub Issues** — Existing issue trackers

The Task Loader handles all of them, so teams don't need to change how they plan work.

### Why Paging Instead of Pre-loading?

Pre-loading everything into context is wasteful and often exceeds limits. Paging:
- **Loads only what's needed** — Agents retrieve context on demand
- **Respects budgets** — Paging costs are tracked and capped
- **Mirrors real workflows** — Engineers don't re-read the entire codebase before each change

---

## Next Steps

- [Component Definitions](COMPONENTS.md) — What each component does in detail
- [Data Flow](DATA_FLOW.md) — How information moves through the system
- [Context Budgeting](../context-budgeting/README.md) — How tokens are managed
- [Memory & Paging](../memory/README.md) — How memory is structured and accessed
- [Skills](../skills/README.md) — The specialist agent catalog
- [Verification Engine](../verification/README.md) — How independent verification works
- [Operations Guide](../operations/README.md) — Configuration, task formats, and daily operations
