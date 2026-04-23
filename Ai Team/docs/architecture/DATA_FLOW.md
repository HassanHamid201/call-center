# Data Flow

How information moves through the system, from task submission to completion.

---

## End-to-End Flow

The complete lifecycle of a task through the system.

```
┌──────────┐     ┌─────────────┐     ┌──────────────┐     ┌───────────────┐
│  Client   │────▶│ Orchestrator │────▶│   Context    │────▶│     Skill     │
│  Request  │     │   Planning   │     │   Budgeter   │     │   Registry    │
└──────────┘     └─────────────┘     └──────────────┘     └───────────────┘
                                                                  │
       ┌──────────────────────────────────────────────────────────┘
       │
       ▼
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  Execution   │────▶│  Budget      │────▶│    Review    │────▶│  Resolution  │
│  Engine      │     │  Compliance  │     │    Agent     │     │  & Memory    │
│  (Agent Run) │     │  Check       │     │              │     │  Update      │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
```

---

## Phase Details

### Phase 1: Intake & Budgeting

**What happens:** A task arrives and the system prepares to execute it.

```
Client submits task prompt
         │
         ▼
┌─────────────────────────────────┐
│ Orchestrator: Planning Engine   │
│                                 │
│ 1. Parse intent from prompt     │
│ 2. Extract requirements         │
│ 3. Score complexity (1-10)      │
│ 4. Load minimal project context │
│    from Memory Manager          │
│                                 │
│ Output: Structured Task Object  │
└───────────────┬─────────────────┘
                │
                ▼
┌─────────────────────────────────┐
│ Context Budgeter                │
│                                 │
│ 1. Receive complexity score     │
│ 2. Select allocation strategy:  │
│    - Simple: 70/10/10           │
│    - Complex: 50/25/15          │
│    - Research: 40/30/20         │
│ 3. Calculate token allocations  │
│ 4. Reserve emergency 10%        │
│                                 │
│ Output: Budget Allocation Plan  │
└───────────────┬─────────────────┘
                │
                ▼
         Task Object + Budget Plan
```

**Data produced:**
- Structured Task Object (requirements, complexity score, strategy)
- Budget Allocation Plan (tokens per phase, paging quotas)

---

### Phase 2: Skill Selection & Configuration

**What happens:** The system matches task requirements to available skills and prepares agent configurations.

```
Task Object + Budget Plan
         │
         ▼
┌─────────────────────────────────┐
│ Skill Registry                  │
│                                 │
│ 1. Match task requirements      │
│    to skill capabilities        │
│ 2. Load skill manifests         │
│ 3. Negotiate budget:            │
│    - Skill wants X tokens       │
│    - Budget allows Y tokens     │
│    - Adjust or select alt skill │
│ 4. Assemble specialty prompts   │
│ 5. Configure paging permissions │
│                                 │
│ Output: Agent Config Package    │
└───────────────┬─────────────────┘
                │
                ▼
┌─────────────────────────────────┐
│ Memory Manager: Session Hydrate │
│                                 │
│ 1. Load relevant Working Memory │
│ 2. Pre-fetch likely-needed      │
│    context from Tier 2/3        │
│ 3. Initialize paging toolkit    │
│                                 │
│ Output: Initial context + tools │
└─────────────────────────────────┘
```

**Data produced:**
- Agent Configuration Package (prompts, skills, constraints)
- Paging Toolkit (configured with quota and permissions)
- Initial Working Memory contents

---

### Phase 3: Execution with Paging

**What happens:** The Specialist Agent performs its work, using paging tools when context is insufficient.

```
Agent Config + Task + Paging Tools
         │
         ▼
┌─────────────────────────────────┐
│ Specialist Agent              │
│                                 │
│ 1. Execute task steps           │
│ 2. When blocked or uncertain:   │
│    ┌────────────────────────┐   │
│    │ page_search(query)     │   │
│    │ → metadata results     │   │
│    └───────────┬────────────┘   │
│                │                │
│    ┌───────────▼────────────┐   │
│    │ page_retrieve(ids)     │   │
│    │ → full content loaded  │   │
│    │ → budget deducted      │   │
│    └───────────┬────────────┘   │
│                │                │
│ 3. Apply retrieved knowledge    │
│ 4. Continue execution           │
│ 5. Store findings for future:   │
│    ┌────────────────────────┐   │
│    │ page_store(content,    │   │
│    │   tier=WARM)           │   │
│    │ → budget freed         │   │
│    └────────────────────────┘   │
│                                 │
│ Output: Raw Implementation +    │
│         Test Results +          │
│         Paging Log              │
└─────────────────────────────────┘
```

