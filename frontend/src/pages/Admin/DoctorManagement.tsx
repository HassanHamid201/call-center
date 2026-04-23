import { useState, useMemo } from 'react'
import { useTranslation } from 'react-i18next'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { tValue } from '@/i18n/mappings'
import {
  Stethoscope,
  Search,
  Trash2,
  Plus,
  X,
  Pencil,
  ChevronLeft,
  ChevronRight,
} from 'lucide-react'

interface Doctor {
  id: string
  displayName?: string
  firstName: string
  lastName: string
  fullName: string
  email?: string
  phone?: string
  mobile?: string
  isActive: boolean
  branchId: string
  branchName: string
  specialtyId: string
  specialtyName: string
  sectorId: string
  sectorName: string
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

interface Branch {
  id: string
  name: string
}

interface Specialty {
  id: string
  name: string
}

interface Sector {
  id: string
  name: string
}

const emptyDoctor: Partial<Doctor> = {
  displayName: '',
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  mobile: '',
  isActive: true,
  branchId: '',
  specialtyId: '',
  sectorId: '',
  nationality: '',
  classification: '',
  insuranceAcceptance: '',
  insuranceNotes: '',
  availabilityStatus: '',
  coordinatorName: '',
  internalExtension: '',
  workingHours: '',
  workingDays: '',
  ageGroup: '',
  consultationFee: undefined,
  services: '',
  clinicMechanism: '',
  notes: '',
}

export default function DoctorManagement() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const pageSize = 10

  const [page, setPage] = useState(1)
  const [nameFilter, setNameFilter] = useState('')
  const [classificationFilter, setClassificationFilter] = useState('')
  const [availabilityFilter, setAvailabilityFilter] = useState('')
  const [branchFilter, setBranchFilter] = useState('')

  const [modalOpen, setModalOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [form, setForm] = useState<Partial<Doctor>>(emptyDoctor)

  const filtersKey = useMemo(
    () => [page, nameFilter, classificationFilter, availabilityFilter, branchFilter],
    [page, nameFilter, classificationFilter, availabilityFilter, branchFilter]
  )

  const { data, isLoading } = useQuery({
    queryKey: ['doctors', ...filtersKey],
    queryFn: async () => {
      const params = new URLSearchParams({
        page: String(page),
        pageSize: String(pageSize),
      })
      if (nameFilter) params.set('name', nameFilter)
      if (classificationFilter) params.set('classification', classificationFilter)
      if (availabilityFilter) params.set('availabilityStatus', availabilityFilter)
      if (branchFilter) params.set('branchId', branchFilter)
      const res = await apiClient.get(`/doctors?${params}`)
      return res.data as PagedResult<Doctor>
    },
  })

  const { data: classificationsData } = useQuery({
    queryKey: ['doctors-classifications'],
    queryFn: async () => {
      const res = await apiClient.get('/doctors/classifications')
      return res.data as string[]
    },
  })
  const classifications = Array.isArray(classificationsData) ? classificationsData : []

  const { data: availabilityStatusesData } = useQuery({
    queryKey: ['doctors-availability-statuses'],
    queryFn: async () => {
      const res = await apiClient.get('/doctors/availability-statuses')
      return res.data as string[]
    },
  })
  const availabilityStatuses = Array.isArray(availabilityStatusesData) ? availabilityStatusesData : []

  const { data: branchesData } = useQuery({
    queryKey: ['branches-dropdown'],
    queryFn: async () => {
      const res = await apiClient.get('/branches')
      return res.data as Branch[]
    },
  })
  const branches = Array.isArray(branchesData) ? branchesData : []

  const { data: specialtiesData } = useQuery({
    queryKey: ['specialties-dropdown'],
    queryFn: async () => {
      const res = await apiClient.get('/specialties')
      return res.data as Specialty[]
    },
  })
  const specialties = Array.isArray(specialtiesData) ? specialtiesData : []

  const { data: sectorsData } = useQuery({
    queryKey: ['sectors-dropdown'],
    queryFn: async () => {
      const res = await apiClient.get('/sectors')
      return res.data as Sector[]
    },
  })
  const sectors = Array.isArray(sectorsData) ? sectorsData : []

  const createMutation = useMutation({
    mutationFn: async (payload: Partial<Doctor>) => {
      await apiClient.post('/doctors', payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['doctors'] })
      closeModal()
    },
  })

