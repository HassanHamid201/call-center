import { useLocation, Link } from 'react-router-dom'
import { useAuthStore } from '@/store/authStore'
import {
  LayoutDashboard,
  Building2,
  Stethoscope,
  Search,
  ShieldCheck,
  HelpCircle,
} from 'lucide-react'
import { clsx, type ClassValue } from 'clsx'
import { twMerge } from 'tailwind-merge'

function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

const navItems = [
  { name: 'Dashboard', href: '/', icon: LayoutDashboard, roles: ['Admin', 'Manager', 'CallCenterAgent', 'Receptionist', 'Marketing'] },
  { name: 'Branches', href: '/branches', icon: Building2, roles: ['Admin', 'Manager', 'CallCenterAgent', 'Receptionist', 'Marketing'] },
  { name: 'Doctors', href: '/doctors', icon: Stethoscope, roles: ['Admin', 'Manager', 'CallCenterAgent', 'Receptionist', 'Marketing'] },
  { name: 'Search', href: '/search', icon: Search, roles: ['Admin', 'Manager', 'CallCenterAgent', 'Receptionist', 'Marketing'] },
  { name: 'FAQ', href: '/faq', icon: HelpCircle, roles: ['Admin', 'Manager', 'CallCenterAgent', 'Receptionist', 'Marketing'] },
  { name: 'Admin', href: '/admin', icon: ShieldCheck, roles: ['Admin'] },
]

export function Sidebar() {
  const location = useLocation()
  const { user, isAuthenticated } = useAuthStore()

  if (!isAuthenticated) return null

  const visibleNav = navItems.filter((item) =>
    user?.role && item.roles.includes(user.role)
  )

  return (
    <aside className="hidden lg:block w-64 bg-white border-r border-gray-200 min-h-[calc(100vh-4rem)]">
      <nav className="mt-5 px-2 space-y-1">
        {visibleNav.map((item) => {
          const isActive = location.pathname === item.href
          return (
            <Link
              key={item.name}
              to={item.href}
              className={cn(
                'group flex items-center px-2 py-2 text-sm font-medium rounded-md',
                isActive
                  ? 'bg-primary-50 text-primary-700'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
              )}
            >
              <item.icon
                className={cn(
                  'mr-3 flex-shrink-0 h-5 w-5',
                  isActive ? 'text-primary-500' : 'text-gray-400 group-hover:text-gray-500'
                )}
              />
              {item.name}
            </Link>
          )
        })}
      </nav>
    </aside>
  )
}
