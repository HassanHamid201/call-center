import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { BookOpen, Search, Trash2, Plus, Edit2 } from 'lucide-react'

interface ReferenceItem {
  id: string
  name: string
  isActive: boolean
  createdAt: string
}

const typeOptions = [
  'nationalities',
  'insurance-options',
  'classifications',
  'availability-statuses',
  'clinic-mechanisms',
  'coordinators',
  'working-hours',
  'working-days',
  'age-groups',
  'services',
]

export default function ReferenceManagement() {
  const { t } = useTranslation()
  const [selectedType, setSelectedType] = useState(typeOptions[0])
  const [page, setPage] = useState(1)
  const [searchName, setSearchName] = useState('')
  const [editingItem, setEditingItem] = useState<ReferenceItem | null>(null)
  const [isCreating, setIsCreating] = useState(false)
  const [formName, setFormName] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const queryClient = useQueryClient()
  const pageSize = 10

  const { data, isLoading } = useQuery({
    queryKey: ['references', selectedType, page, searchName],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (searchName) params.set('name', searchName)
      const res = await apiClient.get(`/references/${selectedType}/all?${params}`)
      return res.data as PagedResult<ReferenceItem>
    },
  })

  const createMutation = useMutation({
    mutationFn: async (payload: { name: string; isActive: boolean }) => {
      await apiClient.post(`/references/${selectedType}`, payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['references', selectedType] })
      closeModal()
    },
  })

  const updateMutation = useMutation({
    mutationFn: async ({ id, payload }: { id: string; payload: { name: string; isActive: boolean } }) => {
      await apiClient.put(`/references/${selectedType}/${id}`, payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['references', selectedType] })
      closeModal()
    },
  })

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await apiClient.delete(`/references/${selectedType}/${id}`)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['references', selectedType] })
    },
  })

  const openCreate = () => {
    setIsCreating(true)
    setFormName('')
    setFormIsActive(true)
  }

  const openEdit = (item: ReferenceItem) => {
    setEditingItem(item)
    setFormName(item.name)
    setFormIsActive(item.isActive)
  }

  const closeModal = () => {
    setIsCreating(false)
    setEditingItem(null)
  }

  const handleSave = () => {
    const payload = { name: formName, isActive: formIsActive }
    if (isCreating) {
      createMutation.mutate(payload)
    } else if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, payload })
    }
  }

  const isModalOpen = isCreating || editingItem !== null
  const isPending = createMutation.isPending || updateMutation.isPending

  const handleTypeChange = (type: string) => {
    setSelectedType(type)
    setPage(1)
    setSearchName('')
  }

  if (isLoading) return <div className="p-8 text-center">{t('admin.referenceManagement.loading')}</div>

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center">
          <BookOpen className="h-7 w-7 me-2 text-primary-600" />
          {t('admin.referenceManagement.title')}
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
            placeholder={t('common.searchByName')}
            value={searchName}
            onChange={(e) => { setSearchName(e.target.value); setPage(1) }}
            className="w-full ps-9 pe-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
        </div>
        <select
          value={selectedType}
          onChange={(e) => handleTypeChange(e.target.value)}
          className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
        >
          {typeOptions.map((type) => (
            <option key={type} value={type}>
              {t(`admin.referenceTypes.${type.replace(/-/g, '')}`)}
            </option>
          ))}
        </select>
      </div>

      <div className="bg-white shadow overflow-hidden rounded-md">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.name')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.status')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.createdAt')}</th>
              <th className="px-6 py-3 text-end text-xs font-medium text-gray-500 uppercase tracking-wider">{t('common.actions')}</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data?.items.map((item) => (
              <tr key={item.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{item.name}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                    {item.isActive ? t('common.active') : t('common.inactive')}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {new Date(item.createdAt).toLocaleDateString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-end text-sm font-medium">
                  <button
                    onClick={() => openEdit(item)}
                    className="text-primary-600 hover:text-primary-900 me-3"
                  >
                    <Edit2 className="h-4 w-4 inline" />
                  </button>
                  <button
                    onClick={() => {
                      if (confirm(t('admin.referenceManagement.confirmDelete'))) {
                        deleteMutation.mutate(item.id)
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

      {isModalOpen && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-md w-full">
            <h3 className="text-lg font-medium text-gray-900 mb-4">
              {isCreating ? t('admin.referenceManagement.createItem') : t('admin.referenceManagement.editItem')}
            </h3>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">{t('common.name')}</label>
                <input
                  type="text"
                  value={formName}
                  onChange={(e) => setFormName(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
                />
              </div>
              <div className="flex items-center gap-2">
                <input
                  id="ref-is-active"
                  type="checkbox"
                  checked={formIsActive}
                  onChange={(e) => setFormIsActive(e.target.checked)}
                  className="h-4 w-4 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                />
                <label htmlFor="ref-is-active" className="text-sm font-medium text-gray-700">{t('common.active')}</label>
              </div>
            </div>
            <div className="flex justify-end gap-3 mt-6">
              <button
                onClick={closeModal}
                className="btn-secondary"
              >
                {t('common.cancel')}
              </button>
              <button
                onClick={handleSave}
                disabled={isPending}
                className="btn-primary"
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
