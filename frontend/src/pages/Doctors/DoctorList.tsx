import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { Link } from 'react-router-dom'
import { Stethoscope, Search, Globe, Phone } from 'lucide-react'

interface Doctor {
  id: string
  firstName: string
  lastName: string
  fullName: string
  email?: string
  phone?: string
  mobile?: string
  languages: string[]
  qualifications?: string
  isActive: boolean
  branchName: string
  specialtyName: string
  sectorName: string
}

export default function DoctorList() {
  const [page, setPage] = useState(1)
  const [name, setName] = useState('')
  const pageSize = 10

  const { data, isLoading } = useQuery({
    queryKey: ['doctors', page, name],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (name) params.set('name', name)
      const res = await apiClient.get(`/doctors?${params}`)
      return res.data as PagedResult<Doctor>
    },
  })

  if (isLoading) return <div className="p-8 text-center">Loading doctors...</div>

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-900">Doctors</h1>
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Search by name..."
            value={name}
            onChange={(e) => { setName(e.target.value); setPage(1) }}
            className="pl-9 pr-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
        </div>
      </div>

      <div className="bg-white shadow overflow-hidden rounded-md">
        <ul className="divide-y divide-gray-200">
          {data?.items.map((doctor) => (
            <li key={doctor.id}>
              <Link to={`/doctors/${doctor.id}`} className="block hover:bg-gray-50">
                <div className="px-4 py-4 sm:px-6">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center">
                      <Stethoscope className="h-5 w-5 text-primary-600 mr-3" />
                      <p className="text-sm font-medium text-primary-600 truncate">
                        {doctor.fullName}
                      </p>
                    </div>
                    <div className="ml-2 flex-shrink-0 flex">
                      <span className={doctor.isActive ? 'text-green-600 text-xs' : 'text-gray-400 text-xs'}>
                        {doctor.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </div>
                  </div>
                  <div className="mt-2 sm:flex sm:justify-between">
                    <div className="sm:flex space-y-1 sm:space-y-0 sm:space-x-4">
                      <p className="flex items-center text-sm text-gray-500">
                        {doctor.specialtyName} &middot; {doctor.sectorName}
                      </p>
                      <p className="flex items-center text-sm text-gray-500">
                        <Globe className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                        {doctor.languages?.join(', ')}
                      </p>
                    </div>
                    <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                      <Phone className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                      {doctor.branchName}
                    </div>
                  </div>
                </div>
              </Link>
            </li>
          ))}
        </ul>
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
