import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter } from 'react-router-dom'
import './index.css'
import './i18n/config'
import App from './App'
import { enableMockApi } from './mocks/apiMock'

// Only enable mock API when explicitly configured
// Real backend runs at http://localhost:5000 (proxied via Vite)
if (import.meta.env.VITE_ENABLE_MOCK_API === 'true') {
  enableMockApi()
  console.log('[Mock API] Mock API enabled')
}

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 1,
    },
  },
})

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </QueryClientProvider>
  </StrictMode>,
)
