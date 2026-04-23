# Execution Workflow

The complete task lifecycle: from epic loading through Scrum team execution and verification to completion or retry.

---

## Overview

Work is organized in an **Epic → Feature → Sprint → Task** hierarchy. The Orchestrator (Product Owner) receives an epic, spawns one **Scrum Team** per feature, and runs all teams in parallel. Within each team, a **Scrum Master Agent** plans sprints, sequences task batches, coordinates handoffs, and runs retrospectives. Each task then follows the 6-phase pipeline below.

See [Epic & Sprint Hierarchy](EPIC-SPRINTS.md) and [Scrum Teams](SCRUM-TEAMS.md) for the team-level model.

---

## Work Hierarchy

```
Epic
 └── Feature A ──► Scrum Team A (parallel)
 │     └── Sprint
 │           ├── Batch 1 (parallel tasks)
 │           ├── Standup
 │           ├── Batch 2 (parallel tasks, after Batch 1)
 │           ├── Standup
 │           └── Retrospective on completion
 │
 └── Feature B ──► Scrum Team B (parallel with A)
       └── Sprint
             └── ...
```

Each task inside a batch runs the 6-phase pipeline independently:

---

## The Pipeline (per task)

```
┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐
│  1. LOAD  │─►│ 2. PLAN   │─►│ 3. EXECUTE│─►│ 4. VERIFY │─►│ 5. REVIEW │─►│ 6. RESOLVE│
│  Tasks    │  │ & PREPARE │  │           │  │           │  │           │  │           │
└───────────┘  └───────────┘  └───────────┘  └───────────┘  └───────────┘  └───────────┘
```

---

## Phase 1: Load Tasks

**Purpose:** Ingest tasks from whatever format the team uses.

### Supported Formats

#### Markdown (PRD)

```markdown
## Feature: Authentication

- [ ] Create User model with email and password fields
- [ ] Add password hashing utility function
- [ ] Create signup API endpoint
- [ ] Create login API endpoint
- [ ] Add JWT token generation
- [ ] Create logout endpoint
```

Tasks are `- [ ]` items. Completed tasks show as `- [x]`. Execution order is top-to-bottom.

#### Markdown Folder

```
prd/
  backend.md      # - [ ] create user API
  frontend.md     # - [ ] add login page
  infra.md        # - [ ] setup CI/CD
```

Tasks are tracked per-file so completion updates the correct file.

#### YAML (Structured)

```yaml
tasks:
  - title: "Create patient registration React components"
    specialist: "react"
    parallel_group: 1
    acceptance_criteria:
      - "TypeScript strict mode passes"
      - "Unit tests cover > 80% of new components"
      - "WCAG 2.1 AA accessibility compliance"
    description: |
      Build the patient registration form with Saudi ID
      validation, Arabic/English support, responsive design.

  - title: "Create patient registration API endpoint"
    specialist: "dotnet"
    parallel_group: 1
    dependencies: []  # no task dependencies for this group
    acceptance_criteria:
      - "API responds within 200ms at p95"
      - "Input validation rejects invalid Saudi IDs"
      - "Integration tests pass"
```

#### JSON (CI/CD Friendly)

```json
{
  "tasks": [
    {
      "title": "Create User model",
      "completed": false,
      "parallel_group": 1,
      "specialist": "dotnet",
      "acceptance_criteria": ["All EF Core migrations apply cleanly"]
    }
  ]
}
```

#### GitHub Issues

```
--github owner/repo --github-label "ready"
```

Tasks are open issues with the specified label. Completion closes the issue.

### Task Interface

All formats normalize to this structure:

| Field | Type | Description |
|---|---|---|
| `id` | string | Unique identifier (line number, index, or issue number) |
| `title` | string | Task description |
| `body` | string? | Full description (from GitHub body or YAML `description`) |
| `parallel_group` | number? | Tasks with the same value run concurrently (0 = sequential) |
| `completed` | boolean | Whether the task is done |
| `specialist` | string? | Which specialist skill to activate (auto-detected if omitted) |
| `acceptance_criteria` | string[]? | Conditions that must all pass for the task to be considered complete |
| `dependencies` | string[]? | Task IDs that must complete before this task starts |

---

## Phase 2: Plan & Prepare

**Purpose:** Analyze each task and prepare the agent's execution context.

### Steps

1. **Score complexity** (1–10) based on:
   - Number of acceptance criteria
   - Ambiguity in the description
   - Number of files or components likely affected
   - Whether similar tasks exist in project history

