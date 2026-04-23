import { useParams, Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import { Building2, MapPin, Phone, Mail, ArrowLeft, Stethoscope } from 'lucide-react'
import { useTranslation } from 'react-i18next'

interface Branch {
  id: string
  name: string
  address: string
  city: string
  phone?: string
  email?: string
  latitude?: number
  longitude?: number
  isActive: boolean
  services: string[]
  createdAt: string
}

export default function BranchDetail() {
  const { id } = useParams<{ id: string }>()
  const { t } = useTranslation()

  const { data: branch, isLoading } = useQuery({
    queryKey: ['branch', id],
    queryFn: async () => {
      const res = await apiClient.get(`/branches/${id}`)
      return res.data as Branch
    },
  })

  const { data: doctors } = useQuery({
    queryKey: ['doctors-by-branch', id],
    queryFn: async () => {
      const res = await apiClient.get(`/doctors?branchId=${id}&pageSize=50`)
      return res.data.items as any[]
    },
  })

  if (isLoading) return <div className="p-8 text-center">{t('common.loading')}</div>
  if (!branch) return <div className="p-8 text-center">{t('branches.notFound')}</div>

  return (
    <div className="space-y-6">
      <Link to="/branches" className="inline-flex items-center text-sm text-primary-600 hover:text-primary-700">
        <ArrowLeft className="h-4 w-4 me-1" />
        {t('branches.backToBranches')}
      </Link>

      <div className="card">
        <div className="flex items-center justify-between">
          <div className="flex items-center">
            <Building2 className="h-8 w-8 text-primary-600 me-3" />
            <div>
              <h1 className="text-2xl font-bold text-gray-900">{branch.name}</h1>
              <span className={branch.isActive ? 'text-green-600 text-sm' : 'text-gray-400 text-sm'}>
                {branch.isActive ? t('common.active') : t('common.inactive')}
              </span>
            </div>
          </div>
        </div>

        <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
          <div className="flex items-center text-gray-600">
            <MapPin className="h-4 w-4 me-2" />
            {branch.address}, {branch.city}
          </div>
          {branch.phone && (
            <div className="flex items-center text-gray-600">
              <Phone className="h-4 w-4 me-2" />
              {branch.phone}
            </div>
          )}
          {branch.email && (
            <div className="flex items-center text-gray-600">
              <Mail className="h-4 w-4 me-2" />
              {branch.email}
            </div>
          )}
        </div>

        <div className="mt-4">
          <h3 className="text-sm font-medium text-gray-900 mb-2">{t('branches.services')}</h3>
          <div className="flex flex-wrap gap-2">
            {branch.services.map((s) => (
              <span key={s} className="px-2 py-1 rounded-md bg-primary-50 text-primary-700 text-xs font-medium">
                {s}
              </span>
            ))}
          </div>
        </div>
      </div>

      <div className="card">
        <h2 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
          <Stethoscope className="h-5 w-5 me-2" />
          {t('branches.doctorsAtBranch')}
        </h2>
        {doctors && doctors.length > 0 ? (
          <div className="divide-y divide-gray-200">
            {doctors.map((doc) => (
              <Link
                key={doc.id}
                to={`/doctors/${doc.id}`}
                className="py-3 flex items-center justify-between hover:bg-gray-50 px-2 -mx-2 rounded"
              >
                <div>
                  <p className="text-sm font-medium text-gray-900">{doc.fullName}</p>
                  <p className="text-sm text-gray-500">{doc.specialtyName}</p>
                </div>
                <span className="text-xs text-gray-400">{doc.languages?.join(', ')}</span>
              </Link>
            ))}
          </div>
        ) : (
          <p className="text-sm text-gray-500">{t('branches.noDoctors')}</p>
        )}
      </div>
    </div>
  )
}
