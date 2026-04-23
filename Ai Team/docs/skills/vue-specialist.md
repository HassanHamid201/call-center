# Vue Specialist — Best Practices

## Overview

The Vue Specialist builds and maintains frontend applications using Vue 3 with TypeScript. It handles component composition, reactive state management, routing, testing, and accessibility.

**Skill ID:** `vue-specialist`
**Auto-detected from:** `vue` in `package.json`
**Always paired with:** `app-security` (review), `automation-qa` (review)

---

## Core Technologies

| Category | Technology | Version |
|---|---|---|
| UI Library | Vue | 3+ (Composition API) |
| Language | TypeScript | 5+ strict mode |
| Build Tool | Vite | 5+ |
| State | Pinia | 2+ |
| Routing | Vue Router | 4+ |
| Forms | VeeValidate + Zod | Latest |
| HTTP Client | Axios / TanStack Query for Vue | Latest |
| Testing (unit) | Vitest + Vue Test Utils | Latest |
| Testing (e2e) | Playwright | Latest |
| Linting | ESLint + `@vue/eslint-config-typescript` | Latest |
| Formatting | Prettier | Latest |

---

## Best Practices

### Component Design

- Use the **Composition API with `<script setup>`** syntax exclusively. Avoid Options API in new code.
- Keep components focused on a single responsibility. Extract logic into composables.
- Use **`defineProps` with TypeScript generics** for type-safe props — never use runtime-only prop definitions without types.
- Use **`defineEmits` with type declarations** to make events self-documenting.
- Name components with **PascalCase** in `<script>` and **kebab-case** in templates.

```vue
<script setup lang="ts">
interface Props {
  patient: Patient;
  isSelected?: boolean;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  select: [id: string];
}>();
</script>

<template>
  <article
    :class="{ selected: isSelected }"
    :aria-selected="isSelected"
    @click="emit('select', patient.id)"
  >
    <h2>{{ patient.nameAr }}</h2>
  </article>
</template>
```

### Composables

- Extract reusable logic into composables (`usePatientSearch.ts`, `useAuthSession.ts`).
- Composable files live in `src/composables/`. Name them `useX`.
- Composables must be side-effect free during definition. Use `onMounted`, `watchEffect`, `onUnmounted` for lifecycle-aware logic.
- Return reactive refs and computed properties from composables — not plain values.

### Reactivity

- Prefer `ref()` for primitive values and `reactive()` for objects that you'd destructure (use `toRefs()` when destructuring to preserve reactivity).
- Use `computed()` for derived state — never re-compute in the template.
- Use `watchEffect()` for effects that should re-run when dependencies change automatically. Use `watch()` when you need to compare old and new values.
- **Never mutate props** — use emits to communicate changes to the parent.

### State Management (Pinia)

- One Pinia store per domain (`usePatientStore`, `useAuthStore`).
- Keep store state minimal — only data shared across multiple components.
- Define all store actions as `async` functions. Handle errors inside actions, not in components.
- Use `storeToRefs()` to destructure store state while preserving reactivity.
- Never access one store from another store's state — use actions to coordinate.

### Forms (VeeValidate + Zod)

- Define Zod schemas for all forms. Share schemas with backend when using a monorepo.
- Use `useForm` from VeeValidate with the Zod adapter.
- Show field-level error messages inline using `ErrorMessage` component.
- Disable the submit button while the form is submitting (`isSubmitting` from `useForm`).

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Correct Approach |
|---|---|---|
| Options API in new components | Harder to tree-shake, worse TypeScript inference | Composition API with `<script setup>` |
| Mutating props directly | Violates one-way data flow, causes warnings | Emit events to parent |
| Storing server data in Pinia | Duplicates cache, stale data issues | Use TanStack Query for Vue for server state |
| Reactive object destructuring without `toRefs` | Loses reactivity silently | Always use `toRefs()` or `storeToRefs()` |
| Business logic in templates | Untestable, unreadable | Move to composables or computed properties |
| `v-html` with untrusted content | XSS vulnerability | Sanitize with DOMPurify or avoid entirely |
| `any` in TypeScript | Defeats type safety | Use `unknown`, generics, or proper interfaces |
| Deeply nested component trees passing data via props | Prop drilling, fragile | Use Pinia or provide/inject |

---

## Security

