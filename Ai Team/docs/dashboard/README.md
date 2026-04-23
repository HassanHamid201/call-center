# Web Dashboard Specification

The monitoring and inspection UI for the multi-agent orchestration system.

---

## Overview

The dashboard provides real-time visibility into:
- **Context budget utilization** — How tokens are being consumed
- **Agent activity** — What agents are doing right now
- **Memory state** — What's in each memory tier
- **Paging operations** — What agents are retrieving and why
- **Task progress** — Where tasks are in the execution pipeline

The dashboard is a local web application that reads directly from the project's SQLite databases.

---

## Dashboard Sections

### 1. Context Budget Monitor

Real-time visualization of token allocation and utilization.

#### Budget Overview Panel

```
┌─────────────────────────────────────────────────────────┐
│ CONTEXT BUDGET                          Total: 128K     │
│                                                         │
│ System Reserve  ████████████ 10% (12.8K)     [unused]   │
│ Orchestrator    █████████████████ 15% (19.2K) 72% used  │
│ Active Task     ██████████████████████████████ 60% (76K) │
│                 ████████████████████░░░░░░░░░░  58% used │
│ Review          █████████████████ 15% (19.2K)  unused   │
│                                                         │
│ Emergency Reserve: 12.8K tokens available               │
│ Status: ● ACTIVE                                        │
└─────────────────────────────────────────────────────────┘
```

**Visual Elements:**
- **Progress bars** for each budget tier
- **Color coding:**
  - Green: 0–70% utilization
  - Yellow: 70–90% utilization
  - Red: 90%+ utilization
- **Status indicator**: Active, Exhausted, or Emergency
- **Emergency reserve** shown separately with availability status

#### Budget History Graph

A time-series chart showing budget utilization over the lifetime of the current task:

```
Tokens
 80K │         ╭──────╮
     │        ╱      ╰──╮        ╭─── Review phase
 60K │    ╭──╯          ╰──╮    ╱
     │   ╱                  ╰──╯
 40K │  ╱
     │ ╱ Execution phase
 20K │╱ Orchestrator planning
     ╰────────────────────────────────── Time
       Plan  Execute  Compliance  Review  Store
```

#### Compression Events Feed

A log of all compression events:

| Time | Component | Strategy | Tokens Freed | Trigger |
|---|---|---|---|---|
| 10:23:45 | active-task | summarize | +2,500 | Soft warning (82%) |
| 10:25:12 | active-task | tier-demote | +4,000 | Hard limit (100%) |

---

### 2. Agent Activity Stream

Live view of what each agent is doing.

#### Agent Status Cards

```
┌──────────────────────────────┐  ┌──────────────────────────────┐
│ SPECIALIST AGENT              │  │ REVIEW AGENT                 │
│ Status: ● Running            │  │ Status: ○ Queued             │
│ Task: "Add payment endpoint" │  │ Waiting for: Specialist      │
│ Tokens: 32.1K / 76K (42%)   │  │                              │
│ Paging quota: 8.4K remaining│  │                              │
│ Files modified: 3            │  │                              │
│ Tools invoked: 12            │  │                              │
│ Duration: 4m 23s             │  │                              │
└──────────────────────────────┘  └──────────────────────────────┘
```

**Per-agent information:**
- Current status (running, queued, completed, blocked)
- Active task description
- Token consumption with progress bar
- Remaining paging quota
- Files modified count
- Tool invocation count
- Session duration

#### Activity Log

A chronological stream of agent actions:

```
10:23:01  [specialist] Read file: src/auth/jwt.ts
10:23:03  [specialist] page_search("JWT validation patterns")
10:23:04  [specialist] page_retrieve(mem_456) — 450 tokens
10:23:15  [specialist] Wrote file: src/auth/jwt.ts
10:23:18  [specialist] Ran command: npm test
10:23:22  [specialist] Tests passed: 12/12
10:23:25  [specialist] page_store("JWT fix applied", warm)
10:23:26  [specialist] Session complete
```

---

### 3. Paging Activity Dashboard

Dedicated view for paging operations across all agents.

#### Paging Summary

```
┌──────────────────────────────────────────────────┐
│ PAGING ACTIVITY                     This Task    │
│                                                  │
│ Total operations: 14                             │
│   page_search:     8 (cost: 960 tokens)         │
│   page_retrieve:   4 (cost: 3,200 tokens)       │
│   page_recurse:    1 (cost: 0 tokens, metadata)  │
│   page_store:      1 (freed: 1,800 tokens)      │
│                                                  │
│ Net paging cost: +2,360 tokens                   │
│ Hit rate: 75% (3 useful / 4 retrievals)          │
│ Quota used: 13.1% (2,360 / 18,000)              │
└──────────────────────────────────────────────────┘
```

#### Search Query Log

| Time | Agent | Query | Results | Retrieved | Effective |
|---|---|---|---|---|---|
| 10:23:04 | impl | "JWT validation patterns" | 3 | mem_456 | ✓ |
| 10:23:08 | impl | "auth middleware test patterns" | 2 | mem_891 | ✗ |
| 10:23:15 | impl | "similar authentication errors" | 5 | — | — |

#### Cost Breakdown

A visualization of token spending:

```
Token Budget Allocation
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
████████████████████████░░░░░░░░  Execution (58%)
████░░░░░░░░░░░░░░░░░░░░░░░░░░░  Paging (4%)
████████░░░░░░░░░░░░░░░░░░░░░░░░  Available (38%)
```

---

### 4. Memory Inspector

