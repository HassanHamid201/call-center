# .NET Core Specialist — Best Practices

## Overview

The .NET Core Specialist builds backend APIs, services, and business logic using ASP.NET Core. It handles API design, Clean Architecture, CQRS, Entity Framework Core, dependency injection, and async programming.

**Skill ID:** `dotnet-specialist`
**Auto-detected from:** `.csproj` or `.sln` files
**Always paired with:** `app-security` (review), `automation-qa` (review)

---

## Core Technologies

| Category | Technology | Version |
|---|---|---|
| Framework | ASP.NET Core | .NET 8/9/10 |
| Language | C# | 12+ |
| ORM | Entity Framework Core | 8+ |
| CQRS / Mediator | MediatR | 12+ |
| Validation | FluentValidation | 11+ |
| Mapping | AutoMapper / Mapster | Latest |
| Auth | ASP.NET Core Identity / JWT Bearer | Latest |
| API Docs | Scalar / Swashbuckle | Latest |
| Testing | xUnit + Moq + FluentAssertions | Latest |

---

## Best Practices

### Architecture: Clean Architecture

Organize the solution in four layers. Dependencies point inward only:

```
Solution/
├── Domain/            # Entities, value objects, domain events, interfaces — no dependencies
├── Application/       # Use cases (MediatR handlers), DTOs, validators, service interfaces
├── Infrastructure/    # EF Core, external APIs, email, file storage — implements interfaces
└── API/               # Controllers, middleware, DI registration, minimal API endpoints
```

- **Domain layer**: pure C# classes. No framework dependencies. Entities have private setters; state changes through methods.
- **Application layer**: business logic through MediatR commands/queries. No EF Core — only repository/service interfaces defined in Domain.
- **Infrastructure layer**: EF Core `DbContext`, repositories, external service clients. Implements interfaces from Domain.
- **API layer**: controllers/minimal API endpoints are thin — they receive HTTP input, dispatch to MediatR, and return HTTP output. No business logic.

### CQRS with MediatR

- Every use case is a `IRequest<T>` with a corresponding `IRequestHandler<TRequest, TResponse>`.
- Commands (state mutations): `CreatePatientCommand`, `UpdatePatientCommand`, `DeletePatientCommand`.
- Queries (reads): `GetPatientByIdQuery`, `GetPatientListQuery`.
- Use `IPipelineBehavior<TRequest, TResponse>` for cross-cutting concerns: logging, validation, transactions.

```csharp
// Command
public record CreatePatientCommand(string NationalId, string NameAr, string NameEn) 
    : IRequest<Guid>;

// Handler
public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IPatientRepository _repo;

    public CreatePatientCommandHandler(IPatientRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken ct)
    {
        var patient = Patient.Create(request.NationalId, request.NameAr, request.NameEn);
        await _repo.AddAsync(patient, ct);
        return patient.Id;
    }
}
```

### FluentValidation

- Every command and query has a corresponding `AbstractValidator<T>` in the Application layer.
- Register validators with `services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())`.
- Use a `ValidationBehavior<TRequest, TResponse>` MediatR pipeline behavior to auto-validate all requests.
- Never validate in controllers — validation belongs in the Application layer.

### Entity Framework Core

- Define `DbContext` in the Infrastructure layer, not Application or Domain.
- Use **code-first migrations** managed with `dotnet ef migrations add`. Never run `EnsureCreated` in production.
- Apply **global query filters** for soft-delete and multi-tenancy in `OnModelCreating`.
- Use **`AsNoTracking()`** for read-only queries to improve performance.
- Avoid **N+1 queries** — use `.Include()` and `.ThenInclude()` or projection queries (`.Select()`).
- Use **value converters** for enums and value objects in `OnModelCreating`.
- Always use `CancellationToken` parameters in all async EF Core operations.

```csharp
// Good — projection query, no tracking, cancellable
var patients = await _context.Patients
    .AsNoTracking()
    .Where(p => p.IsActive)
    .Select(p => new PatientSummaryDto(p.Id, p.NameAr, p.NationalId))
    .ToListAsync(cancellationToken);
```

### Dependency Injection

- Register services by lifetime: `AddSingleton` (stateless, thread-safe), `AddScoped` (per request), `AddTransient` (lightweight, per injection).
- Use **extension methods** on `IServiceCollection` for feature registrations (`services.AddPatientModule()`).
- Never use the service locator pattern (injecting `IServiceProvider` to resolve services manually).
- Prefer constructor injection. Avoid property injection.

### async/await

- All I/O operations must be `async/await`. Never `.Result`, `.Wait()`, or `Task.Run()` to wrap async code.
- Always pass `CancellationToken` from the outermost caller down to all async methods.
- Use `ConfigureAwait(false)` in library code (non-UI, non-ASP.NET Core).
- Never use `async void` except for event handlers.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Business logic in controllers | Untestable, bloated | CQRS handlers in Application layer |
| Direct EF Core in controllers | Tight coupling, skips validation | Use MediatR + repository pattern |
| Synchronous I/O (`.Result`, `.Wait()`) | Deadlocks in async context | `async/await` all the way |
| Returning `IEnumerable<T>` from services | Deferred execution leaks outside scope | Materialize with `.ToListAsync()` |
| Catching `Exception` broadly | Swallows errors, hard to debug | Catch specific exceptions; use global exception middleware |
| Storing passwords in plain text | Critical security failure | Use ASP.NET Core Identity's password hasher (bcrypt) |
| Fat DbContext with all entities | God object, merge conflicts | Bounded DbContexts per module |
| Exposing domain entities as API responses | Leaks internals, over-posts | Always use DTOs/response records |

