# Verification Engine

The independent verification system that ensures agent output actually meets requirements before tasks are marked complete.

---

## Why Independent Verification

Agents are instructed to write tests and run them. But without independent checking, an agent can:
- Skip writing tests entirely
- Write superficial tests that pass but don't validate behavior
- Mark a task done without running lint or build
- Ignore acceptance criteria

The Verification Engine solves this by running all checks **after** the agent finishes, independently. A task is not complete until verification passes.

---

## Verification Pipeline

```
Agent completes work
        │
        ▼
┌─────────────────────────────────────────────────────┐
│ Phase 1: Acceptance Criteria Check                  │
│                                                     │
│ For each criterion in the task:                     │
│   Automated patterns:                               │
│     "tests pass"      → run commands.test           │
│     "lint passes"     → run commands.lint           │
│     "build succeeds"  → run commands.build          │
│     "coverage > X%"   → parse test output           │
│     "TypeScript"      → run tsc --noEmit            │
│   Everything else:                                  │
│     → escalate to Phase 3 (specialist review)       │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────┐
│ Phase 2: Automated Verification                     │
│                                                     │
│ Run project commands (from config.yaml):            │
│   commands.test  → must exit 0                      │
│   commands.lint  → must exit 0                      │
│   commands.build → must exit 0                      │
│                                                     │
│ Parse output:                                       │
│   Test count (total, passed, failed, skipped)       │
│   Coverage percentage                               │
│   Lint error count                                  │
│   Build success/failure                             │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────┐
│ Phase 3: Specialist Review                          │
│                                                     │
│ If task has a specialist assigned:                  │
│   • Load specialist's review checklist              │
│   • Review agent evaluates:                         │
│     - Correctness against requirements              │
│     - Pattern compliance (specialist standards)     │
│     - Security considerations                       │
│     - Performance implications                      │
│     - Test quality (meaningful vs. superficial)     │
│                                                     │
│ If any acceptance criteria were unautomatable:      │
│   • Review agent evaluates those criteria           │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
                 ┌─────┴─────┐
                 │  VERDICT   │
                 └─────┬─────┘
                       │
          ┌────────────┼────────────┐
          │            │            │
       All pass    Auto fail    Review fail
          │            │            │
          ▼            ▼            ▼
        PASS         FAIL         FAIL
                     (retry with    (retry with
                      command       review feedback)
                      output)
```

---

## Acceptance Criteria Mapping

| Criterion Pattern | Verification Method | Example |
|---|---|---|
| `"tests pass"` | Run `commands.test`, check exit 0 | `npm test` exits 0 |
| `"lint passes"` | Run `commands.lint`, check exit 0 | `npm run lint` exits 0 |
| `"build succeeds"` | Run `commands.build`, check exit 0 | `npm run build` exits 0 |
| `"TypeScript strict"` | Run `npx tsc --noEmit` | No type errors |
| `"coverage > X%"` | Parse test runner output for % | `coverage 85.3%` |
| `"no vulnerabilities"` | Run security audit | `npm audit` exits 0 |
| `"accessible"` | Specialist review | Review agent checks for ARIA, keyboard nav |
| `"secure"` / `"security"` | Specialist review | App security specialist evaluates |
| `"performance"` | Specialist review | Performance QA evaluates |
| Any other criterion | Specialist review | Review agent evaluates against output |

---

## Verification Result

```typescript
interface VerificationResult {
  passed: boolean;
  checks: CheckResult[];          // Per-criterion results
  testSummary: {
    total: number;
    passed: number;
    failed: number;
    skipped: number;
    coverage?: number;
  };
  lintErrors: number;
  buildSuccess: boolean;
  review?: ReviewReport;          // If specialist review was run
}

interface CheckResult {
  criterion: string;
  method: "automated" | "review";
  passed: boolean;
  evidence: string;               // "exit code 0", "coverage 85.3%", review notes
}
```

---

## Failure Handling

When verification fails:

1. **Collect specific feedback** — Which criteria failed, which commands failed, what the output was
2. **Generate delta-prompt** — Only the failed parts, not the entire task
3. **Retry with feedback** — Agent gets the delta-prompt plus verification output
4. **Track attempt count** — Up to `max_retries` (default 3)
5. **Escalate on exhaustion** — If all retries fail, mark as failed with full context for human review

---

## Specialist Review Dimensions

When a specialist review runs, it evaluates:

| Dimension | What It Checks |
|---|---|
| **Correctness** | Does the implementation do what the task requested? |
| **Completeness** | Are all requirements and acceptance criteria satisfied? |
| **Patterns** | Does it follow the specialist's established patterns? |
| **Security** | Any vulnerability concerns for this technology? |
| **Performance** | Any obvious performance issues? |
| **Test Quality** | Are tests meaningful, or just superficially passing? |

Each dimension produces a pass/fail with notes. The overall verdict requires all dimensions to pass.

---

## Next Steps

- [Workflow](../workflow/README.md) — Where verification fits in the task lifecycle
- [Interfaces](../interfaces/README.md) — VerificationResult and related interfaces
- [Skills](../skills/README.md) — Specialist review checklists per skill
