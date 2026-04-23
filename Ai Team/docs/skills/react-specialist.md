# React Specialist — Best Practices

## Overview

The React Specialist builds and maintains frontend applications using React with TypeScript. It handles component architecture, state management, performance optimization, accessibility, and testing.

**Skill ID:** `react-specialist`
**Auto-detected from:** `react`, `react-dom`, or `next` in `package.json`
**Always paired with:** `app-security` (review), `automation-qa` (review)

---

## Core Technologies

| Category | Technology | Version |
|---|---|---|
| UI Library | React | 16+ (prefer 18+) |
| Language | TypeScript | 5+ strict mode |
| Meta-framework | Next.js | 14+ (App Router) |
| State (global) | Redux Toolkit / Zustand | Latest |
| State (server) | TanStack Query (React Query) | v5+ |
| Forms | React Hook Form + Zod | Latest |
| Testing (unit) | Jest + React Testing Library | Latest |
| Testing (e2e) | Playwright | Latest |
| Linting | ESLint + TypeScript-ESLint | Latest |
| Formatting | Prettier | Latest |
| Styling | Tailwind CSS / CSS Modules | Latest |

---

## Best Practices

### Component Design

- Use **functional components with hooks** exclusively. No class components.
- Follow the **Single Responsibility Principle**: one component, one concern.
- Apply **Container/Presentational separation**: containers fetch data and manage state; presentational components receive props and render UI.
- Use **compound component pattern** for complex UI with shared state (e.g., tabs, accordions, dropdowns).
- Name components with **PascalCase**; name files to match the component name.
- Co-locate component files: `Button/Button.tsx`, `Button/Button.test.tsx`, `Button/Button.module.css`.

```tsx
// Good — single responsibility, typed props
interface PatientCardProps {
  patient: Patient;
  onSelect: (id: string) => void;
}

export function PatientCard({ patient, onSelect }: PatientCardProps) {
  return (
    <article onClick={() => onSelect(patient.id)} aria-label={patient.nameEn}>
      <h2>{patient.nameAr}</h2>
      <p>{patient.nationalId}</p>
    </article>
  );
}
```

### TypeScript

- Enable `strict: true` in `tsconfig.json`. No exceptions.
- Never use `any`. Use `unknown` and type-narrow. Use generics for reusable utilities.
- Define explicit interfaces/types for all props, API responses, and state shapes.
- Use `as const` for fixed value sets instead of enums where possible.
- Export types from a central `types/` directory for cross-component sharing.

### State Management

- **Local state** (`useState`, `useReducer`): component-scoped data.
- **Server state** (TanStack Query): all API data — never store API responses in Redux.
- **Global client state** (Zustand or Redux Toolkit): cross-page UI state, auth tokens, user preferences.
- Never duplicate server state in global client state — use TanStack Query cache as the single source of truth for server data.
- Keep Redux slices small and domain-focused. Avoid a single monolithic slice.

### Hooks

- Extract all business logic into **custom hooks** (`usePatientSearch`, `useAuthSession`).
- Custom hooks must be pure: no side effects outside `useEffect`, no direct DOM access outside `useRef`.
- Name custom hooks with the `use` prefix always.
- Keep the dependency arrays of `useEffect`, `useMemo`, `useCallback` exhaustive. Use `eslint-plugin-react-hooks`.

### Forms

- Use **React Hook Form** for all forms. Never manage form state manually with `useState`.
- Use **Zod** schemas for validation. Share the same schema between frontend and backend (export from a shared package if monorepo).
- Show validation errors inline, adjacent to the field — not only on submit.
- For multi-step forms, use `useFormContext` from React Hook Form to share form state across steps without prop drilling.

### Routing (Next.js App Router)

- Use **Server Components** by default. Only add `"use client"` when needed (event handlers, browser APIs, hooks).
- Use **dynamic segments** and **route groups** to organize the file system.
- Handle errors with `error.tsx` and loading states with `loading.tsx` at the appropriate segment level.
- Use **Server Actions** for mutations — avoid creating API routes just to proxy to the backend.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| `dangerouslySetInnerHTML` | XSS vulnerability | Use `textContent`, DOMPurify if HTML is unavoidable |
| Storing JWT in `localStorage` | Accessible to JS, vulnerable to XSS | Store in `httpOnly` cookie or in-memory only |
| Using `useEffect` to sync state | Creates cascading re-renders | Derive state during render; use event handlers for mutations |
| Prop drilling more than 2 levels | Fragile, hard to maintain | Use Context, Zustand, or component composition |
| `any` in TypeScript | Defeats type safety | Use `unknown`, generics, or proper interfaces |
| Direct DOM manipulation | Breaks React's reconciliation | Use `useRef` and `useEffect` properly |
| Fetching in `useEffect` on component mount | Race conditions, no caching | Use TanStack Query |
| One huge component file | Hard to test and maintain | Split into sub-components + custom hooks |
| Skipping `key` prop in lists | Silent rendering bugs | Always provide stable, unique `key` props |

---

## Security

