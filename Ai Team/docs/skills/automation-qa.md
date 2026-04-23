# Automation QA Specialist — Best Practices

## Overview

The Automation QA Specialist designs, implements, and maintains automated test suites across all layers of the application. It is **always active** in the verification pipeline for every task, ensuring that implementation meets quality and coverage standards before a sprint advances.

**Skill ID:** `automation-qa`
**Always active:** Included in every Scrum Team's reviewer and implementer set
**Activated by:** Automatically; always participates in both implementation and verification
**Coordinates with:** All implementation specialists

---

## Test Pyramid Strategy

```
          /\
         /  \  E2E Tests
        /----\  (few, slow, catch integration)
       /      \
      /--------\  Integration Tests
     /          \  (some, moderate speed)
    /------------\
   /              \  Unit Tests
  /----------------\  (many, fast, cheap to maintain)
```

| Layer | Scope | Tool | Target Coverage |
|---|---|---|---|
| **Unit** | Single function/class/component in isolation | Jest, Vitest, xUnit, NUnit | > 80% of business logic |
| **Integration** | Service-to-service, DB interaction, API endpoints | Jest + Supertest, WebApplicationFactory, Testcontainers | All API endpoints |
| **E2E** | Full user journey through the UI | Playwright, Cypress | All critical user flows |
| **Contract** | API schema validation between producer/consumer | Pact, Schemathesis | All external service contracts |

---

## Best Practices

### Test Design Principles

- **Arrange-Act-Assert (AAA)**: structure every test with a clear setup, execution, and assertion phase. Use blank lines to separate phases visually.
- **One logical assertion per test**: a test should validate one behavior. Multiple technical assertions (e.g., checking multiple fields of one result) are fine if they describe the same behavior.
- **Test behavior, not implementation**: test what the code does, not how it does it. Don't assert on private internals.
- **Descriptive test names**: `should return 400 when email is missing` is better than `test1` or `validateEmailTest`.
- **No test dependencies**: tests must be runnable in any order. No shared mutable state between tests. Use `beforeEach` to reset state.
- **Deterministic tests**: no flakiness. If a test fails intermittently, it is a production bug or a bad test — fix it immediately.

### Unit Tests

```typescript
// Good: AAA structure, descriptive name, tests behavior
describe('AppointmentService', () => {
  it('should throw ConflictException when slot is already booked', async () => {
    // Arrange
    const repository = createMock<AppointmentRepository>();
    repository.findBySlot.mockResolvedValue(existingAppointment);
    const service = new AppointmentService(repository);

    // Act & Assert
    await expect(
      service.book({ clinicianId: 'c1', slotId: 's1', patientId: 'p1' })
    ).rejects.toThrow(ConflictException);
  });
});
```

```csharp
// .NET: xUnit + FluentAssertions
[Fact]
public async Task Book_WhenSlotAlreadyTaken_ThrowsConflictException()
{
    // Arrange
    var repo = Substitute.For<IAppointmentRepository>();
    repo.FindBySlotAsync("s1").Returns(ExistingAppointment);
    var service = new AppointmentService(repo);

    // Act
    var act = () => service.BookAsync(new BookCommand("c1", "s1", "p1"));

    // Assert
    await act.Should().ThrowAsync<ConflictException>();
}
```

### Integration Tests

- Use **Testcontainers** for database integration tests — spin up a real database in Docker for the test run, tear down after.
- Test all **API endpoints** through the HTTP layer using `WebApplicationFactory` (ASP.NET Core) or `supertest` (Node.js).
- Seed test data in `beforeEach` / `beforeAll` — never use production or shared test data.
- Clean up test data in `afterEach` — use transactions that roll back, or truncate tables.

```typescript
// Node.js: Supertest integration test
describe('POST /api/appointments', () => {
  it('should return 201 with created appointment', async () => {
    const response = await request(app)
      .post('/api/appointments')
      .set('Authorization', `Bearer ${testToken}`)
      .send({ clinicianId: clinician.id, slotId: slot.id, patientId: patient.id });

    expect(response.status).toBe(201);
    expect(response.body).toMatchObject({
      id: expect.any(String),
      status: 'CONFIRMED',
    });
  });
});
```

### E2E Tests

- Write E2E tests for **critical user journeys** only: login, registration, core business flows, payment/checkout.
- Use the **Page Object Model (POM)**: encapsulate page interactions in classes; tests should not contain raw Playwright/Cypress selectors.
- Use **test-specific data attributes** (`data-testid`) for selectors — do not use CSS classes or text content that may change.
- Parallelize E2E tests to keep the suite under 10 minutes.

