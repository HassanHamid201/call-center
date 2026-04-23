# Portable Stack Guide

Implementation guidance for a self-contained, no-server deployment that stores all state under a single project directory.

---

## Design Philosophy

**One directory = one orchestrator project.** Everything the system needs — structured data, vector embeddings, workspace files, configuration — lives under a single directory. No Redis, Postgres, Qdrant, or external services required.

```
my-project/
├── config.yaml              # Project configuration
├── orchestrator.db          # SQLite: all structured state
├── vectors.db               # SQLite + sqlite-vec: embeddings
├── workspace/               # Agent workspace (files)
├── logs/                    # Execution logs
└── skills/                  # Custom skill manifests
```

Copy the directory to back up the project. Move it to another machine to transfer the project. Delete it to clean up.

---

## Component-to-Implementation Mapping

| Spec Component | Portable Implementation |
|---|---|
| Orchestrator Core | Python process reading/writing `orchestrator.db`, implementing event-sourced durable execution |
| Context Budgeter | In-process class using token counting plus budget state in SQLite |
| Memory Manager | SQLite tables for Tier 2; in-memory objects for Tier 1 |
| Tier 1 (Working Memory) | Python objects + optional in-memory SQLite for the active task |
| Tier 2 (Structured Storage) | SQLite tables: `tasks`, `decision_logs`, `blockers`, `memory_index`, etc. |
| Tier 3 (Deep Archive) | SQLite + sqlite-vec for embeddings and full historical logs |
| Paging Tools | Python functions querying SQLite + sqlite-vec, wrapped as agent tools |
| Skill Registry | `skills` table in SQLite plus YAML manifest files in `skills/` directory |
| Agent Workers | LangGraph nodes calling paging tools and LLM via LiteLLM |
| Web Dashboard | Local FastAPI server + React SPA, reading SQLite directly |

---

## Storage

### SQLite (Structured Data)

SQLite is the backbone of the portable stack. It provides:
- **Transactions** — Atomic operations for state consistency
- **SQL queries** — Relational access for Tier 2 data
- **JSON support** — Flexible metadata columns
- **WAL mode** — Concurrent reads during writes
- **Portability** — Single file, cross-platform

**Key tables:**

| Table | Purpose |
|---|---|
| `projects` | Project metadata and configuration |
| `tasks` | Task records with status, requirements, results |
| `task_dependencies` | Links between dependent tasks |
| `decision_logs` | Architectural and implementation decisions |
| `blockers` | Blocker records with resolution status |
| `memory_index` | Metadata index for Tier 2 and Tier 3 content |
| `paging_logs` | Record of all paging operations |
| `skills` | Registered skill manifests |
| `budget_snapshots` | Budget utilization history |
| `event_log` | Durable execution event store |

### SQLite + sqlite-vec (Vector Data)

The `sqlite-vec` extension adds vector similarity search to SQLite:

- **KNN queries** via virtual tables for `page_search`
- **No dependencies** — Pure C extension, runs anywhere SQLite runs
- **Single file** — Embeddings stored alongside structured data

**Key tables:**

| Table | Purpose |
|---|---|
| `embeddings` | Vector embeddings for all Tier 3 content |
| `embedding_metadata` | Content metadata associated with each embedding |

### In-Memory (Working Memory)

Tier 1 (Working Memory) is purely in-process:
- Python dictionaries and objects for the current task
- Optional in-memory SQLite (`:memory:`) for structured active context
- Serialized to disk only when a task suspends

---

## Durable Execution

The orchestrator implements an event-sourced pattern on SQLite:

### Event Log

Every orchestrator step is recorded as an event:

