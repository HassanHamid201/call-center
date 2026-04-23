# System Architect — Best Practices

## Overview

The System Architect designs the overall structure of software systems — how services decompose, communicate, scale, and fail. It produces Architecture Decision Records (ADRs), service contracts, component diagrams, and technology choices that all other specialists implement.

**Skill ID:** `system-architect`
**Activated by:** Explicit assignment or tasks mentioning "architecture", "design", "service boundaries", "system design", or "technical decision"
**Works alongside:** All specialists (provides design context); reviewed by `app-security`

---

## Core Domains

| Domain | Focus |
|---|---|
| Microservices | Service decomposition, bounded contexts, API Gateway |
| Event-driven | Async messaging, event sourcing, eventual consistency |
| API Design | REST, GraphQL, gRPC contracts |
| Distributed Systems | CAP theorem, consensus, distributed transactions |
| Scalability | Horizontal/vertical scaling, caching, load balancing |
| Resilience | Circuit breakers, retries, bulkheads, timeouts |
| Data Architecture | CQRS, polyglot persistence, data ownership |

---

## Best Practices

### Service Decomposition

- Decompose along **bounded contexts** (Domain-Driven Design). Each service owns its domain — it is the single source of truth for that domain's data.
- Apply the **Single Responsibility Principle at the service level**: a service should have one primary business capability.
- Services should be **independently deployable**. If deploying service A requires deploying service B simultaneously, they are too coupled.
- Keep teams and services aligned: **Conway's Law** means your architecture will mirror your team structure. Design service boundaries to match team ownership.
- Prefer **fewer, larger services** when starting. Extract microservices only when a clear scaling or team boundary emerges — not preemptively.

### API Design

- Use **RESTful resource-oriented design**: `GET /patients`, `POST /patients`, `GET /patients/{id}`, `PUT /patients/{id}`, `DELETE /patients/{id}`.
- Return consistent **error response shapes**: `{ "type": "...", "title": "...", "status": 422, "errors": [...] }` (RFC 9457 Problem Details).
- Version APIs from day one: `/api/v1/patients`. Never break a versioned contract; release a new version instead.
- Use **OpenAPI 3.x** to define contracts. Generate server stubs and client SDKs from the spec — never hand-write both sides.
- For complex queries and flexible client requirements, consider **GraphQL** — but be aware of N+1 query risks and security (query depth limiting, complexity analysis).

### CQRS & Event Sourcing

- Apply **CQRS** (Command Query Responsibility Segregation) when read and write models diverge significantly in shape or scale requirements.
- Separate the command (write) and query (read) databases — read replicas, materialized views, or entirely different stores (e.g., Elasticsearch for search).
- Apply **Event Sourcing** only when you need a full audit trail, temporal queries, or replay capability. It adds significant complexity — don't use it by default.
- When using events, define **explicit event schemas** (Avro, Protobuf, or JSON Schema). Versioning events is harder than versioning APIs — design carefully.

### Domain-Driven Design (DDD)

- Define **Aggregates**: cluster of entities and value objects that must be consistent together. The Aggregate Root is the only entry point.
- Aggregates should be small. Large aggregates create transaction bottlenecks. Split by invariant boundary.
- Use **Domain Events** to communicate state changes across aggregate boundaries (within the same bounded context).
- Use **Integration Events** to communicate state changes across service boundaries. Publish to a message broker (RabbitMQ, Kafka, Azure Service Bus).

### Resilience Patterns

| Pattern | When to Use |
|---|---|
| **Circuit Breaker** | Calls to external services that may fail. Prevents cascade failures. |
| **Retry with Exponential Backoff** | Transient failures (network blip, rate limit). With jitter to prevent thundering herd. |
| **Timeout** | Every external call must have a timeout. No hanging requests. |
| **Bulkhead** | Isolate thread pools / connection pools per external dependency. |
| **Fallback** | Degrade gracefully when a dependency is unavailable (cache, defaults, empty state). |
| **Idempotency Keys** | All non-idempotent operations (payment, order creation) should accept idempotency keys. |

### Distributed Transactions & Consistency

