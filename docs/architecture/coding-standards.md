# Coding Standards

## C# / Backend

### General
- Use `file-scoped namespaces`
- Prefer `record` for DTOs and immutable data
- Use `nullable` reference types enabled
- XML documentation comments required on all public APIs

### Naming
- `PascalCase` for classes, methods, properties, enums
- `camelCase` for local variables and parameters
- `_camelCase` for private fields
- `IPascalCase` for interfaces

### API Design
- Return `ProblemDetails` (RFC 7807) for all errors
- Use `ActionResult<T>` for controller return types
- Paginate all list endpoints with `page` and `pageSize`
- Use `[ProducesResponseType]` attributes for Swagger documentation

### Data Access
- Use `AsNoTracking()` for read-only queries
- Use eager loading (`Include`) sparingly; prefer projection with `Select`
- Migrations must be reviewed before applying to shared environments

## TypeScript / Frontend

### General
- Strict mode enabled
- Prefer `interface` over `type` for object shapes
- Use functional components with hooks

### Naming
- `PascalCase` for components, interfaces, types
- `camelCase` for functions, variables, hooks
- `UPPER_SNAKE_CASE` for constants

### Components
- One component per file
- Co-locate related sub-components in folder
- Use `export default` for page components
- Use named exports for shared components

### Styling
- Use Tailwind utility classes exclusively
- Extract common patterns to `@layer components` in `index.css`
- Use `clsx` + `tailwind-merge` for conditional classes

## Git

### Commits
- Use conventional commits format
- One logical change per commit
- Reference task IDs in commit messages when applicable

### Branches
- `main` - production-ready code
- `feature/*` - new features
- `fix/*` - bug fixes
