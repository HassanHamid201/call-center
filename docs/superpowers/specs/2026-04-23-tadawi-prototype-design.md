# Tadawi Medical Group Prototype Design

**Date:** 2026-04-23  
**Status:** Approved for Implementation  
**Type:** Wireframe HTML Prototype with Dummy Data

---

## Overview

Single-page HTML prototype demonstrating three key user personas: Admin Portal, Call Center, and Marketing. Uses minimal wireframe-level styling with JavaScript-based interactions and client-side dummy data.

---

## Architecture

### Structure
- **Single HTML file** - SPA architecture with section-based navigation
- **Client-side state management** - Simple JavaScript object tracking current view, user role, and search state
- **Dummy data layer** - Static JavaScript arrays mirroring BRD entities
- **Event-driven interactions** - Search, filter, form submissions update UI without page reloads

### Component Organization
```
index.html
├── Header (global navigation, search bar, user menu)
├── Landing Section (persona selection)
├── Admin Portal Section
│   ├── User Management (list, create, edit)
│   ├── System Settings (basic config)
│   └── Audit Logs (view-only)
├── Call Center Section
│   ├── Quick Doctor Search (name, specialty, branch)
│   ├── Doctor Details (contact info, schedule)
│   └── Contact Actions (copy, print card)
└── Marketing Section
    ├── Branch Browser (by city, services)
    ├── Doctor Directory (credentials, specialties)
    └── Content Export (mock CSV download)
```

---

## Data Model (Dummy Data)

### Users
```javascript
const users = [
  { id: '1', username: 'omar', role: 'admin', name: 'Omar Al-Hassan', branch: 'Riyadh Main' },
  { id: '2', username: 'sarah', role: 'call_center', name: 'Sarah Ahmed', branch: 'Riyadh Main' },
  { id: '3', username: 'ahmed', role: 'marketing', name: 'Ahmed Rahman', branch: 'Jeddah Branch' }
]
```

### Branches
```javascript
const branches = [
  { id: '1', name: 'Tadawi Riyadh Main', city: 'Riyadh', status: 'active', services: ['Cardiology', 'Orthopedics', 'Pediatrics'], phone: '+966 11 234 5678' },
  { id: '2', name: 'Tadawi Jeddah', city: 'Jeddah', status: 'active', services: ['Dermatology', 'General Medicine'], phone: '+966 12 345 6789' },
  { id: '3', name: 'Tadawi Dammam', city: 'Dammam', status: 'maintenance', services: ['Ophthalmology'], phone: '+966 13 456 7890' }
]
```

### Doctors
```javascript
const doctors = [
  { id: '1', name: 'Dr. Khalid Al-Rashid', specialty: 'Cardiology', branch: 'Riyadh Main', status: 'active', phone: '+966 50 123 4567', email: 'khalid@tadawi.med' },
  { id: '2', name: 'Dr. Fatima Zahra', specialty: 'Pediatrics', branch: 'Riyadh Main', status: 'on_leave', phone: '+966 50 234 5678', email: 'fatima@tadawi.med' },
  { id: '3', name: 'Dr. Ahmed Mansour', specialty: 'Dermatology', branch: 'Jeddah', status: 'active', phone: '+966 50 345 6789', email: 'ahmed@tadawi.med' }
]
```

### Specialties
```javascript
const specialties = [
  { id: '1', name: 'Cardiology', sector: 'Medical', description: 'Heart and cardiovascular treatment' },
  { id: '2', name: 'Pediatrics', sector: 'Medical', description: 'Child healthcare and treatment' },
  { id: '3', name: 'Dermatology', sector: 'Medical', description: 'Skin conditions and treatment' }
]
```

---

## Core Features by Persona

### Admin Portal (Omar - IT Administrator)
- **User Management**: View all users, create new user (mock form), edit user details, deactivate users
- **System Settings**: Basic configuration toggles (maintenance mode, announcements)
- **Audit Logs**: Read-only view of recent system actions
- **Navigation**: Sidebar with admin-specific menu items