2. **Assign specialist** (if not explicit in the task):
   - Check task's `specialist` field
   - If absent, auto-detect from project stack and task content
   - If no match, use a generic implementation skill

3. **Allocate budget** — Context Budgeter distributes tokens based on complexity score

4. **Hydrate context** — Memory Manager loads:
   - Relevant past decisions from Structured Storage
   - Similar past tasks from Deep Archive (via paging)
   - Active blockers that might affect this task

5. **Load verification commands** from project config:
   ```yaml
   commands:
     test: "npm test"
     lint: "npm run lint"
     build: "npm run build"
   ```

### Outputs

| Output | Destination |
|---|---|
| Specialist skill | Prompt builder |
| Budget allocation | Execution Engine |
| Hydrated memory context | Prompt builder |
| Verification commands | Verification Engine |

---

## Phase 3: Execute

**Purpose:** The specialist agent performs the work, following enforced steps.

### Prompt Construction

The prompt is composed from four layers:

```
┌──────────────────────────────────────────────────────┐
│  LAYER 1: PROJECT CONTEXT                            │
│  • Project name, language, framework                  │
│  • Rules from project config                         │
│  • Boundaries (files that must not be modified)       │
├──────────────────────────────────────────────────────┤
│  LAYER 2: SPECIALIST EXPERTISE                       │
│  • Technology-specific best practices                 │
│  • Pattern library for the assigned specialist        │
│  • Security and performance considerations            │
├──────────────────────────────────────────────────────┤
│  LAYER 3: TASK + ACCEPTANCE CRITERIA                  │
│  • Task description with full details                 │
│  • Acceptance criteria (all must pass)                │
│  • Dependencies and integration points                │
├──────────────────────────────────────────────────────┤
│  LAYER 4: MEMORY CONTEXT                             │
│  • Paged-in relevant decisions and patterns           │
│  • Heuristics from similar past tasks                 │
├──────────────────────────────────────────────────────┤
│  LAYER 5: EXECUTION INSTRUCTIONS                     │
│  • Step-by-step: implement → test → lint → commit     │
│  • "Focus ONLY on this task" directive                │
│  • Browser commands (if applicable)                   │
└──────────────────────────────────────────────────────┘
```

#### Enforced Execution Steps

The prompt always includes these instructions in order:

```
1. Implement the task described above
2. Write tests for the feature
3. Run tests and ensure they pass before proceeding
4. Run linting and ensure it passes
5. Ensure the code works correctly
6. Commit your changes with a descriptive message
```

If `skipTests` or `skipLint` is configured, those steps are omitted.

#### Focus Enforcement

To prevent agent drift (implementing tangentially related features, refactoring unrelated code):

- The prompt opens with the task and closes with `"Focus only on implementing: {task title}"`
- Boundaries explicitly list files the agent must not modify:
  ```
  Do NOT modify:
  - PRD.md (the task file)
  - .orchestrator/progress.txt
  - .sandboxes/
  - [any project-specific never-touch paths from config]
  ```
- Rules include: `"Keep changes focused and minimal. Do not refactor unrelated code."`

#### Paging During Execution

When the agent encounters insufficient context, it autonomously retrieves more:

```
Agent: "I need to understand how authentication is configured"
  → page_search("authentication configuration")
  → page_retrieve(best_match_id)
  → Context loaded, agent continues
```

### Parallel Execution

Tasks in the same `parallel_group` run concurrently:

```
Group 1 (3 tasks, max_parallel: 3):
  Agent 1 → sandbox → "Create User model"
  Agent 2 → sandbox → "Create Post model"
  Agent 3 → sandbox → "Create Auth module"

Each sandbox:
  - Symlinks: node_modules/, .git/, vendor/
  - Copies: src/, app/, config files

After all agents complete:
  - Verification runs per-task
  - Passed tasks: changes synced back, branches merged
  - Failed tasks: retried or deferred
  - Merge conflicts: AI-assisted resolution
```

### Retry Behavior

| Error Type | Response | Backoff |
|---|---|---|
| Rate limit / quota exceeded | Defer task, retry later | Exponential (5s → 10s → 20s) with jitter |
| Network timeout | Retry immediately | Exponential with jitter |
| Agent fails (non-zero exit) | Log error, mark failed | No automatic retry |
| Verification failure | Retry with feedback | Up to 3 attempts, delta-prompt each time |

Deferred tasks are persisted to `.orchestrator/deferred.json` so they survive restarts.

---

## Phase 4: Verify (Automated)

**Purpose:** Run automated checks independently — don't trust the agent's self-assessment.

