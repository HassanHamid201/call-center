import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useQuery } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { Link } from 'react-router-dom'
import { Stethoscope, Search, Globe, Phone, BadgeCheck, Clock, DollarSign, Shield } from 'lucide-react'
import { tValue } from '@/i18n/mappings'

interface Doctor {
  id: string
  firstName: string
  lastName: string
  fullName: string
  displayName?: string
  email?: string
  phone?: string
  mobile?: string
  languages: string[]
  qualifications?: string
  isActive: boolean
  branchName: string
  specialtyName: string
  sectorName: string
  // New operational fields
  nationality?: string
  classification?: string
  insuranceAcceptance?: string
  availabilityStatus?: string
  coordinatorName?: string
  internalExtension?: string
  workingHours?: string
  workingDays?: string
  ageGroup?: string
  consultationFee?: number
  services?: string
  clinicMechanism?: string
  notes?: string
}

export default function DoctorList() {
  const { t } = useTranslation()
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

  if (isLoading) return <div className="p-8 text-center">{t('doctors.loading')}</div>

  const getAvailabilityBadge = (status?: string) => {
    if (!status) return 'bg-gray-100 text-gray-600'
    if (status.includes('متواجد')) return 'bg-green-100 text-green-700'
    if (status.includes('إجازة') || status.includes('اجازة')) return 'bg-yellow-100 text-yellow-700'
    if (status.includes('تنبيه')) return 'bg-orange-100 text-orange-700'
    if (status.includes('معتذر') || status.includes('لا يوجد')) return 'bg-red-100 text-red-700'
    return 'bg-gray-100 text-gray-600'
  }

  const getInsuranceBadge = (insurance?: string) => {
    if (!insurance) return 'bg-gray-100 text-gray-600'
    if (insurance.includes('يقبل') && !insurance.includes('لا')) return 'bg-green-100 text-green-700'
    if (insurance.includes('لايقبل') || insurance.includes('لا يقبل')) return 'bg-red-100 text-red-700'
    return 'bg-amber-100 text-amber-700'
  }

  const displayName = (d: Doctor) => d.displayName || d.fullName || `${d.firstName} ${d.lastName}`

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-900">{t('doctors.title')}</h1>
        <div className="relative">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder={t('common.searchByName')}
            value={name}
            onChange={(e) => { setName(e.target.value); setPage(1) }}
            className="ps-9 pe-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
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
                      <Stethoscope className="h-5 w-5 text-primary-600 me-3" />
                      <p className="text-sm font-medium text-primary-600 truncate">
                        {displayName(doctor)}
                      </p>
                      {doctor.classification && (
                        <span className="me-2 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800">
                          <BadgeCheck className="h-3 w-3 me-1" />
                          {tValue(doctor.classification, 'classification')}
                        </span>
                      )}
                    </div>
                    <div className="ms-2 flex-shrink-0 flex gap-1">
                      {doctor.availabilityStatus && (
                        <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium ${getAvailabilityBadge(doctor.availabilityStatus)}`}>
                          <Clock className="h-3 w-3 me-1" />
                          {tValue(doctor.availabilityStatus, 'availability')}
                        </span>
                      )}
                      {doctor.insuranceAcceptance && (
                        <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium ${getInsuranceBadge(doctor.insuranceAcceptance)}`}>
                          <Shield className="h-3 w-3 me-1" />
                          {tValue(doctor.insuranceAcceptance, 'insurance')}
                        </span>
                      )}
                    </div>
                  </div>
                  <div className="mt-2 sm:flex sm:justify-between">
                    <div className="flex flex-col sm:flex-row gap-1 sm:gap-4">
                      <p className="flex items-center text-sm text-gray-500">
                        {doctor.specialtyName} &middot; {doctor.sectorName}
                      </p>
                      <p className="flex items-center text-sm text-gray-500">
                        <Globe className="flex-shrink-0 me-1.5 h-4 w-4 text-gray-400" />
                        {doctor.branchName}
                      </p>
                      {doctor.consultationFee && (
                        <p className="flex items-center text-sm text-gray-500">
                          <DollarSign className="flex-shrink-0 me-1.5 h-4 w-4 text-gray-400" />
                          {doctor.consultationFee} SAR
                        </p>
                      )}
                    </div>
                    <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                      <Phone className="flex-shrink-0 me-1.5 h-4 w-4 text-gray-400" />
                      {doctor.internalExtension || doctor.phone || t('common.none')}
                    </div>
                  </div>
                  {doctor.coordinatorName && doctor.coordinatorName !== 'لايوجد تنسيق' && doctor.coordinatorName !== 'لا يوجد' && (
                    <p className="mt-1 text-xs text-gray-400">
                      {t('doctors.coordinator', { name: doctor.coordinatorName })}
                    </p>
                  )}
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
    </div>
  )
}
