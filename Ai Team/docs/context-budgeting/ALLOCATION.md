# Budget Allocation Strategies

How the Context Budgeter distributes tokens across system components based on task characteristics.

---

## Default Allocation

The baseline distribution for tasks of moderate complexity:

| Phase | Percentage | Description |
|---|---|---|
| System Reserve | 10% | Always reserved; not negotiable |
| Orchestrator | 15% | Planning, strategy, task decomposition |
| Active Task | 60% | Agent execution, working memory, skill docs |
| Review | 15% | Multi-agent validation, quality checks |

---

## Dynamic Allocation Modes

The budgeter adjusts allocations based on a **complexity score** (1–10) assigned by the Planning Engine during intent parsing.

### Simple Tasks (Complexity 1–3)

For straightforward, well-defined tasks with clear success criteria:

| Phase | Percentage | Rationale |
|---|---|---|
| System Reserve | 10% | Fixed |
| Orchestrator | 10% | Minimal planning needed |
| Active Task | 70% | Most work is direct execution |
| Review | 10% | Quick validation suffices |

**Example tasks:** Bug fixes with clear reproduction steps, simple configuration changes, documentation updates.

### Complex Tasks (Complexity 4–7)

For multi-step tasks with dependencies and potential ambiguity:

| Phase | Percentage | Rationale |
|---|---|---|
| System Reserve | 10% | Fixed |
| Orchestrator | 25% | Significant planning and decomposition needed |
| Active Task | 50% | Still the largest allocation but reduced |
| Review | 15% | Thorough validation required |

**Example tasks:** Feature implementation across multiple files, API design with multiple consumers, integration work requiring coordination.

### Research Tasks (Complexity 8–10)

For exploratory tasks requiring extensive analysis and iteration:

| Phase | Percentage | Rationale |
|---|---|---|
| System Reserve | 10% | Fixed |
| Orchestrator | 30% | Heavy planning, multiple strategy evaluations |
| Active Task | 40% | Execution is iterative and uncertain |
| Review | 20% | Results need careful validation against open-ended criteria |

**Example tasks:** Architecture redesign, performance optimization requiring investigation, novel problem-solving without established patterns.

---

## Paging Quota Allocation

Within the Active Task Budget, a portion is reserved for paging operations:

| Setting | Value | Description |
|---|---|---|
| Default Paging Quota | 30% of Active Task Budget | Available for `page_retrieve` operations |
| Minimum Paging Quota | 10% | Even simple tasks need some retrieval capacity |
| Maximum Paging Quota | 50% | Prevents excessive retrieval at the expense of execution |

The orchestrator can adjust these based on:
- **Task history** — Tasks similar to past work need less paging
- **Skill requirements** — Skills declare their paging allowance in their manifests
- **Available Tier 2/3 content** — New projects with little history need less paging capacity

---

## Budget Negotiation with Skills

Skills declare their token requirements in their manifests:

```typescript
budgetRequest: {
  minimum: number;       // Won't function below this
  recommended: number;   // Optimal performance level
  pagingAllowance: number; // % of budget for deep retrieval
}
```

The budgeter resolves conflicts between skill requirements and available allocation:

### Happy Path
Skill's recommended budget fits within the Active Task Budget → grant recommended amount.

### Constrained Path
Skill's recommended budget exceeds available allocation:
1. Grant the minimum viable amount
2. Increase the paging quota to compensate (agent can retrieve more context on demand)
3. Log the constraint for post-hoc analysis

### Incompatible Path
Skill's minimum exceeds the Active Task Budget:
1. Reject the skill for this task
2. Attempt to find an alternative skill with lower requirements
3. If no alternative exists, flag the task as requiring budget reallocation or decomposition

---

## Reallocation Rules

During execution, the budgeter may shift tokens between phases:

| Trigger | Action | Constraint |
|---|---|---|
| Execution completes early | Move surplus to Review phase | Review budget cannot exceed 40% |
| Execution is struggling | Move tokens from Orchestrator reserve | Orchestrator cannot drop below 10% |
| Review phase needs more context | Move from completed Execution | Execution budget already released |
| Emergency declared | Consolidate all non-critical budgets | System Reserve never touched |

All reallocations are:
- Logged with a reason code
- Applied atomically (all-or-nothing)
- Reviewable in the dashboard's budget history view

---

## Allocation Lifecycle

```
Task Arrives
     │
     ▼
Complexity Score Assigned
     │
     ▼
Strategy Selected (Simple / Complex / Research)
     │
     ▼
Percentages Calculated → Absolute Token Counts
     │
     ▼
Hard Limit Check (no single allocation > 70%)
     │
     ▼
Skill Budget Negotiation
     │
     ▼
Paging Quotas Set
     │
     ▼
Budget Allocation Plan Finalized
     │
     ▼
Allocation Active → Monitored → Released
```

---

## Next Steps

- [Overflow Handling](OVERFLOW.md) — What happens when allocations aren't enough
- [Skill Registry](../skills/README.md) — How skills declare their budget requirements
- [Interface Contracts](../interfaces/README.md) — The formal allocation API
