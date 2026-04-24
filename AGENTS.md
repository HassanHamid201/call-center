# Tadawi Medical Group — Call Center Portal

## Project Overview

React 19 + Vite frontend with a .NET 10 backend. Arabic-first (RTL) medical call center portal for Tadawi Medical Group with admin dashboard, patient lookup, doctor directory, branch directory, FAQ, and reference data management.

**GitHub:** https://github.com/HassanHamid201/call-center

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | React 19, Vite 6, TypeScript, Tailwind CSS v3 |
| State | Zustand (auth), TanStack Query v5 (server state) |
| Routing | React Router v6 |
| i18n | react-i18next, i18next-http-backend, browser-languagedetector |
| Icons | Lucide React |
| Backend | .NET 10, ASP.NET Core Web API |
| ORM | Entity Framework Core 10 |
| Auth | JWT Bearer tokens (HS256) |
| Database | 4x SQLite files (bounded contexts) |

---

## Running Locally

### Backend
```bash
cd backend/src/Api
dotnet run --urls "http://0.0.0.0:5000"
# Binds to 0.0.0.0:5000 for LAN access
```

### Frontend
```bash
cd frontend
npm run dev
# Binds to 0.0.0.0:3000, proxies /api to localhost:5000
```

### LAN Access
- Frontend: `http://192.168.30.222:3000`
- Backend: `http://192.168.30.222:5000`

### Default Credentials
```
Email:    admin@tadawi.med
Password: Admin123!
Role:     Admin
```

---

## Multi-Database Architecture

| Database File | DbContext | Entities |
|---------------|-----------|----------|
| `tadawi_auth.db` | `AuthDbContext` | User, AuditLog |
| `tadawi_medical.db` | `MedicalDbContext` | Branch, Doctor, Specialty, Sector, FaqItem |
| `tadawi_patients.db` | `PatientDbContext` | Patient, Appointment, Visit, MedicalHistory |
| `tadawi_reference.db` | `ReferenceDbContext` | Nationality, InsuranceOption, Classification, AvailabilityStatus, ClinicMechanism, Coordinator, WorkingHour, WorkingDay, AgeGroup, ServiceCatalog |

**Note:** Cross-context references are logical only (store Guid as string, no FK enforcement across DB files). All contexts call `EnsureCreated()` on startup.

**Old database:** `tadawi.db` is retired and removed from version control.

---

## Backend Auth & Authorization

### JWT Configuration
- Secret from `Jwt:Secret` in appsettings
- Expiration: `Jwt:ExpirationMinutes`
- Issuer/Audience validated

### Role Enum (`Domain/Enums/UserRole.cs`)
```csharp
Admin, Manager, CallCenterAgent, Receptionist, Marketing
```

### Controller Authorization
| Controller | Class-level | Mutating Actions | Delete |
|------------|-------------|------------------|--------|
| AuthController | None | `register` → `[Authorize(Roles="Admin")]` | — |
| AuthController.me | `[Authorize]` | — | — |
| AdminController | `[Authorize(Roles="Admin")]` | — | — |
| AuditController | `[Authorize(Roles="Admin")]` | — | — |
| BranchesController | `[Authorize]` | `[Authorize(Roles="Admin,Manager")]` | `[Authorize(Roles="Admin")]` |
| DoctorsController | `[Authorize]` | `[Authorize(Roles="Admin,Manager")]` | `[Authorize(Roles="Admin")]` |
| PatientsController | `[Authorize]` | — | — |
| ReferencesController | `[Authorize]` | `[Authorize(Roles="Admin,Manager")]` | `[Authorize(Roles="Admin")]` |
| SearchController | `[Authorize]` | — | — |

---

## Frontend Auth System

### Auth Store (`src/store/authStore.ts`)
Uses **Zustand persist middleware** to sync with localStorage automatically:
```ts
import { useAuthStore } from '@/store/authStore'
const { user, token, isAuthenticated, setAuth, logout } = useAuthStore()
```

**Key rule:** Never use `localStorage.getItem('token')` directly. Always read from the store.

### API Client (`src/api/client.ts`)
- Reads token from `useAuthStore.getState().token` for request interceptor
- On 401 response: calls `useAuthStore.getState().logout()` then redirects to `/login`
- Base URL: `/api` (proxied by Vite to localhost:5000)

### Route Guards (`src/components/ProtectedRoute.tsx`)
- All non-login routes wrapped in `<ProtectedRoute>`
- Admin routes use `<ProtectedRoute requiredRole="Admin">`
- Unauthenticated users → redirect to `/login` (preserves `from` location)
- Non-Admin users on admin routes → show unauthorized message

