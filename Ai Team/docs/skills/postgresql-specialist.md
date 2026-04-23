# PostgreSQL Specialist — Best Practices

## Overview

The PostgreSQL Specialist designs schemas, writes queries, manages migrations, and optimizes database performance for PostgreSQL-based systems. It is responsible for the data foundation that all other backend specialists depend on.

**Skill ID:** `postgresql-specialist`
**Auto-detected from:** `prisma/schema.prisma`, `pg` in `package.json`, or `docker-compose.yml` with a `postgres` service
**Always paired with:** `app-security` (review), `automation-qa` (review)

---

## Core Technologies

| Category | Technology | Version |
|---|---|---|
| Database | PostgreSQL | 15+ |
| ORM / Query builder | Prisma (Node.js) / EF Core (C#) / SQLAlchemy (Python) | Latest |
| Migration tool | Prisma Migrate / EF Core Migrations / Flyway | Latest |
| Connection pooling | PgBouncer / built-in pool | Latest |
| Testing | pgTAP / Testcontainers | Latest |

---

## Best Practices

### Schema Design

- Use **UUIDs** (`gen_random_uuid()`) as primary keys for distributed systems. Use `BIGSERIAL` when sequential ordering or range queries on the PK are needed.
- Apply **NOT NULL constraints** by default. Allow NULL only when the absence of a value has explicit semantic meaning.
- Use **check constraints** to enforce business rules at the database level (not just the application level).
- Apply **`TIMESTAMPTZ`** (timezone-aware) for all timestamp columns, not `TIMESTAMP`. Store in UTC.
- Use **enum types** (`CREATE TYPE gender AS ENUM ('M', 'F', 'OTHER')`) for constrained value sets instead of unconstrained `VARCHAR`.
- Apply **naming conventions** consistently: `snake_case` for tables and columns; pluralized table names (`patients`, `appointments`).
- Document columns with comments: `COMMENT ON COLUMN patients.national_id IS 'Saudi national identity number (10 digits)';`

```sql
CREATE TABLE patients (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    national_id     CHAR(10) NOT NULL UNIQUE,
    name_ar         VARCHAR(255) NOT NULL,
    name_en         VARCHAR(255),
    date_of_birth   DATE NOT NULL,
    gender          gender NOT NULL,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_national_id_format CHECK (national_id ~ '^\d{10}$')
);
```

### Indexing Strategy

- **Primary key**: automatically indexed — no action needed.
- **Foreign keys**: always create an index on FK columns. PostgreSQL does not auto-index FK columns.
- **Frequent filter columns**: add a B-tree index on columns used in `WHERE` clauses with high cardinality.
- **Partial indexes**: use for filtered queries on a subset (e.g., `WHERE is_active = TRUE`) — much smaller and faster than full-table indexes.
- **Composite indexes**: create for multi-column `WHERE` clauses. Column order matters — put the most selective column first.
- **GIN indexes**: use for full-text search (`tsvector`) and JSONB queries.
- **Avoid over-indexing**: every index adds write overhead. Remove unused indexes with `pg_stat_user_indexes`.

```sql
-- FK index
CREATE INDEX idx_appointments_patient_id ON appointments(patient_id);

-- Partial index for active records
CREATE INDEX idx_patients_active ON patients(name_ar) WHERE is_active = TRUE;

-- Composite index for common filter + sort
CREATE INDEX idx_appointments_patient_date ON appointments(patient_id, scheduled_at DESC);
```

### Query Optimization

- Use `EXPLAIN (ANALYZE, BUFFERS)` to understand query plans before and after optimization.
- Avoid `SELECT *` — select only required columns.
- Avoid functions on indexed columns in `WHERE` clauses (defeats the index): `WHERE DATE(created_at) = '2026-01-01'` → `WHERE created_at >= '2026-01-01' AND created_at < '2026-01-02'`.
- Use **CTEs** for readability on complex queries, but be aware that CTEs are optimization fences in older PostgreSQL versions (< 12). In PostgreSQL 12+, the planner inlines them by default.
- Use **window functions** instead of self-joins for ranking, running totals, and lead/lag calculations.
- Use **`COPY`** (or `INSERT ... SELECT`) for bulk data loads — never loop-insert in application code.

### Migrations

- Use sequential, numbered migration files: `0001_create_patients.sql`, `0002_add_appointments.sql`.
- Migrations must be **idempotent** where possible — use `CREATE TABLE IF NOT EXISTS`, `CREATE INDEX CONCURRENTLY IF NOT EXISTS`.
- Never modify an already-applied migration. Always create a new migration to fix issues.
- For large tables, use **`CREATE INDEX CONCURRENTLY`** to avoid locking writes during index creation.
- For column renames/type changes on live tables, use expand-contract: add the new column, migrate data, remove the old column in separate migrations.
- Test migrations against a copy of production data in staging before applying to production.

### Connection Pooling

- Use **PgBouncer** in transaction pooling mode for high-concurrency APIs.
- Set `max_connections` in PostgreSQL conservatively. Most of the connections should be handled by PgBouncer, not direct app connections.
- Never open a connection per request in application code without a pool.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| `SELECT *` in application queries | Over-fetches data, breaks on schema changes | Explicit column list |
| Missing FK indexes | Slow FK lookups, slow cascade deletes | Index all FK columns |
| Using `VARCHAR` without length when `CHAR(n)` fits | Misleads developers about data shape | Use `CHAR(n)` for fixed-length, `VARCHAR(n)` for bounded |
| Storing timestamps as `TEXT` or without timezone | Ambiguous, timezone bugs | `TIMESTAMPTZ` always |
| Modifying applied migrations | Breaks migration history, causes drift | New migration for every change |
| Storing JSON as `TEXT` | No indexing, no validation | Use `JSONB` type |
| Accumulating dead rows (no VACUUM) | Table bloat, slow queries | Configure `autovacuum`; run `VACUUM ANALYZE` after bulk changes |
| N+1 queries from ORM | Exponential DB calls | Use `JOIN` or ORM eager loading |

---

## Security

- **Row-level security (RLS)**: enable on tables containing multi-tenant or sensitive data. Define policies that restrict row access per tenant/role.
- **Least privilege**: application database user should only have `SELECT, INSERT, UPDATE, DELETE` on required tables — not `CREATE TABLE`, `DROP`, or superuser access.
- **Encryption at rest**: use disk-level encryption (cloud provider) or PostgreSQL's native pgcrypto for column-level encryption of sensitive fields (PII, PHI).
- **Connection security**: enforce TLS connections (`ssl = on`, `ssl_min_protocol_version = TLSv1.3`). Reject non-SSL connections.
- **Password hashing**: never store passwords in the database — let the application hash them. Store only the hash.
- **Audit logging**: enable `pgaudit` extension for compliance environments (HIPAA, PDPL) to log all DML and DDL operations.
- **No dynamic SQL with user input**: parameterized queries always. In `PL/pgSQL`, use `EXECUTE ... USING` not string concatenation.

---

## Performance

- Run `VACUUM ANALYZE` after large bulk operations to update statistics.
- Use `pg_stat_statements` to identify slow queries in production.
- Partition large tables (> 100M rows) by range (date) or hash for improved query pruning.
- Configure `work_mem` appropriately for sort and hash operations — too low causes disk spills.
- Use **materialized views** for expensive aggregate queries that can tolerate slight staleness. Schedule `REFRESH MATERIALIZED VIEW CONCURRENTLY`.

---

## Testing

- Use **Testcontainers** (for Node.js/Java/C#) to spin up a real PostgreSQL instance in tests — never mock the database layer in integration tests.
- Run all migrations against the test database before test suite starts.
- Seed known test data in fixtures. Reset with `TRUNCATE ... RESTART IDENTITY CASCADE` between test suites.
- Test constraints directly: attempt to insert invalid data and assert on the database error.

---

## Code Quality

- **SQL style**: uppercase keywords (`SELECT`, `FROM`, `WHERE`); lowercase identifiers; one clause per line.
- **Comments**: add `COMMENT ON TABLE` and `COMMENT ON COLUMN` for all non-obvious tables and columns.
- **Migration file naming**: `{sequence}_{description}.sql` (e.g., `0001_create_patients.sql`).

### Review Checklist

- [ ] All FK columns indexed
- [ ] All timestamps use `TIMESTAMPTZ`
- [ ] Check constraints enforce all business rules
- [ ] Migrations are non-destructive (expand-contract pattern)
- [ ] RLS enabled on multi-tenant or sensitive tables
- [ ] Application DB user has minimal privileges
- [ ] `CREATE INDEX CONCURRENTLY` used for large tables
- [ ] No `SELECT *` in any migration or stored function

---

## Scrum Team Collaboration

### Providing to Downstream Specialists (primary upstream role)

The PostgreSQL Specialist is typically in `parallel_group: 1` — it provides the foundation for backend and frontend specialists. Produce a complete Handoff Package before signaling done:

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "schema"
      description: "Table definitions with column types and constraints"
    - type: "migration"
      description: "Migration file applied successfully"
      path: "database/migrations/0001_create_patients.sql"
    - type: "indexes"
      description: "Indexes created"
  handoff_to: ["dotnet-specialist", "nodejs-specialist", "react-specialist"]
  notes: |
    national_id: CHAR(10), unique, validated by check constraint
    date_of_birth: DATE (Gregorian); Hijri conversion in app layer
    Connection string key: ConnectionStrings__DefaultConnection
```

### Working in Parallel

- If another specialist also modifies the schema in the same batch, flag to the Scrum Master immediately — schema migrations must be sequenced, not parallel.
- Provide schema documentation (table + column summary) as early as possible so parallel backend specialists can start with type-accurate models.

---

## Verification Checklist

- [ ] All migrations apply cleanly from empty database: `flyway migrate` or `prisma migrate deploy` exits 0
- [ ] All migrations are idempotent (safe to re-run)
- [ ] `EXPLAIN ANALYZE` on all new queries — no sequential scans on large tables
- [ ] FK indexes present for all new FK columns
- [ ] RLS policies tested: correct rows returned per role
- [ ] Testcontainer integration tests pass
- [ ] No plaintext sensitive data in migration seed files
