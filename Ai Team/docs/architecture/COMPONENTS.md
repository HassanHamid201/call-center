# Component Definitions

Detailed breakdown of every component in the system, its responsibilities, and its relationships.

---

## Orchestrator Core

The central coordinating system. It is not an agent — it is the infrastructure agents operate within.

### Epic Loader

**Purpose:** Ingest epics, features, and tasks from multiple formats and normalize them into a unified hierarchy.

| Responsibility | Description |
|---|---|
| Format detection | Identify whether input is an Epic YAML, plain task YAML/JSON, Markdown, or GitHub Issues repo |
| Epic/feature parsing | Extract epic name, features, feature-level acceptance criteria, and team composition hints |
| Task parsing | Extract title, description, acceptance criteria, specialist assignment, parallel group, dependencies |
| Dependency resolution | Build a dependency graph for both feature-level and task-level dependencies |
| Progress tracking | Track completion state per feature and per task; update source files in-place |
| GitHub sync | Optionally sync progress back to GitHub Issues |

**Inputs:** Epic/task file path or GitHub repo from client
**Outputs:** Normalized Epic → Feature → Task tree for the Orchestrator and Planning Engine

---

## Scrum Team Pool

A dynamic agent layer between the Orchestrator Core and the Agent Workers. The Orchestrator spawns one Scrum Team per feature.

### Team Pool Manager

**Purpose:** Manages the lifecycle of all active Scrum Teams.

| Responsibility | Description |
|---|---|
| Team formation | Compose a team from the feature's task specialist list; always add Scrum Master, `app-security`, `automation-qa` |
| Concurrency cap | Enforce `max_parallel_features` (default: 4); queue remaining features until a team slot frees |
| Budget allocation | Slice a context budget for each team from the global pool |
| Sprint monitoring | Track sprint state per team (planning, executing, verifying, complete) |
| Team recycling | Dissolve teams on sprint completion; release budget; notify Orchestrator |

**Inputs:** Feature definitions from Epic Loader, global budget from Context Budgeter
**Outputs:** Sprint completion events with telemetry back to Orchestrator

### Scrum Master Agent

**Purpose:** Per-team coordinator. Does not write code — coordinates specialists within its team.

| Responsibility | Description |
|---|---|
| Sprint planning | Decompose feature into tasks, assign parallel groups and dependencies, produce Sprint Plan |
| Standup | Collect agent status (done/in-progress/blocked) after each task batch; record in Tier 2 memory |
| Handoff coordination | Package upstream artifacts, inject Handoff Package into downstream specialist context |
| Blocker escalation | Level 1–2 autonomous resolution; Level 3 escalation to Orchestrator; Level 4 to human |
| Retrospective | Post-sprint: collect metrics, extract lessons, write Retrospective Record to Tier 2, promote heuristics to Tier 3 |

See [Scrum Teams](../workflow/SCRUM-TEAMS.md) for the full protocol.

---

## Orchestrator Core

The central coordinating system. It is not an agent — it is the infrastructure agents operate within.

### Task Loader

**Purpose:** Ingest tasks from multiple formats and normalize them into a unified Task structure.

> **Note:** For epic-based workflows, use the Epic Loader above. The Task Loader handles flat task lists (YAML, JSON, Markdown, GitHub Issues) without the feature/epic hierarchy.

| Responsibility | Description |
|---|---|
| Format detection | Identify whether input is Markdown, YAML, JSON, or a GitHub Issues repo |
| Task parsing | Extract title, description, acceptance criteria, specialist assignment, parallel group |
| Progress tracking | Track completion state in-place (checkboxes in Markdown, `completed` flags in YAML/JSON) |
| Task caching | Reduce file I/O by loading tasks once and batching completion writes |
| GitHub sync | Optionally sync progress back to GitHub Issues |

**Supported formats:**

| Format | Example | Features |
|---|---|---|
| Markdown | `PRD.md` with `- [ ] task` / `- [x] done` | Simple, human-readable, in-place updates |
| Markdown folder | `prd/backend.md`, `prd/frontend.md` | Large projects split across files, tracked per-file |
| YAML | `tasks.yaml` with structured task objects | Parallel groups, dependencies, acceptance criteria |
| JSON | `tasks.json` with task array | Programmatic generation, CI/CD integration |
| GitHub Issues | `--github owner/repo` | Existing issue tracker, label filtering |

