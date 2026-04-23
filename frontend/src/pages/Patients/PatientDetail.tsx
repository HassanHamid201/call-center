import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import {
  Users, ArrowLeft, Phone, FileText, CreditCard, Calendar,
  MapPin, Droplet, AlertTriangle, Heart, User, Mail, Stethoscope,
  Building2, ClipboardList, Clock, CheckCircle2, XCircle, HelpCircle,
} from 'lucide-react'

interface Patient {
  id: string
  fullName: string
  phone: string
  fileNumber: string
  identityNumber: string
  dateOfBirth: string
  gender: string
  email?: string
  address?: string
  bloodType?: string
  allergies?: string
  chronicDiseases?: string
  emergencyContactName?: string
  emergencyContactPhone?: string
  notes?: string
  createdAt: string
}

interface Appointment {
  id: string
  patientId: string
  date: string
  doctorName: string
  branchName: string
  status: string
  notes?: string
}

interface Visit {
  id: string
  patientId: string
  date: string
  doctorName: string
  branchName: string
  type: string
  diagnosis: string
  treatment: string
  notes?: string
}

interface MedicalHistory {
  id: string
  patientId: string
  condition: string
  diagnosisDate: string
  status: string
  notes?: string
}

type TabKey = 'profile' | 'appointments' | 'visits' | 'medicalHistory'

