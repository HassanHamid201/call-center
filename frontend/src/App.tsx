import { Suspense, lazy } from 'react'
import { Routes, Route } from 'react-router-dom'
import { AppShell } from './components/Layout/AppShell'
import { LoadingSpinner } from './components/LoadingSpinner'
import { ProtectedRoute } from './components/ProtectedRoute'

const Dashboard = lazy(() => import('./pages/Dashboard/Dashboard'))
const BranchList = lazy(() => import('./pages/Branches/BranchList'))
const BranchDetail = lazy(() => import('./pages/Branches/BranchDetail'))
const DoctorList = lazy(() => import('./pages/Doctors/DoctorList'))
const DoctorDetail = lazy(() => import('./pages/Doctors/DoctorDetail'))
const SearchPage = lazy(() => import('./pages/Search/SearchPage'))
const LoginPage = lazy(() => import('./pages/Auth/LoginPage'))
const FaqPage = lazy(() => import('./pages/FAQ/FaqPage'))
const PatientLookup = lazy(() => import('./pages/Patients/PatientLookup'))
const PatientDetail = lazy(() => import('./pages/Patients/PatientDetail'))

// Admin pages
const AdminDashboard = lazy(() => import('./pages/Admin/AdminDashboard'))
const UserManagement = lazy(() => import('./pages/Admin/UserManagement'))
const AuditLogs = lazy(() => import('./pages/Admin/AuditLogs'))
const BranchManagement = lazy(() => import('./pages/Admin/BranchManagement'))
const DoctorManagement = lazy(() => import('./pages/Admin/DoctorManagement'))
const SpecialtyManagement = lazy(() => import('./pages/Admin/SpecialtyManagement'))
const SectorManagement = lazy(() => import('./pages/Admin/SectorManagement'))
const FaqManagement = lazy(() => import('./pages/Admin/FaqManagement'))
const ReferenceManagement = lazy(() => import('./pages/Admin/ReferenceManagement'))
const PatientManagement = lazy(() => import('./pages/Admin/PatientManagement'))

function App() {
  return (
    <Routes>
      {/* Public route — no layout, no auth required */}
      <Route path="/login" element={<LoginPage />} />

      {/* All other routes — require auth + layout */}
      <Route
        path="/*"
        element={
          <AppShell>
            <Suspense fallback={<LoadingSpinner />}>
              <Routes>
                {/* Protected regular routes (any authenticated user) */}
                <Route
                  path="/"
                  element={
                    <ProtectedRoute>
                      <Dashboard />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/branches"
                  element={
                    <ProtectedRoute>
                      <BranchList />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/branches/:id"
                  element={
                    <ProtectedRoute>
                      <BranchDetail />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/doctors"
                  element={
                    <ProtectedRoute>
                      <DoctorList />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/doctors/:id"
                  element={
                    <ProtectedRoute>
                      <DoctorDetail />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/search"
                  element={
                    <ProtectedRoute>
                      <SearchPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/faq"
                  element={
                    <ProtectedRoute>
                      <FaqPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/patients"
                  element={
                    <ProtectedRoute>
                      <PatientLookup />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/patients/:id"
                  element={
                    <ProtectedRoute>
                      <PatientDetail />
                    </ProtectedRoute>
                  }
                />

                {/* Admin-only routes */}
                <Route
                  path="/admin"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <AdminDashboard />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/users"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <UserManagement />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/audit"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <AuditLogs />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/branches"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <BranchManagement />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/doctors"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <DoctorManagement />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/specialties"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <SpecialtyManagement />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/sectors"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <SectorManagement />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/faq"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <FaqManagement />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/references"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <ReferenceManagement />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/patients"
                  element={
                    <ProtectedRoute requiredRole="Admin">
                      <PatientManagement />
                    </ProtectedRoute>
                  }
                />
              </Routes>
            </Suspense>
          </AppShell>
        }
      />
    </Routes>
  )
}

export default App
