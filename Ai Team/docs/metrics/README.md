# Success Metrics & SLAs

Measurable targets for system quality, efficiency, and performance.

---

## Metric Categories

| Category | What It Measures | Why It Matters |
|---|---|---|
| **Stability** | Does the system stay within resource limits? | Prevents crashes, cost overruns, and unpredictable behavior |
| **Efficiency** | Is the system making good use of its resources? | Lower cost, faster completion, better retrieval |
| **Performance** | How fast does the system operate? | User experience, task throughput |
| **Quality** | Are the outputs good enough? | Autonomous completion without human correction |

---

## Stability Metrics

### Budget Violation Rate

**Definition:** Percentage of tasks where any component exceeds its allocated context budget.

**Measurement:**
```
budget_violation_rate = tasks_with_violations / total_tasks * 100
```

| Rating | Range |
|---|---|
| Excellent | < 0.5% |
| Good | 0.5% – 1% |
| Acceptable | 1% – 3% |
| Needs Attention | > 3% |

**Target: < 1%**

**What to do if missing:**
- Review budget allocation profiles (see [Budget Tuning](../operations/TUNING.md))
- Check if tasks are being under-scored on complexity
- Verify that compression strategies are working effectively

### Emergency Mode Frequency

**Definition:** How often the emergency protocol activates (Phase 3 overflow).

**Measurement:**
```
emergency_frequency = emergency_activations / total_tasks * 100
```

| Rating | Range |
|---|---|
| Excellent | 0% |
| Good | < 0.1% |
| Acceptable | 0.1% – 0.5% |
| Needs Attention | > 0.5% |

**Target: < 0.1%**

**What to do if missing:**
- Emergency triggers indicate systemic under-budgeting
- Increase default allocations or improve complexity scoring
- Check if specific task types consistently trigger emergencies

### Compression Success Rate

**Definition:** Percentage of compression operations that successfully free enough tokens to continue.

**Measurement:**
```
compression_success = successful_compressions / total_compressions * 100
```

**Target: > 90%**

**What to do if missing:**
- Review compression strategy order (summarize before truncate?)
- Check if Working Memory content is too large for effective compression
- Consider increasing context window or decreasing task scope

---

## Efficiency Metrics

### Paging Hit Rate

**Definition:** Percentage of `page_retrieve` operations that return content the agent subsequently finds useful.

**Measurement:**
```
paging_hit_rate = effective_retrievals / total_retrievals * 100
```

| Rating | Range |
|---|---|
| Excellent | > 80% |
| Good | 70% – 80% |
| Acceptable | 50% – 70% |
| Needs Attention | < 50% |

**Target: > 70%**

**What to do if missing:**
- Review embedding quality (use a better embedding model)
- Adjust relevance threshold (may be too low, returning irrelevant results)
- Check if the Deep Archive has sufficient relevant content
- Improve tagging and metadata on stored memories

### Paging Efficiency Ratio

**Definition:** Average tokens of useful content retrieved per token spent on searching.

**Measurement:**
```
paging_efficiency = total_useful_tokens_retrieved / total_search_tokens_spent
```

**Target: > 10:1** (for every token spent searching, retrieve 10+ tokens of useful content)

**What to do if missing:**
- Reduce `resultLimit` to return fewer, more relevant results
- Increase `relevanceThreshold` to filter low-quality matches
- Improve search query formulation in skill auto-paging rules

### Autonomous Resolution Rate

**Definition:** Percentage of blockers resolved without human intervention (Levels 1 and 2).

**Measurement:**
```
autonomous_resolution = (L1_resolved + L2_resolved) / total_blockers * 100
```

**Target: > 80%**

**What to do if missing:**
- Check if the Deep Archive has sufficient historical context
- Review heuristic extraction — are resolved blockers producing good heuristics?
- Evaluate if auto-paging rules are triggering appropriately

### Budget Overhead

**Definition:** Percentage of tokens spent on orchestration (planning, budgeting, scheduling) vs. productive agent work.

**Measurement:**
```
budget_overhead = orchestrator_tokens / (orchestrator_tokens + agent_tokens) * 100
```

**Target: < 15%**

**What to do if missing:**
- Streamline orchestrator prompts
- Reduce unnecessary planning overhead for simple tasks
- Use the "simple" profile for straightforward work

