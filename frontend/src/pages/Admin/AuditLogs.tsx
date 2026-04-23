import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useQuery } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { ClipboardList, Search, Filter } from 'lucide-react'

interface AuditLogItem {
  id: string
  entityType: string
  entityId: string
  action: string
  oldValues?: string
  newValues?: string
  userEmail?: string
  ipAddress?: string
  createdAt: string
}

export default function AuditLogs() {
  const { t } = useTranslation()
  const [page, setPage] = useState(1)
  const [entityType, setEntityType] = useState('')
  const [action, setAction] = useState('')
  const pageSize = 20

  const { data, isLoading } = useQuery({
    queryKey: ['audit-logs', page, entityType, action],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (entityType) params.set('entityType', entityType)
      if (action) params.set('action', action)
      const res = await apiClient.get(`/audit?${params}`)
      return res.data as PagedResult<AuditLogItem>
    },
  })

  if (isLoading) return <div className="p-8 text-center">{t('admin.auditLogs.loading')}</div>

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center">
          <ClipboardList className="h-7 w-7 me-2 text-primary-600" />
          {t('admin.auditLogs.title')}
        </h1>
      </div>

      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder={t('admin.auditLogs.entityTypePlaceholder')}
            value={entityType}
            onChange={(e) => { setEntityType(e.target.value); setPage(1) }}
            className="w-full ps-9 pe-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
        </div>
        <div className="relative flex-1">
          <Filter className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder={t('admin.auditLogs.actionPlaceholder')}
            value={action}
            onChange={(e) => { setAction(e.target.value); setPage(1) }}
            className="w-full ps-9 pe-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500"
          />
        </div>
      </div>

      <div className="bg-white shadow overflow-hidden rounded-md">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.auditLogs.time')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.auditLogs.action')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.auditLogs.entity')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.auditLogs.user')}</th>
              <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{t('admin.auditLogs.ipAddress')}</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data?.items.map((log) => (
              <tr key={log.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {new Date(log.createdAt).toLocaleString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                    log.action === 'Create' ? 'bg-green-100 text-green-800' :
                    log.action === 'Delete' ? 'bg-red-100 text-red-800' :
                    'bg-yellow-100 text-yellow-800'
                  }`}>
                    {log.action}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {log.entityType} <span className="text-gray-400">({log.entityId.slice(0, 8)}...)</span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {log.userEmail || t('common.system')}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {log.ipAddress || '-'}
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
    </div>
  )
}
