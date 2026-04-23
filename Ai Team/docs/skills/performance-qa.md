# Performance QA Specialist — Best Practices

## Overview

The Performance QA Specialist designs and executes load tests, identifies performance bottlenecks, defines and validates SLA targets, and provides optimization guidance across the stack. It is activated when tasks explicitly mention performance, or when a feature involves high-volume data operations, real-time requirements, or public-facing endpoints.

**Skill ID:** `performance-qa`
**Activated by:** Explicit assignment or tasks mentioning "performance", "load test", "SLA", "throughput", "latency", "scalability", or "optimization"
**Coordinates with:** All implementation specialists, System Architect, Data Architect

---

## Performance Testing Types

| Type | Purpose | Load Pattern | Duration |
|---|---|---|---|
| **Smoke** | Verify system runs under minimal load | 1–5 VUs, baseline traffic | 1–2 min |
| **Load** | Validate performance at expected production load | Ramp to expected peak, hold, ramp down | 15–30 min |
| **Stress** | Find the breaking point; observe failure behavior | Ramp beyond expected peak | Until failure or 60 min |
| **Soak / Endurance** | Detect memory leaks and degradation over time | Sustained expected load | 4–24 hours |
| **Spike** | Validate behavior under sudden traffic surges | Instant ramp to 10× normal, then drop | 10–15 min |
| **Breakpoint** | Find exact saturation point | Slow ramp until failure | Until failure |

---

## SLA Targets (Default Baselines)

These are default targets. Override per feature based on business requirements.

| Metric | Target | Critical Threshold |
|---|---|---|
| API response time P50 | < 200 ms | < 500 ms |
| API response time P95 | < 500 ms | < 1000 ms |
| API response time P99 | < 1000 ms | < 2000 ms |
| Error rate under load | < 0.1% | < 1% |
| Throughput | As defined per endpoint | — |
| DB query time P95 | < 50 ms | < 200 ms |
| Page load (FCP) | < 1.5 s | < 3 s |
| Core Web Vitals LCP | < 2.5 s | < 4 s |

---

## Load Testing with k6

k6 is the recommended tool for API-level performance testing.

### Smoke Test Script

```javascript
// smoke.js — verify system handles minimal load
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 5,
  duration: '1m',
  thresholds: {
    http_req_duration: ['p(95)<500'],
    http_req_failed: ['rate<0.01'],
  },
};

export default function () {
  const res = http.get('https://api.example.com/health', {
    headers: { Authorization: `Bearer ${__ENV.API_TOKEN}` },
  });
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
  sleep(1);
}
```

### Load Test Script

```javascript
// load.js — ramp to expected production traffic
export const options = {
  stages: [
    { duration: '2m', target: 50 },   // Ramp up
    { duration: '10m', target: 50 },  // Hold at peak
    { duration: '2m', target: 0 },    // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(50)<200', 'p(95)<500', 'p(99)<1000'],
    http_req_failed: ['rate<0.001'],
  },
};
```

### Stress Test Script

```javascript
// stress.js — find the breaking point
export const options = {
  stages: [
    { duration: '2m', target: 50 },
    { duration: '5m', target: 100 },
    { duration: '5m', target: 200 },
    { duration: '5m', target: 400 },
    { duration: '5m', target: 0 },
  ],
};
```

---

## Profiling by Stack

### .NET Core

1. Use **dotnet-trace** and **dotnet-counters** for live profiling in staging:
   ```bash
   dotnet-trace collect --process-id <pid> --duration 00:00:30
   dotnet-counters monitor --process-id <pid>
   ```
2. Use **Visual Studio Profiler** or **JetBrains dotTrace** for detailed flame graphs.
3. Watch for: excessive allocations (GC pressure), blocking async calls (`Result` / `.Wait()`), inefficient LINQ-to-SQL queries (check EF Core SQL logs).

### Node.js / NestJS

1. Use `node --prof` and `node --prof-process` for CPU profiling.
2. Use **clinic.js** (`clinic doctor`, `clinic flame`) for comprehensive profiling.
3. Watch for: event loop blocking (synchronous operations on the main thread), memory leaks (process memory growing in soak test), N+1 queries.

### Frontend

1. Use **Lighthouse** in CI (via `lighthouse-ci`) to track Core Web Vitals per deployment.
2. Use **Chrome DevTools Performance panel** for runtime profiling.
3. Watch for: render blocking resources, large JavaScript bundles, layout shifts (CLS), missing `React.memo` / `v-once` for expensive components.

---

## Database Performance

- Run **EXPLAIN ANALYZE** (PostgreSQL) or **Execution Plan** (SQL Server) for all queries in the hot path.
- Any query with a Seq Scan on a large table is a candidate for indexing.
- Target: no query in a hot path should exceed 50 ms at P95 under expected load.

```sql
-- PostgreSQL: check query performance
EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT)
SELECT a.*, p.FullName
FROM Appointments a
JOIN Patients p ON a.PatientId = p.Id
WHERE a.ClinicianId = 'c1'
  AND a.AppointmentDate >= CURRENT_DATE
ORDER BY a.AppointmentDate;
```