- Avoid distributed transactions (2PC). They are slow, fragile, and cause availability problems.
- Use the **Saga pattern** for multi-service business transactions: choreography (events trigger next step) or orchestration (a saga coordinator).
- Design for **eventual consistency** in distributed systems. Accept that different services may briefly see stale data.
- Use **outbox pattern** to ensure reliable event publishing: write to a local `outbox` table in the same DB transaction as the business write; a relay process publishes outbox events to the broker.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Distributed monolith | Microservice deployment with monolith coupling | Enforce service boundaries via API contracts only |
| Shared database across services | Coupling defeats service independence | Each service owns its own schema |
| Synchronous chaining (A calls B calls C) | Cascade failures, high latency | Async events or choreography |
| Over-engineering for day-1 scale | Premature complexity, slower delivery | Start simple; extract when needed |
| Anemic domain model | No business logic in entities, all in services | Rich domain model with methods on entities |
| Ignoring CAP theorem | Unexpected consistency failures | Explicitly decide: availability vs. consistency |
| No API versioning | Breaking changes break consumers | Version APIs from day one |

---

## Security

- Apply **zero-trust networking**: services authenticate each other (mTLS or service tokens) — never trust the internal network.
- Use an **API Gateway** as the single entry point: authentication, rate limiting, request validation, TLS termination.
- Apply **OAuth 2.0 + OIDC** for user authentication. Issue short-lived JWTs. Use refresh token rotation.
- Design **fine-grained authorization**: resource-level permissions, not just role-level.
- Document the threat model for each service boundary: what can an attacker do if they compromise service X?

---

## Performance

- Use **async messaging** (event queues) to decouple slow operations from request/response paths.
- Apply **caching at multiple layers**: CDN (static assets), API Gateway (response caching), application (Redis), database (query cache).
- Design **read replicas** for read-heavy workloads. Direct writes to primary, reads to replicas.
- Use **connection pooling** at every layer. Services should share pools — not open a connection per request.
- Profile before optimizing. Measure latency and throughput at each service boundary.

---

## Architecture Decision Records (ADRs)

Every significant architectural decision must be documented as an ADR:

```markdown
# ADR-001: Use Event Sourcing for Appointment Scheduling

**Date:** 2026-04-23
**Status:** Accepted
**Context:** The appointment system needs full audit history and the ability to replay past states for compliance.
**Decision:** Use event sourcing for the appointments bounded context. Events stored in EventStoreDB.
**Consequences:**
  + Full audit trail without additional audit tables
  + Temporal queries enabled (as-of queries)
  - Read side requires projection rebuild capability
  - Team must learn event sourcing concepts
**Alternatives considered:**
  - Traditional CRUD with audit table: rejected due to complexity of audit queries
```

---

## Scrum Team Collaboration

### When Architecture Tasks Are in the Sprint

- Architecture tasks should be in `parallel_group: 1` — design decisions are a dependency for all implementation specialists.
- The Handoff Package from the System Architect contains:
  - Service contracts (OpenAPI specs, message schemas)
  - ADRs for key decisions
  - Diagram of component interactions (Mermaid or ASCII)
  - Data ownership map (which service owns which entity)

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "adr"
      description: "Architecture Decision Records for this feature"
      path: "docs/architecture/decisions/"
    - type: "service_contracts"
      description: "OpenAPI specs and message schemas"
      path: "docs/contracts/"
    - type: "component_diagram"
      description: "Component interaction diagram"
  handoff_to: ["dotnet-specialist", "nodejs-specialist", "postgresql-specialist"]
  notes: "Patients service owns patient data. Appointments service subscribes to PatientRegistered event."
```

### Working in Parallel

- Architecture definition should complete before implementation specialists start. If necessary to parallelize, provide partial contracts (stubs) early and update when finalized.
- Coordinate with Data Architect on data ownership and schema boundaries before backend specialists begin.

---

## Verification Checklist

- [ ] All significant decisions documented as ADRs
- [ ] Service contracts defined in OpenAPI or schema files
- [ ] No shared databases across service boundaries
- [ ] All synchronous service-to-service calls have timeout and circuit breaker defined
- [ ] Event schemas versioned and documented
- [ ] Authentication/authorization model documented
- [ ] Threat model reviewed by App Security specialist
- [ ] Scalability approach documented (horizontal/vertical, caching strategy)
