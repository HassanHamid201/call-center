# Epic & Sprint Hierarchy

How work is organized from high-level business goals down to individual agent tasks, and how multiple feature teams run in parallel.

---

## Hierarchy

```
Epic
 └── Feature 1 ──► Scrum Team A ──► Sprint (feature-complete)
 │     ├── Task 1.1 (parallel_group: 1)
 │     ├── Task 1.2 (parallel_group: 1)
 │     └── Task 1.3 (parallel_group: 2, depends on 1.1)
 │
 └── Feature 2 ──► Scrum Team B ──► Sprint (feature-complete)  (parallel with Team A)
 │     ├── Task 2.1
 │     └── Task 2.2
 │
 └── Feature 3 ──► Scrum Team C ──► Sprint (feature-complete)  (parallel with A & B)
       └── Task 3.1
```

| Level | Owner | Granularity |
|---|---|---|
| **Epic** | Orchestrator (Product Owner) | A major deliverable spanning multiple features (e.g., "Healthcare Platform v1") |
| **Feature** | Scrum Team | A user-facing capability (e.g., "Patient Registration", "Authentication") |
| **Sprint** | Scrum Master Agent | The complete execution cycle for one feature; ends when all tasks are verified done |
| **Task** | Specialist Agent | A single unit of work with explicit acceptance criteria |

---

## Task Definition Schema

The full schema for defining epics, features, and tasks:

```yaml
# tasks.yaml — top-level epic definition
epic: "Healthcare Platform v1"
version: "1.0"
description: "Core patient management capabilities for the clinic portal"

features:
  - feature: "Patient Registration"
    description: "Full patient onboarding flow with Saudi ID validation"
    team: auto                    # auto = Orchestrator selects specialists from task list
    priority: high                # high | medium | low (affects scheduling order)
    acceptance_criteria:
      - "Patient can register with Saudi national ID"
      - "Arabic and English name fields supported"
      - "Form accessible at WCAG 2.1 AA"
    tasks:
      - id: "reg-001"
        title: "Create patients DB schema and migration"
        specialist: "postgresql"
        parallel_group: 1
        estimated_complexity: 4
        acceptance_criteria:
          - "Migration applies cleanly from empty DB"
          - "national_id column has unique index"
          - "Supports both Hijri and Gregorian date storage"

      - id: "reg-002"
        title: "Create patient registration API endpoint"
        specialist: "dotnet"
        parallel_group: 2
        dependencies: ["reg-001"]
        estimated_complexity: 6
        acceptance_criteria:
          - "POST /api/patients returns 201 with patient ID"
          - "422 returned for invalid national ID format"
          - "Integration tests pass"
          - "Response time < 200ms at p95"

      - id: "reg-003"
        title: "Create patient registration React form"
        specialist: "react"
        parallel_group: 2
        dependencies: ["reg-001"]        # needs schema; API can be mocked
        estimated_complexity: 5
        acceptance_criteria:
          - "TypeScript strict mode passes"
          - "Unit test coverage > 80%"
          - "WCAG 2.1 AA compliance"
          - "Arabic/English field labels"

  - feature: "Authentication"
    description: "JWT-based login and session management"
    team: auto
    priority: high
    tasks:
      - id: "auth-001"
        title: "Create users table and password hashing utility"
        specialist: "postgresql"
        parallel_group: 1
        acceptance_criteria:
          - "Passwords stored as bcrypt hash (cost factor >= 12)"
          - "Migration applies cleanly"

      - id: "auth-002"
        title: "Create login and refresh token API"
        specialist: "dotnet"
        parallel_group: 2
        dependencies: ["auth-001"]
        acceptance_criteria:
          - "JWT signed with RS256"
          - "Refresh token stored in httpOnly cookie"
          - "Rate limiting: max 10 login attempts per minute per IP"

      - id: "auth-003"
        title: "Create login form and auth state management"
        specialist: "react"
        parallel_group: 2
        dependencies: ["auth-001"]
        acceptance_criteria:
          - "JWT stored in memory only (not localStorage)"
          - "Silent refresh before token expiry"
          - "Unit tests cover auth state transitions"
```

### Schema Reference

| Field | Level | Type | Description |
|---|---|---|---|
| `epic` | Epic | string | Epic name |
| `version` | Epic | string | Version tag for this epic |
| `features` | Epic | array | List of features in this epic |
| `feature` | Feature | string | Feature name |
| `team` | Feature | `auto` \| specialist list | Team composition: auto-detect or explicit |
| `priority` | Feature | `high`\|`medium`\|`low` | Scheduling priority relative to other features |
| `acceptance_criteria` | Feature | string[] | Feature-level criteria (all tasks must pass these) |
| `tasks` | Feature | array | Task list |
| `id` | Task | string | Unique task identifier |
| `title` | Task | string | Task description |
| `specialist` | Task | string | Specialist skill ID |
| `parallel_group` | Task | number | Tasks sharing this value run concurrently |
| `dependencies` | Task | string[] | Task IDs that must complete before this task |
| `estimated_complexity` | Task | 1–10 | Planning hint; Orchestrator may override |
| `acceptance_criteria` | Task | string[] | Conditions for task verification |

---

## Multi-Feature Parallel Execution

The Orchestrator spawns one Scrum Team per feature and runs them in parallel, up to `max_parallel_features` (default: 4).

### Orchestration Flow