### Acceptance Criteria Checking

Each criterion is mapped to a verification method:

| Criterion Pattern | Verification Method |
|---|---|
| `"tests pass"` | Run `commands.test`, check exit code 0 |
| `"TypeScript strict"` / `"type check"` | Run `npx tsc --noEmit` or equivalent |
| `"lint passes"` | Run `commands.lint`, check exit code 0 |
| `"build succeeds"` | Run `commands.build`, check exit code 0 |
| `"coverage > X%"` | Run test command with coverage flag, parse output |
| `"no security vulnerabilities"` | Run security audit tool, check output |
| Any unparseable criterion | Escalate to Phase 5 (specialist review) |

### Command Execution

```yaml
# From project config
commands:
  test: "npm test"
  lint: "npm run lint"
  build: "npm run build"
```

The Verification Engine runs each command and checks:

| Check | Pass Condition |
|---|---|
| Test command | Exit code 0 |
| Lint command | Exit code 0 |
| Build command | Exit code 0 |
| Test output parsing | Count pass/fail/skip, extract coverage % |
| Lint output parsing | Count errors and warnings |

### Verification Result

```typescript
interface VerificationResult {
  taskId: string;
  passed: boolean;
  checks: CheckResult[];
  testSummary: { total: number; passed: number; failed: number; coverage?: number };
  lintErrors: number;
  buildSuccess: boolean;
}

interface CheckResult {
  criterion: string;
  method: "automated" | "review";
  passed: boolean;
  evidence: string;  // e.g., "exit code 0", "coverage 85.3%"
}
```

If all automated checks pass → proceed to Phase 5 (specialist review).
If any automated check fails → proceed directly to Phase 6 (resolve as FAIL).

---

## Phase 5: Review (Specialist)

**Purpose:** A review agent with the relevant specialist expertise evaluates quality.

### When Review Runs

- **Always** if the task has a specialist assigned
- **Always** if any acceptance criteria couldn't be automatically verified
- **Optional** if all criteria passed automation and no specialist was assigned

### Review Dimensions

| Dimension | What's Checked |
|---|---|
| Correctness | Does the implementation do what the task requested? |
| Completeness | Are all requirements and acceptance criteria satisfied? |
| Pattern compliance | Does it follow the specialist's established patterns? |
| Security | Any vulnerability concerns for this technology? |
| Performance | Any obvious performance issues? |
| Test quality | Are tests meaningful, or just superficially passing? |

### Review Output

```typescript
interface ReviewReport {
  verdict: "pass" | "fail" | "blocked";
  dimensions: {
    correctness: DimensionResult;
    completeness: DimensionResult;
    patterns: DimensionResult;
    security: DimensionResult;
    performance: DimensionResult;
    testQuality: DimensionResult;
  };
  feedback: string[];     // Specific, actionable issues
  suggestions: string[];  // Optional improvements
}

interface DimensionResult {
  passed: boolean;
  notes: string;
}
```

---

## Phase 6: Resolve

**Purpose:** Act on the verification and review results.

### PASS Path

```
Verification: PASS
Review: PASS
  │
  ├── Mark task complete in source file
  │   (update checkbox in MD, set completed: true in YAML/JSON, close GitHub issue)
  │
  ├── Store results in memory:
  │   • Task record → Structured Storage
  │   • Execution log → Deep Archive
  │   • Extracted heuristics → Deep Archive
  │   • Verification results → Structured Storage
  │
  ├── Sync changes back:
  │   • Sandbox: sync modified files to original directory
  │   • Worktree: merge branch to base (or create PR)
  │
  ├── Record telemetry (tokens used, duration, test counts)
  │
  └── Release budget, move to next task
```

### FAIL Path (Iterative Refinement)

```
Verification or Review: FAIL
  │
  ├── Generate delta-prompt:
  │   • Original task requirements
  │   + Specific verification failures (what failed, expected vs. actual)
  │   + Specific review feedback (which dimensions failed, why)
  │   - Everything that passed (to save tokens)
  │
  ├── Preserve paging insights (what was retrieved, what was useful)
  │
  ├── Store partial results in memory for reference
  │
  └── Return to Phase 3 with:
      • Delta-prompt (smaller than original — only what needs fixing)
      • Reduced budget (~60% of original)
      • Preserved insights from previous attempt
```

Each retry is cheaper than the previous one because the delta shrinks. Maximum retries configurable (default: 3).

### BLOCKED Path

