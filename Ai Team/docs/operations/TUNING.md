# Budget Tuning

How to adjust context budget parameters for your specific environment and workload.

---

## Understanding Your Context Window

The total context window size determines everything else. Know your limit:

| Model | Context Window | Recommended System Reserve |
|---|---|---|
| GPT-4 Turbo | 128K tokens | 12.8K |
| GPT-4o | 128K tokens | 12.8K |
| Claude 3.5 Sonnet | 200K tokens | 20K |
| Claude 3 Opus | 200K tokens | 20K |
| Local (7B) | 8K–32K tokens | Proportional |
| Local (70B) | 32K–128K tokens | Proportional |

**Rule of thumb:** Smaller context windows need more aggressive compression and stricter budgets.

---

## Tuning Knobs

### Overflow Thresholds

Control when the system starts reacting to budget pressure:

```yaml
context_budget:
  overflow:
    soft_warning: 0.80    # Default: 80%
    hard_limit: 1.00      # Default: 100%
    emergency: 1.10       # Default: 110%
```

| Scenario | Recommended Settings |
|---|---|
| Large context (128K+), reliable model | `soft: 0.85`, `hard: 1.0`, `emergency: 1.15` |
| Medium context (32-128K), standard tasks | `soft: 0.80`, `hard: 1.0`, `emergency: 1.10` (defaults) |
| Small context (<32K), complex tasks | `soft: 0.70`, `hard: 0.95`, `emergency: 1.05` |
| Cost-sensitive deployment | `soft: 0.75`, `hard: 0.90`, `emergency: 1.00` |

**Why adjust:** Lower soft warnings give agents more time to compress before hitting hard limits. Lower emergency thresholds provide a larger safety margin.

### Compression Strategy Order

Control which compression strategies are tried first:

```yaml
context_budget:
  overflow:
    compression_order:
      - summarize       # Try this first
      - tier_demote     # Then this
      - truncate        # Last resort
```

| Scenario | Recommended Order |
|---|---|
| Tasks with verbose conversation history | `summarize`, `tier_demote`, `truncate` (default) |
| Tasks with lots of retrieved documentation | `tier_demote`, `summarize`, `truncate` |
| Short tasks where history isn't valuable | `truncate`, `tier_demote`, `summarize` |

### Paging Quota

What fraction of the Active Task Budget is available for paging:

```yaml
paging:
  default_quota: 0.30    # Default: 30%
```

| Scenario | Recommended Quota |
|---|---|
| New project with little history | 0.10 — Less history to retrieve |
| Mature project with rich history | 0.35 — More value in retrieval |
| Research/exploratory tasks | 0.40 — Heavy reliance on past patterns |
| Simple, well-defined tasks | 0.15 — Minimal retrieval needed |

### Truncation Priority Thresholds

When to truncate content at each priority level:

```yaml
context_budget:
  overflow:
    truncation_priority:
      critical: never      # Never truncate task instructions
      high: 0.80           # Truncate at 80% of allocation
      medium: 0.60         # Truncate at 60%
      low: 0.40            # Truncate at 40%
```

Tighten these (lower values) for smaller context windows.

---

## Tuning by Task Type

### Bug Fix Tasks

**Profile:** Simple (70% execution, 10% orchestrator, 10% review)

**Budget emphasis:** Maximize execution budget. Bug fixes are usually direct — the agent needs space to read code, identify the issue, and apply the fix.

**Paging tuning:** Lower quota (15%). Bug fixes rarely need deep retrieval.

```yaml
context_budget:
  strategy: "fixed"
  default_profile: "simple"
paging:
  default_quota: 0.15
```

### Feature Implementation

**Profile:** Complex (50% execution, 25% orchestrator, 15% review)

**Budget emphasis:** Balanced. The orchestrator needs room for decomposition. The review phase needs room for thorough validation.

**Paging tuning:** Standard quota (30%). Features often reference past patterns and decisions.

```yaml
context_budget:
  strategy: "dynamic"
  default_profile: "balanced"
paging:
  default_quota: 0.30
```

### Architecture / Refactoring

**Profile:** Research (40% execution, 30% orchestrator, 20% review)

**Budget emphasis:** Heavy orchestrator allocation for planning. Generous review for validating architectural changes.

**Paging tuning:** High quota (40%). Architecture work benefits greatly from past decisions and patterns.

```yaml
context_budget:
  strategy: "dynamic"
  default_profile: "research"
paging:
  default_quota: 0.40
```

---

## Diagnosing Budget Issues

### Symptom: Frequent Soft Warnings

**Indicates:** Agents consistently using near-maximum budget.

**Fix:**
1. Check if tasks are genuinely complex or if prompts are too verbose
2. Increase context window if possible
3. Lower soft warning threshold to give more reaction time
4. Increase paging quota so agents offload more content

### Symptom: Regular Emergency Triggers

**Indicates:** Tasks routinely exceed their allocation.

**Fix:**
1. Switch to a more aggressive allocation profile
2. Decompose tasks into smaller units at intake
3. Increase the emergency threshold slightly
4. Review skill requirements — may need skills with lower minimum budgets

### Symptom: Low Paging Hit Rate (<40%)

**Indicates:** Agents are searching for context that doesn't exist or is poorly indexed.

**Fix:**
1. Check if the Deep Archive has relevant content
2. Improve embedding quality (use a better embedding model)
3. Adjust relevance threshold (lower it if too strict)
4. Add more tags and metadata to stored memories

### Symptom: Paging Quota Frequently Exhausted

**Indicates:** Agents need more retrieval capacity than allocated.

**Fix:**
1. Increase the paging quota percentage
2. Pre-fetch more context during session hydration (reduce the need for runtime paging)
3. Improve Working Memory hydration to include commonly needed content

---

## Tuning Workflow

1. **Start with defaults** for your profile (simple/balanced/research)
2. **Run 10–20 representative tasks** through the system
3. **Review metrics** in the dashboard:
   - Budget violation rate
   - Emergency trigger frequency
   - Paging hit rate and quota utilization
4. **Adjust one parameter at a time** based on observed issues
5. **Re-run and compare** metrics against previous configuration
6. **Lock in** when metrics are within target ranges

---

## Next Steps

- [Overflow Handling](../context-budgeting/OVERFLOW.md) — What happens when budgets are exceeded
- [Safety Mechanisms](SAFETY.md) — System protections against resource issues
- [Success Metrics](../metrics/README.md) — Target values for budget metrics
