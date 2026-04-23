# Paging System

The MemGPT-style autonomous retrieval system that lets agents access deeper memory tiers when their working context is insufficient.

---

## Overview

Paging is the mechanism by which agents self-service historical context. Instead of relying on the orchestrator to pre-load everything an agent *might* need, agents decide for themselves when they need more information and retrieve it on demand.

The system provides four paging tools:

| Tool | Purpose | Budget Cost |
|---|---|---|
| `page_search` | Find relevant memories across Tier 2 and Tier 3 | Minimal |
| `page_retrieve` | Load specific memories into Working Memory | Proportional to content size |
| `page_recurse` | Follow relationship chains between memories | Configurable |
| `page_store` | Save Working Memory content to deeper tiers | Frees budget |

---

## Tool Specifications

### `page_search`

Semantic search across Structured Storage and Deep Archive.

**When to use:** When the agent needs to find relevant historical context but doesn't know exactly what it's looking for.

**Input:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `query` | string | Yes | Natural language search query |
| `resultLimit` | number | No | Maximum results to return (default: 5) |
| `relevanceThreshold` | number | No | Minimum relevance score 0–1 (default: 0.7) |
| `tiers` | Tier[] | No | Which tiers to search (default: all) |
| `contentTypes` | string[] | No | Filter by content type (decision, blocker, task, log) |
| `timeRange` | TimeRange | No | Limit to a time period |

**Output:**

An array of search results, each containing:

| Field | Description |
|---|---|
| `memoryId` | Unique reference for `page_retrieve` |
| `tier` | Which tier the content lives in |
| `contentType` | Category of the content |
| `summary` | Brief description (not full content) |
| `relevanceScore` | Similarity to the query (0–1) |
| `estimatedTokens` | Token cost if retrieved |
| `timestamp` | When the content was created |
| `relationships` | Related memory IDs |

**Budget Impact:** Minimal — only metadata is returned, not full content.

**Example:**

```
Agent encounters: "JWT token validation failures"

→ page_search({
    query: "JWT token validation failures",
    resultLimit: 5,
    relevanceThreshold: 0.7
  })

← [
    {
      memoryId: "mem_456",
      tier: TIER_2,
      contentType: "blocker",
      summary: "JWT 'invalid signature' error resolved by rotating secret key",
      relevanceScore: 0.94,
      estimatedTokens: 450,
      timestamp: "2026-03-15T10:30:00Z",
      relationships: ["mem_457", "mem_458"]
    },
    {
      memoryId: "mem_123",
      tier: TIER_3,
      contentType: "decision",
      summary: "Decision to use RS256 over HS256 for JWT signing",
      relevanceScore: 0.78,
      estimatedTokens: 1200,
      timestamp: "2026-02-01T14:00:00Z",
      relationships: []
    }
  ]
```

---

### `page_retrieve`

Fetch the full content of specific memory fragments into Working Memory.

**When to use:** After `page_search` has identified relevant memories, and the agent decides which ones are worth the token cost.

**Input:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `memoryIds` | string[] | Yes | IDs from search results or known references |
| `includeRelationships` | boolean | No | Also load linked memories (default: false) |
| `maxTokens` | number | No | Cap on total tokens to retrieve |

**Output:**

An array of retrieved memories, each containing:

| Field | Description |
|---|---|
| `memoryId` | The identifier |
| `content` | Full text content |
| `metadata` | Type, tier, timestamps, tags |
| `relatedIds` | IDs of linked memories |
| `tokensUsed` | Token count (deducted from budget) |

**Budget Impact:** Deducted from the Active Task Budget's paging quota. The agent sees the cost before confirming (via `estimatedTokens` in search results).

**Example:**

```
Agent reviews search results, decides mem_456 is most relevant.

→ page_retrieve({
    memoryIds: ["mem_456"]
  })

← [
    {
      memoryId: "mem_456",
      content: "Previous JWT fix: The 'invalid signature' error in Task #123
                was caused by a stale JWT_SECRET environment variable after a
                key rotation. Resolution: Updated the env var and added a
                health check that validates the current secret on startup.
                Tests: Added test case 'should reject tokens signed with old secret'.",
      metadata: { type: "blocker", tier: TIER_2, resolved: true },
      relatedIds: ["mem_457", "mem_458"],
      tokensUsed: 450
    }
  ]

Budget update: Paging quota reduced by 450 tokens.
```

---

### `page_recurse`

Follow relationship chains between memories — graph traversal across the memory graph.

**When to use:** When the agent needs connected context: "Get this blocker, then the decision that resolved it, then the tasks affected by that decision."

