import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { apiClient, type PagedResult } from '@/api/client'
import { Search, HelpCircle, ChevronDown, ChevronUp } from 'lucide-react'
import { useTranslation } from 'react-i18next'

interface FaqItem {
  id: string
  question: string
  answer: string
  category?: string
}

export default function FaqPage() {
  const [page, setPage] = useState(1)
  const [query, setQuery] = useState('')
  const [openItem, setOpenItem] = useState<string | null>(null)
  const pageSize = 20
  const { t } = useTranslation()

  const { data, isLoading } = useQuery({
    queryKey: ['faq', page, query],
    queryFn: async () => {
      const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
      if (query) params.set('query', query)
      const res = await apiClient.get(`/faq?${params}`)
      return res.data as PagedResult<FaqItem>
    },
  })

  if (isLoading) return <div className="p-8 text-center">{t('faq.loading')}</div>

  const toggleItem = (id: string) => {
    setOpenItem(openItem === id ? null : id)
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div className="flex items-center">
          <HelpCircle className="h-6 w-6 text-primary-600 me-2" />
          <h1 className="text-2xl font-bold text-gray-900">{t('faq.title')}</h1>
        </div>
        <div className="relative">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder={t('faq.searchPlaceholder')}
            value={query}
            onChange={(e) => { setQuery(e.target.value); setPage(1) }}
            className="ps-9 pe-4 py-2 border border-gray-300 rounded-md text-sm focus:ring-primary-500 focus:border-primary-500 w-64"
          />
        </div>
      </div>

      <div className="bg-white shadow overflow-hidden rounded-md">
        <ul className="divide-y divide-gray-200">
          {data?.items.map((item) => (
            <li key={item.id}>
              <button
                onClick={() => toggleItem(item.id)}
                className="w-full text-start px-4 py-4 sm:px-6 hover:bg-gray-50 focus:outline-none"
              >
                <div className="flex items-center justify-between">
                  <p className="text-sm font-medium text-gray-900">{item.question}</p>
                  {openItem === item.id ? (
                    <ChevronUp className="h-5 w-5 text-gray-400 flex-shrink-0 me-2" />
                  ) : (
                    <ChevronDown className="h-5 w-5 text-gray-400 flex-shrink-0 me-2" />
                  )}
                </div>
                {openItem === item.id && (
                  <div className="mt-3 pe-2">
                    <p className="text-sm text-gray-600 bg-primary-50 p-3 rounded">
                      {item.answer || t('faq.noAnswer')}
                    </p>
                    {item.category && (
                      <span className="mt-2 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-600">
                        {item.category}
                      </span>
                    )}
                  </div>
                )}
              </button>
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

      {data?.items.length === 0 && (
        <div className="text-center py-12 text-gray-500">
          <HelpCircle className="h-12 w-12 mx-auto text-gray-300 mb-3" />
          <p>{t('faq.noItems')}</p>
        </div>
      )}
    </div>
  )
}
