# Multi-Agent Orchestration System

> Coordinate technology-specialist AI agents through a task-driven loop with enforced verification, persistent memory, and strict resource management.

---

## What Is This?

The Multi-Agent Orchestration System runs specialist AI agents on task lists, verifies their output independently, and learns from every execution. You define tasks (with acceptance criteria), the system assigns the right specialist, executes the work, runs tests and lint, and only marks a task done when everything passes.

**Core loop:** Load task → Assign specialist → Execute → Verify (test/lint/build + acceptance criteria) → Review → Pass or retry.

---

## Key Capabilities

| Capability | Description |
|---|---|
| **Specialist Agents** | Technology-specific experts (React, .NET, PostgreSQL, Security, QA) loaded as skills |
| **Task-Driven** | Load tasks from Markdown, YAML, JSON, or GitHub Issues |
| **Independent Verification** | Test, lint, build commands + acceptance criteria checked independently — agents can't self-certify |
| **Parallel Execution** | Tasks grouped by `parallel_group` run concurrently in isolated sandboxes |
| **Persistent Memory** | Three-tier memory with autonomous paging — agents learn from past work across sessions |
| **Resource Safety** | Strict context budgeting prevents token overflow |
| **Auto-Detection** | Project stack, commands, and specialists detected from project files |

---

## Architecture at a Glance

```
Task Sources (MD / YAML / JSON / GitHub)
         │
         ▼
┌─────────────────────┐
│  Orchestrator Core   │  ← Task Loader, Planning, Budgeting, Scheduling
│  ├─ Verification     │  ← Acceptance criteria, test/lint/build, specialist review
│  ├─ Memory Manager   │  ← Three-tier memory with paging
│  └─ Skill Registry   │  ← 13 specialist skills (React, .NET, PostgreSQL, etc.)
└────────┬────────────┘
         │
         ▼
┌─────────────────────┐
│   Execution Engine   │  ← Sandbox / worktree isolation, retry, git workflow
│   ├─ Specialist      │  ← Technology-specific agents with deep expertise
│   └─ Review Agents   │  ← Independent quality verification
└─────────────────────┘
```

See [Architecture Overview](docs/architecture/OVERVIEW.md) for the full diagram and component definitions.

---

## Documentation Index

### Architecture & Design
- [Architecture Overview](docs/architecture/OVERVIEW.md) — System design, specialist agents, verification engine
- [Component Definitions](docs/architecture/COMPONENTS.md) — Detailed responsibilities of every component
- [Data Flow](docs/architecture/DATA_FLOW.md) — How information moves through the system

### Core Subsystems
- [Context Budgeting](docs/context-budgeting/README.md) — Token allocation, overflow handling, and recovery
- [Memory & Paging](docs/memory/README.md) — Three-tier memory architecture with autonomous retrieval
- [Skill Registry & Specialist Catalog](docs/skills/README.md) — All specialist skills, auto-detection, how to add new ones

### Workflows & Operations
- [Execution Workflow](docs/workflow/README.md) — Complete task lifecycle with task formats, verification, and retry
- [Blocker Handling](docs/workflow/BLOCKER-HANDLING.md) — How the system resolves obstacles
- [Operations Guide](docs/operations/README.md) — Configuration, task formats, verification, isolation modes

### References
- [Interface Contracts](docs/interfaces/README.md) — TypeScript-style API definitions for all interfaces
- [Verification Engine](docs/verification/README.md) — How independent verification works
- [Web Dashboard Spec](docs/dashboard/README.md) — Monitoring and inspection UI specification
- [Success Metrics & SLAs](docs/metrics/README.md) — Measurable targets for system quality
- [Portable Stack Guide](docs/stack/README.md) — Implementation guidance for self-contained deployment

### General
- [Glossary](GLOSSARY.md) — Definitions of all domain-specific terms
- [Contributing](CONTRIBUTING.md) — How to contribute to this project