**Input:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `startId` | string | Yes | Memory ID to start from |
| `relationshipType` | string | No | Type of relationship to follow (default: all) |
| `depth` | number | No | Maximum traversal depth (default: 2) |
| `mode` | "metadata" \| "full" | No | Return metadata only or full content (default: "metadata") |

**Relationship Types:**

| Type | Description |
|---|---|
| `resolved_by` | Blocker → Decision or solution |
| `depends_on` | Task → Task dependency |
| `related_to` | General relationship |
| `supersedes` | Newer version of a memory |
| `referenced_by` | Citation or cross-reference |

**Output:**

A graph traversal result containing:

| Field | Description |
|---|---|
| `nodes` | Array of discovered memories (metadata or full content) |
| `edges` | Array of relationships (source, target, type) |
| `depthReached` | How deep the traversal went |
| `tokensUsed` | Token cost (0 for metadata mode) |

**Budget Impact:**
- **Metadata mode**: Minimal — only structure is returned
- **Full mode**: Deducted based on total content size across all retrieved nodes

**Example:**

```
Agent wants to understand the full context around a blocker resolution.

→ page_recurse({
    startId: "mem_456",
    relationshipType: "resolved_by",
    depth: 3,
    mode: "metadata"
  })

← {
    nodes: [
      { memoryId: "mem_456", summary: "JWT invalid signature blocker", tier: TIER_2 },
      { memoryId: "mem_457", summary: "Decision: Rotate JWT secret", tier: TIER_2 },
      { memoryId: "mem_458", summary: "Health check implementation for JWT", tier: TIER_2 },
      { memoryId: "mem_460", summary: "Test case for old secret rejection", tier: TIER_2 }
    ],
    edges: [
      { source: "mem_456", target: "mem_457", type: "resolved_by" },
      { source: "mem_457", target: "mem_458", type: "related_to" },
      { source: "mem_458", target: "mem_460", type: "referenced_by" }
    ],
    depthReached: 3,
    tokensUsed: 0
  }

Agent now has the full dependency graph and can selectively retrieve specific nodes.
```

---

### `page_store`

Save content from Working Memory to deeper tiers. This both persists the information and frees Working Memory budget.

**When to use:** When the agent has produced insights, decisions, or intermediate results that should be preserved and/or removed from Working Memory.

**Input:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `content` | string | Yes | The content to store |
| `targetTier` | "warm" \| "cold" | Yes | Which tier to store in |
| `contentType` | string | No | Category tag |
| `tags` | string[] | No | Searchable keywords |
| `relationships` | Relationship[] | No | Links to existing memories |
| `retentionPolicy` | string | No | "permanent", "standard", "temporary" |

**Output:**

| Field | Description |
|---|---|
| `memoryId` | The ID assigned to the stored content |
| `tokensFreed` | Working Memory tokens freed by this operation |
| `embeddingGenerated` | Whether a vector embedding was created |

**Budget Impact:** Negative — frees Working Memory tokens.

**Example:**

```
Agent has completed analysis and wants to offload findings.

→ page_store({
    content: "Authentication module analysis: Uses JWT with HS256.
              Secret from JWT_SECRET env var. 3600s expiry.
              No refresh token mechanism. Recommend adding refresh
              tokens in Phase 2.",
    targetTier: "warm",
    contentType: "decision",
    tags: ["authentication", "jwt", "security"],
    relationships: [
      { targetId: "mem_456", type: "related_to" }
    ],
    retentionPolicy: "standard"
  })

← {
    memoryId: "mem_789",
    tokensFreed: 1800,
    embeddingGenerated: true
  }

Budget update: Working Memory gains 1800 tokens.
```

---

## Paging Triggers

Agents invoke paging tools autonomously when they detect specific situations:

### 1. Blocker Detection

The agent encounters an obstacle that its current context can't resolve.

```
Agent: "I'm getting an unfamiliar error: E_CONNREFUSED on port 5432"
Agent → page_search("database connection refused port 5432")
```

### 2. Pattern Recognition

The agent recognizes similarity to past situations.

```
Agent: "This feels like the same authentication flow we fixed last week"
Agent → page_search("authentication flow fix recent")
```

### 3. Requirement Ambiguity

The agent needs to retrieve original requirements or decision rationale.

```
Agent: "The spec says 'handle edge cases' but doesn't specify which ones"
Agent → page_search("edge case requirements for payment processing")
```

### 4. Optimization Seeking

The agent is looking for proven solutions rather than reinventing approaches.

```
Agent: "I need to implement batch processing. Let me check how we did it before."
Agent → page_search("batch processing implementation pattern")
```

### 5. Validation