- **XSS prevention**: never use `dangerouslySetInnerHTML`. If rendering user-provided HTML is required, sanitize with DOMPurify before rendering.
- **CSRF protection**: include CSRF tokens in all forms that mutate server state. Use `sameSite: strict` cookies.
- **Auth token storage**: store JWTs in memory (React state) + refresh token in `httpOnly` cookie. Never in `localStorage` or `sessionStorage`.
- **Input validation**: validate all user input client-side with Zod before sending to the server. Never trust client-side validation alone — server must re-validate.
- **Dependency security**: run `npm audit` as part of CI. Do not import from unknown CDN URLs.
- **Environment variables**: never expose server secrets in `NEXT_PUBLIC_*` variables. Prefix only values safe to expose to the browser.

---

## Performance

- Use **`React.memo`** on expensive presentational components that receive stable props.
- Use **`useMemo`** and **`useCallback`** only when profiling shows a bottleneck — premature memoization adds overhead.
- Use **`React.lazy` + `Suspense`** to code-split heavy routes and components (charts, editors, maps).
- Use **`Next.js Image`** component for all images — automatic WebP conversion, lazy loading, and size optimization.
- Use **`Next.js Font`** to eliminate layout shift from web fonts.
- Virtualize long lists with **`@tanstack/react-virtual`** or **`react-window`** (lists > 100 items).
- Monitor Core Web Vitals (LCP, CLS, INP) in development using Next.js Analytics or Lighthouse CI.

---

## Testing

### Strategy: Test Pyramid

- **Unit tests (70%)**: individual components, custom hooks, utility functions
- **Integration tests (20%)**: multi-component flows, form submissions, data fetching with mocked API
- **E2E tests (10%)**: critical user journeys (login, key form submissions)

### Unit & Integration Tests (Jest + React Testing Library)

- Test **behavior, not implementation**. Use `screen.getByRole`, `getByLabelText`, `getByText` — not CSS selectors or component internals.
- Avoid testing internal state directly. Test what the user sees and does.
- Mock API calls with **MSW (Mock Service Worker)** — avoid jest.mock for fetch/axios.
- Test accessibility: use `@testing-library/jest-dom` matchers like `toBeVisible`, `toHaveAccessibleName`.
- Target **> 80% line coverage**, but prioritize testing critical paths over hitting the percentage.

```tsx
// Good test — tests behavior, not implementation
test("shows validation error when national ID is invalid", async () => {
  render(<PatientRegistrationForm />);
  await userEvent.type(screen.getByLabelText(/national id/i), "123");
  await userEvent.click(screen.getByRole("button", { name: /register/i }));
  expect(screen.getByText(/national id must be 10 digits/i)).toBeVisible();
});
```

### E2E Tests (Playwright)

- Write E2E tests for critical user flows only: registration, login, key CRUD operations.
- Use **Page Object Model** to encapsulate selectors and actions.
- Run E2E tests in CI against a staging environment with test data.

---

## Code Quality

- **ESLint rules**: `react/hooks`, `react/jsx-no-target-blank`, `@typescript-eslint/no-explicit-any`, `jsx-a11y` plugin for accessibility.
- **Formatting**: Prettier with `"singleQuote": true, "semi": false, "printWidth": 100`.
- **Naming**: components PascalCase; hooks `useX`; event handlers `handleX`; constants UPPER_SNAKE_CASE.
- **Imports**: absolute imports via `tsconfig` paths (`@/components`, `@/lib`). No `../../../` chains.

### Review Checklist

- [ ] No `any` types
- [ ] All props have explicit TypeScript interfaces
- [ ] Forms use React Hook Form + Zod
- [ ] No `localStorage` JWT storage
- [ ] No `dangerouslySetInnerHTML`
- [ ] Accessibility: interactive elements have labels, focus management is correct
- [ ] Test coverage > 80% on new code
- [ ] No fetch calls in `useEffect` — uses TanStack Query

---

## Scrum Team Collaboration

### Receiving from Upstream Specialists

The React Specialist typically depends on:

- **PostgreSQL Specialist** (if building a form that needs to understand the schema) — receive: schema summary + data types
- **.NET Core / Node.js Specialist** — receive: API contract (endpoints, request/response shapes, error codes)

Wait for the Handoff Package from the Scrum Master before starting work that depends on an API that isn't mocked.

### Providing to Downstream Specialists

Signal `READY_FOR_HANDOFF` to the Scrum Master with:

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "component_contracts"
      description: "Props interfaces for all new components"
      path: "src/types/patient.ts"
    - type: "api_integration"
      description: "Which endpoints the UI calls, with expected shapes"
  handoff_to: ["app-security", "automation-qa"]
  notes: "Registration form uses POST /api/patients; expects 201 or 422 response"
```

### Working in Parallel

When working in the same batch as a backend specialist:

- Agree on the API contract in the Sprint Plan (or use an OpenAPI spec generated from the backend)
- Mock the API using MSW during development so work is independent
- Do not modify shared config files (`tsconfig.json`, `package.json`) without coordinating through the Scrum Master to avoid merge conflicts

---

## Verification Checklist

- [ ] `npm run build` exits 0
- [ ] `npm run lint` exits 0
- [ ] `npx tsc --noEmit` exits 0
- [ ] `npm test -- --coverage` exits 0 with coverage ≥ 80%
- [ ] No `dangerouslySetInnerHTML` usage
- [ ] No `localStorage` or `sessionStorage` for auth tokens
- [ ] All form fields have associated `<label>` elements
- [ ] All images use `alt` attributes
- [ ] Interactive elements are keyboard accessible
- [ ] WCAG 2.1 AA compliance verified (axe-core or Lighthouse)
- [ ] No `any` TypeScript types in new code
