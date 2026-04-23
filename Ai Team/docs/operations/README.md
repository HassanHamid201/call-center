# Operational Guide

Configuration, project setup, and day-to-day operations for the orchestration system.

---

## Project Setup

### Initialize a New Project

```bash
# In your project directory
orchestrator init
```

This auto-detects:
- **Language** and **framework** from project files
- **Test/lint/build commands** from package.json scripts or equivalent
- **Technology specialists** to activate

Creates:

```
my-project/
├── config.yaml              # Auto-generated project configuration
├── orchestrator.db          # SQLite: structured memory (auto-created on first run)
├── vectors.db               # SQLite + sqlite-vec: embeddings (auto-created)
├── PRD.md                   # Create manually, or use tasks.yaml / tasks.json
├── .orchestrator/            # Runtime state (auto-created)
│   ├── progress.txt
│   └── deferred.json
├── skills/                  # Optional: custom skill manifests
└── logs/                    # Execution logs
```

### Project Directory Structure

```
my-project/
├── config.yaml              # Project configuration
├── orchestrator.db          # SQLite: tasks, decisions, blockers, memory index
├── vectors.db               # SQLite + sqlite-vec: embeddings (Deep Archive)
├── PRD.md                   # Task list (Markdown format)
│                            #   OR tasks.yaml (YAML format)
│                            #   OR tasks.json (JSON format)
│                            #   OR GitHub Issues
├── workspace/               # Agent workspace (source code)
├── .orchestrator/            # Runtime state
│   ├── progress.txt         # Task completion log
│   └── deferred.json        # Deferred task state (survives restarts)
├── skills/                  # Custom skill manifests
└── logs/                    # Execution logs
```

---

## Configuration

### config.yaml

```yaml
# Project metadata (auto-detected on init)
project:
  name: "my-app"
  language: "TypeScript"
  framework: "Next.js"

# Verification commands (auto-detected from package.json / pyproject.toml / etc.)
commands:
  test: "npm test"
  lint: "npm run lint"
  build: "npm run build"

# Rules injected into every agent prompt
rules:
  - "Use server actions, not API routes"
  - "Follow error pattern in src/utils/errors.ts"
  - "All components must use TypeScript strict mode"

# Files agents must never modify
boundaries:
  never_touch:
    - "src/legacy/**"
    - "*.lock"
    - ".env.production"

# Execution settings
execution:
  max_retries: 3
  retry_delay: 5              # Base seconds for exponential backoff
  max_parallel: 3             # Max concurrent agents in parallel mode
  default_isolation: "sandbox" # sandbox | worktree
  skip_tests: false
  skip_lint: false

# Webhook notifications
notifications:
  discord_webhook: ""
  slack_webhook: ""
  custom_webhook: ""
```

### Auto-Detection Reference

| Signal | Detects |
|---|---|
| `package.json` present | Node.js project |
| `tsconfig.json` present | TypeScript |
| `react` in dependencies | React specialist |
| `vue` in dependencies | Vue specialist |
| `next` in dependencies | Next.js (React specialist) |
| `express` / `@nestjs/core` in dependencies | Node.js specialist |
| `.csproj` / `.sln` present | .NET project |
| `pyproject.toml` / `requirements.txt` | Python project |
| `go.mod` present | Go project |
| `Cargo.toml` present | Rust project |
| `scripts.test` in package.json | Test command |
| `scripts.lint` in package.json | Lint command |
| `scripts.build` in package.json | Build command |

---

## Task Formats

### Markdown (Simplest)

```markdown
## Feature: User Authentication

- [ ] Create User model with email and password fields
- [ ] Add password hashing utility function
- [ ] Create signup API endpoint
- [ ] Create login API endpoint
- [x] Set up project structure
```

Best for: Simple projects, quick prototypes, linear task lists.

### YAML (Full Features)

```yaml
tasks:
  - title: "Create patient registration components"
    specialist: "react"
    parallel_group: 1
    acceptance_criteria:
      - "TypeScript strict mode passes"
      - "Unit tests cover > 80% of new components"
      - "WCAG 2.1 AA accessibility compliance"
    description: |
      Build the patient registration form with:
      - Saudi ID validation
      - Arabic/English support
      - Responsive mobile-first design

  - title: "Create patient API endpoint"
    specialist: "dotnet"
    parallel_group: 1
    acceptance_criteria:
      - "API responds within 200ms at p95"
      - "Integration tests pass"

  - title: "Integration testing"
    parallel_group: 2
    specialist: "automation-qa"
    dependencies: ["Create patient registration components", "Create patient API endpoint"]
```

