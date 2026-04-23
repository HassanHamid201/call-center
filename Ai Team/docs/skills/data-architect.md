# Data Architect — Best Practices

## Overview

The Data Architect designs the overall data model — how data is structured, stored, governed, migrated, and accessed across the system. It works at a level above individual database specialists, defining schemas, data ownership, migration strategies, and data quality standards.

**Skill ID:** `data-architect`
**Activated by:** Explicit assignment or tasks mentioning "data model", "data governance", "schema design", "data migration", "master data", or "data quality"
**Coordinates with:** PostgreSQL Specialist, SQL Server Specialist, System Architect

---

## Core Domains

| Domain | Focus |
|---|---|
| Schema Design | Logical and physical data modeling |
| Normalization | Relational design trade-offs |
| Data Migration | Extract-Transform-Load (ETL), schema evolution |
| Master Data | Canonical entities, reference data governance |
| Data Quality | Validation rules, constraints, anomaly detection |
| Data Governance | Ownership, lineage, retention, classification |
| Polyglot Persistence | Choosing the right database type per use case |

---

## Best Practices

### Logical Data Modeling

- Start with a **conceptual model** (entity-relationship diagram) before writing DDL. Identify entities, relationships (cardinality), and key attributes.
- Apply **Third Normal Form (3NF)** as the default for transactional (OLTP) systems. Reduce redundancy, prevent update anomalies.
- Document the **ubiquitous language**: use domain terminology in entity and attribute names — not technical abbreviations.
- Define **cardinality explicitly**: one-to-one, one-to-many, many-to-many (resolved through junction tables).

```
ERD fragment:
  Patient (1) ──── (M) Appointment
  Appointment (M) ──── (1) Clinician
  Patient (M) ──── (M) Clinician   →  PatientClinician (junction)
```

### Physical Data Modeling

- Map logical entities to physical tables using database-specific types (see PostgreSQL / SQL Server specialists for type guidance).
- Identify and document **natural keys** (business identifiers like National ID) vs. **surrogate keys** (database-generated UUID/IDENTITY).
- Apply **denormalization deliberately** for read performance — document the trade-off. Common patterns: pre-computed aggregates, flattened summary tables, materialized views.
- Use **partitioning** for tables that will exceed 100M rows (range partition by date for time-series; hash partition for even distribution).

### Normalization vs. Denormalization

| Use Case | Approach | Reason |
|---|---|---|
| OLTP (transactions, writes) | 3NF — normalized | Minimize write anomalies |
| OLAP (reporting, aggregations) | Star/snowflake schema — denormalized | Fast reads, simpler queries |
| Search | Document store (Elasticsearch) | Full-text + faceted search |
| Graph relationships | Graph DB (Neo4j) or adjacency list | Efficient traversal |
| Time-series | TimescaleDB / specialized schema | Optimized time-range queries |
| Key-value lookups | Redis / DynamoDB | Sub-millisecond reads |

### Data Migration Strategy

- Never perform in-place destructive migrations on production data. Use the **expand-contract pattern**:
  1. **Expand**: add new columns/tables alongside old
  2. **Migrate**: backfill new from old in batches
  3. **Contract**: remove old once consumers are updated

- For large backfills, process in batches (1K–10K rows per transaction) to avoid long-running locks.
- Always create a **migration runbook**: steps, validation queries, rollback procedure, estimated duration.
- Test migration on a production-size database copy with production-like data volume and distribution.
- Define **data quality checkpoints**: row count before and after, checksum on key columns, sample validation.

```sql
-- Migration with batched backfill
DECLARE @BatchSize INT = 5000;
DECLARE @Rows INT = 1;

WHILE @Rows > 0
BEGIN
    UPDATE TOP (@BatchSize) dbo.Patients
    SET NameFull = NameAr + ' ' + ISNULL(NameEn, '')
    WHERE NameFull IS NULL;
    
    SET @Rows = @@ROWCOUNT;
END;
```

### Master Data Management

- Identify **master entities** (patients, clinicians, facilities) and designate one system as the **System of Record** per entity.
- All other systems reference the master via the canonical ID — they do not maintain their own copy of the master data.
- Use **reference data tables** for constrained value sets (countries, cities, specializations) — not hardcoded enums in application code. Reference data changes over time.
- Implement a **data change notification** mechanism: when master data changes, downstream systems are notified (via events or CDC — Change Data Capture).

### Data Quality

- Enforce quality at the database level: `NOT NULL`, `CHECK`, `UNIQUE`, `FK` constraints are the last line of defense.
- Define **data quality rules** in a structured catalog: field, rule type (format/range/referential), severity (error/warning), owner.
- Implement **input validation at all layers**: database constraints, application-layer validation (FluentValidation/Zod), and UI-layer validation.
- Schedule **data quality reports** for production: count nulls in required fields, count FK violations (after disabling constraints), count format violations.