---

## Performance Metrics

### Paging Latency

**Definition:** P95 time from `page_search` invocation to result delivery.

**Measurement:**
```
paging_latency_p95 = 95th percentile of all page_search durations
```

| Tier | Target P95 |
|---|---|
| Tier 2 (Structured Storage) | < 500ms |
| Tier 3 (Deep Archive) | < 2,000ms |

**What to do if missing:**
- Add database indexes for common query patterns
- Pre-compute embeddings at store time (not search time)
- Use SQLite WAL mode for concurrent read performance
- Consider LanceDB for vector operations if sqlite-vec is too slow

### Task Throughput

**Definition:** Average number of tasks completed per hour.

**Measurement:**
```
throughput = completed_tasks / total_elapsed_hours
```

No fixed target — this is heavily dependent on task complexity and model speed. Track it over time to identify regressions.

**What to watch for:**
- Sudden drops may indicate model API issues
- Gradual decreases may indicate growing memory sizes slowing retrieval
- Compare against task complexity mix (complex tasks take longer)

### Context Refresh Rate

**Definition:** How often a successful deep retrieval prevents the need to restart an agent session.

**Measurement:**
```
refresh_rate = sessions_saved_by_paging / sessions_that_hit_blockers * 100
```

**Target: > 60%**

**What to do if missing:**
- Increase paging quota so agents can retrieve more context
- Improve initial Working Memory hydration to prevent blockers
- Review blocker categories to identify systemic knowledge gaps

---

## Quality Metrics

### Task Completion Rate

**Definition:** Percentage of tasks that reach PASS status without human escalation.

**Measurement:**
```
completion_rate = tasks_completed / total_tasks * 100
```

**Target: > 85%**

### First-Pass Success Rate

**Definition:** Percentage of tasks that PASS on the first review (no Ralph Loop retries).

**Measurement:**
```
first_pass_rate = tasks_passed_first_review / total_tasks * 100
```

**Target: > 60%**

**What to do if missing:**
- Review common review failures — are they addressing the same issues?
- Improve Specialist Agent prompts or skills
- Increase execution budget for better-quality first attempts

### Human Escalation Rate

**Definition:** Percentage of tasks that require human intervention (Level 4 blocker).

**Measurement:**
```
escalation_rate = human_escalated_tasks / total_tasks * 100
```

**Target: < 10%**

---

## Reporting

### Dashboard Integration

All metrics are visible in the web dashboard:

| Metric | Dashboard Location |
|---|---|
| Budget violation rate | Budget Monitor → History |
| Emergency frequency | Alerts Panel |
| Paging hit rate | Paging Activity → Summary |
| Task completion rate | Task Pipeline → Overview |
| Human escalation rate | Task Pipeline → Blocked view |

### Metric Collection

Metrics are collected from:

| Source | Metrics |
|---|---|
| Context Budgeter | Budget violations, emergency triggers, compression events |
| Paging Controller | Search/retrieve counts, hit rates, latency |
| Task Scheduler | Completion rates, retry counts, escalation counts |
| Event Log | Throughput, duration distributions |

### Review Cadence

| Metric Type | Review Frequency |
|---|---|
| Stability (budget, emergency) | Every session |
| Efficiency (paging, overhead) | Weekly |
| Performance (latency, throughput) | Weekly |
| Quality (completion, escalation) | Per-project milestone |

---

## SLA Summary

| Metric | Target | Critical Threshold |
|---|---|---|
| Budget violation rate | < 1% | > 5% |
| Emergency frequency | < 0.1% | > 1% |
| Paging hit rate | > 70% | < 40% |
| Paging latency (P95) | < 2s | > 10s |
| Budget overhead | < 15% | > 30% |
| Task completion rate | > 85% | < 60% |
| Human escalation rate | < 10% | > 30% |
| First-pass success rate | > 60% | < 30% |

**Target** = The system is operating as designed.
**Critical Threshold** = The system needs immediate attention.

---

## Next Steps

- [Operations Guide](../operations/README.md) — How to configure and tune the system
- [Budget Tuning](../operations/TUNING.md) — Adjusting parameters to improve metrics
- [Dashboard Specification](../dashboard/README.md) — Where to find these metrics in the UI
