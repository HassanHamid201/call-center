import { Header } from './Header'
import { Sidebar } from './Sidebar'
import { useDirection } from '@/i18n/useDirection'

export function AppShell({ children }: { children: React.ReactNode }) {
  useDirection()

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