### Data Governance

- Classify all data fields by sensitivity: `PUBLIC`, `INTERNAL`, `CONFIDENTIAL`, `RESTRICTED` (PII/PHI).
- Document **data ownership** per entity: which team/service is responsible for each data domain.
- Define **data retention policies** per table: how long records are kept, when they are archived, when they are deleted.
- Implement **soft delete** (`IsActive`, `DeletedAt`) for most entities — hard delete only when legally required (right to erasure under PDPL/GDPR).
- Document **data lineage**: where each field originates, how it is transformed, where it flows.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Storing business logic in column names | Hard to evolve schema | Normalize; use reference data tables |
| Multiple systems owning the same entity | Conflicting data, sync problems | Single System of Record per entity |
| Destructive migrations without rollback | Data loss, no recovery path | Expand-contract with rollback plan |
| No data retention policy | Unbounded growth, compliance risk | Define retention per entity upfront |
| Storing derived/computed data without documentation | Stale derived data, confusion | Document all denormalized/derived fields |
| EAV (Entity-Attribute-Value) pattern | Poor query performance, no type safety | Use JSONB or proper columns |
| Ignoring NULL semantics | Silent errors, incorrect aggregations | Explicit NULL handling policy per column |

---

## Security

- **PII/PHI field catalog**: document every field containing personal information. Tag in schema comments.
- **Field-level encryption**: encrypt PII/PHI at rest (column-level) for the most sensitive fields (national ID, health records, financial data).
- **Pseudonymization**: where possible, use pseudonymized IDs in analytics/reporting pipelines instead of real personal identifiers.
- **Access control**: database roles aligned to data classification. `CONFIDENTIAL` data accessible only by application service accounts, not analytics tools.
- **Audit trail**: all writes to sensitive tables must be logged with `who`, `when`, `what changed`. Use CDC or audit triggers.

---

## Performance

- Profile queries against production-scale data volumes — development data is too small to reveal performance issues.
- Identify and document **hot tables**: tables with the highest read/write volume. Prioritize indexing and caching for these.
- Design read and write paths separately (CQRS at the data layer): read models optimized for query patterns, write models optimized for consistency.
- Archive historical data to a separate schema or database when it is no longer needed for transactional queries (e.g., completed appointments > 2 years old).

---

## Data Architecture Decision Records

Every significant data design decision should be documented:

```markdown
# DADR-001: Use Soft Delete for Patients Entity

**Date:** 2026-04-23
**Status:** Accepted
**Context:** PDPL requires the ability to respond to "right to erasure" requests. However, appointment history must remain valid.
**Decision:** Implement soft delete (IsActive = FALSE, DeletedAt timestamp). Physical deletion only on verified erasure request, with anonymization of FK references.
**Consequences:**
  + Preserves referential integrity for historical records
  + Supports PDPL erasure workflow
  - Queries must always filter IsActive to avoid returning deleted records
  - Requires global query filter in EF Core / Prisma middleware
```

---

## Scrum Team Collaboration

### Architecture Tasks Are Upstream

Data architecture tasks belong in `parallel_group: 1` — the physical schema and data ownership model is a dependency for all database and backend specialists.

Handoff Package from Data Architect:

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "logical_data_model"
      description: "ERD with entities, relationships, and cardinality"
      path: "docs/data-model/erd.md"
    - type: "data_dictionary"
      description: "All fields with types, constraints, classification, and descriptions"
      path: "docs/data-model/dictionary.md"
    - type: "master_data_ownership"
      description: "System of Record per entity"
    - type: "data_governance_rules"
      description: "Retention policies, classification, audit requirements"
  handoff_to: ["postgresql-specialist", "sqlserver-specialist", "dotnet-specialist"]
  notes: "Patient is master entity owned by PatientService. NationalId is PII RESTRICTED. Soft delete required."
```

### Working in Parallel

- Provide the data dictionary and ERD as early as possible so database specialists can start DDL in parallel.
- Coordinate with System Architect on data ownership boundaries before database specialists begin physical design.

---

## Verification Checklist

- [ ] ERD covers all entities in the feature scope
- [ ] All tables have documented data classification (PUBLIC / INTERNAL / CONFIDENTIAL / RESTRICTED)
- [ ] Retention policies defined for all new tables
- [ ] PII/PHI fields identified and documented
- [ ] Migration runbook complete with validation queries and rollback procedure
- [ ] Data quality rules catalog updated
- [ ] System of Record documented for all master entities
- [ ] Data dictionary includes all new fields with descriptions