---

## Security

- **Authentication**: JWT Bearer tokens. Use RS256 (asymmetric signing). Store signing key in Azure Key Vault or environment secrets, never in `appsettings.json`.
- **Authorization**: policy-based authorization. Define policies in `Program.cs`; apply `[Authorize(Policy = "PolicyName")]` on controllers/endpoints.
- **Password storage**: ASP.NET Core Identity's `IPasswordHasher` (PBKDF2 by default). Never bcrypt directly without Identity.
- **Input validation**: FluentValidation pipeline behavior validates every command/query automatically.
- **SQL injection**: parameterized queries through EF Core. Never string-concatenated SQL. Use `.FromSqlRaw` only with parameters, never with user input.
- **Secrets management**: use `IConfiguration` with environment variables / Azure Key Vault. Never hardcode secrets.
- **CORS**: configure explicitly in `Program.cs`. Do not use `AllowAnyOrigin` in production.
- **Rate limiting**: use ASP.NET Core's built-in rate limiting middleware for sensitive endpoints (login, registration).

---

## Performance

- Use **response caching** for read-heavy endpoints with stable data.
- Use **pagination** (cursor or offset/limit) for all list endpoints. Never return unbounded collections.
- Use **projection queries** (`.Select()`) to fetch only required columns — not `.Include()` full graphs for read-only scenarios.
- Use **compiled queries** (`EF.CompileAsyncQuery`) for high-frequency, simple queries.
- Consider **Dapper** for complex read queries where EF Core projection becomes unwieldy.
- Profile with `MiniProfiler` or `dotnet-trace` before optimizing. Measure first.
- Use **output caching** or **distributed cache** (Redis) for expensive computed results.

---

## Testing

### Strategy: Test Pyramid

- **Unit tests (60%)**: MediatR handlers, validators, domain entities, service logic — with mocked dependencies
- **Integration tests (30%)**: full API pipeline with `WebApplicationFactory<Program>`, EF Core with an in-memory or SQLite test database
- **E2E tests (10%)**: critical API flows with a real running instance and seeded test data

### Unit Tests (xUnit + Moq + FluentAssertions)

- Test MediatR handlers by mocking repository interfaces with Moq.
- Test FluentValidation validators using `.TestValidate()`.
- Use FluentAssertions for readable, expressive assertions.

```csharp
[Fact]
public async Task Handle_CreatesPatient_WhenRequestIsValid()
{
    var mockRepo = new Mock<IPatientRepository>();
    var handler = new CreatePatientCommandHandler(mockRepo.Object);
    var command = new CreatePatientCommand("1234567890", "محمد", "Mohammed");

    var result = await handler.Handle(command, CancellationToken.None);

    result.Should().NotBeEmpty();
    mockRepo.Verify(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Integration Tests (WebApplicationFactory + EF Core)

- Override `DbContext` registration with an in-memory or SQLite test database.
- Seed test data in a `TestFixture` or `IAsyncLifetime`.
- Test the full HTTP pipeline: deserialize request, assert on response status and body.

---

## Code Quality

- **Nullable reference types**: `<Nullable>enable</Nullable>` in all projects. Fix all warnings.
- **Records for DTOs**: use C# records for immutable DTOs and value objects.
- **Naming**: `IXRepository`, `XService`, `CreateXCommand`, `GetXQuery`, `XCreatedEvent`.
- **No magic strings**: use `nameof()`, constants, or enum members.

### Review Checklist

- [ ] No business logic in controllers
- [ ] All commands/queries validated with FluentValidation
- [ ] No synchronous I/O (`.Result`, `.Wait()`)
- [ ] All async methods accept `CancellationToken`
- [ ] No plain-text passwords or secrets in code
- [ ] `AsNoTracking()` on all read-only EF Core queries
- [ ] All list endpoints paginated
- [ ] Test coverage ≥ 80% on new code

---

## Scrum Team Collaboration

### Receiving from Upstream Specialists

- **PostgreSQL / SQL Server Specialist**: receive Handoff Package with schema definition, migration status, and connection string config path.
- **System Architect**: if the task involves a new bounded context or service boundary, receive the architecture decision before starting.

### Providing to Downstream Specialists

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "api_contract"
      description: "OpenAPI spec or endpoint summary"
      path: "docs/api/patients.yaml"
    - type: "dto_types"
      description: "Request/response DTOs"
      path: "src/Application/Patients/Dtos/"
  handoff_to: ["react-specialist", "app-security", "automation-qa"]
  notes: "POST /api/patients: 201 on success, 422 on validation failure, 409 on duplicate national ID"
```

### Working in Parallel

- When working in the same batch as a frontend specialist, generate and share an OpenAPI spec early so the frontend can work against a typed contract.
- If modifying `DbContext`, coordinate with the database specialist to avoid migration conflicts through the Scrum Master.

---

## Verification Checklist

- [ ] `dotnet build` exits 0 with no warnings
- [ ] `dotnet test` exits 0 with ≥ 80% coverage
- [ ] `dotnet format --verify-no-changes` exits 0
- [ ] No synchronous I/O on hot paths
- [ ] JWT signing key not in `appsettings.json`
- [ ] CORS policy is explicit (no `AllowAnyOrigin` in production)
- [ ] Rate limiting configured on sensitive endpoints
- [ ] All list endpoints paginated
- [ ] FluentValidation pipeline behavior registered and active