```sql
CREATE TABLE event_log (
  event_id    INTEGER PRIMARY KEY AUTOINCREMENT,
  task_id     TEXT NOT NULL,
  event_type  TEXT NOT NULL,  -- 'step_started', 'step_completed', etc.
  step_name   TEXT NOT NULL,  -- 'planning', 'execution', 'review', etc.
  payload     JSON,           -- Step-specific data
  timestamp   DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

### Recovery on Restart

When the orchestrator restarts:
1. Read the event log for each in-progress task
2. Identify the last completed step
3. Replay any necessary state from events
4. Resume from the last checkpoint

### Checkpoint Strategy

| Step | Checkpoint Event |
|---|---|
| Intent parsed | `step_completed: planning` |
| Budget allocated | `step_completed: budgeting` |
| Agent session started | `step_started: execution` |
| Agent session completed | `step_completed: execution` |
| Review started | `step_started: review` |
| Review completed | `step_completed: review` |
| Memory updated | `step_completed: memory_update` |

---

## Agent Framework

### Recommended: LangGraph

LangGraph supports the cyclic, stateful workflows this system requires:

- **State graph** — Define nodes (agents) and edges (transitions)
- **Cyclic edges** — Implement the Ralph Loop (implement → review → retry)
- **State management** — Global `AgentState` holds references to paging toolkit
- **Tool calling** — Agents invoke `page_search`, `page_retrieve`, etc. as tools

### LLM Integration: LiteLLM

LiteLLM provides a unified API across providers:
- OpenAI, Anthropic, local models (Ollama)
- Built-in token counting for budget tracking
- Streaming support for real-time dashboard updates

---

## SQLite Schema Outline

### Core Tables

```sql
-- Project metadata
CREATE TABLE projects (
  project_id   TEXT PRIMARY KEY,
  name         TEXT NOT NULL,
  config       JSON,            -- Full config.yaml as JSON
  detected     JSON,            -- Auto-detected stack info
  created_at   DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at   DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Task records
CREATE TABLE tasks (
  task_id          TEXT PRIMARY KEY,
  parent_task_id   TEXT REFERENCES tasks(task_id),
  project_id       TEXT REFERENCES projects(project_id),
  source_type      TEXT NOT NULL,        -- 'markdown', 'yaml', 'json', 'github'
  source_ref       TEXT,                 -- File path or GitHub repo
  status           TEXT DEFAULT 'queued', -- queued, running, verifying, reviewing, completed, failed, blocked, deferred
  title            TEXT NOT NULL,
  body             TEXT,
  specialist       TEXT,                 -- Specialist skill ID
  parallel_group   INTEGER DEFAULT 0,
  dependencies     JSON,                 -- Array of task IDs
  acceptance_criteria JSON,              -- Array of criterion strings
  complexity_score INTEGER,
  strategy         TEXT,
  budget_plan      JSON,
  results          JSON,
  verification     JSON,                 -- VerificationResult
  retry_count      INTEGER DEFAULT 0,
  created_at       DATETIME DEFAULT CURRENT_TIMESTAMP,
  started_at       DATETIME,
  completed_at     DATETIME
);

-- Deferred tasks (persists across restarts)
CREATE TABLE deferred_tasks (
  task_key     TEXT PRIMARY KEY,   -- source_type:source_ref:task_id
  task_id      TEXT NOT NULL,
  attempt_count INTEGER DEFAULT 1,
  last_attempt  DATETIME,
  title        TEXT NOT NULL
);

-- Decision log
CREATE TABLE decision_logs (
  decision_id      TEXT PRIMARY KEY,
  task_id          TEXT REFERENCES tasks(task_id),
  decision         TEXT NOT NULL,
  rationale        TEXT,
  alternatives     JSON,
  created_at       DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Blocker records
CREATE TABLE blockers (
  blocker_id        TEXT PRIMARY KEY,
  task_id           TEXT REFERENCES tasks(task_id),
  description       TEXT NOT NULL,
  category          TEXT,
  escalation_level  INTEGER DEFAULT 0,
  resolution_status TEXT DEFAULT 'open',
  resolution        TEXT,
  paging_history    JSON,
  tokens_spent      INTEGER DEFAULT 0,
  created_at        DATETIME DEFAULT CURRENT_TIMESTAMP,
  resolved_at       DATETIME
);

-- Memory index (metadata for Tier 2 and Tier 3)
CREATE TABLE memory_index (
  memory_id       TEXT PRIMARY KEY,
  tier            TEXT NOT NULL,          -- 'warm' or 'cold'
  content_type    TEXT NOT NULL,
  summary         TEXT,
  tags            JSON,                   -- string array
  relevance_score REAL,
  embedding_id    TEXT,                   -- reference to vectors.db
  relationships   JSON,                   -- [{target, type}]
  retention       TEXT DEFAULT 'standard',
  created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
  accessed_at     DATETIME,
  access_count    INTEGER DEFAULT 0
);

-- Paging operation log
CREATE TABLE paging_logs (
  log_id         INTEGER PRIMARY KEY AUTOINCREMENT,
  task_id        TEXT REFERENCES tasks(task_id),
  agent_type     TEXT,
  tool           TEXT,       -- 'page_search', 'page_retrieve', etc.
  input          JSON,
  result_summary TEXT,
  token_cost     INTEGER,
  effective      BOOLEAN,
  timestamp      DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Budget snapshots (for dashboard history)
CREATE TABLE budget_snapshots (
  snapshot_id    INTEGER PRIMARY KEY AUTOINCREMENT,
  task_id        TEXT REFERENCES tasks(task_id),
  component      TEXT,
  allocated      INTEGER,
  utilized       INTEGER,
  paging_quota   INTEGER,
  paging_used    INTEGER,
  status         TEXT,
  timestamp      DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

### Vector Tables (vectors.db)

```sql
-- Vector embeddings
CREATE VIRTUAL TABLE vec_embeddings USING vec0(
  memory_id TEXT PRIMARY KEY,
  embedding FLOAT[1536]  -- dimension depends on embedding model
);

-- Embedding metadata
CREATE TABLE embedding_metadata (
  memory_id   TEXT PRIMARY KEY,
  content     TEXT,
  content_type TEXT,
  tier        TEXT,
  created_at  DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

---

## Concurrency Considerations

SQLite handles concurrent reads well but has limited multi-writer concurrency:

### Recommendations

| Pattern | Implementation |
|---|---|
| Single orchestrator process | One process owns all writes |
| WAL mode | Enable for concurrent reads during writes |
| Connection pooling | Reuse connections within the orchestrator process |
| Write batching | Batch memory updates rather than per-operation writes |

SQLite WAL mode allows readers to proceed without blocking writers, which is critical for dashboard responsiveness during agent execution.

---

## Trade-offs

| Aspect | Portable Stack | Server-Based Stack |
|---|---|---|
| **Setup** | Zero infrastructure | Redis, Postgres, Qdrant, Temporal |
| **Portability** | Copy directory | Export/import databases |
| **Concurrency** | Single writer | Multiple writers |
| **Scale** | Moderate (single machine) | Large (distributed) |
| **Vector search performance** | Good for <1M vectors | Better for >1M vectors |
| **Implementation effort** | Higher (custom durable execution) | Lower (use Temporal, etc.) |
| **Operational complexity** | Very low | Higher (multiple services) |

For a development tool running on a single machine, the portable stack is the right choice. The trade-offs only matter at scales this system isn't designed for.

---

## Next Steps

- [Architecture Overview](../architecture/OVERVIEW.md) — How these components fit together
- [Memory Tiers](../memory/TIERS.md) — How SQLite maps to the three-tier model
- [Operations Guide](../operations/README.md) — Running and configuring the portable stack