**Data produced:**
- Implementation artifacts (code, files, changes)
- Test results (pass/fail, coverage)
- Paging operation log (queries, retrievals, costs)

---

### Phase 4: Budget Compliance Check

**What happens:** The system verifies that execution stayed within budget before proceeding to review.

```
Execution Results + Original Budget
         │
         ▼
┌─────────────────────────────────┐
│ Budget Compliance Check         │
│                                 │
│ 1. Calculate total tokens used  │
│ 2. Compare against allocation   │
│ 3. Check paging quota:          │
│    - Was quota exceeded?        │
│    - Were there compression     │
│      events?                    │
│ 4. Assess paging efficiency:    │
│    - Hit rate (useful results / │
│      total queries)             │
│    - Cost ratio (tokens spent   │
│      vs. value gained)          │
│                                 │
│ If PASS: Proceed to review      │
│ If FAIL: Log violation,         │
│          adjust review budget,  │
│          flag for post-hoc      │
│          analysis               │
└─────────────────────────────────┘
```

**Data produced:**
- Budget Status Report (utilization, violations, efficiency metrics)

---

### Phase 5: Validation (Review)

**What happens:** A Review Agent validates the implementation against requirements.

```
Implementation + Requirements + Review Budget
         │
         ▼
┌─────────────────────────────────┐
│ Review Agent                    │
│                                 │
│ 1. Load quality standards via   │
│    paging (if not in context)   │
│ 2. Compare implementation to    │
│    requirements                 │
│ 3. Multi-dimensional analysis:  │
│    - Correctness                │
│    - Completeness               │
│    - Code quality               │
│    - Test coverage              │
│    - Standards compliance       │
│ 4. Generate review report       │
│                                 │
│ Output: Review Report           │
│   - PASS with notes             │
│   - FAIL with specific feedback │
│   - BLOCKED (needs escalation)  │
└─────────────────────────────────┘
```

**Data produced:**
- Review Report (verdict, feedback, specific issues)

---

### Phase 6: Resolution & Memory Update

**What happens:** The system acts on the review result and updates all memory tiers.

```
Review Report + Paging History
         │
    ┌────┴──────────────┬─────────────────┐
    │                   │                 │
    ▼ PASS              ▼ FAIL            ▼ BLOCKED
┌──────────┐   ┌──────────────┐   ┌──────────────────┐
│ Store    │   │ Generate     │   │ Create blocker   │
│ artifacts│   │ delta-prompt │   │ record in Tier 2 │
│          │   │ incorporating│   │                  │
│ Update   │   │ review       │   │ Attach paging    │
│ all tiers│   │ feedback     │   │ history          │
│          │   │              │   │                  │
│ Release  │   │ Preserve     │   │ Escalate to      │
│ task     │   │ paging       │   │ orchestrator     │
│ budget   │   │ insights     │   │ or human         │
└──────────┘   │              │   └──────────────────┘
               │ Loop back to │
               │ Phase 3 with │
               │ refined      │
               │ prompt       │
               └──────────────┘
```

**Data produced (PASS):**
- Stored artifacts in workspace
- Updated memory across all tiers
- Released budget returned to pool

**Data produced (FAIL):**
- Delta-prompt incorporating review feedback
- Preserved paging insights for next iteration
- Adjusted budget for retry

**Data produced (BLOCKED):**
- Blocker record with full context
- Paging history for resolver
- Escalation notification

---

## Data That Flows Between Phases

| Data Item | Produced By | Consumed By | Storage Location |
|---|---|---|---|
| Task Object | Planning Engine | Budgeter, Scheduler, Skills | Tier 2 (Structured Storage) |
| Budget Allocation Plan | Context Budgeter | All phases | In-memory (Orchestrator) |
| Agent Config Package | Skill Registry | Execution Engine | In-memory (per session) |
| Paging Log | Agent Workers | Budget Compliance, Resolution | Tier 2 + Tier 3 |
| Implementation Artifacts | Specialist Agent | Review Agent | Workspace (files) |
| Test Results | Testing Agent | Review Agent | Tier 2 |
| Review Report | Review Agent | Resolution phase | Tier 2 |
| Memory Fragments | All agents (via `page_store`) | Future sessions via paging | Tier 2 or Tier 3 |

---

## Next Steps

- [Execution Workflow](../workflow/README.md) — The complete task lifecycle as an operational guide
- [Blocker Handling](../workflow/BLOCKER-HANDLING.md) — Detailed blocker resolution protocol
- [Memory & Paging](../memory/README.md) — How memory tiers support this flow