### Mock API
Mock API is **disabled by default** via `VITE_ENABLE_MOCK_API=false` in `.env`.
Only enable when explicitly needed for frontend-only development without backend.

---

## Frontend Routes

### Public
| Route | Page |
|-------|------|
| `/login` | LoginPage |

### Protected (any authenticated user)
| Route | Page |
|-------|------|
| `/` | Dashboard |
| `/branches` | BranchList |
| `/branches/:id` | BranchDetail |
| `/doctors` | DoctorList |
| `/doctors/:id` | DoctorDetail |
| `/search` | SearchPage |
| `/faq` | FaqPage |
| `/patients` | PatientLookup |
| `/patients/:id` | PatientDetail |

### Admin-only
| Route | Page |
|-------|------|
| `/admin` | AdminDashboard |
| `/admin/users` | UserManagement |
| `/admin/audit` | AuditLogs |
| `/admin/branches` | BranchManagement |
| `/admin/doctors` | DoctorManagement |
| `/admin/specialties` | SpecialtyManagement |
| `/admin/sectors` | SectorManagement |
| `/admin/faq` | FaqManagement |
| `/admin/references` | ReferenceManagement |
| `/admin/patients` | PatientManagement |

---

## i18n System

- Default language: **Arabic** (`ar`)
- Fallback: Arabic
- Supported: `ar`, `en`
- Translation files: `public/locales/{lng}/translation.json`
- Language switcher in Header
- `useDirection()` hook syncs `<html dir>` and `<html lang>` with i18next

### Logical Tailwind Utilities
Use logical properties for RTL/LTR compatibility:
- `ms-*` (margin-start) instead of `ml-*` / `mr-*`
- `me-*` (margin-end)
- `ps-*` / `pe-*` for padding
- `text-start` / `text-end` for text alignment

### Backend Enum Mappings (`src/i18n/mappings.ts`)
Backend stores Arabic enum values. Frontend maps them to English for display:
```ts
classificationMap = { 'أستشاري': 'Consultant', 'أخصائي': 'Specialist', ... }
availabilityMap = { 'متواجد': 'Available', 'إجازة': 'On Leave', ... }
```
Use `tValue(value, type)` to translate backend values based on current language.

---

## Key Entities

### Patient
- FullName, Phone, IdentityNumber, FileNumber, DateOfBirth, Gender, Nationality, Address, Email

### Doctor (extended fields)
- Nationality, Classification, InsuranceAcceptance, InsuranceNotes, AvailabilityStatus, CoordinatorName, InternalExtension, WorkingHours, WorkingDays, AgeGroup, ConsultationFee, Services, ClinicMechanism, VacationStart, VacationEnd, Notes

### Reference Entities
All have: `Id`, `Name`, `IsActive`, `CreatedAt`

---

## Known Patterns & Conventions

### Backend
- DTOs in `Api/DTOs/`
- Controllers in `Api/Controllers/`
- Entities in `Domain/Entities/` (organized by context)
- DbContexts in `Infrastructure/Data/Contexts/`
- Seed data JSON files in `Api/SeedData/`

### Frontend
- Pages in `src/pages/{Feature}/`
- Shared components in `src/components/`
- Layout components in `src/components/Layout/`
- API client: `src/api/client.ts`
- Store: `src/store/authStore.ts`
- i18n config: `src/i18n/config.ts`
- Vite alias `@` maps to `./src`

---

## Git

**Remote:** `git@github.com:HassanHamid201/call-center.git`

**Do NOT commit:**
- SQLite `.db`, `.db-shm`, `.db-wal` files
- `.env` files
- `node_modules/`, `bin/`, `obj/`, `dist/`

---

## Active Services (last known)

| Service | PID | Port | Status |
|---------|-----|------|--------|
| Backend | varies | 5000 | Running |
| Frontend | varies | 3000 | Running |

To check:
```bash
lsof -i :5000  # backend
lsof -i :3000  # frontend
```

---

## Troubleshooting

### Auth redirect loop on refresh
**Cause:** Zustand state initialized to `false` before `init()` could read localStorage.
**Fix:** Uses `persist` middleware — auth restored on store creation, before any render.

### Frontend not hitting real backend
**Cause:** `enableMockApi()` was unconditionally called in `main.tsx`.
**Fix:** Mock is now conditional on `VITE_ENABLE_MOCK_API === 'true'`.

### DoctorManagement crash
**Cause:** `apiMock.ts` generic `/doctors/:id` route shadowed `/doctors/classifications`.
**Fix:** Specific routes moved before generic regex routes in mock handler.

### SearchController crash
**Cause:** SQLite cannot translate `.Take(3)` inside `.Select()`.
**Fix:** Materialize with `.ToListAsync()` first, then `.Select()` in memory.