**Inputs:** Task file path or GitHub repo from client
**Outputs:** Normalized Task objects for the Planning Engine and Task Scheduler

### Planning Engine

**Purpose:** Translates incoming tasks into executable plans.

| Responsibility | Description |
|---|---|
| Intent parsing | Interprets task descriptions to extract requirements and constraints |
| Complexity scoring | Estimates difficulty (1–10) based on requirements count, ambiguity, and affected scope |
| Task decomposition | Breaks large tasks into sub-tasks with dependencies |
| Specialist assignment | Matches task requirements to specialist skills (auto-detected or explicitly assigned) |
| Strategy selection | Chooses sequential vs. parallel execution based on task structure and parallel groups |
| Dependency resolution | Orders sub-tasks so prerequisites complete before dependents |

**Inputs:** Normalized Task objects from Task Loader
**Outputs:** Structured Task Plan, execution strategy, specialist assignments

### Context Budgeter

**Purpose:** Guarantees the system stays within token limits.

| Responsibility | Description |
|---|---|
| Budget allocation | Distributes available tokens across orchestrator, execution, and review phases |
| Utilization tracking | Monitors real-time token usage per component |
| Overflow detection | Identifies when a component approaches its limit |
| Compression triggering | Initiates summarization, tier demotion, or truncation as needed |
| Emergency management | Handles critical-path preservation when hard limits are exceeded |
| Budget recovery | Reclaims tokens from completed phases and successful compression |

See [Context Budgeting](../context-budgeting/README.md) for the full specification.

### Task Scheduler

**Purpose:** Manages the lifecycle and execution order of work items.

| Responsibility | Description |
|---|---|
| Queue management | Priority-ordered task queues with dependency awareness |
| Parallel group batching | Tasks sharing a `parallel_group` run concurrently in isolated environments |
| Agent assignment | Matches tasks to appropriate specialist agents |
| Progress tracking | Monitors task status (queued, running, verifying, completed, failed, blocked) |
| Deferred task tracking | Temporarily failed tasks (rate limits, transient errors) are deferred with attempt counts |
| Retry logic | Exponential backoff with jitter (configurable max retries, default 3) |

**Parallel Execution Model:**

Tasks with matching `parallel_group` values are batched and run concurrently:

```
Group 1 (parallel):  [Create User model] [Create Post model] [Create Auth module]
                                    ↓ (all complete)
Group 2 (parallel):  [Add relationships] [Wire authentication]
                                    ↓ (all complete)
Group 3 (sequential): [Integration tests] [Deploy]
```

Each parallel agent gets an isolated environment (sandbox or worktree).

### Memory Manager

**Purpose:** Maintains the three-tier memory model and coordinates data movement.

| Responsibility | Description |
|---|---|
| Tier management | Manages Working Memory, Structured Storage, and Deep Archive |
| Session hydration | Loads initial context when an agent session starts (relevant decisions, patterns, heuristics) |
| Data promotion | Moves relevant content from cold to hot tiers (via paging) |
| Data demotion | Moves content from hot to cold tiers (during compression) |
| Index maintenance | Keeps search indexes current for paging operations |

See [Memory & Paging](../memory/README.md) for the full specification.

### Overflow Handler

**Purpose:** Executes compression strategies when the Context Budgeter detects approaching limits.

| Responsibility | Description |
|---|---|
| Summarization | Condenses detailed history into compact summaries |
| Tier demotion | Moves Working Memory content to Structured Storage or Deep Archive |
| Truncation | Removes oldest, lowest-priority context (FIFO with priority weighting) |
| Critical path preservation | During emergencies, identifies and keeps only task-essential context |
| Recovery coordination | Works with Context Budgeter to restore normal operation after overflow |

See [Overflow Handling](../context-budgeting/OVERFLOW.md) for the full protocol.

### Paging Controller

**Purpose:** Coordinates deep retrieval requests from agents.

| Responsibility | Description |
|---|---|
| Request routing | Directs paging queries to appropriate memory tiers |
| Cost estimation | Pre-calculates token cost of retrieval operations |
| Quota enforcement | Ensures agents don't exceed their paging budget |
| Result caching | Caches retrieved content for zero-cost reuse within a session |
| Deduplication | Detects and eliminates redundant queries |

