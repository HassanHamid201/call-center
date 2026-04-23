# Node.js Specialist — Best Practices

## Overview

The Node.js Specialist builds backend APIs and services using Node.js with TypeScript. It handles REST/GraphQL API design, middleware architecture, dependency injection (NestJS), async error handling, and testing.

**Skill ID:** `nodejs-specialist`
**Auto-detected from:** `express`, `@nestjs/core`, or `fastify` in `package.json`
**Always paired with:** `app-security` (review), `automation-qa` (review)

---

## Core Technologies

| Category | Technology | Version |
|---|---|---|
| Runtime | Node.js | 20 LTS+ |
| Language | TypeScript | 5+ strict mode |
| Framework | NestJS (preferred) / Express | NestJS 10+ |
| Validation | class-validator + class-transformer | Latest |
| ORM | Prisma / TypeORM | Latest |
| Auth | Passport.js + JWT | Latest |
| API Docs | Swagger (NestJS built-in) | Latest |
| Testing (unit) | Jest + Supertest | Latest |
| Linting | ESLint + `@typescript-eslint` | Latest |

---

## Best Practices

### Architecture (NestJS)

- Organize code into **feature modules** (`PatientModule`, `AuthModule`). Each module encapsulates its own controllers, services, and repositories.
- Follow the **Controller → Service → Repository** pattern:
  - Controllers: HTTP handling only — parse request, call service, return response.
  - Services: business logic, orchestration, no direct DB access.
  - Repositories: all database interactions (Prisma or TypeORM repositories).
- Use NestJS's **dependency injection** exclusively. Never instantiate services with `new`. Always inject via constructor.
- Use **`@Module` metadata** to declare providers, controllers, and exports. Keep modules focused.

```typescript
// Controller: thin, delegates to service
@Controller('patients')
export class PatientController {
  constructor(private readonly patientService: PatientService) {}

  @Post()
  @HttpCode(HttpStatus.CREATED)
  async create(@Body() dto: CreatePatientDto): Promise<{ id: string }> {
    const id = await this.patientService.create(dto);
    return { id };
  }
}
```

### TypeScript

- Enable `strict: true` in `tsconfig.json`. No exceptions.
- Use `class-validator` decorators on DTOs for input validation.
- Use `class-transformer` with `plainToInstance` to transform and validate incoming request bodies.
- Never use `any`. Use `unknown` and narrow, or use generics.
- Define explicit return types on all public functions and methods.

### Input Validation

- All request body/query DTOs must have `class-validator` decorators.
- Apply NestJS `ValidationPipe` globally with `whitelist: true, forbidNonWhitelisted: true` — this strips unknown properties and prevents over-posting.
- Apply `transform: true` to auto-cast types (string query params to numbers, etc.).

```typescript
// DTO with validation
export class CreatePatientDto {
  @IsString()
  @Length(10, 10)
  @Matches(/^\d{10}$/, { message: 'National ID must be 10 digits' })
  nationalId: string;

  @IsString()
  @IsNotEmpty()
  nameAr: string;
}
```

### Error Handling

- Use NestJS's built-in **`HttpException`** and its subclasses (`NotFoundException`, `BadRequestException`, `ConflictException`) for HTTP errors. Never throw raw `Error` objects from controllers or services.
- Define a **global exception filter** for unexpected errors — log them with context and return a sanitized 500 response.
- Never expose stack traces or internal error messages in production responses.
- Use `try/catch` only in infrastructure layer (repositories, external HTTP calls). Let the exception bubble up from services through the exception filter.

### async/await & Error Safety

- All async operations must use `async/await` with proper `try/catch` at the right layer.
- Never ignore promise rejections. Use `unhandledRejection` listener on the process to catch missed async errors.
- Use `AbortController` / `AbortSignal` for cancellable operations.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Business logic in controllers | Untestable, violates SRP | Move to services |
| Direct DB access in controllers | Tight coupling | Repository/service pattern |
| `any` TypeScript | Defeats type safety | Explicit types, generics |
| Missing `ValidationPipe` globally | Over-posting, invalid data accepted | Apply globally with `whitelist: true` |
| Synchronous file/crypto operations | Blocks event loop | Use `fs/promises`, `crypto` async APIs |
| JWT secrets in code | Credential exposure | Use environment variables / secrets manager |
| Unhandled promise rejections | Silent failures | Catch all async errors; use global handler |
| Returning internal error details to clients | Information disclosure | Sanitize all error responses |
| Circular module dependencies | NestJS bootstrap failure | Refactor to break cycles; use `forwardRef` as last resort |

---

## Security