```
Agent reports blocker
  │
  ├── Create blocker record in Structured Storage:
  │   • Full description
  │   • Paging history (what was searched, what was found)
  │   • What was attempted
  │
  ├── Escalation protocol:
  │   Level 1: Agent pages memory for similar blockers
  │   Level 2: Extended paging quota for deeper search
  │   Level 3: Orchestrator decomposes task or changes strategy
  │   Level 4: Human escalation with full context package
  │
  └── See [Blocker Handling](BLOCKER-HANDLING.md) for the full protocol
```

---

## Task State Machine

```
                    ┌──────────┐
                    │  QUEUED   │
                    └────┬─────┘
                         │ (scheduler assigns)
                         ▼
                    ┌──────────┐
                    │ RUNNING   │
                    └────┬─────┘
                         │
               ┌─────────┼──────────┐
               │         │          │
          (complete) (blocked)  (transient error)
               │         │          │
               ▼         ▼          ▼
         ┌──────────┐ ┌──────────┐ ┌──────────┐
         │VERIFYING │ │ BLOCKED  │ │ DEFERRED │
         └────┬─────┘ └────┬─────┘ └────┬─────┘
              │              │            │
      ┌───────┴───────┐  (resolved)  (retry after
      │               │       │       backoff)
 (pass verify)  (fail verify)  │       │
      │               │       ▼       ▼
      ▼               ▼     QUEUED   QUEUED
┌──────────┐    ┌──────────┐
│ REVIEWING│    │ RETRYING │──► RUNNING (with delta-prompt)
└────┬─────┘    └──────────┘
     │
  ┌──┴──────────┐
  │             │
(pass review) (fail review)
  │             │
  ▼             ▼
┌──────────┐  RETRYING
│COMPLETE  │
└──────────┘
```

---

## Configuration

```yaml
# config.yaml — project-level configuration
project:
  name: "my-app"
  language: "TypeScript"
  framework: "Next.js"

commands:
  test: "npm test"
  lint: "npm run lint"
  build: "npm run build"

rules:
  - "Use server actions, not API routes"
  - "Follow error pattern in src/utils/errors.ts"
  - "All components must be TypeScript strict mode"

boundaries:
  never_touch:
    - "src/legacy/**"
    - "*.lock"
    - ".env.production"

execution:
  max_retries: 3
  retry_delay: 5          # seconds (base for exponential backoff)
  max_parallel: 3         # concurrent agents in parallel mode
  default_isolation: "sandbox"  # sandbox | worktree
  skip_tests: false
  skip_lint: false

notifications:
  discord_webhook: ""
  slack_webhook: ""
  custom_webhook: ""
```

### Auto-Detection

Running `orchestrator init` (or equivalent) auto-detects:

| Signal | Detection |
|---|---|
| `package.json` present | Node.js project → extract language, framework from dependencies |
| `tsconfig.json` present | TypeScript |
| `pyproject.toml` / `requirements.txt` | Python project |
| `go.mod` present | Go project |
| `Cargo.toml` present | Rust project |
| `.csproj` / `.sln` present | .NET project |
| `scripts.test` in package.json | Test command |
| `scripts.lint` in package.json | Lint command |
| `scripts.build` in package.json | Build command |

---

## Project Directory Structure

```
my-project/
├── config.yaml              # Project configuration
├── orchestrator.db          # SQLite: structured memory (Tier 2)
├── vectors.db               # SQLite + sqlite-vec: embeddings (Tier 3)
├── PRD.md                   # Task list (or tasks.yaml / tasks.json)
├── workspace/               # Agent workspace (source code)
├── .orchestrator/            # Runtime state
│   ├── progress.txt         # Task completion log
│   ├── deferred.json        # Deferred task tracking
│   └── config.yaml          # Symlink or copy of project config
├── skills/                  # Custom skill manifests (optional)
└── logs/                    # Execution logs
```

---

## Next Steps

- [Scrum Teams](SCRUM-TEAMS.md) — Team formation, Scrum Master Agent, ceremonies, handoff protocol
- [Epic & Sprint Hierarchy](EPIC-SPRINTS.md) — Epic → Feature → Sprint → Task model, multi-team orchestration
- [Blocker Handling](BLOCKER-HANDLING.md) — Detailed blocker resolution protocol
- [Data Flow](../architecture/DATA_FLOW.md) — How data moves between pipeline phases
- [Context Budgeting](../context-budgeting/README.md) — How budgets are allocated across phases
- [Verification Engine](../verification/README.md) — Detailed verification and review specification
- [Skill Registry](../skills/README.md) — The specialist skill catalog