Best for: Complex projects, parallel execution, explicit quality requirements.

### JSON (CI/CD Integration)

```json
{
  "tasks": [
    {
      "title": "Create User model",
      "completed": false,
      "parallel_group": 1,
      "specialist": "dotnet",
      "acceptance_criteria": ["Migrations apply cleanly"]
    }
  ]
}
```

Best for: Generated task lists, CI/CD pipelines, automated workflows.

### GitHub Issues

```
--github owner/repo --github-label "ready"
```

Best for: Teams already using GitHub Issues for task tracking.

---

## Common Operations

### Running Tasks

```bash
# Sequential (default)
orchestrator run

# With specific task file
orchestrator run --prd tasks.yaml

# Parallel mode
orchestrator run --parallel

# Limit iterations
orchestrator run --max-iterations 5

# Dry run (preview only)
orchestrator run --dry-run

# Skip tests and lint
orchestrator run --fast
```

### Resuming After Interruption

The system persists state to disk:
- Completed tasks are marked in the source file
- Deferred tasks are tracked in `.orchestrator/deferred.json`
- Memory is persisted in `orchestrator.db` and `vectors.db`

Resume by running the same command again — it picks up where it left off.

### Adding Custom Rules

```bash
orchestrator --add-rule "All API responses must use ProblemDetails format"
```

This appends to the `rules` list in `config.yaml`.

### Backing Up

```bash
cp -r my-project/ my-project-backup/
```

Everything is contained in the project directory.

---

## Verification

### How Acceptance Criteria Are Checked

| Criterion | How It's Verified |
|---|---|
| `"tests pass"` | Run `commands.test`, check exit code 0 |
| `"lint passes"` | Run `commands.lint`, check exit code 0 |
| `"build succeeds"` | Run `commands.build`, check exit code 0 |
| `"TypeScript strict passes"` | Run `npx tsc --noEmit` |
| `"coverage > X%"` | Run tests with coverage, parse output for percentage |
| `"accessible"` / `"accessibility"` | Specialist review agent evaluates |
| `"secure"` / `"security"` | App security specialist reviews |
| Any unparseable criterion | Specialist review agent evaluates against the output |

### Default Verification Pipeline

Every task goes through this pipeline after the agent completes:

```
1. Acceptance criteria check (per-criterion)
2. Run commands.test    → must exit 0
3. Run commands.lint    → must exit 0
4. Run commands.build   → must exit 0
5. Specialist review    → if specialist assigned
```

Steps can be skipped with `skip_tests: true`, `skip_lint: true`, or `--fast`.

---

## Isolation Modes

### Sandbox (Default)

- Symlinks: `node_modules/`, `.git/`, `vendor/`, `.venv/` (read-only, shared)
- Copies: `src/`, `app/`, `lib/`, config files (writable, isolated)
- Fast: no gigabyte duplication
- Sync: modified files detected by timestamp, synced back on success

### Worktree

- Full git worktree with own branch
- Agents can run git commands, view history
- After verification: branch merged back, conflicts AI-resolved
- Use when: agent needs git operations, small repo

---

## Monitoring

### What to Watch

| Metric | Where | Healthy Range |
|---|---|---|
| Task completion rate | Dashboard → Pipeline | > 85% |
| First-pass success rate | Dashboard → Pipeline | > 60% |
| Budget violation rate | Dashboard → Budget | < 1% |
| Paging hit rate | Dashboard → Paging | > 60% |
| Verification pass rate | Dashboard → Verification | > 70% |
| Human escalation rate | Dashboard → Pipeline | < 10% |

---

## Next Steps

- [Budget Tuning](TUNING.md) — Adjusting allocation parameters
- [Safety Mechanisms](SAFETY.md) — Protections against runaway execution
- [Success Metrics](../metrics/README.md) — Target values for all metrics
- [Workflow](../workflow/README.md) — The complete task lifecycle
