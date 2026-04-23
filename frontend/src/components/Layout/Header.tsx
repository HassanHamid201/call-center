import { useAuthStore } from '@/store/authStore'
import { LogOut, User, Menu, X } from 'lucide-react'
import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { LanguageSwitcher } from '@/components/LanguageSwitcher'

export function Header() {
  const { t } = useTranslation()
  const { user, logout, isAuthenticated } = useAuthStore()
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)

  return (
    <header className="bg-white shadow-sm border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <button
              className="lg:hidden p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
              onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            >
              {mobileMenuOpen ? <X className="h-6 w-6" /> : <Menu className="h-6 w-6" />}
            </button>
            <a href="/" className="flex items-center ms-2 lg:ms-0">
              <span className="text-xl font-bold text-primary-700">Tadawi</span>
              <span className="ms-2 text-sm text-gray-500 hidden sm:inline">Medical Group Portal</span>
            </a>
          </div>

          {isAuthenticated && user && (
            <div className="flex items-center gap-4">
              <LanguageSwitcher />
              <div className="hidden md:flex items-center text-sm text-gray-700">
                <User className="h-4 w-4 me-1" />
                {user.firstName} {user.lastName}
                <span className="ms-2 px-2 py-0.5 rounded-full bg-primary-100 text-primary-700 text-xs">
                  {user.role}
                </span>
              </div>
              <button
                onClick={logout}
                className="p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
                title={t('common.logout')}
              >
                <LogOut className="h-5 w-5" />
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  )
}
