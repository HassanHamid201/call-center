# Safety Mechanisms

Protections built into the system to prevent runaway resource consumption, infinite loops, and unstable behavior.

---

## Why Safety Matters

The system gives agents significant autonomy — they can invoke tools, retrieve memory, and modify files. Without safety mechanisms, a misbehaving agent could:

- Exhaust the context window, crashing the session
- Enter infinite paging loops, burning through tokens
- Modify critical files without validation
- Consume unbounded LLM API credits

The safety mechanisms described here are **non-optional** — they are always active and cannot be disabled by configuration.

---

## 1. Paging Loop Prevention

Prevents agents from getting stuck in infinite retrieval cycles.

### Maximum Operations Per Session

Each agent session has a hard cap on paging operations:

| Setting | Default | Description |
|---|---|---|
| `max_operations_per_session` | 50 | Total paging tool invocations per session |
| `max_searches_per_topic` | 5 | Maximum unique searches on the same topic |
| `max_retrieves_per_search` | 3 | Maximum retrievals per search result set |

When any limit is reached, the corresponding paging tool returns an error and the agent must proceed with current context.

### Cooldown Periods

Minimum intervals between similar operations:

```yaml
paging:
  cooldown_period: 2  # seconds between similar queries
```

If an agent issues the same (or very similar) query within the cooldown period, the system returns cached results from the previous query instead of executing a new search.

### Redundant Query Detection

The system detects and blocks queries that are substantively identical to recent queries:

- **Exact match**: Same query text → return cached results
- **Semantic match**: Cosine similarity > 0.95 to a recent query → return cached results with note
- **Subsumption**: New query is a subset of a recent broader query → return filtered cached results

### Cost Escalation

Repeated searches on the same topic face increasing costs:

| Attempt | Cost Multiplier | Relevance Threshold |
|---|---|---|
| 1st | 1.0x | 0.70 |
| 2nd | 1.0x | 0.70 |
| 3rd | 1.1x | 0.75 |
| 4th | 1.2x | 0.80 |
| 5th+ | 1.5x | 0.90 |

This makes repeated retrieval progressively more expensive and selective, naturally discouraging loops.

---

## 2. Budget Exhaustion Recovery

Graceful degradation when a component runs out of tokens.

### Graceful Degradation Steps

```
Budget approaching limit
       │
       ▼
Step 1: Switch to minimal viable context
  • Only task-critical instructions remain
  • All historical context demoted or discarded
  
       │
       ▼
Step 2: Suspend non-critical operations
  • Paging is restricted to critical-path only
  • Tool invocations limited to essential operations
  • Review phase shortened (basic checks only)
  
       │
       ▼
Step 3: Preserve state and suspend
  • Agent session state serialized to disk
  • All Working Memory flushed to Tier 2/3
  • Task marked as "suspended" with budget context
  
       │
       ▼
Step 4: Notify for intervention
  • Human receives notification with:
    - Why the task was suspended
    - Current state snapshot
    - Options: increase budget, simplify task, cancel
```

### Automatic Task Suspension

When suspension is triggered, the system preserves:

| What's Preserved | How |
|---|---|
| Task instructions | Stored in Tier 2 |
| Partial results | Stored in workspace with `.partial` suffix |
| Paging log | Archived in Tier 3 |
| Budget consumption record | Logged for analysis |
| Agent's last known state | Serialized to JSON in project directory |

This allows tasks to be resumed with increased budget without starting over.

---

## 3. Retry Limits

Prevents the Ralph Loop from running indefinitely.

### Iteration Caps

```yaml
orchestrator:
  max_retries: 3  # Maximum retry iterations per task
```

| Iteration | What Happens |
|---|---|
| 1st attempt | Full execution budget |
| 1st retry | Delta-prompt with reduced budget (~60% of original) |
| 2nd retry | Further reduced delta (~40% of original) |
| 3rd retry | Minimal delta (~20% of original) |
| Max reached | Task escalated to human |

### Diminishing Returns Detection

