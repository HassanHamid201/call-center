# Memory Tiers

Detailed specification of the three-tier memory model: Working Memory (hot), Structured Storage (warm), and Deep Archive (cold).

---

## Tier 1: Working Memory (Hot)

The agent's immediate execution context. Everything the agent can directly reference without a paging operation.

### Characteristics

| Property | Value |
|---|---|
| **Scope** | Single task execution context |
| **Lifetime** | Duration of the agent execution session |
| **Access Pattern** | High-frequency read/write, zero latency |
| **Budget Impact** | Deducted from Active Task Budget |
| **Persistence** | None — lost when session ends unless explicitly stored |

### Content Types

- **Conversation turns** — The current back-and-forth between agent and its task
- **Intermediate calculations** — Partial results, draft code, in-progress analysis
- **Temporary variables** — Values needed for current reasoning
- **Recently paged data** — Content retrieved from deeper tiers, now in immediate context
- **Active task instructions** — The task prompt and current execution plan
- **Skill documentation** — Relevant portions of loaded skill manifests

### Size Management

Working Memory is the most constrained resource in the system. It's bounded by the Active Task Budget minus the paging quota:

```
Available Working Memory = Active Task Budget - Paging Quota

Example:
  Active Task Budget = 60,000 tokens
  Paging Quota = 18,000 tokens (30%)
  Available Working Memory = 42,000 tokens
```

When Working Memory fills up:
1. The agent can use `page_store` to offload content (frees budget)
2. The Overflow Handler can trigger compression (summarize or truncate)
3. The Paging Controller can evict cached items via LRU policy

### Session Hydration

When an agent session starts, the Memory Manager hydrates Working Memory with:

1. **Task instructions** — From the Structured Task Object
2. **Relevant project context** — Recent decisions, active blockers (from Tier 2)
3. **Skill documentation** — Relevant portions of skill manifests
4. **Pre-fetched content** — Content the orchestrator predicts will be needed

The goal is to provide enough context for the agent to start work without immediate paging, while leaving room for the agent to operate.

---

## Tier 2: Structured Storage (Warm)

The project's structured, queryable knowledge base. Persistent across sessions and designed for relational access.

### Characteristics

| Property | Value |
|---|---|
| **Scope** | Entire project lifecycle |
| **Lifetime** | Persistent across sessions |
| **Access Pattern** | Transactional updates, relational queries, indexed retrieval |
| **Budget Impact** | Minimal — metadata only until paged into Working Memory |
| **Persistence** | Full — stored in the project's SQLite database |

### Content Types

#### Task Records

Every task the system has processed:

| Field | Description |
|---|---|
| Task ID | Unique identifier |
| Parent Task | For sub-tasks, reference to parent |
| Status | Queued, Running, Completed, Failed, Blocked |
| Requirements | Extracted from intent parsing |
| Complexity Score | Assigned by Planning Engine |
| Results | Artifacts, test results, review outcomes |
| Timestamps | Created, started, completed |

#### Decision Logs

Architectural and implementation decisions with full rationale:

| Field | Description |
|---|---|
| Decision ID | Unique identifier |
| Task Context | Which task prompted this decision |
| Decision | What was decided |
| Rationale | Why this option was chosen |
| Alternatives Considered | What was rejected and why |
| Related Decisions | Links to related decisions |

#### Blocker Records

Obstacles encountered and their resolution status:

| Field | Description |
|---|---|
| Blocker ID | Unique identifier |
| Task Context | Which task encountered the blocker |
| Description | What the obstacle is |
| Resolution Status | Open, Resolved, Escalated |
| Resolution | How it was resolved (if applicable) |
| Paging History | What was searched and retrieved during resolution attempts |

#### Requirements History

Evolution of project requirements over time:

| Field | Description |
|---|---|
| Requirement ID | Unique identifier |
| Version | Incremented on changes |
| Content | The requirement text |
| Change Reason | Why it was modified |
| Source | Original prompt, clarification, blocker resolution |

#### Paging Index

Metadata about all content in Tier 2 and Tier 3, enabling fast search:

