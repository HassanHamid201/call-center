import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { Link } from 'react-router-dom'
import { Building2, MapPin, Phone, Search } from 'lucide-react'

interface Branch {
  id: string
  name: string
  address: string
  city: string
  phone?: string
  email?: string
  isActive: boolean
  services: string[]
  doctorCount: number
}

export default function BranchList() {
  const [page, setPage] = useState(1)
  const [city, setCity] = useState('')
  const pageSize = 10

  const { data, isLoading } = useQuery({
    queryKey: ['branches', page, city],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (city) params.set('city', city)
      const res = await apiClient.get(`/branches?${params}`)
      return res.data as PagedResult<Branch>
    },
  })

  if (isLoading) return <div className="p-8 text-center">Loading branches...</div>

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-900">Branches</h1>
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Filter by city..."
            value={city}
            onChange={(e) => { setCity(e.target.value); setPage(1) }}
            className="pl-9 pr-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {data?.items.map((branch) => (
          <Link
            key={branch.id}
            to={`/branches/${branch.id}`}
            className="card hover:shadow-lg transition-shadow"
          >
            <div className="flex items-start justify-between">
              <div className="flex items-center">
                <Building2 className="h-5 w-5 text-primary-600 mr-2" />
                <h3 className="text-lg font-medium text-gray-900">{branch.name}</h3>
              </div>
              <span className={branch.isActive ? 'text-green-600 text-xs' : 'text-gray-400 text-xs'}>
                {branch.isActive ? 'Active' : 'Inactive'}
              </span>
            </div>
            <div className="mt-3 space-y-1 text-sm text-gray-500">
              <div className="flex items-center">
                <MapPin className="h-4 w-4 mr-1" />
                {branch.address}, {branch.city}
              </div>
              {branch.phone && (
                <div className="flex items-center">
                  <Phone className="h-4 w-4 mr-1" />
                  {branch.phone}
                </div>
              )}
            </div>
            <div className="mt-3 flex flex-wrap gap-1">
              {branch.services.slice(0, 3).map((s) => (
                <span key={s} className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-primary-100 text-primary-800">
                  {s}
                </span>
              ))}
            </div>
            <div className="mt-3 text-sm text-gray-500">
              {branch.doctorCount} doctor{branch.doctorCount !== 1 ? 's' : ''}
            </div>
          </Link>
        ))}
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-6">
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
            className="px-3 py-1 border rounded text-sm disabled:opacity-50"
          >
            Prev
          </button>
          <span className="px-3 py-1 text-sm text-gray-600">
            Page {page} of {data.totalPages}
          </span>
          <button
            onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
            disabled={page === data.totalPages}
            className="px-3 py-1 border rounded text-sm disabled:opacity-50"
          >
            Next
          </button>
        </div>
      )}
    </div>
  )
}
