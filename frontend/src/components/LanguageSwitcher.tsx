import { useTranslation } from 'react-i18next'
import { Globe } from 'lucide-react'

export function LanguageSwitcher() {
  const { i18n } = useTranslation()
  const current = i18n.language

  const toggle = () => {
    const next = current === 'ar' ? 'en' : 'ar'
    i18n.changeLanguage(next)
  }

  return (
    <button
      onClick={toggle}
      className="flex items-center gap-1.5 px-2.5 py-1.5 rounded-md text-sm font-medium
        text-gray-600 hover:text-gray-900 hover:bg-gray-100 transition-colors"
      title={current === 'ar' ? 'Switch to English' : 'التبديل إلى العربية'}
    >
      <Globe className="h-4 w-4" />
      <span>{current === 'ar' ? 'EN' : 'عربي'}</span>
    </button>
  )
}
