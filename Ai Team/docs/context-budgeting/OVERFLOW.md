# Overflow Handling & Recovery

The multi-phase protocol that activates when a component approaches or exceeds its token budget.

---

## Overview

Overflow handling is a graduated response system. It doesn't wait for budgets to be exhausted — it starts responding at 80% utilization and escalates through three phases.

```
                    80%         100%         110%
─────────────────────┬───────────┬────────────┬──────────
   NORMAL OPERATION  │ SOFT      │ HARD       │ EMERGENCY
                     │ WARNING   │ LIMIT      │ PROTOCOL
                     │           │            │
                     ▼           ▼            ▼
                  Notify +    Compress +    Halt + Preserve
                  Suggest     Enforce      Critical Only
```

---

## Phase 1: Soft Warning (80% Utilization)

**Trigger:** A component reaches 80% of its allocated budget.

**What Happens:**

1. The Context Budgeter sends a notification to the component with:
   - Current utilization (absolute tokens and percentage)
   - Remaining budget
   - Estimated tokens needed for typical completion

2. The component can respond proactively:
   - **Summarize** conversation history to reduce tokens
   - **Offload** completed intermediate results to Working Memory → Structured Storage via `page_store`
   - **Truncate** older context that's no longer directly relevant
   - **Continue** if it expects to complete within the remaining 20%

**Key Characteristics:**
- Advisory, not mandatory
- Component retains full control over its behavior
- No automatic actions taken by the system
- Logged as a soft warning event for dashboard visibility

**Example Scenario:**

```
Specialist Agent at 80% of Active Task Budget:
  - 48,000 of 60,000 tokens used
  - 12,000 remaining
  - Agent decides to:
    1. page_store(completed research notes) → frees 3,000 tokens
    2. Summarize first 10 conversation turns → frees 2,500 tokens
    3. Continue with 14,500 remaining tokens
```

---

## Phase 2: Hard Limit (100% Utilization)

**Trigger:** A component exhausts its allocated budget.

**What Happens:**

The system automatically applies compression strategies in priority order:

### Strategy 1: Summarization (Highest Priority)

Replace detailed history with condensed versions.

```
Before: "I examined the authentication module. The JWT validation
in src/auth/jwt.ts uses HS256 algorithm with a secret loaded from
environment variable JWT_SECRET. The token expiry is set to 3600
seconds. I noticed there's no refresh token mechanism."
                                            (58 tokens)

After:  "Auth uses JWT/HS256 from env JWT_SECRET, 3600s expiry,
no refresh tokens."
                                            (14 tokens)
```

**When it's effective:** When conversation history or intermediate reasoning is verbose but the key points can be compressed.

### Strategy 2: Tier Demotion

Move Working Memory content to Structured Storage or Deep Archive.

```
Working Memory:
  - 15 detailed error analysis reports (8,000 tokens)
  - 3 API documentation references (2,000 tokens)
  - Current task instructions (1,500 tokens)

Action:
  page_store(error analyses, targetTier=DEEP_ARCHIVE)  → -8,000 tokens
  page_store(API docs, targetTier=STRUCTURED_STORAGE)   → -2,000 tokens

Result:
  Working Memory now holds only current task instructions (1,500 tokens)
  Content available via paging if needed later
```

**When it's effective:** When there's content in Working Memory that isn't immediately needed but might be useful later.

### Strategy 3: Truncation (Lowest Priority)

Remove oldest, lowest-priority context using FIFO with priority weighting.

**Priority Classes:**
| Priority | Content Type | Truncation Order |
|---|---|---|
| Critical | Current task instructions, system prompts | Never truncated |
| High | Recent conversation turns, active code context | Last to truncate |
| Medium | Retrieved documentation, intermediate results | Middle |
| Low | Historical context, older conversation turns | First to truncate |

**When it's effective:** When summarization and tier demotion don't free enough tokens. This is the strategy of last resort before emergency mode.