| Field | Description |
|---|---|
| Memory ID | Unique identifier |
| Tier | Which tier holds the full content |
| Content Type | Task, decision, blocker, log, etc. |
| Summary | Brief description for search results |
| Tags | Categorization keywords |
| Relevance Score | Computed relevance to current project focus |
| Embedding Reference | Link to vector embedding in Tier 3 |
| Relationships | Links to related memories |

### Query Capabilities

Tier 2 supports:
- **Relational queries** — "Get all decisions for task X" or "Get all open blockers"
- **Filtering** — "Decisions made in the last week" or "Blockers with status=OPEN"
- **Relationship traversal** — "Get the decision that resolved blocker Y"
- **Full-text search** — Keyword search across summaries and tags
- **Vector search** — Semantic search via embeddings stored in Tier 3

---

## Tier 3: Deep Archive (Cold)

The complete historical record of everything the system has done. Accessed exclusively through paging operations.

### Characteristics

| Property | Value |
|---|---|
| **Scope** | Cross-project organizational knowledge + full project history |
| **Lifetime** | Long-term with intelligent decay |
| **Access Pattern** | Paging only, higher latency acceptable |
| **Budget Impact** | None until explicitly paged into Working Memory |
| **Persistence** | Full — stored in SQLite + vector store |

### Content Types

#### Full Execution Logs

Complete records of every agent execution:

- Full conversation transcripts (not summaries)
- Every tool invocation and its result
- Intermediate reasoning and decision-making steps
- Token consumption breakdown

#### Historical Code Versions

Snapshots of files at various points in the project lifecycle:

- Before and after each agent modification
- Version metadata (which task, which agent, what changed)
- Diff annotations explaining the changes

#### Resolved Blockers with Full Context

Complete records of how past blockers were resolved:

- Original blocker description and context
- All paging operations attempted
- Solution that ultimately worked
- Lessons learned or heuristics extracted

#### Domain Heuristics

Extracted patterns and rules specific to the project's domain:

- "For this project, always use dependency injection"
- "Authentication tests require a fresh database fixture"
- "The payment module has a specific error handling pattern"

These are extracted automatically from successful resolutions and reinforced over time.

#### Semantic Embeddings

Vector representations of all content for similarity search:

- Embeddings of task descriptions, decisions, and blockers
- Code embeddings for pattern matching
- Document embeddings for knowledge retrieval
- Cross-reference embeddings for relationship discovery

### Decay Policy

Not everything in Tier 3 stays relevant forever. An intelligent decay policy manages content:

| Content Age | Policy |
|---|---|
| 0–7 days | Full retention, highest search ranking |
| 7–30 days | Full retention, normal ranking |
| 30–90 days | Summarized unless frequently accessed |
| 90+ days | Summarized to essentials, reduced search ranking |

Content that is frequently paged (accessed by agents) is promoted in ranking and protected from summarization.

### Vector Storage

Tier 3 uses vector embeddings for semantic search:

- All content is embedded when stored
- `page_search` performs similarity matching against embeddings
- Results include a relevance score for agent evaluation
- Embeddings are updated when content is modified

The recommended storage is **SQLite + sqlite-vec** for maximum portability (see [Portable Stack Guide](../stack/README.md)).

---

## Tier Comparison Summary

| Property | Tier 1 (Hot) | Tier 2 (Warm) | Tier 3 (Cold) |
|---|---|---|---|
| **Scope** | Single task | Project lifecycle | Cross-project |
| **Lifetime** | Session | Persistent | Long-term |
| **Latency** | Zero | Low | Higher |
| **Budget Impact** | High | Minimal (metadata) | None until paged |
| **Access** | Direct | Query + Page | Page only |
| **Content** | Active work | Structured records | Full history |
| **Storage** | In-memory | SQLite | SQLite + Vectors |

---

## Next Steps

- [Paging System](PAGING.md) — How content moves between tiers
- [Context Budgeting](../context-budgeting/README.md) — How tier content affects budgets
- [Portable Stack Guide](../stack/README.md) — Implementation guidance for storage
