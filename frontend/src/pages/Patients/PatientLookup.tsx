import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useQuery } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { Link } from 'react-router-dom'
import { Users, Search, Phone, FileText, CreditCard } from 'lucide-react'

interface Patient {
  id: string
  fullName: string
  phone: string
  fileNumber: string
  identityNumber: string
}

export default function PatientLookup() {
  const { t } = useTranslation()
  const [phone, setPhone] = useState('')
  const [fileNumber, setFileNumber] = useState('')
  const [identityNumber, setIdentityNumber] = useState('')
  const [page, setPage] = useState(1)
  const pageSize = 10

  const [searchParams, setSearchParams] = useState<{ phone: string; fileNumber: string; identityNumber: string } | null>(null)

  const { data, isLoading } = useQuery({
    queryKey: ['patients', page, searchParams],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (searchParams?.phone) params.set('phone', searchParams.phone)
      if (searchParams?.fileNumber) params.set('fileNumber', searchParams.fileNumber)
      if (searchParams?.identityNumber) params.set('identityNumber', searchParams.identityNumber)
      const res = await apiClient.get(`/patients?${params}`)
      return res.data as PagedResult<Patient>
    },
    enabled: !!searchParams,
  })

  const handleSearch = () => {
    setPage(1)
    setSearchParams({ phone, fileNumber, identityNumber })
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') handleSearch()
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-3">
        <Users className="h-7 w-7 text-primary-600" />
        <h1 className="text-2xl font-bold text-gray-900">{t('patients.title')}</h1>
      </div>

      <div className="bg-white shadow rounded-md p-4 sm:p-6">
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('patients.searchByPhone')}</label>
            <div className="relative">
              <Phone className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
                onKeyDown={handleKeyDown}
                placeholder={t('patients.searchByPhone')}
                className="w-full ps-9 pe-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('patients.searchByFileNumber')}</label>
            <div className="relative">
              <FileText className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                value={fileNumber}
                onChange={(e) => setFileNumber(e.target.value)}
                onKeyDown={handleKeyDown}
                placeholder={t('patients.searchByFileNumber')}
                className="w-full ps-9 pe-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('patients.searchByIdentity')}</label>
            <div className="relative">
              <CreditCard className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                value={identityNumber}
                onChange={(e) => setIdentityNumber(e.target.value)}
                onKeyDown={handleKeyDown}
                placeholder={t('patients.searchByIdentity')}
                className="w-full ps-9 pe-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
              />
            </div>
          </div>
        </div>
        <div className="mt-4 flex justify-end">
          <button
            onClick={handleSearch}
            className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
          >
            <Search className="h-4 w-4 me-2" />
            {t('common.search')}
          </button>
        </div>
      </div>

      {isLoading && (
        <div className="p-8 text-center text-gray-500">{t('common.loading')}</div>
      )}

      {data && (
        <div className="bg-white shadow overflow-hidden rounded-md">
          {data.items.length === 0 ? (
            <div className="p-8 text-center text-gray-500">{t('common.noResults')}</div>
          ) : (
            <>
              <ul className="divide-y divide-gray-200">
                {data.items.map((patient) => (
                  <li key={patient.id}>
                    <Link to={`/patients/${patient.id}`} className="block hover:bg-gray-50">
                      <div className="px-4 py-4 sm:px-6">
                        <div className="flex items-center justify-between">
                          <div className="flex items-center">
                            <Users className="h-5 w-5 text-primary-600 me-3" />
                            <p className="text-sm font-medium text-primary-600 truncate">
                              {patient.fullName}
                            </p>
                          </div>
                        </div>
                        <div className="mt-2 grid grid-cols-1 sm:grid-cols-3 gap-2 text-sm text-gray-500">
                          <p className="flex items-center">
                            <Phone className="flex-shrink-0 me-1.5 h-4 w-4 text-gray-400" />
                            {patient.phone}
                          </p>
                          <p className="flex items-center">
                            <FileText className="flex-shrink-0 me-1.5 h-4 w-4 text-gray-400" />
                            {patient.fileNumber}
                          </p>
                          <p className="flex items-center">
                            <CreditCard className="flex-shrink-0 me-1.5 h-4 w-4 text-gray-400" />
                            {patient.identityNumber}
                          </p>
                        </div>
                      </div>
                    </Link>
                  </li>
                ))}
              </ul>

              {data.totalPages > 1 && (
                <div className="flex justify-center gap-2 p-4">
                  <button
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                    disabled={page === 1}
                    className="px-3 py-1 border rounded text-sm disabled:opacity-50"
                  >
                    {t('common.prev')}
                  </button>
                  <span className="px-3 py-1 text-sm text-gray-600">
                    {t('common.pageOf', { page, total: data.totalPages })}
                  </span>
                  <button
                    onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
                    disabled={page === data.totalPages}
                    className="px-3 py-1 border rounded text-sm disabled:opacity-50"
                  >
                    {t('common.next')}
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      )}
    </div>
  )
}
