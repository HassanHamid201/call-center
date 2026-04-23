# Memory & Paging

The three-tier memory architecture with autonomous paging that lets agents retrieve historical context when their working memory is insufficient.

---

## In This Section

| Document | Description |
|---|---|
| [Memory Tiers](TIERS.md) | Detailed specification of the three-tier memory model |
| [Paging System](PAGING.md) | MemGPT-style autonomous retrieval tools and workflows |

---

## The Problem

Agents have limited context windows. During execution, they need access to:

- The current task's requirements and constraints
- Project history (past decisions, resolved blockers, established patterns)
- Technical documentation and standards
- Results from previous tasks in the same project

Loading everything upfront is wasteful and often impossible. The memory architecture solves this by organizing information into tiers with different access patterns, and giving agents tools to retrieve deeper context on demand.

---

## Architecture Overview

```
┌──────────────────────────────────────────────────────────┐
│                    AGENT WORKER                           │
│                                                          │
│  ┌────────────────────────────────────┐                  │
│  │   TIER 1: WORKING MEMORY (HOT)     │  Zero latency    │
│  │   • Current conversation            │  Deducted from   │
│  │   • Active task instructions        │  Active Task     │
│  │   • Recently paged content          │  Budget          │
│  └────────────────┬───────────────────┘                  │
│                   │                                      │
│         page_search / page_retrieve                      │
│                   │                                      │
│  ┌────────────────▼───────────────────┐                  │
│  │   TIER 2: STRUCTURED STORAGE (WARM)│  Transactional   │
│  │   • Task hierarchies                │  Persistent      │
│  │   • Decision logs                   │  across sessions │
│  │   • Blocker records                 │  Minimal budget  │
│  │   • Requirements history            │  impact          │
│  │   • Paging index                    │                  │
│  └────────────────┬───────────────────┘                  │
│                   │                                      │
│         page_search / page_retrieve                      │
│                   │                                      │
│  ┌────────────────▼───────────────────┐                  │
│  │   TIER 3: DEEP ARCHIVE (COLD)      │  Paging only     │
│  │   • Full execution logs             │  No budget       │
│  │   • Historical code versions        │  impact until    │
│  │   • Resolved blockers               │  paged in        │
│  │   • Domain heuristics               │                  │
│  │   • Semantic embeddings             │                  │
│  └────────────────────────────────────┘                  │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

---

## Key Principles

### 1. Content Lives at the Deepest Appropriate Tier

Information is stored at the lowest tier where it remains accessible. A resolved blocker belongs in Tier 3 (Deep Archive), not cluttering Tier 1 (Working Memory). It can be paged in when needed.

### 2. Agents Retrieve On Demand

Instead of the orchestrator guessing what an agent will need, agents decide for themselves when they need more context. This is more efficient because:
- The agent knows best what information gap it's experiencing
- Not all context ends up being needed
- The agent can evaluate search results before committing to retrieval

### 3. Retrieval Has a Cost

Every `page_retrieve` deducts from the Active Task Budget. This prevents infinite retrieval loops and forces agents to be selective about what they bring into Working Memory.

### 4. Paging Is Self-Service

Agents don't need to ask the orchestrator for permission to page (within their quota). They detect when they're blocked or uncertain, search for relevant context, and retrieve what they need — all autonomously.

---

## How Tiers Interact

### During Task Execution

```
Agent starts task
       │
       ▼
Working Memory hydrated with:
  - Task requirements
  - Relevant skill documentation
  - Recent project context
       │
       ▼
Agent executes...
       │
       ├── Encounters unfamiliar error ──► page_search("similar errors")
       │                                        │
       │                                        ▼
       │                                  Results from Tier 2/3
       │                                        │
       │                                        ▼
       │                                  page_retrieve(best match)
       │                                        │
       │                                        ▼
       │                                  Content loaded into Tier 1
       │                                  (budget deducted)
       │
       ├── Makes architectural decision ──► page_store(decision, TIER_2)
       │                                        │
       │                                        ▼
       │                                  Decision logged in Tier 2
       │                                  (budget freed from Tier 1)
       │
       └── Task completes ──► All results stored in Tier 2/3
                               Working Memory released
```

### Across Sessions

```
Session 1:
  Agent implements feature X
  Decisions stored in Tier 2
  Full logs archived in Tier 3
  
       │  (time passes)
       ▼

Session 2:
  New agent starts task related to feature X
  Working Memory hydrated with minimal context
  Agent pages Tier 2 for decisions about X
  Agent pages Tier 3 for implementation details
  Agent has full context without pre-loading everything
```

---

## Quick Reference: Paging Tools

| Tool | Purpose | Budget Impact |
|---|---|---|
| `page_search` | Semantic search across Tier 2 and Tier 3 | Minimal (metadata only) |
| `page_retrieve` | Fetch full content into Working Memory | Deducted (size of content) |
| `page_recurse` | Follow relationship chains between memories | Configurable (metadata or full) |
| `page_store` | Save Working Memory content to deeper tiers | Negative (frees budget) |

For detailed tool specifications, see [Paging System](PAGING.md).

---

## Next Steps

- [Memory Tiers](TIERS.md) — Detailed specification of each tier's structure, content, and access patterns
- [Paging System](PAGING.md) — The four paging tools, their interfaces, and workflow examples
- [Context Budgeting](../context-budgeting/README.md) — How paging interacts with the token budget system
