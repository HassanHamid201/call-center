# Blocker Handling

The protocol for resolving obstacles that prevent agents from completing their tasks.

---

## Overview

A **blocker** is any obstacle that prevents an agent from making progress. The blocker handling protocol is a graduated escalation system that starts with autonomous resolution (via paging) and escalates to human intervention only when all automated approaches are exhausted.

---

## Escalation Levels

```
Level 1: Autonomous Paging
    Agent searches memory for similar blockers and attempts resolution
         │
         │ (not resolved)
         ▼
Level 2: Extended Paging
    Orchestrator grants additional paging budget for deeper search
         │
         │ (not resolved)
         ▼
Level 3: Orchestrator Intervention
    Orchestrator attempts task decomposition or strategy change
         │
         │ (not resolved)
         ▼
Level 4: Human Escalation
    Full context dump to human with suggested options
```

---

## Level 1: Autonomous Paging

**Who acts:** The agent itself, using its paging toolkit.

**When:** The agent detects it cannot proceed with its current context.

### Process

```
Agent encounters blocker
         │
         ▼
┌─────────────────────────────────────────┐
│ Step 1: Identify the gap                │
│                                         │
│ Agent self-diagnoses:                   │
│ "I don't know why this API returns 403" │
│ "I'm not sure which database schema     │
│  version is current"                    │
│ "This dependency has conflicting        │
│  version requirements"                  │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│ Step 2: Formulate search                │
│                                         │
│ Agent creates targeted search query     │
│ based on the specific gap               │
│                                         │
│ page_search({                           │
│   query: "403 forbidden API auth",      │
│   resultLimit: 5                        │
│ })                                      │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│ Step 3: Evaluate results                │
│                                         │
│ Agent reviews search results:           │
│ - Any relevant past blockers?           │
│ - Any decisions about this topic?       │
│ - Any code patterns that apply?         │
└─────┬───────────────────┬───────────────┘
      │                   │
 (results found)    (no useful results)
      │                   │
      ▼                   ▼
 page_retrieve()     Proceed with current
 best match           knowledge or
      │               escalate to Level 2
      ▼
 Apply learned
 solution
      │
      ├── Resolved → Continue execution
      │
      └── Not resolved → Try next result
                          or escalate to L2
```

### Success Criteria

- Agent finds relevant historical context
- Agent successfully applies the learned solution
- Task continues without human involvement

### Typical Paging Patterns for Blockers

| Blocker Type | Search Query Pattern |
|---|---|
| Unfamiliar error | `"similar {error_message} resolved"` |
| Missing knowledge | `"{topic} implementation pattern"` |
| Conflicting requirements | `"decision about {conflicting_topic}"` |
| Dependency issue | `"{dependency_name} version resolution"` |
| API uncertainty | `"{api_name} usage documentation"` |

---

## Level 2: Extended Paging

**Who acts:** The agent with additional budget from the orchestrator.

**When:** Level 1 failed — the agent searched but couldn't find useful context with its current quota.

### Process

```
Level 1 failed
       │
       ▼
┌─────────────────────────────────────────┐
│ Agent reports to orchestrator:           │
│                                         │
│ "Blocker: Cannot resolve API 403 error  │
│  Paging attempts:                       │
│   - page_search('403 API auth'): 3      │
│     results, none relevant              │
│   - page_search('authentication         │
│     configuration'): 1 result,          │
│     partially relevant, retrieved but   │
│     didn't solve the problem            │
│  Paging quota remaining: 200 tokens     │
│  Requesting: Extended quota for deeper  │
│  search"                                │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│ Orchestrator evaluates:                 │
│                                         │
│ 1. Is the task important enough?        │
│ 2. Is extended paging likely to help?   │
│ 3. Is there budget available?           │
│                                         │
│ Decision: GRANT extended paging quota   │
│   Additional quota: 5,000 tokens        │
│   Source: Orchestrator reserve          │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│ Agent retries with extended quota:      │
│                                         │
│ - Broader searches (lower threshold)    │
│ - Deeper Tier 3 retrieval               │
│ - Recursive relationship traversal      │
│   (page_recurse)                        │
└─────────────────────────────────────────┘
```

### Orchestrator Decision Criteria

| Factor | Grant More Budget | Escalate Further |
|---|---|---|
| Task priority | High or critical | Low |
| Likelihood of resolution | Similar blockers resolved in past | No history of similar issues |
| Budget availability | Reserve has capacity | Reserve is thin |
| Attempts so far | 1–2 paging rounds | 3+ paging rounds |
| Task deadline | Approaching | Flexible |

---

## Level 3: Orchestrator Intervention

**Who acts:** The Orchestrator directly modifies the task strategy.

**When:** Extended paging also failed, but the orchestrator has other strategies to try.

### Strategies

#### Strategy: Task Decomposition

Break the blocked task into smaller pieces, some of which may not be blocked.

```
Original Task: "Implement payment processing with Stripe"
       │
       ▼ (blocked on: Stripe API credentials not available)

Decomposed:
  ✓ "Implement payment data model" (not blocked)
  ✓ "Write payment validation logic" (not blocked)
  ✗ "Integrate Stripe API calls" (blocked → escalate credential issue)
  ✓ "Write unit tests for payment model" (not blocked)
```