### Verification Engine

**Purpose:** Independently validates agent output against acceptance criteria and project standards.

| Responsibility | Description |
|---|---|
| Acceptance criteria checking | Parse each criterion and verify (automated or via review agent) |
| Test execution | Run project's test command, parse results (pass/fail, coverage) |
| Lint execution | Run project's lint command, check exit code |
| Build execution | Run project's build command, check exit code |
| Specialist review | Coordinate review agents with relevant specialist expertise |
| Verdict generation | Produce pass/fail verdict with evidence and feedback |

The Verification Engine acts independently from the Specialist Agent. It does not trust the agent's self-assessment.

**Verification pipeline:**

```
Agent completes task
        │
        ▼
Phase 1: Acceptance criteria check
  • Automated: "tests pass" → run test command, check exit code
  • Automated: "TypeScript strict" → run tsc --noEmit
  • Automated: "coverage > 80%" → parse test runner output
  • Review: "accessible" → spawn review agent with accessibility specialist
        │
        ▼
Phase 2: Automated verification
  • Run commands.test   → must exit 0
  • Run commands.lint   → must exit 0
  • Run commands.build  → must exit 0
  • Parse output for test counts, coverage %, lint errors
        │
        ▼
Phase 3: Specialist review (if specialist assigned)
  • Load specialist's review checklist
  • Review agent evaluates: correctness, patterns, security, performance
        │
        ▼
Verdict:
  All pass → PASS
  Any fail → FAIL with specific feedback → retry with delta-prompt
```

---

## Execution Engine

The runtime environment for agent workers.

### Isolation Modes

Agents run in isolated environments to prevent interference, especially during parallel execution:

#### Sandbox Mode (default)

Lightweight isolation using filesystem-level separation:

- **Symlinks** for read-only dependencies (`node_modules/`, `.git/`, `vendor/`, `.venv/`)
- **Copies** of source files the agent might modify (`src/`, `app/`, `lib/`, config files)
- **Fast** — avoids duplicating gigabytes of dependencies across agents
- **Sync** — Modified files are detected by timestamp comparison and synced back to the original directory

#### Worktree Mode

Full git worktree isolation when agents need git history access:

- Each agent gets a full git worktree with its own branch
- Agents can run git commands, view history, create commits
- After completion, branches are merged back (with AI-assisted conflict resolution if needed)
- Used when: agent needs git operations, repo is small enough that worktree overhead is negligible

### Git Branch Workflow

For tasks that modify code:

| Mode | Behavior |
|---|---|
| Branch-per-task | Each task gets its own branch (`orchestrator/<task-slug>`) |
| Auto-merge | After verification passes, branch merges back to base |
| Auto-PR | Optionally create pull requests (or draft PRs) |
| Conflict resolution | AI-assisted merge conflict resolution when parallel agents touch overlapping files |

### Agent Session Lifecycle

Each agent session follows this lifecycle:

```
Session starts
  │
  ├── Memory Manager hydrates Working Memory
  │   (relevant decisions, patterns, paged-in context)
  │
  ├── Specialist skill loaded into prompt
  │   (technology expertise, best practices, standards)
  │
  ├── Agent executes with enforced steps:
  │   1. Implement the task
  │   2. Write tests for the implementation
  │   3. Run tests — ensure they pass
  │   4. Run lint — ensure it passes
  │   5. Ensure the code works correctly
  │   6. Commit changes
  │
  ├── Agent may page memory when blocked or uncertain
  │
  └── Agent session ends → Verification Engine runs
```

### Retry and Deferred Tasks

When a task fails transiently:

| Error Type | Response |
|---|---|
| **Retryable** (rate limit, timeout, network) | Exponential backoff with jitter; task deferred for later retry |
| **Fatal** (auth failure, command not found) | Abort task immediately; log for human investigation |
| **Verification failure** | Retry with delta-prompt containing verification feedback |
| **Max retries exceeded** | Mark as failed, escalate to human with full context |

Deferred tasks are persisted to disk so they survive restarts.

---

## Skill Registry

The specialist catalog. See [Skill Registry](../skills/README.md) for the full specification.

### Specialist Catalog

