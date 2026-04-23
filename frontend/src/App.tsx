import { Suspense, lazy } from 'react'
import { Routes, Route } from 'react-router-dom'
import { AppShell } from './components/Layout/AppShell'
import { LoadingSpinner } from './components/LoadingSpinner'

const Dashboard = lazy(() => import('./pages/Dashboard/Dashboard'))
const BranchList = lazy(() => import('./pages/Branches/BranchList'))
const BranchDetail = lazy(() => import('./pages/Branches/BranchDetail'))
const DoctorList = lazy(() => import('./pages/Doctors/DoctorList'))
const DoctorDetail = lazy(() => import('./pages/Doctors/DoctorDetail'))
const SearchPage = lazy(() => import('./pages/Search/SearchPage'))
const LoginPage = lazy(() => import('./pages/Auth/LoginPage'))

function App() {
  return (
    <AppShell>
      <Suspense fallback={<LoadingSpinner />}>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/" element={<Dashboard />} />
          <Route path="/branches" element={<BranchList />} />
          <Route path="/branches/:id" element={<BranchDetail />} />
          <Route path="/doctors" element={<DoctorList />} />
          <Route path="/doctors/:id" element={<DoctorDetail />} />
          <Route path="/search" element={<SearchPage />} />
        </Routes>
      </Suspense>
    </AppShell>
  )
}

export default App