### Call Center (Sarah - Call Center Agent)
- **Quick Doctor Search**: Search by name, specialty, branch with instant filtering
- **Doctor Details**: Full profile view with contact information, availability status, schedule
- **Contact Actions**: "Copy Phone" button, "Copy Email" button, "Print Contact Card" (opens print dialog)
- **Recent Searches**: Display last 5 searches for quick access
- **Keyboard Shortcuts**: Ctrl+K for global search focus

### Marketing (Ahmed - Marketing Coordinator)  
- **Branch Browser**: Filter branches by city, status, view services offered
- **Doctor Directory**: Search doctors by credentials, years of experience, branch assignment
- **Content Export**: Mock "Export to CSV" button for marketing lists
- **Multi-branch Comparison**: Side-by-side view of branch services

---

## Interaction Design

### Navigation
- **Landing Page**: Three cards for persona selection ("Admin Portal", "Call Center", "Marketing")
- **Section Switching**: JavaScript hides/shows sections, updates browser history
- **Breadcrumbs**: "Home > Call Center > Doctor Search" style navigation

### Search & Filtering
- **Global Search**: Header search bar filters across currently active section's data
- **Instant Results**: Real-time filtering as user types (debounced)
- **Filter Chips**: Clickable filter buttons (e.g., "Active Only", "Cardiology", "Riyadh")

### Form Interactions
- **Client-side Validation**: Required fields, email format checks
- **Mock Submission**: Forms show "Success" toast, update local dummy data
- **Modal Dialogs**: For create/edit actions to keep context

### Feedback
- **Toast Notifications**: Success/error messages (e.g., "User created successfully")
- **Loading States**: Brief "Searching..." indicators for realistic feel
- **Empty States**: "No results found" messages with clear call-to-action

---

## Styling Approach

**Wireframe-level minimal styling:**
- CSS Variables for basic colors (gray-scale with minimal accent)
- Flexbox/Grid layouts only (no complex frameworks)
- Responsive breakpoints: Mobile (320px+), Tablet (768px+), Desktop (1024px+)
- Accessibility: Semantic HTML, ARIA labels, keyboard navigation support
- No external libraries (pure vanilla HTML/CSS/JS)

**Color Palette:**
- Primary: `#2563eb` (blue) - for actions and highlights
- Background: `#f8fafc` (light gray) - page backgrounds  
- Surface: `#ffffff` (white) - cards and containers
- Text: `#1e293b` (dark gray) - primary text
- Muted: `#64748b` (medium gray) - secondary text

---

## Technical Implementation

### HTML Structure
- `<header>`: Logo, global search, user menu
- `<main>`: Dynamic content sections (hidden/shown via JS)
- `<aside>`: Navigation sidebar (persona-specific)
- `<footer>`: Basic footer with copyright

### JavaScript State Management
```javascript
const state = {
  currentPersona: null,
  currentSection: 'landing',
  searchQuery: '',
  filters: { status: 'all', specialty: 'all' },
  recentSearches: []
};
```

### Key JavaScript Functions
- `showSection(sectionId)` - Switch between landing/admin/callcenter/marketing
- `searchEntities(query, type)` - Filter dummy data arrays
- `renderEntityList(entities, container)` - Generate HTML from data
- `showToast(message, type)` - Display notification
- `handleFormSubmit(event)` - Mock form processing
- `copyToClipboard(text)` - Contact copy functionality

---

## Success Criteria

- ✅ Three persona views accessible from landing page
- ✅ Search functionality works across all entity types
- ✅ Filters apply correctly to displayed results
- ✅ Forms can be submitted (with mock success feedback)
- ✅ Copy-to-clipboard works for contact information
- ✅ Responsive layout works on mobile/tablet/desktop
- ✅ No page reloads during interactions
- ✅ Clean wireframe visual presentation

---

## Next Steps (Post-Prototype)

1. **Stakeholder Review**: Present prototype, gather feedback on user flows
2. **Refine Interactions**: Adjust based on real user testing feedback
3. **Phase 2 Handoff**: Provide validated wireframes to UX Agent for full design
4. **Backend Planning**: Use validated interactions to inform API design (BE Agent tasks)
5. **Database Schema**: Confirmed entity relationships from prototype usage patterns

---

**Document Status:** Ready for implementation  
**Implementation Plan:** Next step using writing-plans skill