Browsable view of the three memory tiers.

#### Tier Overview

```
┌──────────────────────────────────────────────────┐
│ MEMORY TIERS                                     │
│                                                  │
│ Tier 1 (Hot) — Working Memory                    │
│   Size: 18,200 tokens                            │
│   Contents: 12 items                             │
│   [View Contents]                                │
│                                                  │
│ Tier 2 (Warm) — Structured Storage               │
│   Size: 142,000 tokens (indexed)                 │
│   Records: 847 (tasks: 124, decisions: 312,      │
│             blockers: 89, other: 322)            │
│   [Browse] [Search]                              │
│                                                  │
│ Tier 3 (Cold) — Deep Archive                     │
│   Size: 2.4M tokens                              │
│   Records: 4,891                                 │
│   Embeddings: 4,891 vectors                      │
│   [Search] [Paging Simulator]                    │
└──────────────────────────────────────────────────┘
```

#### Tier 1 (Hot) Contents

Detailed view of current Working Memory:

| Item | Type | Tokens | Age | Source |
|---|---|---|---|---|
| Task instructions | instruction | 2,100 | 4m 23s | Initial hydration |
| JWT fix pattern | paged | 450 | 2m 01s | page_retrieve |
| Auth module analysis | working | 1,800 | 1m 30s | Agent output |
| API documentation | paged | 1,200 | 45s | page_retrieve |

Each item can be expanded to view full content or manually demoted to free budget.

#### Tier 2 (Warm) Browser

Structured browsing of project records:

```
Browse by category:
  ├── Tasks (124)
  │   ├── Completed (98)
  │   ├── Failed (8)
  │   └── Blocked (18)
  ├── Decisions (312)
  │   ├── Architecture (45)
  │   ├── Implementation (189)
  │   └── Testing (78)
  ├── Blockers (89)
  │   ├── Resolved (72)
  │   └── Open (17)
  └── Requirements (322)
      ├── Current (156)
      └── Historical (166)
```

Each record shows a summary with options to view full content, see relationships, or trace the paging history.

#### Tier 3 (Cold) Search

Semantic search interface for the Deep Archive:

```
┌──────────────────────────────────────────────────┐
│ Search Deep Archive                              │
│                                                  │
│ Query: [___________________________________]     │
│                                                  │
│ Filters:                                         │
│   Content type: [All ▾]                          │
│   Time range:    [All time ▾]                    │
│   Min relevance: [0.5 ─────●──── 1.0]           │
│                                                  │
│ [Search]                                         │
│                                                  │
│ Estimated cost per retrieval: ~500 tokens        │
│ Would appear as: page_search → page_retrieve     │
└──────────────────────────────────────────────────┘
```

#### Paging Simulator

A tool for testing paging queries before agents use them:

```
┌──────────────────────────────────────────────────┐
│ Paging Simulator                                 │
│                                                  │
│ Test query: "authentication error patterns"      │
│                                                  │
│ Simulated results:                               │
│   #1 mem_456 — Blocker (relevance: 0.91)         │
│      "JWT invalid signature resolved by          │
│       rotating secret key"                        │
│      Cost: 450 tokens                            │
│                                                  │
│   #2 mem_789 — Decision (relevance: 0.84)        │
│      "Use RS256 for external, HS256 for          │
│       internal JWT signing"                       │
│      Cost: 620 tokens                            │
│                                                  │
│ Suggestion: Query "auth error resolution"         │
│ may return more targeted results.                 │
└──────────────────────────────────────────────────┘
```

---

### 5. Task Pipeline View

High-level view of all tasks in the system.

```
┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐
│  QUEUED   │  │  RUNNING  │  │ REVIEWING │  │ RETRYING  │  │ COMPLETE  │
│           │  │           │  │           │  │           │  │           │
│ Task #5   │  │ Task #3   │  │ Task #2   │  │ Task #4   │  │ Task #1   │
│ Task #6   │  │           │  │           │  │           │  │           │
│ Task #7   │  │           │  │           │  │           │  │           │
└───────────┘  └───────────┘  └───────────┘  └───────────┘  └───────────┘

       ┌───────────┐
       │ BLOCKED   │
       │           │
       │ Task #8   │  ← Click for escalation details
       └───────────┘
```

Each task card shows:
- Task ID and description
- Current phase
- Budget consumed
- Duration so far
- Status (with color coding)

---

## Technical Implementation

### Data Sources

The dashboard reads from:
- **orchestrator.db** (SQLite) — Tasks, decisions, blockers, memory index
- **vectors.db** (SQLite + sqlite-vec) — Embeddings and search
- **Workspace directory** — File listing and modification status

### Recommended Stack

- **Backend**: Local FastAPI server reading SQLite directly
- **Frontend**: React SPA with real-time updates via WebSocket
- **Packaging**: Tauri or Electron for desktop app (optional)

### Refresh Strategy

| Data | Update Frequency | Method |
|---|---|---|
| Budget utilization | Real-time | WebSocket push |
| Agent activity | Real-time | WebSocket push |
| Paging operations | Real-time | WebSocket push |
| Memory contents | On demand | REST API calls |
| Task pipeline | On change | WebSocket push |

---

## Next Steps

- [Architecture Overview](../architecture/OVERVIEW.md) — Where the dashboard fits in the system
- [Interface Contracts](../interfaces/README.md) — The APIs the dashboard consumes
- [Verification Engine](../verification/README.md) — Verification results shown in the task pipeline
- [Portable Stack Guide](../stack/README.md) — Implementation guidance for the dashboard backend
