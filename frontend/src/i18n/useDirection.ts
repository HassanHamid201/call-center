import { useEffect } from 'react'
import { useTranslation } from 'react-i18next'

export function useDirection() {
  const { i18n } = useTranslation()

  useEffect(() => {
    const handleLanguageChanged = (lng: string) => {
      const dir = lng === 'ar' ? 'rtl' : 'ltr'
      document.documentElement.dir = dir
      document.documentElement.lang = lng
    }

    handleLanguageChanged(i18n.language)
    i18n.on('languageChanged', handleLanguageChanged)
    return () => {
      i18n.off('languageChanged', handleLanguageChanged)
    }
  }, [i18n])
}