export default function PatientDetail() {
  const { t } = useTranslation()
  const { id } = useParams<{ id: string }>()
  const [activeTab, setActiveTab] = useState<TabKey>('profile')

  const { data: patient, isLoading: patientLoading } = useQuery({
    queryKey: ['patient', id],
    queryFn: async () => {
      const res = await apiClient.get(`/patients/${id}`)
      return res.data as Patient
    },
  })

  const { data: appointments, isLoading: appointmentsLoading } = useQuery({
    queryKey: ['patient-appointments', id],
    queryFn: async () => {
      const res = await apiClient.get(`/patients/${id}/appointments`)
      return res.data as Appointment[]
    },
    enabled: !!id,
  })

  const { data: visits, isLoading: visitsLoading } = useQuery({
    queryKey: ['patient-visits', id],
    queryFn: async () => {
      const res = await apiClient.get(`/patients/${id}/visits`)
      return res.data as Visit[]
    },
    enabled: !!id,
  })

  const { data: medicalHistory, isLoading: historyLoading } = useQuery({
    queryKey: ['patient-medical-history', id],
    queryFn: async () => {
      const res = await apiClient.get(`/patients/${id}/medical-history`)
      return res.data as MedicalHistory[]
    },
    enabled: !!id,
  })

  if (patientLoading) return <div className="p-8 text-center">{t('common.loading')}</div>
  if (!patient) return <div className="p-8 text-center">{t('common.noResults')}</div>

  const tabs: { key: TabKey; label: string }[] = [
    { key: 'profile', label: t('patients.profile') },
    { key: 'appointments', label: t('patients.appointments') },
    { key: 'visits', label: t('patients.visits') },
    { key: 'medicalHistory', label: t('patients.medicalHistory') },
  ]

  const getStatusIcon = (status: string) => {
    if (status.includes('مؤكد') || status.includes('مسيطر')) return <CheckCircle2 className="h-4 w-4 text-green-500 me-1" />
    if (status.includes('معلق') || status.includes('نشط')) return <Clock className="h-4 w-4 text-amber-500 me-1" />
    if (status.includes('منتهي')) return <CheckCircle2 className="h-4 w-4 text-blue-500 me-1" />
    if (status.includes('ملغي')) return <XCircle className="h-4 w-4 text-red-500 me-1" />
    return <HelpCircle className="h-4 w-4 text-gray-400 me-1" />
  }

  const getStatusBadge = (status: string) => {
    let color = 'bg-gray-100 text-gray-700'
    if (status.includes('مؤكد') || status.includes('مسيطر')) color = 'bg-green-100 text-green-700'
    else if (status.includes('معلق') || status.includes('نشط')) color = 'bg-amber-100 text-amber-700'
    else if (status.includes('منتهي')) color = 'bg-blue-100 text-blue-700'
    else if (status.includes('ملغي')) color = 'bg-red-100 text-red-700'
    return (
      <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium ${color}`}>
        {getStatusIcon(status)}
        {status}
      </span>
    )
  }

  return (
    <div className="space-y-6">
      <Link to="/patients" className="inline-flex items-center text-sm text-primary-600 hover:text-primary-700">
        <ArrowLeft className="h-4 w-4 me-1" />
        {t('common.back')}
      </Link>

      <div className="card">
        <div className="flex items-center">
          <Users className="h-8 w-8 text-primary-600 me-3" />
          <div>
            <h1 className="text-2xl font-bold text-gray-900">{patient.fullName}</h1>
            <p className="text-sm text-gray-500">{patient.fileNumber}</p>
          </div>
        </div>
      </div>

      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8" aria-label="Tabs">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`
                whitespace-nowrap py-3 px-1 border-b-2 font-medium text-sm
                ${activeTab === tab.key
                  ? 'border-primary-500 text-primary-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }
              `}
            >
              {tab.label}
            </button>
          ))}
        </nav>
      </div>

      {activeTab === 'profile' && (
        <div className="card">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
            <div className="flex items-center text-gray-600">
              <Phone className="h-4 w-4 me-2" />
              {patient.phone}
            </div>
            <div className="flex items-center text-gray-600">
              <FileText className="h-4 w-4 me-2" />
              {patient.fileNumber}
            </div>
            <div className="flex items-center text-gray-600">
              <CreditCard className="h-4 w-4 me-2" />
              {patient.identityNumber}
            </div>
            <div className="flex items-center text-gray-600">
              <Calendar className="h-4 w-4 me-2" />
              {patient.dateOfBirth}
            </div>
            <div className="flex items-center text-gray-600">
              <User className="h-4 w-4 me-2" />
              {t('patients.gender')}: {patient.gender === 'male' ? t('patients.male') : patient.gender === 'female' ? t('patients.female') : patient.gender}
            </div>
            {patient.email && (
              <div className="flex items-center text-gray-600">
                <Mail className="h-4 w-4 me-2" />
                {patient.email}
              </div>
            )}
            {patient.address && (
              <div className="flex items-center text-gray-600">
                <MapPin className="h-4 w-4 me-2" />
                {patient.address}
              </div>
            )}
            {patient.bloodType && (
              <div className="flex items-center text-gray-600">
                <Droplet className="h-4 w-4 me-2" />
                {t('patients.bloodType')}: {patient.bloodType}
              </div>
            )}
            {patient.allergies && (
              <div className="flex items-center text-gray-600">
                <AlertTriangle className="h-4 w-4 me-2" />
                {t('patients.allergies')}: {patient.allergies}
              </div>
            )}
            {patient.chronicDiseases && (
              <div className="flex items-center text-gray-600">
                <Heart className="h-4 w-4 me-2" />
                {t('patients.chronicDiseases')}: {patient.chronicDiseases}
              </div>
            )}
            {patient.emergencyContactName && (
              <div className="flex items-center text-gray-600">
                <User className="h-4 w-4 me-2" />
                {t('patients.emergencyContact')}: {patient.emergencyContactName} {patient.emergencyContactPhone && `- ${patient.emergencyContactPhone}`}
              </div>
            )}
          </div>
          {patient.notes && (
            <div className="mt-6">
              <h3 className="text-sm font-medium text-gray-900 mb-2">{t('common.notes')}</h3>
              <p className="text-sm text-gray-600 bg-gray-50 p-3 rounded">{patient.notes}</p>
            </div>
          )}
        </div>
      )}

      {activeTab === 'appointments' && (
        <div className="bg-white shadow overflow-hidden rounded-md">
          {appointmentsLoading ? (
            <div className="p-8 text-center">{t('common.loading')}</div>
          ) : !appointments || appointments.length === 0 ? (
            <div className="p-8 text-center text-gray-500">{t('common.noResults')}</div>
          ) : (
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.date')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('doctors.doctorDetail')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.branch')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.status')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.notes')}</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {appointments.map((appt) => (
                    <tr key={appt.id}>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                        <div className="flex items-center">
                          <Calendar className="h-4 w-4 me-1 text-gray-400" />
                          {new Date(appt.date).toLocaleDateString('ar-SA')}
                        </div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                        <div className="flex items-center">
                          <Stethoscope className="h-4 w-4 me-1 text-gray-400" />
                          {appt.doctorName}
                        </div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
                        <div className="flex items-center">
                          <Building2 className="h-4 w-4 me-1 text-gray-400" />
                          {appt.branchName}
                        </div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm">
                        {getStatusBadge(appt.status)}
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-500">{appt.notes || '-'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {activeTab === 'visits' && (
        <div className="bg-white shadow overflow-hidden rounded-md">
          {visitsLoading ? (
            <div className="p-8 text-center">{t('common.loading')}</div>
          ) : !visits || visits.length === 0 ? (
            <div className="p-8 text-center text-gray-500">{t('common.noResults')}</div>
          ) : (
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.date')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('doctors.doctorDetail')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.branch')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('patients.visitType')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('patients.diagnosis')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('patients.treatment')}</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {visits.map((visit) => (
                    <tr key={visit.id}>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                        <div className="flex items-center">
                          <Calendar className="h-4 w-4 me-1 text-gray-400" />
                          {new Date(visit.date).toLocaleDateString('ar-SA')}
                        </div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                        <div className="flex items-center">
                          <Stethoscope className="h-4 w-4 me-1 text-gray-400" />
                          {visit.doctorName}
                        </div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
                        <div className="flex items-center">
                          <Building2 className="h-4 w-4 me-1 text-gray-400" />
                          {visit.branchName}
                        </div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-700">
                        <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-700">
                          <ClipboardList className="h-3 w-3 me-1" />
                          {visit.type}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-500">{visit.diagnosis}</td>
                      <td className="px-4 py-3 text-sm text-gray-500">{visit.treatment}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {activeTab === 'medicalHistory' && (
        <div className="bg-white shadow overflow-hidden rounded-md">
          {historyLoading ? (
            <div className="p-8 text-center">{t('common.loading')}</div>
          ) : !medicalHistory || medicalHistory.length === 0 ? (
            <div className="p-8 text-center text-gray-500">{t('common.noResults')}</div>
          ) : (
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('patients.condition')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('patients.diagnosisDate')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.status')}</th>
                    <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.notes')}</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {medicalHistory.map((item) => (
                    <tr key={item.id}>
                      <td className="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900">
                        {item.condition}
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
                        {new Date(item.diagnosisDate).toLocaleDateString('ar-SA')}
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-sm">
                        {getStatusBadge(item.status)}
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-500">{item.notes || '-'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}
    </div>
  )
}