The agent wants to cross-check against established standards or past reviews.

```
Agent: "Let me verify this approach meets our quality standards"
Agent → page_search("code review standards error handling")
```

---

## Paging Budget Management

### Quota System

Each agent session has a paging budget — a portion of the Active Task Budget reserved for retrieval operations.

```
Active Task Budget: 60,000 tokens
Paging Quota (30%): 18,000 tokens
  ├── page_search operations: ~100 tokens each (metadata only)
  ├── page_retrieve operations: Variable (content-sized)
  ├── page_recurse operations: Variable (depends on depth + mode)
  └── page_store operations: FREE (actually frees tokens)
```

### Cost Awareness

Agents receive budget metadata before and after every paging operation:

**Before retrieval:**
```
Estimated cost: 450 tokens
Remaining paging quota: 14,200 tokens
Proceed? [This happens automatically based on estimated cost < remaining quota]
```

**After retrieval:**
```
Actual cost: 438 tokens
Remaining paging quota: 13,762 tokens
Quota utilization: 23.6%
```

### Quota Exhaustion

When the paging quota is exhausted:

1. **`page_search`** still works (minimal cost)
2. **`page_retrieve`** is blocked — agent must proceed with current context
3. **`page_store`** still works (frees budget, may enable further retrieval)
4. Agent can **request a quota increase** from the orchestrator with justification

### Caching

Retrieved content stays in Working Memory for the session duration:

- **Subsequent references** to the same memory ID are zero-cost
- **LRU eviction** removes cached content when Working Memory is pressured
- Evicted content can be retrieved again (costing paging quota) if needed

### Loop Prevention

To prevent agents from getting stuck in infinite retrieval cycles:

| Safeguard | Description |
|---|---|
| Maximum operations | Configurable limit per session (default: 50 paging operations) |
| Cooldown period | Minimum interval between similar searches (default: 2 seconds) |
| Deduplication | Identical queries within a session return cached results |
| Cost escalation | Repeated searches on the same topic show diminishing results |
| Budget hard stop | No paging when quota exhausted (no override possible) |

---

## Workflow Example: Full Paging Sequence

```
Specialist Agent encounters unfamiliar error pattern
        │
        ▼
┌───────────────────────────────────────┐
│ Step 1: page_search                   │
│ Query: "similar authentication errors │
│         with JWT"                     │
│ Cost: 120 tokens                      │
│                                       │
│ Results: 3 candidates (metadata only) │
│   #1 mem_456 — relevance 0.94 (450t)  │
│   #2 mem_123 — relevance 0.78 (1200t) │
│   #3 mem_789 — relevance 0.65 (300t)  │
└───────────────┬───────────────────────┘
                │
                ▼
┌───────────────────────────────────────┐
│ Step 2: Agent evaluates results       │
│                                       │
│ Decision: Retrieve #1 only            │
│   - Highest relevance (0.94)          │
│   - Reasonable cost (450 tokens)      │
│   - Recent (Tier 2, likely detailed)  │
│ Skip #2: Low relevance, high cost     │
│ Skip #3: Below threshold              │
└───────────────┬───────────────────────┘
                │
                ▼
┌───────────────────────────────────────┐
│ Step 3: page_retrieve                 │
│ IDs: ["mem_456"]                      │
│ Cost: 450 tokens                      │
│                                       │
│ Content: "Previous JWT fix: secret    │
│ key rotation resolved 'invalid        │
│ signature' error in Task #123..."     │
└───────────────┬───────────────────────┘
                │
                ▼
┌───────────────────────────────────────┐
│ Step 4: Agent applies learning        │
│                                       │
│ "The previous fix was key rotation.   │
│  Let me check if the current secret   │
│  is stale."                           │
│                                       │
│ Agent checks JWT_SECRET → finds       │
│ mismatch → rotates key → tests pass   │
└───────────────┬───────────────────────┘
                │
                ▼
┌───────────────────────────────────────┐
│ Step 5: page_store                    │
│ Content: "Applied JWT secret rotation │
│  fix for stale env var. Tests pass."  │
│ Target: Tier 2 (Structured Storage)   │
│                                       │
│ Result: mem_892 created               │
│         380 tokens freed from         │
│         Working Memory                │
└───────────────────────────────────────┘
```

---

## Next Steps

- [Memory Tiers](TIERS.md) — What content lives in each tier
- [Context Budgeting](../context-budgeting/README.md) — How paging costs are managed
- [Blocker Handling](../workflow/BLOCKER-HANDLING.md) — How paging integrates with blocker resolution
- [Interface Contracts](../interfaces/README.md) — Formal TypeScript interfaces for paging tools
