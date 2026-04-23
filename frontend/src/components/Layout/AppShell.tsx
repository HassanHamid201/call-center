import { useEffect } from 'react'
import { Header } from './Header'
import { Sidebar } from './Sidebar'
import { useAuthStore } from '@/store/authStore'

export function AppShell({ children }: { children: React.ReactNode }) {
  const init = useAuthStore((s) => s.init)

  useEffect(() => {
    init()
  }, [init])

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <div className="flex">
        <Sidebar />
        <main className="flex-1 p-6 lg:p-8 overflow-auto">
          <div className="max-w-7xl mx-auto">
            {children}
          </div>
        </main>
      </div>
    </div>
  )
}