- **`v-html` restriction**: never use `v-html` with user-supplied content. If unavoidable, sanitize with DOMPurify first.
- **CSRF protection**: use CSRF tokens on forms. Axios interceptors can inject them automatically.
- **Auth token storage**: tokens in memory (Pinia store) + refresh token in `httpOnly` cookie. Never `localStorage`.
- **Input validation**: all form data validated client-side with Zod before sending. Server must re-validate — client validation is UX only.
- **CSP headers**: configure Content Security Policy headers in server/Vite config to restrict script sources.

---

## Performance

- Use **`v-once`** for static content that never changes after initial render.
- Use **`v-memo`** to skip subtree re-renders when dependencies haven't changed (Vue 3.2+).
- Lazy-load routes with `defineAsyncComponent` and dynamic `import()`.
- Virtualize long lists with `@tanstack/vue-virtual` or `vue-virtual-scroller`.
- Use **`shallowRef`** and **`shallowReactive`** for large data structures where deep reactivity is unnecessary.
- Profile with Vue DevTools (Performance tab) before optimizing — measure first.

---

## Testing

### Strategy: Test Pyramid

- **Unit (70%)**: composables, stores, utility functions, individual components in isolation
- **Integration (20%)**: multi-component flows, form submissions, Pinia store integration
- **E2E (10%)**: critical user journeys with Playwright

### Unit & Integration Tests (Vitest + Vue Test Utils)

- Mount components with `mountedComponent` (full mount) or `shallowMount` (when children are irrelevant).
- Use `getByRole`, `getByLabelText`, `getByText` from `@testing-library/vue` for queries.
- Mock Pinia stores using `createTestingPinia` from `@pinia/testing`.
- Mock HTTP calls with MSW (Mock Service Worker) — do not mock axios directly.
- Target **> 80% line coverage** on new code.

```typescript
// Good — tests user-visible behavior
test("shows error when national ID is invalid", async () => {
  const wrapper = mount(PatientRegistrationForm, {
    global: { plugins: [createTestingPinia()] }
  });
  await wrapper.find('[aria-label="National ID"]').setValue("123");
  await wrapper.find('[type="submit"]').trigger("click");
  expect(wrapper.text()).toContain("National ID must be 10 digits");
});
```

### E2E Tests (Playwright)

- Test only critical paths: registration, login, key CRUD operations.
- Use Page Object Model for maintainable selectors.
- Run against a staging environment in CI with seeded test data.

---

## Code Quality

- **ESLint**: `@vue/eslint-config-typescript`, `vue/no-v-html`, `vue/require-explicit-emits`.
- **Prettier**: `"singleQuote": true, "semi": false`.
- **File naming**: components `PascalCase.vue`; composables `useX.ts`; stores `useXStore.ts`.
- **Imports**: use Vite's `@/` alias for absolute imports from `src/`.

### Review Checklist

- [ ] All components use `<script setup lang="ts">`
- [ ] All props typed with `defineProps<T>()`
- [ ] All emits typed with `defineEmits<...>()`
- [ ] No `v-html` with untrusted content
- [ ] No `localStorage` for auth tokens
- [ ] Pinia stores used for shared state only
- [ ] Forms use VeeValidate + Zod
- [ ] Test coverage > 80% on new code

---

## Scrum Team Collaboration

### Receiving from Upstream Specialists

- **PostgreSQL / backend specialist**: receive the Handoff Package with API contract (endpoints, input/output shapes, error codes) before starting API integration. Use MSW to mock the API during parallel development.

### Providing to Downstream Specialists

```yaml
task_completion_signal:
  status: "READY_FOR_HANDOFF"
  artifacts:
    - type: "component_props_interfaces"
      path: "src/types/patient.ts"
    - type: "api_integration"
      description: "Endpoints used and expected response shapes"
  handoff_to: ["app-security", "automation-qa"]
```

### Working in Parallel

- Agree on API contract with backend specialist before the batch starts (use OpenAPI spec if available).
- Mock all external API dependencies with MSW to remain independent during the batch.
- Do not modify shared `package.json` or `vite.config.ts` without Scrum Master coordination.

---

## Verification Checklist

- [ ] `npm run build` exits 0
- [ ] `npm run lint` exits 0 (no ESLint errors)
- [ ] `npx vue-tsc --noEmit` exits 0
- [ ] `npm test -- --coverage` exits 0 with coverage ≥ 80%
- [ ] No `v-html` on untrusted content
- [ ] No `localStorage` / `sessionStorage` for auth tokens
- [ ] All form fields have associated labels
- [ ] All interactive elements keyboard accessible
- [ ] WCAG 2.1 AA compliance verified