#### Strategy: Skill Swap

Replace the current skill with an alternative approach.

```
Current: code-generation skill (blocked on pattern)
       │
       ▼
Alternative: debugging skill (different prompt, different paging triggers)
```

#### Strategy: Context Refresh

Clear the agent's Working Memory and re-hydrate with different initial context.

```
Current context: Led agent into a dead end
       │
       ▼
Fresh start: New initial context emphasizing a different approach
             + All previous attempts logged for reference
```

---

## Level 4: Human Escalation

**Who acts:** A human reviews the blocker and provides resolution.

**When:** All automated approaches have been exhausted.

### Escalation Package

The human receives a complete context package:

```
┌─────────────────────────────────────────────────────┐
│ BLOCKER ESCALATION REPORT                            │
│                                                      │
│ Task: [task description]                             │
│ Priority: [critical/high/normal]                     │
│ Agent: [implementation-agent]                        │
│                                                      │
│ BLOCKER DESCRIPTION:                                 │
│ [What the agent couldn't resolve]                    │
│                                                      │
│ ATTEMPTED RESOLUTIONS:                               │
│                                                      │
│ Level 1 - Autonomous Paging:                         │
│   • Searched: "API 403 auth error" → 0 relevant     │
│   • Searched: "authentication config" → partial     │
│   • Retrieved: mem_456 → Applied, didn't resolve    │
│                                                      │
│ Level 2 - Extended Paging (5,000 tokens granted):   │
│   • Searched broader: "API permission" → 2 results  │
│   • Retrieved: mem_789, mem_801 → Neither resolved  │
│   • Recursive: mem_789 → mem_800 → dead end         │
│                                                      │
│ Level 3 - Orchestrator Intervention:                 │
│   • Attempted decomposition → Partial success        │
│   • Attempted skill swap → Same blocker encountered  │
│                                                      │
│ TOTAL RESOURCES SPENT:                               │
│   Execution: 45,000 tokens                           │
│   Paging: 12,000 tokens                              │
│   Review: 0 (never reached)                          │
│                                                      │
│ SUGGESTED OPTIONS:                                   │
│   1. Provide Stripe API credentials                  │
│   2. Switch to mock payment provider                 │
│   3. Defer payment integration to next sprint        │
│                                                      │
│ CONTEXT SNAPSHOT:                                    │
│ [Full working memory at time of blocker]             │
│ [Relevant file contents]                             │
│ [Error messages and logs]                            │
└─────────────────────────────────────────────────────┘
```

### Human Response Options

The human can:

1. **Provide information** — Supply the missing knowledge directly
2. **Modify requirements** — Change the task to avoid the blocker
3. **Provide access** — Grant credentials, permissions, or resources
4. **Defer** — Mark the task as deferred and unblock dependent tasks
5. **Cancel** — Mark the task as not feasible and clean up

---

## Blocker Record Schema

Every blocker (resolved or not) is recorded in Tier 2 (Structured Storage):

| Field | Description |
|---|---|
| `blockerId` | Unique identifier |
| `taskId` | Which task encountered the blocker |
| `description` | What the obstacle is |
| `category` | Type: `missing_knowledge`, `access_denied`, `dependency`, `ambiguity`, `technical` |
| `escalationLevel` | Maximum level reached (1–4) |
| `resolutionStatus` | `open`, `resolved_autonomous`, `resolved_orchestrator`, `resolved_human`, `deferred`, `cancelled` |
| `resolution` | How it was eventually resolved |
| `pagingHistory` | All paging operations attempted |
| `tokensSpent` | Total tokens spent on resolution attempts |
| `timeToResolve` | Duration from detection to resolution |
| `extractedHeuristic` | Any pattern or rule extracted for future use |

---

## Heuristic Extraction

When a blocker is resolved, the system extracts reusable knowledge:

```
Blocker: "JWT invalid signature after key rotation"
Resolution: "Updated JWT_SECRET environment variable"
Escalation: Level 1 (autonomous)

Heuristic extracted:
  "After any key rotation, verify all services have the
   updated secret. Common symptom: 'invalid signature'
   errors. Check environment variables first."
  
Stored in: Tier 3 (Deep Archive) as domain heuristic
Tagged: ["jwt", "key-rotation", "environment-variables", "auth"]
```

These heuristics are ranked highly in future `page_search` results, making the system progressively better at autonomous resolution.

---

## Metrics

| Metric | Target | Description |
|---|---|---|
| Level 1 Resolution Rate | > 60% | Blockers resolved by autonomous paging |
| Level 2 Resolution Rate | > 20% | Blockers resolved with extended paging |
| Level 3 Resolution Rate | > 10% | Blockers resolved by orchestrator intervention |
| Human Escalation Rate | < 10% | Blockers requiring human involvement |
| Mean Time to Resolution (L1) | < 30s | How quickly autonomous resolution works |
| Heuristic Reuse Rate | > 40% | How often extracted heuristics help future tasks |

---

## Next Steps

- [Execution Workflow](README.md) — Where blocker handling fits in the task lifecycle
- [Paging System](../memory/PAGING.md) — The tools used in Level 1 and 2 resolution
- [Memory Tiers](../memory/TIERS.md) — Where blocker records and heuristics are stored
