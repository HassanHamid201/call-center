# Context Budgeting

The Context Budgeting subsystem is responsible for ensuring the system never exceeds available token capacity. It allocates budgets, monitors utilization, and manages overflow situations.

---

## In This Section

| Document | Description |
|---|---|
| [Allocation Strategies](ALLOCATION.md) | How tokens are distributed across phases and the strategies that govern distribution |
| [Overflow Handling](OVERFLOW.md) | The multi-phase protocol triggered when budgets are approached or exceeded |

---

## The Problem

LLM context windows are finite. A system coordinating multiple agents, each consuming tokens for prompts, history, and tool results, can easily exceed this limit. When that happens:

- **Quality degrades** — Truncated context leads to confused agents
- **Costs spike** — Retrying failed operations wastes tokens
- **State is lost** — Unsaved work disappears when a session terminates

The Context Budgeter prevents these outcomes by treating tokens as a managed resource, not an unlimited well.

---

## Budget Hierarchy

The total context window is divided into strict allocations:

```
┌─────────────────────────────────────────┐
│         TOTAL CONTEXT WINDOW            │
│         (e.g., 128K tokens)             │
├─────────────────────────────────────────┤
│  SYSTEM RESERVE (10%)                   │
│  • Core orchestrator functions          │
│  • Emergency overflow handling          │
│  • Critical safety prompts              │
├─────────────────────────────────────────┤
│  ORCHESTRATOR BUDGET (15%)              │
│  • Planning and strategy                │
│  • Task decomposition                   │
│  • Budget reallocation decisions        │
├─────────────────────────────────────────┤
│  ACTIVE TASK BUDGET (60%)               │
│  • Primary agent execution context      │
│  • Working memory and immediate history │
│  • Retrieved skill documentation        │
├─────────────────────────────────────────┤
│  REVIEW BUDGET (15%)                    │
│  • Multi-agent review coordination      │
│  • Validation context                   │
│  • Comparison and diff operations       │
└─────────────────────────────────────────┘
```

These percentages are defaults. The system adjusts them based on task complexity.

---

## Key Concepts

### Budget Allocation

A budget allocation is a reserved token quantity for a specific component. Each allocation tracks:

- **Allocated tokens** — The maximum this component can use
- **Utilized tokens** — How many have been consumed so far
- **Paging quota** — A sub-allocation for deep retrieval operations
- **Compression strategies** — Ordered list of approaches to try if the limit is approached

### Hard Limits

Non-negotiable boundaries that preserve system stability:

| Limit | Value | Purpose |
|---|---|---|
| Minimum Viable Context | 20% of window | Essential operations always have room |
| Maximum Single Allocation | 70% of window | No single component can monopolize resources |
| Emergency Reserve | 10% of window | Always available for critical-path preservation |

### Budget Recovery

Tokens aren't just consumed — they're also reclaimed:

- **Task completion** — The Active Task Budget releases immediately to the pool
- **Context pruning** — Superseded or irrelevant information is removed
- **Compression** — Summarization reduces token count while preserving meaning
- **Early termination** — When tests pass clearly, review cycles can be shortened

---

## How It Works

### 1. Initial Allocation

When a task arrives, the Context Budgeter:
1. Receives the complexity score from the Planning Engine
2. Selects an allocation strategy (see [Allocation Strategies](ALLOCATION.md))
3. Calculates token amounts for each phase
4. Reserves the emergency 10%
5. Sets paging quotas within each allocation
6. Returns the Budget Allocation Plan

### 2. Ongoing Monitoring

During execution, the Budgeter:
1. Receives token consumption reports from the Execution Engine
2. Tracks utilization as a percentage of each allocation
3. Compares against warning thresholds (80% soft, 100% hard, 110% emergency)
4. Triggers the Overflow Handler when thresholds are crossed

### 3. Reallocation

The Budgeter can shift tokens between phases:
- If the execution phase finishes early, surplus tokens move to the review phase
- If a phase is struggling, the Budgeter evaluates whether to extend or compress
- All reallocations are logged for post-hoc analysis

### 4. Release

When a task completes:
1. All phase allocations are released
2. Unconsumed tokens return to the total pool
3. Memory updates from the task are stored in the appropriate tiers
4. Budget utilization metrics are recorded for analysis

---

## Interface Summary

The Context Budgeter exposes these operations:

| Operation | Purpose |
|---|---|
| `requestAllocation` | Request tokens for a component |
| `releaseAllocation` | Return tokens to the pool |
| `triggerCompression` | Manually initiate compression on a component |
| `enterEmergencyMode` | Activate emergency protocol system-wide |

For the full interface definition, see [Interface Contracts](../interfaces/README.md#context-budget-interface).

---

## Next Steps

- [Allocation Strategies](ALLOCATION.md) — How the budgeter decides how to distribute tokens
- [Overflow Handling](OVERFLOW.md) — What happens when things get tight
- [Memory & Paging](../memory/README.md) — How paging interacts with budgets
