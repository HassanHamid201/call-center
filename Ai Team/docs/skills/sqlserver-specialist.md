# SQL Server Specialist — Best Practices

## Overview

The SQL Server Specialist designs schemas, writes T-SQL, manages migrations, and optimizes performance for Microsoft SQL Server environments. It is the primary database layer for .NET-stack projects using EF Core or ADO.NET.

**Skill ID:** `sqlserver-specialist`
**Auto-detected from:** `.csproj` with `Microsoft.EntityFrameworkCore.SqlServer` or `System.Data.SqlClient`
**Always paired with:** `app-security` (review), `automation-qa` (review)

---

## Core Technologies

| Category | Technology | Version |
|---|---|---|
| Database | SQL Server | 2022+ |
| Language | T-SQL | Latest |
| ORM | Entity Framework Core with SQL Server provider | 8+ |
| Migration | EF Core Migrations / Flyway / DbUp | Latest |
| Query analysis | SQL Server Management Studio (SSMS) / Azure Data Studio | Latest |
| Testing | tSQLt / Testcontainers | Latest |

---

## Best Practices

### Schema Design

- Use **`UNIQUEIDENTIFIER`** PKs (with `NEWSEQUENTIALID()`) for distributed systems. Use `INT IDENTITY` or `BIGINT IDENTITY` when sequential generation is preferred and distribution is not required.
- Apply **NOT NULL constraints** by default. Nullable columns require explicit justification.
- Use **check constraints** to enforce business rules: `CONSTRAINT CK_Patients_NationalId CHECK (NationalId LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')`.
- Use **`DATETIMEOFFSET`** for all timestamp columns in multi-timezone or compliance environments. Use `DATETIME2(7)` when timezone isn't needed but precision is.
- Use **`NVARCHAR`** for all Arabic/Unicode text. Use `NCHAR(n)` for fixed-length Unicode.
- Follow **PascalCase** naming for tables, columns, and procedures to match .NET conventions.
- Use schemas (`dbo`, `hr`, `clinical`) to namespace and secure related tables.

```sql
CREATE TABLE dbo.Patients (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    NationalId      NCHAR(10)        NOT NULL,
    NameAr          NVARCHAR(255)    NOT NULL,
    NameEn          NVARCHAR(255)    NULL,
    DateOfBirth     DATE             NOT NULL,
    Gender          NCHAR(1)         NOT NULL,
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAt       DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt       DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT UQ_Patients_NationalId UNIQUE (NationalId),
    CONSTRAINT CK_Patients_NationalId CHECK (NationalId LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    CONSTRAINT CK_Patients_Gender CHECK (Gender IN (N'M', N'F'))
);
```

### Indexing Strategy

- **Clustered index**: every table should have a clustered index (usually the PK). The clustered index determines physical row order — choose the PK type carefully.
- **Non-clustered indexes**: add on columns frequently used in `WHERE`, `JOIN ON`, and `ORDER BY` clauses.
- **FK columns**: SQL Server does not auto-index FK columns — always create a non-clustered index on FK columns.
- **Covered indexes** (`INCLUDE`): add non-key columns that are frequently `SELECT`ed alongside the indexed column to avoid key lookups.
- **Filtered indexes**: use for queries on a subset of rows (e.g., `WHERE IsActive = 1`).
- **Columnstore indexes**: use for analytic/reporting queries on large tables.
- Monitor with `sys.dm_db_missing_index_details` and `sys.dm_db_index_usage_stats`.

```sql
-- FK index with cover columns
CREATE NONCLUSTERED INDEX IX_Appointments_PatientId
    ON dbo.Appointments (PatientId)
    INCLUDE (ScheduledAt, Status);

-- Filtered index for active-only queries
CREATE NONCLUSTERED INDEX IX_Patients_Active_NameAr
    ON dbo.Patients (NameAr)
    WHERE IsActive = 1;
```

### T-SQL Best Practices

- Use **set-based operations** — never cursor loops for data transformations. If a loop seems necessary, find the set-based approach.
- Use **`WITH (NOLOCK)`** only when stale reads are explicitly acceptable (reporting queries). Never on transactional code.
- Use **`TRY...CATCH`** blocks in stored procedures. Re-throw with `THROW` (not `RAISERROR`).
- Use **`sp_executesql`** for dynamic SQL with parameters — never `EXEC` with string concatenation.
- Always **`SET NOCOUNT ON`** at the start of stored procedures to suppress row count messages.
- Use **explicit transactions** (`BEGIN TRANSACTION ... COMMIT/ROLLBACK`) in procedures that modify multiple tables.

```sql
CREATE PROCEDURE dbo.RegisterPatient
    @NationalId  NCHAR(10),
    @NameAr      NVARCHAR(255),
    @NameEn      NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM dbo.Patients WHERE NationalId = @NationalId)
        THROW 50001, 'National ID already registered.', 1;
    
    INSERT INTO dbo.Patients (NationalId, NameAr, NameEn)
    VALUES (@NationalId, @NameAr, @NameEn);
    
    SELECT CAST(SCOPE_IDENTITY() AS UNIQUEIDENTIFIER);
END;
```

### Migrations