  const updateMutation = useMutation({
    mutationFn: async ({ id, payload }: { id: string; payload: Partial<Doctor> }) => {
      await apiClient.put(`/doctors/${id}`, payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['doctors'] })
      closeModal()
    },
  })

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await apiClient.delete(`/doctors/${id}`)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['doctors'] })
    },
  })

  const openCreate = () => {
    setEditingId(null)
    setForm(emptyDoctor)
    setModalOpen(true)
  }

  const openEdit = async (doctor: Doctor) => {
    setEditingId(doctor.id)
    try {
      const res = await apiClient.get(`/doctors/${doctor.id}`)
      setForm(res.data as Doctor)
    } catch {
      setForm({ ...doctor })
    }
    setModalOpen(true)
  }

  const closeModal = () => {
    setModalOpen(false)
    setEditingId(null)
    setForm(emptyDoctor)
  }

  const handleSave = () => {
    const payload = { ...form }
    if (editingId) {
      updateMutation.mutate({ id: editingId, payload })
    } else {
      createMutation.mutate(payload)
    }
  }

  const handleDelete = (id: string) => {
    if (confirm(t('admin.confirmDeleteDoctor'))) {
      deleteMutation.mutate(id)
    }
  }

  const updateField = <K extends keyof Doctor>(key: K, value: Doctor[K]) => {
    setForm((prev) => ({ ...prev, [key]: value }))
  }

  const isSaving = createMutation.isPending || updateMutation.isPending

  if (isLoading) {
    return <div className="p-8 text-center">{t('admin.loadingDoctors')}</div>
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center">
          <Stethoscope className="h-7 w-7 me-2 text-primary-600" />
          {t('admin.doctors')}
        </h1>
        <button
          onClick={openCreate}
          className="inline-flex items-center px-4 py-2 bg-primary-600 text-white text-sm font-medium rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
        >
          <Plus className="h-4 w-4 me-1" />
          {t('admin.addDoctor')}
        </button>
      </div>

      <div className="flex flex-col lg:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder={t('admin.searchDoctors')}
            value={nameFilter}
            onChange={(e) => {
              setNameFilter(e.target.value)
              setPage(1)
            }}
            className="w-full ps-9 pe-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
        </div>
        <select
          value={classificationFilter}
          onChange={(e) => {
            setClassificationFilter(e.target.value)
            setPage(1)
          }}
          className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
        >
          <option value="">{t('admin.allClassifications')}</option>
          {classifications.map((c) => (
            <option key={c} value={c}>
              {tValue(c, 'classification')}
            </option>
          ))}
        </select>
        <select
          value={availabilityFilter}
          onChange={(e) => {
            setAvailabilityFilter(e.target.value)
            setPage(1)
          }}
          className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
        >
          <option value="">{t('admin.allStatuses')}</option>
          {availabilityStatuses.map((s) => (
            <option key={s} value={s}>
              {tValue(s, 'availability')}
            </option>
          ))}
        </select>
        <select
          value={branchFilter}
          onChange={(e) => {
            setBranchFilter(e.target.value)
            setPage(1)
          }}
          className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
        >
          <option value="">{t('admin.allBranches')}</option>
          {branches.map((b) => (
            <option key={b.id} value={b.id}>
              {b.name}
            </option>
          ))}
        </select>
      </div>

      <div className="bg-white shadow overflow-hidden rounded-md">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('admin.displayName')}
                </th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('doctors.classification')}
                </th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('common.branch')}
                </th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('common.specialty')}
                </th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('admin.fee')}
                </th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('doctors.availability')}
                </th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('doctors.insurance')}
                </th>
                <th className="px-6 py-3 text-end text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t('common.actions')}
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {data?.items.map((doctor) => (
                <tr key={doctor.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {doctor.displayName || doctor.fullName}
                    {!doctor.isActive && (
                      <span className="ms-2 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-800">
                        {t('common.inactive')}
                      </span>
                    )}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {doctor.classification ? tValue(doctor.classification, 'classification') : '—'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {doctor.branchName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {doctor.specialtyName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {doctor.consultationFee != null
                      ? doctor.consultationFee.toFixed(2)
                      : '—'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {doctor.availabilityStatus ? tValue(doctor.availabilityStatus, 'availability') : '—'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {doctor.insuranceAcceptance ? tValue(doctor.insuranceAcceptance, 'insurance') : '—'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-end text-sm font-medium">
                    <button
                      onClick={() => openEdit(doctor)}
                      className="text-primary-600 hover:text-primary-900 me-3"
                      title={t('common.edit')}
                    >
                      <Pencil className="h-4 w-4 inline" />
                    </button>
                    <button
                      onClick={() => handleDelete(doctor.id)}
                      className="text-red-600 hover:text-red-900"
                      title={t('common.delete')}
                    >
                      <Trash2 className="h-4 w-4 inline" />
                    </button>
                  </td>
                </tr>
              ))}
              {data && data.items.length === 0 && (
                <tr>
                  <td
                    colSpan={8}
                    className="px-6 py-8 text-center text-sm text-gray-500"
                  >
                    {t('common.noResults')}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-600">
            {t('admin.showingResults', {
              start: (data.page - 1) * data.pageSize + 1,
              end: Math.min(data.page * data.pageSize, data.totalCount),
              total: data.totalCount,
            })}
          </p>
          <div className="flex items-center gap-2">
            <button
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-3 py-1 border rounded text-sm disabled:opacity-50 flex items-center"
            >
              <ChevronLeft className="h-4 w-4 me-1" />
              {t('common.previous')}
            </button>
            <span className="px-3 py-1 text-sm text-gray-600">
              {t('common.page')} {page} {t('common.of')} {data.totalPages}
            </span>
            <button
              onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
              disabled={page === data.totalPages}
              className="px-3 py-1 border rounded text-sm disabled:opacity-50 flex items-center"
            >
              {t('common.next')}
              <ChevronRight className="h-4 w-4 ms-1" />
            </button>
          </div>
        </div>
      )}

      {modalOpen && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-start justify-center z-50 overflow-y-auto py-8">
          <div className="bg-white rounded-lg p-6 max-w-4xl w-full mx-4 shadow-xl">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-lg font-medium text-gray-900">
                {editingId ? t('admin.editDoctor') : t('admin.createDoctor')}
              </h3>
              <button
                onClick={closeModal}
                className="text-gray-400 hover:text-gray-500"
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 max-h-[70vh] overflow-y-auto pe-2">
              <h4 className="md:col-span-2 text-sm font-semibold text-gray-700 uppercase tracking-wide mt-2">
                {t('admin.basicInfo')}
              </h4>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('admin.displayName')}
                </label>
                <input
                  type="text"
                  value={form.displayName || ''}
                  onChange={(e) => updateField('displayName', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.email')}
                </label>
                <input
                  type="email"
                  value={form.email || ''}
                  onChange={(e) => updateField('email', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('admin.firstName')} <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  required
                  value={form.firstName || ''}
                  onChange={(e) => updateField('firstName', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('admin.lastName')} <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  required
                  value={form.lastName || ''}
                  onChange={(e) => updateField('lastName', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.phone')}
                </label>
                <input
                  type="text"
                  value={form.phone || ''}
                  onChange={(e) => updateField('phone', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.mobile')}
                </label>
                <input
                  type="text"
                  value={form.mobile || ''}
                  onChange={(e) => updateField('mobile', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <h4 className="md:col-span-2 text-sm font-semibold text-gray-700 uppercase tracking-wide mt-2 border-t pt-4">
                {t('admin.relations')}
              </h4>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.branch')} <span className="text-red-500">*</span>
                </label>
                <select
                  required
                  value={form.branchId || ''}
                  onChange={(e) => updateField('branchId', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">{t('admin.selectBranch')}</option>
                  {branches.map((b) => (
                    <option key={b.id} value={b.id}>
                      {b.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.specialty')} <span className="text-red-500">*</span>
                </label>
                <select
                  required
                  value={form.specialtyId || ''}
                  onChange={(e) => updateField('specialtyId', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">{t('admin.selectSpecialty')}</option>
                  {specialties.map((s) => (
                    <option key={s.id} value={s.id}>
                      {s.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.sector')} <span className="text-red-500">*</span>
                </label>
                <select
                  required
                  value={form.sectorId || ''}
                  onChange={(e) => updateField('sectorId', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">{t('admin.selectSector')}</option>
                  {sectors.map((s) => (
                    <option key={s.id} value={s.id}>
                      {s.name}
                    </option>
                  ))}
                </select>
              </div>

              <h4 className="md:col-span-2 text-sm font-semibold text-gray-700 uppercase tracking-wide mt-2 border-t pt-4">
                {t('admin.operationalDetails')}
              </h4>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.nationality')}
                </label>
                <input
                  type="text"
                  value={form.nationality || ''}
                  onChange={(e) => updateField('nationality', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.classification')}
                </label>
                <select
                  value={form.classification || ''}
                  onChange={(e) => updateField('classification', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">{t('admin.selectClassification')}</option>
                  {classifications.map((c) => (
                    <option key={c} value={c}>
                      {tValue(c, 'classification')}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('admin.insuranceAcceptance')}
                </label>
                <input
                  type="text"
                  value={form.insuranceAcceptance || ''}
                  onChange={(e) =>
                    updateField('insuranceAcceptance', e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('admin.availabilityStatus')}
                </label>
                <select
                  value={form.availabilityStatus || ''}
                  onChange={(e) =>
                    updateField('availabilityStatus', e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">{t('admin.selectStatus')}</option>
                  {availabilityStatuses.map((s) => (
                    <option key={s} value={s}>
                      {tValue(s, 'availability')}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.coordinator')}
                </label>
                <input
                  type="text"
                  value={form.coordinatorName || ''}
                  onChange={(e) =>
                    updateField('coordinatorName', e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.extension')}
                </label>
                <input
                  type="text"
                  value={form.internalExtension || ''}
                  onChange={(e) =>
                    updateField('internalExtension', e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.workingHours')}
                </label>
                <input
                  type="text"
                  value={form.workingHours || ''}
                  onChange={(e) => updateField('workingHours', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.workingDays')}
                </label>
                <input
                  type="text"
                  value={form.workingDays || ''}
                  onChange={(e) => updateField('workingDays', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.ageGroup')}
                </label>
                <input
                  type="text"
                  value={form.ageGroup || ''}
                  onChange={(e) => updateField('ageGroup', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.consultationFee')}
                </label>
                <input
                  type="number"
                  step="0.01"
                  value={form.consultationFee ?? ''}
                  onChange={(e) =>
                    updateField(
                      'consultationFee',
                      e.target.value === '' ? undefined : parseFloat(e.target.value)
                    )
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.services')}
                </label>
                <textarea
                  rows={3}
                  value={form.services || ''}
                  onChange={(e) => updateField('services', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('doctors.clinicMechanism')}
                </label>
                <textarea
                  rows={3}
                  value={form.clinicMechanism || ''}
                  onChange={(e) =>
                    updateField('clinicMechanism', e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('admin.insuranceNotes')}
                </label>
                <textarea
                  rows={2}
                  value={form.insuranceNotes || ''}
                  onChange={(e) =>
                    updateField('insuranceNotes', e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('common.notes')}
                </label>
                <textarea
                  rows={3}
                  value={form.notes || ''}
                  onChange={(e) => updateField('notes', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div className="md:col-span-2 flex items-center gap-2 border-t pt-4">
                <input
                  id="isActive"
                  type="checkbox"
                  checked={form.isActive ?? true}
                  onChange={(e) => updateField('isActive', e.target.checked)}
                  className="h-4 w-4 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                />
                <label
                  htmlFor="isActive"
                  className="text-sm font-medium text-gray-700"
                >
                  {t('common.active')}
                </label>
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6 border-t pt-4">
              <button
                onClick={closeModal}
                className="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
              >
                {t('common.cancel')}
              </button>
              <button
                onClick={handleSave}
                disabled={isSaving}
                className="px-4 py-2 bg-primary-600 text-white rounded-md text-sm font-medium hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
              >
                {isSaving ? t('common.saving') : editingId ? t('common.update') : t('common.create')}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