```
Epic loaded
    │
    ▼
Orchestrator parses features → sorts by priority
    │
    ▼
For each feature (up to max_parallel_features simultaneously):
    │
    ├── Spawn Scrum Team (form specialists from task list)
    ├── Allocate context budget (per-team budget)
    ├── Inject Scrum Master Agent
    └── Start feature sprint

Each team runs independently:
    Team A: Planning → Batch 1 → Standup → Batch 2 → Standup → Verify → Retro
    Team B: Planning → Batch 1 → Standup → Batch 2 → Standup → Verify → Retro
    Team C: Planning → Batch 1 → Standup → Verify → Retro

Teams complete at different times.
As each team completes, Orchestrator:
    └── Records feature as done
    └── Starts next queued feature (if any)
    └── Updates epic progress
```

### Cross-Feature Dependencies

Occasionally a feature depends on another feature completing first. Define this at the feature level:

```yaml
features:
  - feature: "Authentication"
    id: "feat-auth"
    priority: high
    tasks: [...]

  - feature: "Patient Registration"
    id: "feat-patient-reg"
    priority: high
    dependencies: ["feat-auth"]    # cannot start until Authentication is complete
    tasks: [...]
```

The Orchestrator will not spawn the dependent team until all its feature dependencies are verified complete.

---

## Sprint Lifecycle Detail

```
┌────────────────────────────────────────────────────────────────┐
│                      FEATURE SPRINT                            │
│                                                                │
│  1. PLANNING                                                   │
│     Scrum Master reads feature + tasks                         │
│     Resolves parallel groups and dependency order              │
│     Produces Sprint Plan → stored in Tier 2 memory             │
│                                                                │
│  2. EXECUTION BATCHES (repeated until all tasks done)          │
│     ┌──────────────────────────────────────┐                  │
│     │  Batch N (parallel tasks)            │                  │
│     │  Agent 1 → Sandbox → Task X          │                  │
│     │  Agent 2 → Sandbox → Task Y          │                  │
│     │  Agent 3 → Sandbox → Task Z          │                  │
│     └──────────────────────────────────────┘                  │
│          │                                                     │
│          ▼                                                     │
│     Verification Engine: automated checks + specialist review  │
│          │                                                     │
│          ▼                                                     │
│     Standup: Scrum Master records results                      │
│          │                                                     │
│          ├── All passed → next batch (or step 3)               │
│          └── Any failed → retry with delta-prompt              │
│                                                                │
│  3. FEATURE ACCEPTANCE                                         │
│     All tasks verified ✅                                      │
│     Feature-level acceptance criteria checked                  │
│          │                                                     │
│          ├── PASS → step 4                                     │
│          └── FAIL → Scrum Master identifies gap task           │
│                                                                │
│  4. RETROSPECTIVE                                              │
│     Scrum Master writes retro record → Tier 2                  │
│     Key heuristics promoted → Tier 3                          │
│          │                                                     │
│          ▼                                                     │
│     SPRINT COMPLETE → Orchestrator notified                    │
└────────────────────────────────────────────────────────────────┘
```

---

## Context Budget Allocation for Teams

Each Scrum Team gets an isolated budget slice from the global context pool:

```
Global Context Budget (e.g., 500K tokens/session)
    │
    ├── Orchestrator overhead: 50K
    ├── Team A budget: 150K
    │     ├── Scrum Master: 10K
    │     ├── Specialist agent budgets: 120K (split across active agents)
    │     └── Verification: 20K
    ├── Team B budget: 150K
    └── Team C budget: 150K
```

Budget rules:
- Teams cannot exceed their allocation; Orchestrator will queue rather than overspend
- If a team needs more, Scrum Master requests a budget increase from Orchestrator
- Budget released back to pool when sprint completes

See [Context Budgeting](../context-budgeting/README.md) for the full specification.

---

## Epic Completion

An epic is complete when all its features have completed their sprints (all tasks verified done, all feature-level acceptance criteria passed).

```yaml
# Epic completion record (stored in Tier 2)
epic_completion:
  epic: "Healthcare Platform v1"
  completed_at: "2026-04-23T18:45:00Z"
  features_completed: 3
  features_total: 3
  total_tasks: 24
  total_tokens_used: 320000
  overall_first_pass_rate: 71%
  key_decisions:
    - "Used bcrypt cost factor 12 for password hashing"
    - "Saudi national ID validated via NIC regex pattern"
  promoted_to_archive: true
```

---

## File Structure for Epic-Based Projects

```
my-project/
├── epic.yaml              # Epic + feature + task definitions
├── config.yaml            # Project config (commands, rules, boundaries)
├── orchestrator.db        # Tier 2: structured memory (tasks, sprints, retros)
├── vectors.db             # Tier 3: embeddings + decision archive
├── workspace/             # Agent workspace (source code)
├── .orchestrator/
│   ├── progress/
│   │   ├── feat-auth.json          # Feature progress
│   │   └── feat-patient-reg.json
│   ├── sprints/
│   │   ├── feat-auth-retro.yaml    # Retrospective records
│   │   └── feat-patient-reg-retro.yaml
│   └── deferred.json               # Deferred tasks
└── logs/
    ├── team-a/
    └── team-b/
```

---

## Related Documents

- [Scrum Teams](SCRUM-TEAMS.md) — Team formation, ceremonies, Scrum Master Agent protocol
- [Workflow Pipeline](README.md) — The 6-phase task execution pipeline (runs within each sprint batch)
- [Blocker Handling](BLOCKER-HANDLING.md) — Escalation protocol
- [Context Budgeting](../context-budgeting/README.md) — How budget is allocated across teams
- [Memory Tiers](../memory/TIERS.md) — Where sprint plans, retros, and heuristics are stored
