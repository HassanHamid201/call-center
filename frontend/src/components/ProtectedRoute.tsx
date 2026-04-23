import { Navigate, useLocation } from 'react-router-dom'
import { useAuthStore } from '@/store/authStore'
import { useTranslation } from 'react-i18next'

interface ProtectedRouteProps {
  children: React.ReactNode
  requiredRole?: string
}

export function ProtectedRoute({ children, requiredRole }: ProtectedRouteProps) {
  const { isAuthenticated, user } = useAuthStore()
  const location = useLocation()
  const { t } = useTranslation()

  // Not authenticated → redirect to login, preserving intended destination
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location.pathname }} replace />
  }

  // Role check for admin routes
  if (requiredRole && user?.role !== requiredRole) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh]">
        <div className="bg-red-50 border border-red-200 rounded-lg p-8 text-center max-w-md">
          <h2 className="text-xl font-bold text-red-700 mb-2">
            {t('auth.unauthorizedTitle', 'غير مصرح')}
          </h2>
          <p className="text-red-600 mb-4">
            {t('auth.unauthorizedMessage', 'ليس لديك صلاحية الوصول إلى هذه الصفحة')}
          </p>
          <a
            href="/"
            className="inline-block px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700"
          >
            {t('common.back', 'رجوع')}
          </a>
        </div>
      </div>
    )
  }

  return <>{children}</>
}
