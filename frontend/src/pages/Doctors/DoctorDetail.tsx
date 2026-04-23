import { useParams, Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import {
  Stethoscope, Mail, Phone, Globe, ArrowLeft, Building2, Award,
  BadgeCheck, Clock, Shield, DollarSign, Calendar, User,
  MapPin, Briefcase
} from 'lucide-react'

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
  biography?: string
  isActive: boolean
  branchId: string
  branchName: string
  specialtyName: string
  sectorName: string
  createdAt: string
  // Operational fields
  nationality?: string
  classification?: string
  insuranceAcceptance?: string
  insuranceNotes?: string
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

  const displayName = doctor.displayName || doctor.fullName || `${doctor.firstName} ${doctor.lastName}`

  const getAvailabilityColor = (status?: string) => {
    if (!status) return 'text-gray-600 bg-gray-50'
    if (status.includes('متواجد')) return 'text-green-700 bg-green-50'
    if (status.includes('إجازة') || status.includes('اجازة')) return 'text-yellow-700 bg-yellow-50'
    if (status.includes('تنبيه')) return 'text-orange-700 bg-orange-50'
    if (status.includes('معتذر') || status.includes('لا يوجد')) return 'text-red-700 bg-red-50'
    return 'text-gray-600 bg-gray-50'
  }

  const getInsuranceColor = (insurance?: string) => {
    if (!insurance) return 'text-gray-600 bg-gray-50'
    if (insurance.includes('يقبل') && !insurance.includes('لا')) return 'text-green-700 bg-green-50'
    if (insurance.includes('لايقبل') || insurance.includes('لا يقبل')) return 'text-red-700 bg-red-50'
    return 'text-amber-700 bg-amber-50'
  }

  return (
    <div className="space-y-6">
      <Link to="/doctors" className="inline-flex items-center text-sm text-primary-600 hover:text-primary-700">
        <ArrowLeft className="h-4 w-4 mr-1" />
        Back to doctors
      </Link>

      <div className="card">
        <div className="flex items-center justify-between flex-wrap gap-2">
          <div className="flex items-center">
            <Stethoscope className="h-8 w-8 text-primary-600 mr-3" />
            <div>
              <h1 className="text-2xl font-bold text-gray-900">{displayName}</h1>
              <p className="text-sm text-gray-500">{doctor.specialtyName} &middot; {doctor.sectorName}</p>
            </div>
          </div>
          <div className="flex gap-2">
            {doctor.classification && (
              <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800">
                <BadgeCheck className="h-4 w-4 mr-1" />
                {doctor.classification}
              </span>
            )}
            {doctor.availabilityStatus && (
              <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${getAvailabilityColor(doctor.availabilityStatus)}`}>
                <Clock className="h-4 w-4 mr-1" />
                {doctor.availabilityStatus}
              </span>
            )}
          </div>
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
            {doctor.languages?.join(', ') || '—'}
          </div>
          <div className="flex items-center text-gray-600">
            <Building2 className="h-4 w-4 mr-2" />
            {doctor.branchName}
          </div>
          {doctor.internalExtension && (
            <div className="flex items-center text-gray-600">
              <Phone className="h-4 w-4 mr-2" />
              Ext: {doctor.internalExtension}
            </div>
          )}
          {doctor.nationality && (
            <div className="flex items-center text-gray-600">
              <MapPin className="h-4 w-4 mr-2" />
              {doctor.nationality}
            </div>
          )}
          {doctor.consultationFee && (
            <div className="flex items-center text-gray-600">
              <DollarSign className="h-4 w-4 mr-2" />
              {doctor.consultationFee} SAR
            </div>
          )}
          {doctor.ageGroup && (
            <div className="flex items-center text-gray-600">
              <User className="h-4 w-4 mr-2" />
              {doctor.ageGroup}
            </div>
          )}
          {doctor.insuranceAcceptance && (
            <div className="flex items-center text-gray-600">
              <Shield className="h-4 w-4 mr-2" />
              <span className={`px-2 py-0.5 rounded text-xs font-medium ${getInsuranceColor(doctor.insuranceAcceptance)}`}>
                {doctor.insuranceAcceptance}
              </span>
            </div>
          )}
          {doctor.coordinatorName && doctor.coordinatorName !== 'لايوجد تنسيق' && doctor.coordinatorName !== 'لا يوجد' && (
            <div className="flex items-center text-gray-600">
              <Briefcase className="h-4 w-4 mr-2" />
              المنسقة: {doctor.coordinatorName}
            </div>
          )}
          {doctor.workingDays && (
            <div className="flex items-center text-gray-600">
              <Calendar className="h-4 w-4 mr-2" />
              {doctor.workingDays}
            </div>
          )}
          {doctor.workingHours && (
            <div className="flex items-center text-gray-600">
              <Clock className="h-4 w-4 mr-2" />
              {doctor.workingHours}
            </div>
          )}
          {doctor.qualifications && (
            <div className="flex items-center text-gray-600">
              <Award className="h-4 w-4 mr-2" />
              {doctor.qualifications}
            </div>
          )}
        </div>

        {doctor.clinicMechanism && (
          <div className="mt-6">
            <h3 className="text-sm font-medium text-gray-900 mb-2">ألية العيادة / Clinic Mechanism</h3>
            <p className="text-sm text-gray-600 bg-gray-50 p-3 rounded">{doctor.clinicMechanism}</p>
          </div>
        )}

        {doctor.services && (
          <div className="mt-6">
            <h3 className="text-sm font-medium text-gray-900 mb-2">أبرز الخدمات / Services</h3>
            <p className="text-sm text-gray-600 bg-gray-50 p-3 rounded">{doctor.services}</p>
          </div>
        )}

        {doctor.insuranceNotes && (
          <div className="mt-6">
            <h3 className="text-sm font-medium text-gray-900 mb-2">ملاحظات التأمين / Insurance Notes</h3>
            <p className="text-sm text-amber-700 bg-amber-50 p-3 rounded">{doctor.insuranceNotes}</p>
          </div>
        )}

        {doctor.notes && (
          <div className="mt-6">
            <h3 className="text-sm font-medium text-gray-900 mb-2">ملاحظات / Notes</h3>
            <p className="text-sm text-gray-600 bg-gray-50 p-3 rounded">{doctor.notes}</p>
          </div>
        )}

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