If each retry produces the same review failure, the system detects the pattern:

```
Attempt 1: FAIL — "missing error handling"
Attempt 2: FAIL — "missing error handling" (same issue)
Attempt 3: FAIL — "missing error handling" (same issue)
  │
  ▼
System detects: Agent is not making progress on this issue
Action: Escalate immediately instead of exhausting retries
```

### Budget Cap Per Task

Even with retries, a single task cannot consume more than a configurable percentage of the total context window:

```yaml
orchestrator:
  max_task_budget_percentage: 0.80  # 80% of total window max
```

This prevents a single stubborn task from consuming the entire session.

---

## 4. Emergency Reserve Protection

The system reserve (default 10%) is protected by hard rules:

### What Can Use the Reserve

| Operation | Can Access Reserve | Conditions |
|---|---|---|
| Emergency overflow handling | Yes | Only to preserve critical-path context |
| Human escalation context dump | Yes | Only for the escalation package |
| Normal agent execution | No | Never |
| Paging operations | No | Never |
| Orchestrator planning | No | Never |

### Reserve Depletion Recovery

If the emergency reserve itself is depleted:

1. **All operations halt immediately** — No agent can execute
2. **State is flushed to disk** — Everything is persisted to SQLite
3. **System enters maintenance mode** — Only human can interact
4. **Human must explicitly acknowledge** the depletion and choose:
   - Increase context window and restart
   - Reduce task scope and restart
   - Cancel the current task and recover

---

## 5. Filesystem Safety

Protections for the agent workspace.

### Write Restrictions

Agents cannot modify:

| Path Pattern | Reason |
|---|---|
| `**/config.yaml` | System configuration is read-only during execution |
| `**/orchestrator.db` | Database is accessed through APIs, not direct file writes |
| `**/vectors.db` | Same as above |
| `**/.git/**` | Git operations go through the git tool, not direct file access |

### Backup Before Modification

Before an agent modifies a file:
1. The current version is copied to `.agent-backup/` with a timestamp
2. The modification proceeds
3. If the agent session fails, the backup is automatically restored

### Workspace Size Limits

```yaml
orchestrator:
  max_workspace_size_mb: 500  # Agent cannot create files exceeding this total
```

---

## 6. LLM API Safety

Protections against unbounded API usage.

### Token Budget Enforcement

The context budgeter enforces token limits **before** sending requests to the LLM:

```
Agent prepares request
       │
       ▼
Budgeter checks: Will this request exceed the allocation?
       │
       ├── No → Allow request
       │
       └── Yes → Block request
                  │
                  ▼
              Trigger compression
                  │
                  ▼
              Retry with compressed context
```

### Rate Limiting

```yaml
llm:
  max_requests_per_minute: 30
  max_tokens_per_minute: 100000
```

These prevent runaway agents from consuming API credits faster than expected.

### Failure Circuit Breaker

If LLM calls start failing:

| Consecutive Failures | Action |
|---|---|
| 1–2 | Retry with exponential backoff |
| 3–5 | Switch to fallback model (if configured) |
| 6+ | Halt all agent operations and notify human |

---

## Safety Configuration Summary

All safety parameters with their defaults:

```yaml
# Paging safety
paging:
  max_operations_per_session: 50
  cooldown_period: 2
  max_retries_per_query: 5

# Budget safety
context_budget:
  overflow:
    soft_warning: 0.80
    hard_limit: 1.00
    emergency: 1.10

orchestrator:
  max_retries: 3
  max_task_budget_percentage: 0.80

# API safety
llm:
  max_requests_per_minute: 30
  max_tokens_per_minute: 100000
  circuit_breaker_threshold: 6

# Workspace safety
orchestrator:
  max_workspace_size_mb: 500
```

---

## Next Steps

- [Budget Tuning](TUNING.md) — How to adjust these parameters
- [Overflow Handling](../context-budgeting/OVERFLOW.md) — How the overflow protocol works
- [Success Metrics](../metrics/README.md) — How to measure safety effectiveness
