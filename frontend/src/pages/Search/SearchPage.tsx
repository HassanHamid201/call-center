import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { Link } from 'react-router-dom'
import { Search, Stethoscope, Building2 } from 'lucide-react'

interface SearchResult {
  id: string
  type: string
  title: string
  subtitle?: string
  description?: string
  url?: string
}

export default function SearchPage() {
  const [query, setQuery] = useState('')
  const [city, setCity] = useState('')
  const [page, setPage] = useState(1)
  const pageSize = 20

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['search', query, city, page],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (query) params.set('query', query)
      if (city) params.set('city', city)
      const res = await apiClient.get(`/search?${params}`)
      return res.data as PagedResult<SearchResult>
    },
    enabled: false,
  })

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setPage(1)
    refetch()
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Global Search</h1>

      <form onSubmit={handleSearch} className="card">
        <div className="flex flex-col sm:flex-row gap-3">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
            <input
              type="text"
              placeholder="Search doctors, branches, specialties..."
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              className="w-full pl-9 pr-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
            />
          </div>
          <input
            type="text"
            placeholder="City"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            className="w-full sm:w-40 px-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
          <button type="submit" className="btn-primary">
            Search
          </button>
        </div>
      </form>

      {isLoading && <div className="p-8 text-center">Searching...</div>}

      {data && (
        <div className="space-y-4">
          <p className="text-sm text-gray-500">{data.totalCount} results found</p>

          <div className="bg-white shadow overflow-hidden rounded-md">
            <ul className="divide-y divide-gray-200">
              {data.items.map((item) => (
                <li key={`${item.type}-${item.id}`}>
                  <Link
                    to={item.type === 'Doctor' ? `/doctors/${item.id}` : `/branches/${item.id}`}
                    className="block hover:bg-gray-50"
                  >
                    <div className="px-4 py-4 sm:px-6">
                      <div className="flex items-center">
                        {item.type === 'Doctor' ? (
                          <Stethoscope className="h-5 w-5 text-primary-600 mr-3" />
                        ) : (
                          <Building2 className="h-5 w-5 text-primary-600 mr-3" />
                        )}
                        <div>
                          <p className="text-sm font-medium text-primary-600">{item.title}</p>
                          <p className="text-sm text-gray-500">
                            {item.subtitle} {item.description && `&middot; ${item.description}`}
                          </p>
                        </div>
                        <span className="ml-auto inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                          {item.type}
                        </span>
                      </div>
                    </div>
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {data.totalPages > 1 && (
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
      )}
    </div>
  )
}
