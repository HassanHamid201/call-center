# Scrum Teams

How specialist agents form self-organized feature teams, run ceremonies, and collaborate within a feature-complete sprint.

---

## Overview

Each feature is delivered by a dedicated **Scrum Team** — a small, cross-functional group of specialist agents coordinated by a **Scrum Master Agent**. Multiple teams run in parallel across different features, all reporting back to the Orchestrator (which plays the Product Owner role).

```
ORCHESTRATOR (Product Owner)
       │
       ├── Scrum Team A ── Feature: Authentication
       ├── Scrum Team B ── Feature: Patient Registration   (all parallel)
       └── Scrum Team C ── Feature: Reporting
```

Teams are ephemeral: they form when a feature begins and dissolve when the feature sprint completes.

---

## Team Composition

### Formation Rules

The Orchestrator forms a team for each feature by selecting specialists based on the feature's task set:

| Rule | Behavior |
|---|---|
| **Auto-composition** | Orchestrator inspects task `specialist` fields and activates required specialists |
| **Always included** | Scrum Master Agent (1 per team), `app-security` (review), `automation-qa` (review) |
| **Explicit override** | `team` field in feature definition can list exact specialists |
| **Max team size** | Default 6 agents (1 Scrum Master + 5 specialists); configurable via `max_team_size` |

### Typical Team Structure

```yaml
# Auto-composed team for a fullstack feature
team:
  scrum_master: scrum-master
  specialists:
    - dotnet-specialist      # backend
    - react-specialist       # frontend (parallel with backend)
    - postgresql-specialist  # database (parallel with backend)
  reviewers:
    - app-security           # always active
    - automation-qa          # always active
    - compliance-specialist  # activated if healthcare/finance project
```

### Specialist Roles Within a Team

| Role | Responsibility |
|---|---|
| **Scrum Master Agent** | Sprint planning, standups, handoff coordination, blocker escalation, retrospective |
| **Implementation Specialists** | Execute assigned tasks, follow best practices, signal readiness for handoff |
| **Review Specialists** | Evaluate completed work: security, QA, compliance — run during verification phases |

---

## Sprint Model: Feature-Complete

A sprint ends when **all tasks for the assigned feature are verified done** — not on a time boundary. This means:

- Sprint length is determined by feature complexity, not a calendar
- No partial deliveries: the feature is either complete or in-progress
- Multiple features (teams) can complete at different times; the Orchestrator tracks each independently

### Sprint Lifecycle

```
SPRINT START
     │
     ▼
1. Sprint Planning
   Scrum Master decomposes feature → tasks → parallel groups → dependencies
     │
     ▼
2. Task Execution (parallel batches)
   Specialists execute their tasks in parallel within each group
   Standup runs after each batch
     │
     ▼
3. Verification
   Verification Engine + Review Specialists validate each completed task
     │
     ▼
4. Handoff (if dependencies exist)
   Scrum Master signals downstream specialists to begin
     │
     ▼
5. Retrospective (on sprint completion)
   Scrum Master collects lessons, stores in memory Tier 2
     │
     ▼
SPRINT COMPLETE — Orchestrator notified, next feature assigned if available
```

---

## Scrum Master Agent

The Scrum Master Agent is a coordination specialist, not an implementation agent. It does not write code.

### Responsibilities

#### 1. Sprint Planning

On receiving a feature from the Orchestrator, the Scrum Master:

1. Reads the feature description and all task definitions
2. Identifies dependencies between tasks (what must complete before what)
3. Groups independent tasks into `parallel_group` batches
4. Assigns or confirms specialist assignments
5. Estimates complexity (1–10) for each task
6. Produces the **Sprint Plan** (a structured task execution schedule)

```yaml
# Sprint Plan output (stored in Tier 2 memory)
sprint:
  feature: "Patient Registration"
  tasks:
    batch_1:  # parallel
      - { task: "Create DB schema", specialist: "postgresql", estimated_complexity: 4 }
    batch_2:  # parallel — starts after batch_1 completes
      - { task: "Create API endpoints", specialist: "dotnet", estimated_complexity: 6 }
      - { task: "Create React form", specialist: "react", estimated_complexity: 5 }
    batch_3:  # sequential — verification-only batch
      - { task: "Security review", specialist: "app-security", type: "review" }
```

#### 2. Standup (After Each Batch)

After each parallel batch completes, the Scrum Master runs a standup:

```
Standup format per specialist:
  ✅ DONE    — task completed, verification passed
  🔄 IN PROGRESS — still executing (should not happen across batch boundary)
  ❌ BLOCKED — describe blocker, what was tried
  ⚠️  NEEDS HANDOFF — work ready for dependent specialist
```

The Scrum Master records standup results in Tier 2 memory and triggers the next batch.

#### 3. Handoff Coordination

When a task's output is required by another specialist:

1. Scrum Master verifies the upstream task passed verification
2. Generates a **Handoff Package** — a compact summary of what was produced, relevant interfaces/contracts, and file paths
3. Injects the Handoff Package into the downstream specialist's context (via Tier 1 memory)
4. Signals the downstream specialist to begin

**Example Handoff:**
```
FROM: PostgreSQL Specialist (batch_1 complete)
TO:   .NET Core Specialist (starting batch_2)

Handoff Package:
  - Schema created: patients table with columns [id, national_id, name_ar, name_en, ...]
  - Migrations: 0001_create_patients.sql applied successfully
  - Connection string: available in appsettings.Development.json
  - Key constraints: national_id is unique, not null; date_of_birth validates Hijri/Gregorian
```

#### 4. Blocker Escalation

```
Level 1 (Agent-level): Specialist pages memory for similar blockers — autonomous
Level 2 (Team-level):  Scrum Master consults other team specialists or adjusts task scope
Level 3 (Orchestrator): Scrum Master escalates to Orchestrator with full context package
Level 4 (Human):       Orchestrator escalates if no automated resolution found
```

The Scrum Master attempts Level 2 resolution before escalating to Level 3. It documents all resolution attempts in the blocker record.

#### 5. Retrospective

On sprint completion, the Scrum Master:

1. Collects metrics: tasks completed, blockers hit, retry counts, verification pass rates
2. Identifies patterns: what slowed the team, what patterns worked well
3. Writes a **Retrospective Record** to memory Tier 2 with actionable improvements
4. Tags high-value heuristics for promotion to Tier 3 (Deep Archive)

```yaml
# Retrospective Record (Tier 2)
sprint_retro:
  feature: "Patient Registration"
  completed_at: "2026-04-23T14:30:00Z"
  metrics:
    tasks_total: 7
    tasks_passed_first_try: 5
    tasks_retried: 2
    blockers: 1
    total_tokens_used: 48200
  lessons:
    - "React specialist needs DB schema + API contract before starting form — handoff order matters"
    - "Arabic text validation requires custom regex; standard validators fail silently"
  improvements:
    - "Add Saudi national ID validation utility to shared library"
    - "Pre-generate API typings from OpenAPI spec before frontend batch starts"
```

---

## Intra-Team Communication Protocol

Agents within a team do not communicate directly — all coordination flows through the Scrum Master or memory.

### Channels

| Channel | Used For |
|---|---|
| **Handoff Package** (Tier 1 memory injection) | Passing artifacts between dependent specialists |
| **Sprint Plan** (Tier 2 memory) | Shared task schedule all agents can read |
| **Standup Record** (Tier 2 memory) | Status tracking after each batch |
| **Blocker Record** (Tier 2 memory) | Documented obstacles and resolution attempts |
| **Retrospective Record** (Tier 2 memory) | Post-sprint lessons |

### Signaling Conventions

Specialists signal the Scrum Master using structured status outputs at task end:

```yaml
task_completion_signal:
  task_id: "create-api-endpoints"
  status: "READY_FOR_HANDOFF"         # DONE | READY_FOR_HANDOFF | BLOCKED
  artifacts:
    - type: "api_contract"
      path: "docs/api/patients.yaml"
  notes: "POST /api/patients returns 201 with patient ID; validation errors return 422"
  handoff_to: ["react-specialist"]
```

---

## Parallel Execution Within a Team

Specialists in the same `parallel_group` work simultaneously in isolated sandboxes:

```
Batch 2 (parallel):
  Sandbox A: .NET Core Specialist — building API endpoints
  Sandbox B: React Specialist     — building form components

Both running at the same time. Merge strategy:
  - If they touch different files → auto-merge after both complete
  - If they touch the same file  → Scrum Master coordinates conflict resolution
```

### Merge Conflict Resolution

1. Verification Engine detects overlapping changes
2. Scrum Master reviews both changesets
3. If resolvable: generates merge with both sets of changes preserved
4. If not resolvable: one specialist's work is the base, other is re-run with updated context

---

## Team Configuration

```yaml
# config.yaml — team settings
scrum:
  max_team_size: 6              # max specialists per team (excl. Scrum Master)
  max_parallel_features: 4      # max features running simultaneously
  standup_after_each_batch: true
  retrospective_enabled: true
  handoff_packages: true        # inject handoff summaries into downstream context
  escalation_wait_ms: 30000     # time to attempt team-level resolution before Orchestrator
```

---

## Related Documents

- [Epic & Sprint Hierarchy](EPIC-SPRINTS.md) — How epics, features, sprints, and tasks relate
- [Blocker Handling](BLOCKER-HANDLING.md) — Full blocker resolution protocol
- [Workflow Pipeline](README.md) — The 6-phase task execution pipeline
- [Scrum Master Agent Best Practices](../skills/scrum-master.md) — Scrum Master skill manifest