```sql
-- SQL Server: check missing indexes from DMVs
SELECT TOP 10
    migs.avg_total_user_cost * migs.avg_user_impact * (migs.user_seeks + migs.user_scans) AS improvement_measure,
    mid.statement AS table_name,
    mid.equality_columns, mid.inequality_columns, mid.included_columns
FROM sys.dm_db_missing_index_group_stats migs
JOIN sys.dm_db_missing_index_groups mig ON migs.group_handle = mig.index_group_handle
JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
ORDER BY improvement_measure DESC;
```

---

## Caching Strategy

| Cache Location | Use Case | Tool | TTL Guidance |
|---|---|---|---|
| CDN | Static assets, public API responses | Cloudflare, Azure CDN | 1 day – 1 year |
| API Gateway | Repeated identical requests | Kong, APIM | 30 s – 5 min |
| Application (distributed) | Reference data, session state | Redis | 5 min – 1 hour |
| Application (in-process) | Very hot, rarely changed data | MemoryCache | 1–5 min |
| Database | Query result cache | pg_prewarm, buffer pool | Managed by DB |

Cache invalidation rules:
- Write-through: update cache on every write.
- Cache-aside: read from cache; on miss, read from DB and populate cache.
- Always set a TTL — never cache indefinitely.
- Document every caching decision: what is cached, why, and the TTL.

---

## Performance Budgets

Define performance budgets per feature and enforce in CI:

```yaml
# lighthouserc.yml
ci:
  collect:
    url:
      - http://localhost:3000/
      - http://localhost:3000/appointments
  assert:
    preset: lighthouse:recommended
    assertions:
      first-contentful-paint:
        - warn
        - maxNumericValue: 1500
      largest-contentful-paint:
        - error
        - maxNumericValue: 2500
      total-blocking-time:
        - error
        - maxNumericValue: 300
      cumulative-layout-shift:
        - error
        - maxNumericValue: 0.1
```

---

## Anti-Patterns

| Anti-Pattern | Impact | Correct Approach |
|---|---|---|
| N+1 query problem | DB queries grow with data set size | Eager loading / JOIN / DataLoader |
| Synchronous calls in hot path | Blocks threads, crushes throughput | Async/await throughout |
| No connection pooling | Connection exhaustion under load | Pooled connections (Prisma, EF Core, pg-pool) |
| Fetching entire table | Memory exhaustion, slow queries | Pagination (`LIMIT`/`OFFSET` or keyset) |
| No caching for reference data | Unnecessary DB load | Cache static/reference data |
| Loading test with production data but tiny dataset | Misleading results | Load test on production-scale data volumes |
| Ignoring P99 latency | Tail latency causes user experience problems | Track and enforce P99 thresholds |
| Missing indexes on FK columns | Slow joins at scale | Index all FK columns by default |

---

## CI/CD Performance Gate

```yaml
# In CI pipeline: fail if performance regresses
performance_gate:
  - step: lighthouse_ci
    command: lhci autorun
    fail_on_assertion_error: true
  - step: k6_smoke
    command: k6 run tests/performance/smoke.js
    fail_on_threshold_breach: true
  - step: k6_load  # Run on staging only, not per-PR
    environment: staging
    command: k6 run tests/performance/load.js
    fail_on_threshold_breach: true
```

---

## Scrum Team Collaboration

### When Performance Tasks Are in the Sprint

Performance QA runs in **two modes**:

1. **Continuous**: Lighthouse CI and smoke tests run on every PR as a quality gate.
2. **Sprint verification**: full load test runs in staging at the end of the sprint, before the feature is marked complete.

### Handoff Output

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "k6_report"
      description: "Load test results"
      path: "reports/performance/load-test-{date}.html"
    - type: "lighthouse_report"
      description: "Lighthouse CI report"
  metrics:
    p50_ms: 145
    p95_ms: 380
    p99_ms: 820
    error_rate: "0.03%"
    throughput_rps: 420
  sla_status: "PASS"
  verdict: "PASS"
  notes: "All SLA targets met. DB query P95 at 42ms (target: 50ms). Recommend caching clinician list endpoint."
  handoff_to: ["scrum-master"]
```

### Bottleneck Communication

When a bottleneck is found, Performance QA creates a structured finding:

```yaml
performance_finding:
  severity: "HIGH"     # CRITICAL | HIGH | MEDIUM | LOW
  category: "database" # database | api | frontend | network | memory
  description: "N+1 query on GET /api/appointments: each row triggers a separate patient lookup"
  measurement: "1000 appointment page: 1001 queries, P95=4200ms (target: 500ms)"
  recommendation: "Add .Include(a => a.Patient) in AppointmentRepository.GetByClinicianAsync"
  assigned_to: "dotnet-specialist"
  blocking: true
```

---

## Verification Checklist

- [ ] SLA targets defined for all new endpoints
- [ ] k6 smoke test covers all new endpoints
- [ ] Smoke test passes with all thresholds met
- [ ] Load test run on staging with production-scale data
- [ ] No N+1 queries in hot paths (verified via DB query logs)
- [ ] EXPLAIN ANALYZE run on all new DB queries
- [ ] Caching strategy documented for reference data
- [ ] Lighthouse CI passes performance budget assertions
- [ ] P95 and P99 latency within SLA targets
- [ ] Performance findings communicated to relevant specialists