- Use **EF Core Migrations** for code-first workflows. Review every auto-generated migration before applying.
- Use **DbUp** or **Flyway** for script-based migrations in non-EF workflows.
- Never modify an already-applied migration. Always create a new one.
- For schema changes on large tables, use **online operations** where possible: `CREATE INDEX ... WITH (ONLINE = ON)`.
- Test migrations against a restored production backup in staging before applying to production.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Cursor loops for set operations | Orders of magnitude slower | Set-based T-SQL |
| Dynamic SQL with string concatenation | SQL injection risk | `sp_executesql` with parameters |
| Missing FK indexes | Slow joins, slow cascades | Index all FK columns |
| `SELECT *` in stored procedures/views | Breaks on schema changes, over-fetches | Explicit column list |
| `WITH (NOLOCK)` in transactional code | Dirty reads, phantom data | Remove; use appropriate isolation level |
| Storing datetime without `DATETIMEOFFSET` in multi-tz apps | Timezone bugs | `DATETIMEOFFSET` |
| Using `NTEXT`/`TEXT`/`IMAGE` types | Deprecated, limited functionality | `NVARCHAR(MAX)`, `VARBINARY(MAX)` |
| God stored procedures doing everything | Untestable, unmaintainable | Single-responsibility procedures |

---

## Security

- **Transparent Data Encryption (TDE)**: enable TDE for databases storing PII or PHI. This encrypts data files on disk.
- **Always Encrypted**: for column-level encryption of the most sensitive fields (national IDs, health records) where even DBAs should not see plaintext.
- **Row-level security**: use RLS for multi-tenant tables to enforce tenant isolation at the DB level.
- **Least privilege**: application SQL login should have only the minimum required permissions. Create a dedicated application login — not `sa` or `sysadmin`.
- **Parameterized queries**: all application queries through EF Core or `sp_executesql`. Never string-concatenated SQL from application code.
- **SQL Server Audit**: enable audit logging for compliance environments (HIPAA, PDPL, ISO 27001).
- **Firewall**: restrict SQL Server port (1433) to application servers only. Never expose directly to the internet.
- **Backup encryption**: encrypt all backups with a certificate or asymmetric key.

---

## Performance

- Use **Execution Plan Analyzer** (SSMS or `SET STATISTICS IO, TIME ON`) to diagnose slow queries.
- Address **key lookups** with covering indexes.
- Address **implicit conversions** by matching parameter data types exactly to column data types.
- Use **`READ_COMMITTED_SNAPSHOT`** isolation level (RCSI) at the database level to reduce blocking without resorting to `NOLOCK`.
- Use **table variables** for small result sets; **temp tables** for larger ones with complex operations.
- Use **`OPTION (RECOMPILE)`** on queries with local variable predicates that cause poor plan reuse.
- Consider **Query Store** (enabled by default in SQL Server 2016+) to track plan regressions.

---

## Testing

- Use **tSQLt** for unit testing stored procedures and functions in-database.
- Use **Testcontainers** (with `mcr.microsoft.com/mssql/server`) for integration tests that run a real SQL Server instance in Docker.
- Test constraints: attempt to insert violating data and assert on the error.
- Test stored procedures with `tSQLt.AssertEquals`, `tSQLt.AssertEqualsTable`.

---

## Code Quality

- **Naming**: Tables `PascalCase`; columns `PascalCase`; SPs `usp_VerbNoun`; indexes `IX_Table_Columns`; constraints `PK_`, `FK_`, `UQ_`, `CK_`, `DF_` prefixes.
- **Script style**: uppercase T-SQL keywords; one clause per line; explicit `GO` batch separators.

### Review Checklist

- [ ] All FK columns indexed
- [ ] `DATETIMEOFFSET` used for all timestamps in multi-timezone context
- [ ] All dynamic SQL uses `sp_executesql` with parameters
- [ ] No `SELECT *` in procedures/views
- [ ] TDE enabled on database (for production)
- [ ] Application login has minimal privileges
- [ ] `SET NOCOUNT ON` in all stored procedures
- [ ] Migrations tested against a production-size database before applying

---

## Scrum Team Collaboration

### Providing to Downstream Specialists

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "schema"
      description: "Table definitions with T-SQL column types and constraints"
    - type: "migration"
      description: "EF Core migration applied / DbUp script applied"
      path: "src/Infrastructure/Migrations/"
    - type: "stored_procedures"
      description: "Procedures ready for use (if any)"
  handoff_to: ["dotnet-specialist", "net-mvc-specialist"]
  notes: |
    NationalId: NCHAR(10), UNIQUE constraint enforced
    DateOfBirth: DATE (Gregorian); Hijri in app layer
    Connection string key: ConnectionStrings__DefaultConnection
    EF Core DbContext: ApplicationDbContext in Infrastructure project
```

### Working in Parallel

- If the .NET Core specialist also creates an EF Core migration in the same batch, flag to Scrum Master immediately — migrations must be sequenced.
- Provide the entity model classes (or a schema summary) early so the .NET specialist can start writing queries in parallel.

---

## Verification Checklist

- [ ] EF Core `dotnet ef database update` or DbUp `Execute` exits 0 on clean database
- [ ] All migrations are idempotent or sequenced correctly
- [ ] Execution plans reviewed — no table scans on large tables
- [ ] FK indexes present for all new FK columns
- [ ] TDE enabled (production environment)
- [ ] Application login permissions verified (least privilege)
- [ ] Testcontainer integration tests pass
- [ ] No hardcoded connection strings in migration scripts