### Strategy 4: Paging Recommendation

The system suggests the agent use paging tools for future deep retrieval instead of holding content in Working Memory.

This isn't a compression action itself, but a behavioral suggestion: "Instead of keeping all API docs in context, store them and page them when needed."

**When it's effective:** As a companion to tier demotion — ensuring the agent knows it can get content back.

---

## Phase 3: Emergency Protocol (110% Utilization)

**Trigger:** A critical operation exceeds the hard limit despite compression.

**What Happens:**

1. **Halt Non-Essential Operations**
   - All non-critical agent operations are suspended
   - Only operations on the critical path continue

2. **Preserve Critical Context Only**
   - The system identifies task-critical context using priority classes
   - Everything else is immediately demoted or discarded
   - Critical context is moved into the System Reserve (the untouchable 10%)

3. **Escalate to Orchestrator**
   - The orchestrator receives an emergency notification with:
     - Current state snapshot
     - What was preserved and what was lost
     - Compression actions already taken
     - Estimated additional tokens needed

4. **Orchestrator Response**
   - **Option A: Reallocate** — Shift tokens from other phases if available
   - **Option B: Decompose** — Split the task into smaller pieces requiring less context each
   - **Option C: Queue for Human Intervention** — If neither option works, suspend the task with a full context dump for human review

**Example Emergency Flow:**

```
Specialist Agent at 110%:
  - 66,000 of 60,000 tokens used (overshoot of 6,000)
  
  Actions:
  1. HALT — Stop all agent work immediately
  2. PRESERVE:
     - Keep: Current code being written, task requirements
     - Demote: All conversation history, research notes
     - Discard: Old error logs, completed intermediate results
  3. ESCALATE to Orchestrator:
     "Agent exceeded budget by 6,000 tokens.
      Preserved: task instructions + current code (4,200 tokens)
      Lost: 61,800 tokens demoted to Tier 2/3
      Requesting: 8,000 additional tokens for completion"
  4. ORCHESTRATOR DECISION:
     "Granting 8,000 from Orchestrator reserve.
      Agent may resume with preserved context only."
```

---

## Budget Recovery

After overflow handling or task completion, tokens are reclaimed:

### Automatic Recovery

| Event | Recovery Action |
|---|---|
| Task completes successfully | Entire Active Task Budget released |
| Compression succeeds | Freed tokens return to the component's allocation |
| Phase completes early | Surplus tokens move to the general pool |
| Task fails and is retried | Only the delta-prompt budget is allocated (not the full original) |

### Recovery Metrics

The system tracks recovery effectiveness:

| Metric | Description | Target |
|---|---|---|
| Compression Ratio | Tokens before / after compression | > 3:1 |
| Recovery Rate | Tokens recovered / tokens over budget | > 80% |
| Data Loss Rate | Content permanently lost during emergency | < 5% |
| Recovery Time | Time from overflow to stable operation | < 30 seconds |

---

## Configuration

Overflow thresholds and strategies are configurable per project:

```yaml
context_budget:
  total_window: 128000
  
  allocation:
    strategy: "dynamic"          # dynamic | fixed
    default_profile: "balanced"  # simple | balanced | research
  
  overflow:
    soft_warning: 0.80           # Trigger Phase 1 at 80%
    hard_limit: 1.00             # Trigger Phase 2 at 100%
    emergency: 1.10              # Trigger Phase 3 at 110%
    
    compression_order:
      - summarize
      - tier_demote
      - truncate
    
    truncation_priority:
      critical: never            # Never truncate
      high: 0.8                  # Truncate at 80% of allocation
      medium: 0.6
      low: 0.4
```

---

## Next Steps

- [Allocation Strategies](ALLOCATION.md) — How budgets are initially distributed
- [Memory & Paging](../memory/README.md) — How tier demotion works in practice
- [Operations Guide](../operations/README.md) — Tuning overflow parameters for production