- **JWT**: sign with RS256 (asymmetric). Store private key in environment secrets. Set `expiresIn` to short values (15 min access, 7 day refresh).
- **Rate limiting**: use `@nestjs/throttler` on authentication endpoints. Configure per-IP limits.
- **Helmet**: apply `helmet()` middleware to set security headers (CSP, HSTS, X-Frame-Options).
- **CORS**: configure explicitly with an allowlist. Never use `origin: '*'` in production.
- **Input sanitization**: `whitelist: true` on `ValidationPipe` prevents property injection. Do not trust client-supplied types.
- **SQL injection**: use Prisma/TypeORM parameterized queries. Never template-string SQL.
- **Dependency auditing**: `npm audit` in CI. Address critical and high vulnerabilities before merging.
- **Environment secrets**: use `@nestjs/config` with validation (`Joi` or `zod`) to ensure required secrets are present at startup.

---

## Performance

- Use **compression middleware** (`compression` package) for text responses.
- Implement **pagination** on all list endpoints — never return unbounded arrays.
- Use **`select` projections** in Prisma/TypeORM to avoid over-fetching columns.
- Use a **cache layer** (Redis via `@nestjs/cache-manager`) for expensive read-only queries.
- Avoid **N+1 queries** — use Prisma's `include` or TypeORM's `leftJoinAndSelect` with care; prefer `select` projections.
- Profile with `clinic.js` or Node.js built-in `--prof` flag before optimizing.
- Use **worker threads** for CPU-intensive tasks (image processing, report generation) to avoid blocking the event loop.

---

## Testing

### Strategy: Test Pyramid

- **Unit tests (60%)**: services and validators in isolation with mocked repositories
- **Integration tests (30%)**: full NestJS app with `TestingModule`, real (SQLite/in-memory) database
- **E2E tests (10%)**: critical HTTP flows with Supertest against a running app

### Unit Tests (Jest)

- Create the NestJS `TestingModule` with mocked providers using `jest.fn()` or `@golevelup/ts-jest`.
- Test services by mocking repository dependencies.
- Assert on returned values and on mock calls.

```typescript
describe('PatientService', () => {
  let service: PatientService;
  let mockRepo: jest.Mocked<PatientRepository>;

  beforeEach(async () => {
    mockRepo = { create: jest.fn(), findById: jest.fn() } as any;
    const module = await Test.createTestingModule({
      providers: [PatientService, { provide: PatientRepository, useValue: mockRepo }],
    }).compile();
    service = module.get(PatientService);
  });

  it('throws ConflictException when national ID already exists', async () => {
    mockRepo.findByNationalId = jest.fn().mockResolvedValue({ id: 'existing' });
    await expect(service.create({ nationalId: '1234567890', ... }))
      .rejects.toThrow(ConflictException);
  });
});
```

### Integration & E2E Tests (Supertest)

- Use `@nestjs/testing` to bootstrap a full module with a test database.
- Send real HTTP requests with Supertest; assert on status codes and response bodies.
- Reset database state between test suites.

---

## Code Quality

- **Naming**: controllers `XController`, services `XService`, repositories `XRepository`, DTOs `CreateXDto`/`UpdateXDto`/`XResponseDto`.
- **Folder structure per module**: `patients/patients.controller.ts`, `patients/patients.service.ts`, `patients/dto/`, `patients/patients.module.ts`.
- **Prettier**: `"singleQuote": true, "semi": true, "trailingComma": "all"`.
- **ESLint**: `@typescript-eslint/no-explicit-any`, `@typescript-eslint/no-unused-vars`.

### Review Checklist

- [ ] `ValidationPipe` applied globally with `whitelist: true, forbidNonWhitelisted: true`
- [ ] No `any` types
- [ ] JWT secret from environment, not hardcode
- [ ] Helmet and CORS configured
- [ ] Rate limiting on auth endpoints
- [ ] All async operations use `async/await` with error handling
- [ ] All list endpoints paginated
- [ ] Test coverage ≥ 80% on new code

---

## Scrum Team Collaboration

### Receiving from Upstream Specialists

- **PostgreSQL Specialist**: receive Handoff Package with schema, migration status, Prisma schema path, connection string config key.
- **System Architect**: if this service is part of a microservice topology, receive the service contract (API gateway routing, event topics, message contracts) before starting.

### Providing to Downstream Specialists

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "openapi_spec"
      description: "Swagger/OpenAPI spec auto-generated by NestJS"
      path: "docs/api-spec.json"
    - type: "dto_types"
      path: "src/patients/dto/"
  handoff_to: ["react-specialist", "vue-specialist", "app-security"]
  notes: "Swagger UI available at /api/docs in development"
```

### Working in Parallel

- Share the OpenAPI spec as soon as the controller stubs are written — frontend can start with mock data against the typed contract.
- Do not modify `prisma/schema.prisma` in parallel with the PostgreSQL specialist — coordinate through Scrum Master to sequence schema changes.

---

## Verification Checklist

- [ ] `npm run build` exits 0
- [ ] `npm run lint` exits 0
- [ ] `npm test -- --coverage` exits 0 with coverage ≥ 80%
- [ ] `ValidationPipe` global configuration applied
- [ ] Helmet middleware registered
- [ ] No hardcoded secrets
- [ ] Rate limiting on authentication routes
- [ ] All endpoints return typed response shapes (no `any`)
- [ ] Swagger documentation generated and accessible
