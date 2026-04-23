# .NET MVC Specialist — Best Practices

## Overview

The .NET MVC Specialist builds server-side rendered web applications using ASP.NET Core MVC with Razor Pages/Views. It handles controller design, Razor templating, model binding, validation, and server-rendered UI patterns.

**Skill ID:** `net-mvc-specialist`
**Auto-detected from:** `.csproj` with MVC references (`Microsoft.AspNetCore.Mvc`)
**Always paired with:** `app-security` (review), `automation-qa` (review)

---

## Core Technologies

| Category | Technology | Version |
|---|---|---|
| Framework | ASP.NET Core MVC | .NET 8/9/10 |
| Language | C# | 12+ |
| Templating | Razor Pages / Razor Views | Latest |
| Validation | Data Annotations + FluentValidation | Latest |
| ORM | Entity Framework Core | 8+ |
| Client scripting | Alpine.js / vanilla JS | Latest |
| Styling | Bootstrap 5 / Tailwind CSS | Latest |
| Testing | xUnit + Moq + AngleSharp | Latest |
| Auth | ASP.NET Core Identity / Cookie Auth | Latest |

---

## Best Practices

### Controller Design

- Keep controllers **thin**: validate input, call a service, return a result. No business logic in controllers.
- Use **attribute routing** consistently. Do not mix convention-based and attribute routing.
- One controller per aggregate root or feature area. Group related actions in the same controller.
- Use **[BindingSource] attributes** explicitly (`[FromBody]`, `[FromRoute]`, `[FromQuery]`) to make binding intent clear.
- Return typed `IActionResult` subtypes (`Ok`, `NotFound`, `BadRequest`, `View`, `RedirectToAction`) — never `object`.

```csharp
[Route("patients")]
public class PatientController : Controller
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
        => _patientService = patientService;

    [HttpGet("{id}")]
    public async Task<IActionResult> Detail([FromRoute] Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null) return NotFound();
        return View(patient);
    }
}
```

### Razor Views & Partial Views

- Use **strongly typed views** (`@model PatientViewModel`). Avoid `ViewBag` and `ViewData` — they are stringly typed and refactor-hostile.
- Extract repeated markup into **Partial Views** or **View Components**. Partial Views for stateless markup; View Components for markup with their own data requirements.
- Use **Tag Helpers** (`asp-for`, `asp-validation-for`, `asp-action`, `asp-controller`) instead of HTML helpers — they are cleaner and more readable.
- Use **Layouts** to enforce consistent page structure. Keep `_Layout.cshtml` free of feature-specific code.
- Apply **Display Templates** (`~/Views/Shared/DisplayTemplates/`) and **Editor Templates** (`~/Views/Shared/EditorTemplates/`) for reusable field rendering.

```html
<!-- Good: Tag Helpers -->
<form asp-controller="Patient" asp-action="Register" method="post">
    @Html.AntiForgeryToken()
    <div class="form-group">
        <label asp-for="NationalId"></label>
        <input asp-for="NationalId" class="form-control" />
        <span asp-validation-for="NationalId" class="text-danger"></span>
    </div>
    <button type="submit" class="btn btn-primary">Register</button>
</form>
```

### Model Binding & ViewModels

- Use **dedicated ViewModels** for each view — never pass domain entities directly to views.
- Validate ViewModels using **FluentValidation** (preferred) or Data Annotations. Call `ModelState.IsValid` in the action before processing.
- Use **AutoMapper** to map between ViewModels and domain entities. Configure profiles explicitly — avoid inline `ProjectTo` without a profile.

### Anti-Forgery

- **Always** include `@Html.AntiForgeryToken()` (or `<form ... >` with Tag Helpers, which auto-include it) in all state-changing forms.
- Apply `[ValidateAntiForgeryToken]` on all `POST`, `PUT`, `DELETE` actions.
- Configure global anti-forgery validation in `Program.cs` so it applies system-wide.

### Route Design

- Follow RESTful conventions: `GET /patients`, `GET /patients/{id}`, `POST /patients`, `PUT /patients/{id}`.
- Use **route constraints** (`:guid`, `:int`, `:alpha`) to reject invalid input at the routing layer.
- Avoid exposing internal IDs in URLs where possible; use slugs or opaque identifiers.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Business logic in controllers | Untestable, bloated controllers | Move to service layer |
| Passing domain entities to views | Exposes internals, over-posts | Use dedicated ViewModels |
| `ViewBag` / `ViewData` | Stringly typed, refactor-hostile | Strongly typed `@model` |
| Missing `[ValidateAntiForgeryToken]` | CSRF vulnerability | Apply globally or on every state-changing action |
| Synchronous database calls | Blocks thread pool | Always `async/await` with EF Core |
| Catching `Exception` generically | Swallows errors silently | Catch specific exceptions; use middleware for unhandled |
| HTML helpers instead of Tag Helpers | Verbose, harder to read | Use Tag Helpers consistently |
| Rendering user input without encoding | XSS vulnerability | Razor auto-encodes; never use `@Html.Raw` with user data |

