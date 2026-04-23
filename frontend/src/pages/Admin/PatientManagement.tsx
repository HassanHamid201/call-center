import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { Users, Search, Trash2, Plus, Edit2, X } from 'lucide-react'

interface Patient {
  id: string
  fullName: string
  phone?: string
  identityNumber?: string
  fileNumber?: string
  dateOfBirth?: string
  gender?: string
  nationality?: string
  address?: string
  email?: string
  createdAt: string
}

const emptyPatient: Omit<Patient, 'id' | 'createdAt'> = {
  fullName: '',
  phone: '',
  identityNumber: '',
  fileNumber: '',
  dateOfBirth: '',
  gender: '',
  nationality: '',
  address: '',
  email: '',
}

export default function PatientManagement() {
  const { t } = useTranslation()
  const [page, setPage] = useState(1)
  const [searchName, setSearchName] = useState('')
  const [searchPhone, setSearchPhone] = useState('')
  const [searchFileNumber, setSearchFileNumber] = useState('')
  const [modalOpen, setModalOpen] = useState(false)
  const [editingPatient, setEditingPatient] = useState<Patient | null>(null)
  const [form, setForm] = useState<Omit<Patient, 'id' | 'createdAt'>>({ ...emptyPatient })
  const queryClient = useQueryClient()
  const pageSize = 10

  const { data, isLoading } = useQuery({
    queryKey: ['patients', page, searchName, searchPhone, searchFileNumber],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (searchName) params.set('name', searchName)
      if (searchPhone) params.set('phone', searchPhone)
      if (searchFileNumber) params.set('fileNumber', searchFileNumber)
      const res = await apiClient.get(`/patients?${params}`)
      return res.data as PagedResult<Patient>
    },
  })

  const createMutation = useMutation({
    mutationFn: async (payload: Omit<Patient, 'id' | 'createdAt'>) => {
      await apiClient.post('/patients', payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['patients'] })
      closeModal()
    },
  })

  const updateMutation = useMutation({
    mutationFn: async ({ id, payload }: { id: string; payload: Omit<Patient, 'id' | 'createdAt'> }) => {
      await apiClient.put(`/patients/${id}`, payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['patients'] })
      closeModal()
    },
  })

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await apiClient.delete(`/patients/${id}`)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['patients'] })
    },
  })

  const openCreate = () => {
    setEditingPatient(null)
    setForm({ ...emptyPatient })
    setModalOpen(true)
  }

  const openEdit = (patient: Patient) => {
    setEditingPatient(patient)
    setForm({
      fullName: patient.fullName,
      phone: patient.phone ?? '',
      identityNumber: patient.identityNumber ?? '',
      fileNumber: patient.fileNumber ?? '',
      dateOfBirth: patient.dateOfBirth ? patient.dateOfBirth.split('T')[0] : '',
      gender: patient.gender ?? '',
      nationality: patient.nationality ?? '',
      address: patient.address ?? '',
      email: patient.email ?? '',
    })
    setModalOpen(true)
  }

  const closeModal = () => {
    setModalOpen(false)
    setEditingPatient(null)
  }

  const handleSave = () => {
    const payload = {
      ...form,
      dateOfBirth: form.dateOfBirth ? new Date(form.dateOfBirth).toISOString() : undefined,
    }
    if (editingPatient) {
      updateMutation.mutate({ id: editingPatient.id, payload })
    } else {
      createMutation.mutate(payload)
    }
  }

  const updateField = <K extends keyof Omit<Patient, 'id' | 'createdAt'>>(field: K, value: Omit<Patient, 'id' | 'createdAt'>[K]) => {
    setForm((prev) => ({ ...prev, [field]: value }))
  }

  const isPending = createMutation.isPending || updateMutation.isPending

  if (isLoading) return <div className="p-8 text-center">{t('admin.patientManagement.loading')}</div>

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center">
          <Users className="h-7 w-7 me-2 text-primary-600" />
          {t('admin.patientManagement.title')}
        </h1>
        <button
          onClick={openCreate}
          className="btn-primary inline-flex items-center gap-2"
        >
          <Plus className="h-4 w-4" />
          {t('common.createNew')}
        </button>
      </div>

      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder={t('admin.patientManagement.searchByName')}
            value={searchName}
            onChange={(e) => { setSearchName(e.target.value); setPage(1) }}
            className="w-full ps-9 pe-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
        </div>
        <input
          type="text"
          placeholder={t('admin.patientManagement.searchByPhone')}
          value={searchPhone}
          onChange={(e) => { setSearchPhone(e.target.value); setPage(1) }}
          className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
        />
        <input
          type="text"
          placeholder={t('admin.patientManagement.searchByFileNumber')}
          value={searchFileNumber}
          onChange={(e) => { setSearchFileNumber(e.target.value); setPage(1) }}
          className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
        />
      </div>

      <div className="bg-white shadow overflow-hidden rounded-md">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.patientManagement.fullName')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.phone')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.patientManagement.fileNumber')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.patientManagement.identityNumber')}</th>
              <th className="px-6 py-3 text-end text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.actions')}</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data?.items.map((patient) => (
              <tr key={patient.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{patient.fullName}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{patient.phone ?? '-'}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{patient.fileNumber ?? '-'}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{patient.identityNumber ?? '-'}</td>
                <td className="px-6 py-4 whitespace-nowrap text-end text-sm font-medium">
                  <button
                    onClick={() => openEdit(patient)}
                    className="text-primary-600 hover:text-primary-900 me-3"
                  >
                    <Edit2 className="h-4 w-4 inline" />
                  </button>
                  <button
                    onClick={() => {
                      if (confirm(t('admin.patientManagement.confirmDelete'))) {
                        deleteMutation.mutate(patient.id)
                      }
                    }}
                    className="text-red-600 hover:text-red-900"
                  >
                    <Trash2 className="h-4 w-4 inline" />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
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

      {modalOpen && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-lg w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">
                {editingPatient ? t('admin.patientManagement.editPatient') : t('admin.patientManagement.createPatient')}
              </h3>
              <button onClick={closeModal} className="text-gray-400 hover:text-gray-600">
                <X className="h-5 w-5" />
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">{t('admin.patientManagement.fullName')} *</label>
                <input
                  type="text"
                  value={form.fullName}
                  onChange={(e) => updateField('fullName', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('common.phone')}</label>
                  <input
                    type="text"
                    value={form.phone}
                    onChange={(e) => updateField('phone', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('common.email')}</label>
                  <input
                    type="email"
                    value={form.email}
                    onChange={(e) => updateField('email', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('admin.patientManagement.fileNumber')}</label>
                  <input
                    type="text"
                    value={form.fileNumber}
                    onChange={(e) => updateField('fileNumber', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('admin.patientManagement.identityNumber')}</label>
                  <input
                    type="text"
                    value={form.identityNumber}
                    onChange={(e) => updateField('identityNumber', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('admin.patientManagement.dateOfBirth')}</label>
                  <input
                    type="date"
                    value={form.dateOfBirth}
                    onChange={(e) => updateField('dateOfBirth', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('admin.patientManagement.gender')}</label>
                  <select
                    value={form.gender}
                    onChange={(e) => updateField('gender', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  >
                    <option value="">{t('admin.patientManagement.selectGender')}</option>
                    <option value="Male">{t('admin.patientManagement.male')}</option>
                    <option value="Female">{t('admin.patientManagement.female')}</option>
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('doctors.nationality')}</label>
                  <input
                    type="text"
                    value={form.nationality}
                    onChange={(e) => updateField('nationality', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">{t('common.address')}</label>
                  <input
                    type="text"
                    value={form.address}
                    onChange={(e) => updateField('address', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <button
                onClick={closeModal}
                className="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50"
              >
                {t('common.cancel')}
              </button>
              <button
                onClick={handleSave}
                disabled={isPending || !form.fullName.trim()}
                className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 disabled:opacity-50"
              >
                {isPending ? t('common.saving') : t('common.save')}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
