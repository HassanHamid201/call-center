import { useParams, Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import { Stethoscope, Mail, Phone, Globe, ArrowLeft, Building2, Award } from 'lucide-react'

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
  biography?: string
  isActive: boolean
  branchId: string
  branchName: string
  specialtyName: string
  sectorName: string
  createdAt: string
}

export default function DoctorDetail() {
  const { id } = useParams<{ id: string }>()

  const { data: doctor, isLoading } = useQuery({
    queryKey: ['doctor', id],
    queryFn: async () => {
      const res = await apiClient.get(`/doctors/${id}`)
      return res.data as Doctor
    },
  })

  if (isLoading) return <div className="p-8 text-center">Loading...</div>
  if (!doctor) return <div className="p-8 text-center">Doctor not found</div>

  return (
    <div className="space-y-6">
      <Link to="/doctors" className="inline-flex items-center text-sm text-primary-600 hover:text-primary-700">
        <ArrowLeft className="h-4 w-4 mr-1" />
        Back to doctors
      </Link>

      <div className="card">
        <div className="flex items-center justify-between">
          <div className="flex items-center">
            <Stethoscope className="h-8 w-8 text-primary-600 mr-3" />
            <div>
              <h1 className="text-2xl font-bold text-gray-900">{doctor.fullName}</h1>
              <p className="text-sm text-gray-500">{doctor.specialtyName} &middot; {doctor.sectorName}</p>
            </div>
          </div>
          <span className={doctor.isActive ? 'text-green-600 text-sm' : 'text-gray-400 text-sm'}>
            {doctor.isActive ? 'Active' : 'Inactive'}
          </span>
        </div>

        <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
          {doctor.email && (
            <div className="flex items-center text-gray-600">
              <Mail className="h-4 w-4 mr-2" />
              {doctor.email}
            </div>
          )}
          {doctor.phone && (
            <div className="flex items-center text-gray-600">
              <Phone className="h-4 w-4 mr-2" />
              {doctor.phone}
            </div>
          )}
          {doctor.mobile && (
            <div className="flex items-center text-gray-600">
              <Phone className="h-4 w-4 mr-2" />
              {doctor.mobile}
            </div>
          )}
          <div className="flex items-center text-gray-600">
            <Globe className="h-4 w-4 mr-2" />
            {doctor.languages?.join(', ')}
          </div>
          <div className="flex items-center text-gray-600">
            <Building2 className="h-4 w-4 mr-2" />
            {doctor.branchName}
          </div>
          {doctor.qualifications && (
            <div className="flex items-center text-gray-600">
              <Award className="h-4 w-4 mr-2" />
              {doctor.qualifications}
            </div>
          )}
        </div>

        {doctor.biography && (
          <div className="mt-6">
            <h3 className="text-sm font-medium text-gray-900 mb-2">Biography</h3>
            <p className="text-sm text-gray-600">{doctor.biography}</p>
          </div>
        )}
      </div>
    </div>
  )
}
