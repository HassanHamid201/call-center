import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import { Building2, Stethoscope, Users, Activity } from 'lucide-react'
import { Link } from 'react-router-dom'

interface Stats {
  branches: number
  doctors: number
  specialties: number
  sectors: number
}

function StatCard({ name, value, icon: Icon, href }: { name: string; value: number; icon: any; href: string }) {
  return (
    <Link to={href} className="card hover:shadow-md transition-shadow">
      <div className="flex items-center">
        <div className="flex-shrink-0 p-3 rounded-md bg-primary-100">
          <Icon className="h-6 w-6 text-primary-600" />
        </div>
        <div className="ml-5">
          <p className="text-sm font-medium text-gray-500">{name}</p>
          <p className="text-2xl font-semibold text-gray-900">{value}</p>
        </div>
      </div>
    </Link>
  )
}

export default function Dashboard() {
  const { data: branches } = useQuery({
    queryKey: ['branches-summary'],
    queryFn: async () => {
      const res = await apiClient.get('/branches?pageSize=1')
      return res.data.totalCount as number
    },
  })

  const { data: doctors } = useQuery({
    queryKey: ['doctors-summary'],
    queryFn: async () => {
      const res = await apiClient.get('/doctors?pageSize=1')
      return res.data.totalCount as number
    },
  })

  const { data: specialties } = useQuery({
    queryKey: ['specialties-summary'],
    queryFn: async () => {
      const res = await apiClient.get('/specialties?pageSize=1')
      return res.data.totalCount as number
    },
  })

  const { data: sectors } = useQuery({
    queryKey: ['sectors-summary'],
    queryFn: async () => {
      const res = await apiClient.get('/sectors?pageSize=1')
      return res.data.totalCount as number
    },
  })

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard name="Branches" value={branches ?? 0} icon={Building2} href="/branches" />
        <StatCard name="Doctors" value={doctors ?? 0} icon={Stethoscope} href="/doctors" />
        <StatCard name="Specialties" value={specialties ?? 0} icon={Activity} href="/search" />
        <StatCard name="Sectors" value={sectors ?? 0} icon={Users} href="/search" />
      </div>

      <div className="card">
        <h2 className="text-lg font-medium text-gray-900 mb-4">Quick Actions</h2>
        <div className="flex flex-wrap gap-3">
          <Link to="/branches" className="btn-primary">View Branches</Link>
          <Link to="/doctors" className="btn-primary">View Doctors</Link>
          <Link to="/search" className="btn-secondary">Global Search</Link>
        </div>
      </div>
    </div>
  )
}