The system ships with a curated set of specialist skills:

**Frontend:**

| Specialist | Technologies |
|---|---|
| React Specialist | React 16+, TypeScript, Next.js 12+, Redux Toolkit, Jest, Cypress |
| Vue Specialist | Vue 3+, TypeScript, Pinia, Vitest, Playwright |
| .NET MVC Specialist | .NET 8/9/10 MVC, C#, Razor Pages |

**Backend:**

| Specialist | Technologies |
|---|---|
| .NET Core Specialist | .NET 8/9/10, C# 12+, ASP.NET Core, EF Core |
| Node.js Specialist | Node.js 18+, Express/NestJS, TypeScript |

**Database:**

| Specialist | Technologies |
|---|---|
| PostgreSQL Specialist | PostgreSQL 14+, SQL, indexing, migrations |
| SQL Server Specialist | SQL Server 2022+, T-SQL, stored procedures |

**Architecture:**

| Specialist | Technologies |
|---|---|
| System Architect | Design patterns, microservices, distributed systems |
| Data Architect | Data modeling, migration strategies, governance |

**Security:**

| Specialist | Technologies |
|---|---|
| App Security | OWASP, secure coding, vulnerability assessment |
| Compliance | PDPL, ISO 27001, CBAHI, HIPAA |

**QA:**

| Specialist | Technologies |
|---|---|
| Automation QA | Test frameworks, CI/CD testing, coverage |
| Performance QA | Load testing, profiling, optimization |

### Auto-Detection

The system auto-detects the project's technology stack and activates relevant specialists:

| Detection Signal | Specialists Activated |
|---|---|
| `package.json` with `react` dependency | `react-specialist` |
| `package.json` with `vue` dependency | `vue-specialist` |
| `package.json` with `next` dependency | `react-specialist` |
| `.csproj` or `.sln` files | `dotnet-specialist` |
| `package.json` with `express` or `nestjs` | `nodejs-specialist` |
| `prisma/schema.prisma` | `postgresql-specialist` |
| Any project (always) | `app-security`, `automation-qa` |

Detection can be overridden by explicitly assigning a specialist in the task definition.

---

## Component Interaction Map

```
Epic Loader ─────► Orchestrator (Product Owner)
(epic.yaml /               │
 tasks.yaml /              │ spawn one team per feature (parallel)
 GitHub)                   │
               ┌───────────┼───────────┬───────────┐
               ▼           ▼           ▼
          Team A        Team B        Team C
       (Feature A)  (Feature B)  (Feature C)
             │
             ▼ (within each team)
       Scrum Master Agent
       ├─ Sprint Plan → Task Scheduler → Execution Engine
       ├─ Standup (after each batch)
       ├─ Handoff Packages → downstream specialists
       └─ Retrospective → Memory Manager (Tier 2)
             │
             ▼ (within each task)
Planning Engine → Context Budgeter → Skill Registry → Execution Engine
                                                             │
      ┌──────────────────────────────────────────────────────┘
      │
      ▼
Verification Engine ◄──── Context Budgeter
  (acceptance criteria,
   test/lint/build,
   specialist review)
      │
      ▼
   Verdict ──────────► Scrum Master (standup update)
   (pass/fail/         │
    blocked)           ├── PASS    → mark complete, store memory
                       ├── FAIL    → retry with delta-prompt (max 3)
                       └── BLOCKED → Scrum Master → Orchestrator → human
                                                    │
                                                    ▼
                                          Memory Manager
                                          (store results, update tiers)
```

---

## Next Steps

- [Data Flow](DATA_FLOW.md) — How data moves between these components
- [Scrum Teams](../workflow/SCRUM-TEAMS.md) — Team formation, Scrum Master Agent, ceremonies
- [Epic & Sprint Hierarchy](../workflow/EPIC-SPRINTS.md) — Epic → Feature → Sprint → Task model
- [Context Budgeting](../context-budgeting/README.md) — Deep dive into the Context Budgeter
- [Memory & Paging](../memory/README.md) — Deep dive into the Memory Manager and Paging Controller
- [Skills](../skills/README.md) — The specialist skill catalog
- [Workflow](../workflow/README.md) — The complete task execution lifecycle
- [Verification Engine](../verification/README.md) — How independent verification works