```typescript
// Playwright: Page Object Model
class AppointmentPage {
  constructor(private page: Page) {}
  
  async bookAppointment(clinician: string, slot: string) {
    await this.page.getByTestId('clinician-select').selectOption(clinician);
    await this.page.getByTestId(`slot-${slot}`).click();
    await this.page.getByTestId('confirm-booking').click();
  }
  
  async getConfirmationMessage() {
    return this.page.getByTestId('booking-confirmation').textContent();
  }
}
```

### Flaky Test Prevention

- Avoid `setTimeout` / `sleep` in tests. Use proper async/await and framework-provided waiters.
- Use Playwright's auto-waiting — do not manually add waits for elements to appear.
- Isolate tests completely: parallel test runs should not share database state.
- Run the test suite 3× on each PR to detect intermittent failures before merge.

---

## Coverage Targets

| Layer | Minimum Target | Measured By |
|---|---|---|
| Business logic (services) | > 80% line coverage | Jest/Vitest/xUnit coverage reports |
| API controllers/endpoints | 100% (all endpoints have at least one integration test) | Manual inventory |
| Critical user paths | 100% (all happy + sad paths) | E2E test inventory |
| External service contracts | 100% of contracts tested | Contract test report |

Coverage targets are enforced in CI — a build that drops below the threshold should fail.

---

## Test Data Management

- Use **factories or builders** (factory-boy, AutoFixture, fishery) to create test data — not hardcoded objects.
- Keep **test data close to the test** — in the test file or a local fixture, not a shared global fixture file.
- Use **realistic but synthetic data**: real-looking names, emails, phone numbers using Faker — never real personal data.
- Define a **data seeder** for E2E tests that provisions known entities in the test environment before the suite runs.

---

## CI/CD Integration

Every PR must pass:

```yaml
# CI quality gate
test_pipeline:
  - step: unit_tests
    command: npm run test:unit -- --coverage
    fail_on_coverage_below: 80
  - step: integration_tests
    command: npm run test:integration
    requires: [docker]
  - step: e2e_tests
    command: npx playwright test
    environment: staging
  - step: flaky_detection
    command: npm run test:unit
    repeat: 3
    fail_on_any_failure: true
```

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Testing implementation details | Tests break on refactor even if behavior is correct | Test public behavior and output |
| Shared mutable test state | Tests interfere with each other | `beforeEach` reset; isolated data per test |
| `sleep()` / arbitrary waits | Slow and still flaky | Framework async waiters |
| Giant test (tests 10 behaviors) | Hard to diagnose failures | One behavior per test |
| Mocking everything including the unit under test | No real behavior tested | Only mock external dependencies |
| Deleting flaky tests | Problem hidden, not solved | Fix the flakiness root cause |
| No E2E tests for critical paths | Regressions in user journeys | E2E coverage for every critical flow |
| Coverage theatre (meaningless assertions) | High coverage number, no quality | Assertions must verify real behavior |

---

## Scrum Team Collaboration

The Automation QA Specialist plays two roles in the Scrum Team:

1. **Implementer**: writes tests in parallel with implementation specialists.
2. **Reviewer**: in the verification phase, validates coverage, reviews test quality, and runs the full test suite.

### Handoff Expectations

The QA Specialist expects from implementation specialists:
- Testable code: pure functions, dependency injection, no hidden globals.
- API contract: endpoint list and request/response schemas.
- Acceptance criteria: what defines "done" for the task.

### Handoff from QA to next phase

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "test_coverage_report"
      description: "Coverage summary from test run"
    - type: "e2e_results"
      description: "Playwright test run report"
  metrics:
    unit_coverage: "87%"
    integration_endpoints_covered: "12/12"
    e2e_critical_flows_covered: "5/5"
    flaky_tests: 0
  verdict: "PASS"
  handoff_to: ["app-security", "performance-qa"]
```

---

## Verification Checklist

- [ ] Unit tests written for all new business logic (> 80% coverage)
- [ ] Integration tests cover all new API endpoints
- [ ] E2E tests cover all new critical user flows
- [ ] No flaky tests introduced
- [ ] `data-testid` attributes added for all new interactive UI elements
- [ ] Test data uses factories/builders — no hardcoded real PII
- [ ] CI pipeline passes with coverage threshold enforced
- [ ] Test names are descriptive and follow the convention
