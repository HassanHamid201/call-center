import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import { Users, Building2, Stethoscope, Activity, ClipboardList, ShieldCheck } from 'lucide-react'
import { Link } from 'react-router-dom'

interface AdminStats {
  users: number
  activeUsers: number
  branches: number
  doctors: number
  specialties: number
  sectors: number
  auditLogs: number
}

function StatCard({ name, value, icon: Icon, href }: { name: string; value: number; icon: any; href?: string }) {
  const content = (
    <div className="card hover:shadow-md transition-shadow">
      <div className="flex items-center">
        <div className="flex-shrink-0 p-3 rounded-md bg-primary-100">
          <Icon className="h-6 w-6 text-primary-600" />
        </div>
        <div className="ml-5">
          <p className="text-sm font-medium text-gray-500">{name}</p>
          <p className="text-2xl font-semibold text-gray-900">{value}</p>
        </div>
      </div>
    </div>
  )

  if (href) return <Link to={href}>{content}</Link>
  return content
}

export default function AdminDashboard() {
  const { data: stats, isLoading } = useQuery({
    queryKey: ['admin-stats'],
    queryFn: async () => {
      const res = await apiClient.get('/admin/stats')
      return res.data as AdminStats
    },
  })

  if (isLoading) return <div className="p-8 text-center">Loading admin stats...</div>

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center">
          <ShieldCheck className="h-7 w-7 mr-2 text-primary-600" />
          Admin Dashboard
        </h1>
      </div>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard name="Total Users" value={stats?.users ?? 0} icon={Users} href="/admin/users" />
        <StatCard name="Active Users" value={stats?.activeUsers ?? 0} icon={Users} />
        <StatCard name="Branches" value={stats?.branches ?? 0} icon={Building2} />
        <StatCard name="Doctors" value={stats?.doctors ?? 0} icon={Stethoscope} />
        <StatCard name="Specialties" value={stats?.specialties ?? 0} icon={Activity} />
        <StatCard name="Sectors" value={stats?.sectors ?? 0} icon={Activity} />
        <StatCard name="Audit Logs" value={stats?.auditLogs ?? 0} icon={ClipboardList} href="/admin/audit" />
      </div>

      <div className="card">
        <h2 className="text-lg font-medium text-gray-900 mb-4">Quick Actions</h2>
        <div className="flex flex-wrap gap-3">
          <Link to="/admin/users" className="btn-primary">Manage Users</Link>
          <Link to="/admin/audit" className="btn-secondary">View Audit Logs</Link>
        </div>
      </div>
    </div>
  )
}