---

## Security

- **CSRF**: `[ValidateAntiForgeryToken]` on all POST/PUT/DELETE actions. Configure globally in `Program.cs`.
- **XSS**: Razor auto-encodes HTML output by default. **Never** use `@Html.Raw` with user-supplied input.
- **Authentication**: use ASP.NET Core Identity or cookie authentication with `SlidingExpiration`. Configure `CookieSecurePolicy.Always` and `SameSiteMode.Strict`.
- **Authorization**: apply `[Authorize]` at the controller level, then `[AllowAnonymous]` on specific actions. Prefer policy-based authorization over role strings.
- **Input validation**: validate at both client and server side. Never trust client-side validation alone.
- **HTTP security headers**: configure HSTS, X-Content-Type-Options, X-Frame-Options in middleware.
- **Sensitive data**: never log passwords, tokens, or PII. Use `[DataType(DataType.Password)]` to prevent logging of password fields.

---

## Performance

- Use `async/await` for all I/O operations. Never use `.Result` or `.Wait()` on Tasks in request handling code.
- Use **response caching** (`[ResponseCache]`) for read-heavy pages that change infrequently.
- Use **output caching** (ASP.NET Core 7+) for fine-grained caching by route or query parameters.
- Bundle and minify CSS/JS assets using the **Web Optimizer** or Vite as a build step.
- Use **tag helper caching** (`<cache>`) for expensive partial views.
- Enable **response compression** middleware for text-based content.

---

## Testing

### Strategy

- **Unit tests (60%)**: controllers (mocked services), ViewModels, validators, service layer
- **Integration tests (30%)**: full request pipeline with `WebApplicationFactory<Program>`, rendered HTML assertions with AngleSharp
- **E2E tests (10%)**: critical flows with Playwright

### Unit Tests (xUnit + Moq)

- Test controllers by mocking service dependencies with Moq.
- Assert on `IActionResult` type and view model contents.
- Test FluentValidation validators directly with `TestValidate`.

```csharp
[Fact]
public async Task Detail_ReturnsNotFound_WhenPatientDoesNotExist()
{
    var mockService = new Mock<IPatientService>();
    mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);
    var controller = new PatientController(mockService.Object);

    var result = await controller.Detail(Guid.NewGuid());

    Assert.IsType<NotFoundResult>(result);
}
```

### Integration Tests (WebApplicationFactory + AngleSharp)

- Use `WebApplicationFactory<Program>` to spin up a real in-process server.
- Parse rendered HTML with AngleSharp to assert on DOM elements.
- Seed a test database with known data; reset between tests.
- Test form submissions end-to-end: submit the form, assert on redirect or validation message.

---

## Code Quality

- **Naming**: controllers `XController`; ViewModels `XViewModel`; validators `XValidator`; services `IXService` + `XService`.
- **Folder structure**: `Controllers/`, `Models/ViewModels/`, `Services/`, `Validators/`, `Views/`.
- **Async all the way**: no synchronous wrappers over async code.
- **Nullable reference types**: enable `<Nullable>enable</Nullable>` in project file.

### Review Checklist

- [ ] All `POST`/`PUT`/`DELETE` actions have `[ValidateAntiForgeryToken]`
- [ ] All views use strongly typed `@model` (no `ViewBag`)
- [ ] No `@Html.Raw` with user input
- [ ] All DB calls are `async/await`
- [ ] FluentValidation validates all ViewModels
- [ ] Controllers contain no business logic
- [ ] Test coverage ≥ 80% on new code

---

## Scrum Team Collaboration

### Receiving from Upstream Specialists

- **PostgreSQL / SQL Server Specialist**: receive Handoff Package with connection string path, schema summary, and any stored procedures to call.
- **.NET Core Specialist** (if coexisting): coordinate on shared `DbContext`, migrations, and service contracts to avoid conflicts.

### Providing to Downstream Specialists

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "routes"
      description: "URL routes and controller actions implemented"
    - type: "viewmodel_contracts"
      description: "ViewModels with properties and validation rules"
      path: "Models/ViewModels/"
  handoff_to: ["app-security", "automation-qa"]
  notes: "Anti-forgery configured globally; forms all include token"
```

### Working in Parallel

- If a batch includes both a database specialist and .NET MVC specialist, coordinate on shared `ApplicationDbContext` modifications through Scrum Master to avoid migration conflicts.
- Do not touch `Program.cs` or `appsettings.json` simultaneously — nominate one specialist or sequence these changes.

---

## Verification Checklist

- [ ] `dotnet build` exits 0
- [ ] `dotnet test` exits 0
- [ ] `dotnet format --verify-no-changes` exits 0
- [ ] All state-changing actions protected with `[ValidateAntiForgeryToken]`
- [ ] No `@Html.Raw` with untrusted input
- [ ] Anti-forgery configured globally in `Program.cs`
- [ ] Authentication configured with `CookieSecurePolicy.Always`
- [ ] HSTS and security headers configured
- [ ] Test coverage ≥ 80% on new code
