import { apiClient } from '@/api/client'
import {
  branches,
  doctors,
  specialties,
  sectors,
  users,
  auditLogs,
  faqItems,
  patients,
  patientAppointments,
  patientVisits,
  patientMedicalHistory,
} from './data'

let mockEnabled = false

export function enableMockApi() {
  if (mockEnabled) return
  mockEnabled = true

  // Helper for paged results
  const paged = (items: any[], page: number, pageSize: number) => {
    const totalCount = items.length
    const paged = items.slice((page - 1) * pageSize, page * pageSize)
    return {
      items: paged,
      totalCount,
      page,
      pageSize,
      totalPages: Math.ceil(totalCount / pageSize) || 1,
    }
  }

  apiClient.interceptors.request.use(async (config) => {
    const url = config.url || ''
    const method = config.method?.toUpperCase() || 'GET'

    await new Promise((r) => setTimeout(r, 150))

    let data: any = null
    let status = 200

    // ========== AUTH ==========
    if (url === '/auth/login' && method === 'POST') {
      const body = config.data
      const user = users.find((u: any) => u.email === body.email)
      if (user) {
        data = {
          token: 'mock-jwt-token-' + user.role,
          refreshToken: 'mock-refresh-token',
          user,
        }
      } else {
        status = 401
        data = { title: 'Authentication failed', detail: 'Invalid email or password' }
      }
    }
    else if (url === '/auth/register' && method === 'POST') {
      const body = config.data
      const newUser: any = {
        id: 'mock-' + Date.now(),
        email: body.email,
        firstName: body.firstName,
        lastName: body.lastName,
        role: 'CallCenterAgent',
        branchId: null,
        isActive: true,
      }
      users.push(newUser)
      status = 201
      data = newUser
    }
    else if (url === '/auth/me') {
      data = users[0]
    }

    // ========== BRANCHES ==========
    else if (url.match(/^\/branches\/[^/]+$/) && method === 'GET') {
      const id = url.split('/')[2]
      const branch = branches.find((b: any) => b.id === id)
      data = branch || (status = 404, { title: 'Not found' })
    }
    else if (url.match(/^\/branches\/[^/]+$/) && method === 'PUT') {
      const id = url.split('/')[2]
      const idx = branches.findIndex((b: any) => b.id === id)
      if (idx !== -1) {
        branches[idx] = { ...branches[idx], ...config.data }
        data = branches[idx]
      } else status = 404
    }
    else if (url.match(/^\/branches\/[^/]+$/) && method === 'DELETE') {
      const id = url.split('/')[2]
      const idx = branches.findIndex((b: any) => b.id === id)
      if (idx !== -1) { branches.splice(idx, 1); status = 204; data = null }
      else status = 404
    }
    else if (url.startsWith('/branches') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const city = params.get('city')
      const isActive = params.get('isActive')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')
      let items = [...branches]
      if (city) items = items.filter((b: any) => b.city.toLowerCase().includes(city.toLowerCase()))
      if (isActive !== null) items = items.filter((b: any) => String(b.isActive) === isActive)
      data = paged(items, page, pageSize)
    }
    else if (url === '/branches' && method === 'POST') {
      const newBranch: any = { id: 'mock-b-' + Date.now(), ...config.data, isActive: true }
      branches.push(newBranch)
      status = 201
      data = newBranch
    }

    // ========== DOCTORS ==========
    // Specific routes MUST come before generic /:id regex routes
    else if (url === '/doctors/classifications') {
      const items = [...new Set(doctors.map((d: any) => d.classification).filter(Boolean))]
      data = items
    }
    else if (url === '/doctors/availability-statuses') {
      const items = [...new Set(doctors.map((d: any) => d.availabilityStatus).filter(Boolean))]
      data = items
    }
    else if (url.match(/^\/doctors\/[^/]+$/) && method === 'GET') {
      const id = url.split('/')[2]
      const doctor = doctors.find((d: any) => d.id === id)
      data = doctor || (status = 404, { title: 'Not found' })
    }
    else if (url.match(/^\/doctors\/[^/]+$/) && method === 'PUT') {
      const id = url.split('/')[2]
      const idx = doctors.findIndex((d: any) => d.id === id)
      if (idx !== -1) {
        doctors[idx] = { ...doctors[idx], ...config.data }
        data = doctors[idx]
      } else status = 404
    }
    else if (url.match(/^\/doctors\/[^/]+$/) && method === 'DELETE') {
      const id = url.split('/')[2]
      const idx = doctors.findIndex((d: any) => d.id === id)
      if (idx !== -1) { doctors.splice(idx, 1); status = 204; data = null }
      else status = 404
    }
    else if (url.startsWith('/doctors') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const name = params.get('name')
      const classification = params.get('classification')
      const availabilityStatus = params.get('availabilityStatus')
      const branchId = params.get('branchId')
      const specialtyId = params.get('specialtyId')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')
      let items = [...doctors]
      if (name) items = items.filter((d: any) => (d.displayName || d.fullName || '').toLowerCase().includes(name.toLowerCase()))
      if (classification) items = items.filter((d: any) => d.classification === classification)
      if (availabilityStatus) items = items.filter((d: any) => d.availabilityStatus === availabilityStatus)
      if (branchId) items = items.filter((d: any) => d.branchId === branchId)
      if (specialtyId) items = items.filter((d: any) => d.specialtyId === specialtyId)
      data = paged(items, page, pageSize)
    }
    else if (url === '/doctors' && method === 'POST') {
      const newDoctor: any = { id: 'mock-d-' + Date.now(), ...config.data, isActive: true }
      doctors.push(newDoctor)
      status = 201
      data = newDoctor
    }

    // ========== SPECIALTIES ==========
    else if (url.match(/^\/specialties\/[^/]+$/) && method === 'PUT') {
      const id = url.split('/')[2]
      const idx = specialties.findIndex((s: any) => s.id === id)
      if (idx !== -1) { specialties[idx] = { ...specialties[idx], ...config.data }; data = specialties[idx] }
      else status = 404
    }
    else if (url.match(/^\/specialties\/[^/]+$/) && method === 'DELETE') {
      const id = url.split('/')[2]
      const idx = specialties.findIndex((s: any) => s.id === id)
      if (idx !== -1) { specialties.splice(idx, 1); status = 204; data = null }
      else status = 404
    }
    else if (url.startsWith('/specialties') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '50')
      data = paged([...specialties], page, pageSize)
    }
    else if (url === '/specialties' && method === 'POST') {
      const newSpec: any = { id: 'mock-s-' + Date.now(), ...config.data, isActive: true }
      specialties.push(newSpec)
      status = 201
      data = newSpec
    }

    // ========== SECTORS ==========
    else if (url.match(/^\/sectors\/[^/]+$/) && method === 'PUT') {
      const id = url.split('/')[2]
      const idx = sectors.findIndex((s: any) => s.id === id)
      if (idx !== -1) { sectors[idx] = { ...sectors[idx], ...config.data }; data = sectors[idx] }
      else status = 404
    }
    else if (url.match(/^\/sectors\/[^/]+$/) && method === 'DELETE') {
      const id = url.split('/')[2]
      const idx = sectors.findIndex((s: any) => s.id === id)
      if (idx !== -1) { sectors.splice(idx, 1); status = 204; data = null }
      else status = 404
    }
    else if (url.startsWith('/sectors') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '50')
      data = paged([...sectors], page, pageSize)
    }
    else if (url === '/sectors' && method === 'POST') {
      const newSector: any = { id: 'mock-se-' + Date.now(), ...config.data, isActive: true }
      sectors.push(newSector)
      status = 201
      data = newSector
    }

    // ========== FAQ ==========
    // Specific routes MUST come before generic /:id regex routes
    else if (url === '/faq/categories') {
      const items = [...new Set(faqItems.map((f: any) => f.category).filter(Boolean))]
      data = items
    }
    else if (url.match(/^\/faq\/[^/]+$/) && method === 'PUT') {
      const id = url.split('/')[2]
      const idx = faqItems.findIndex((f: any) => f.id === id)
      if (idx !== -1) { faqItems[idx] = { ...faqItems[idx], ...config.data }; data = faqItems[idx] }
      else status = 404
    }
    else if (url.match(/^\/faq\/[^/]+$/) && method === 'DELETE') {
      const id = url.split('/')[2]
      const idx = faqItems.findIndex((f: any) => f.id === id)
      if (idx !== -1) { faqItems.splice(idx, 1); status = 204; data = null }
      else status = 404
    }
    else if (url.startsWith('/faq') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const q = params.get('query')
      const category = params.get('category')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '50')
      let items = [...faqItems]
      if (q) items = items.filter((f: any) => f.question.toLowerCase().includes(q.toLowerCase()) || f.answer.toLowerCase().includes(q.toLowerCase()))
      if (category) items = items.filter((f: any) => f.category === category)
      data = paged(items, page, pageSize)
    }
    else if (url === '/faq' && method === 'POST') {
      const newFaq: any = { id: 'mock-f-' + Date.now(), ...config.data, isActive: true }
      faqItems.push(newFaq)
      status = 201
      data = newFaq
    }

    // ========== SEARCH ==========
    else if (url.startsWith('/search')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const query = params.get('query')
      const type = params.get('type')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')

      let results: any[] = []

      if (!type || type === 'Doctor') {
        let docs = [...doctors]
        if (query) {
          const q = query.toLowerCase()
          docs = docs.filter((d: any) => (d.displayName || d.fullName || '').toLowerCase().includes(q) || (d.classification && d.classification.toLowerCase().includes(q)))
        }
        results.push(...docs.map((d: any) => ({
          id: d.id, type: 'Doctor',
          title: d.displayName || `Dr. ${d.fullName}`,
          subtitle: d.classification || d.specialtyName,
          description: d.branchName || '',
          url: `/doctors/${d.id}`,
        })))
      }

      if (!type || type === 'Branch') {
        let brs = [...branches]
        if (query) brs = brs.filter((b: any) => b.name.toLowerCase().includes(query.toLowerCase()) || b.city.toLowerCase().includes(query.toLowerCase()))
        results.push(...brs.map((b: any) => ({
          id: b.id, type: 'Branch',
          title: b.name, subtitle: b.city,
          description: b.services.slice(0, 3).join(', '),
          url: `/branches/${b.id}`,
        })))
      }

      if (!type || type === 'Specialty') {
        let specs = [...specialties]
        if (query) specs = specs.filter((s: any) => s.name.toLowerCase().includes(query.toLowerCase()))
        results.push(...specs.map((s: any) => ({
          id: s.id, type: 'Specialty',
          title: s.name, subtitle: s.description || '',
          description: '', url: `/admin/specialties`,
        })))
      }

      if (!type || type === 'Sector') {
        let secs = [...sectors]
        if (query) secs = secs.filter((s: any) => s.name.toLowerCase().includes(query.toLowerCase()))
        results.push(...secs.map((s: any) => ({
          id: s.id, type: 'Sector',
          title: s.name, subtitle: s.description || '',
          description: '', url: `/admin/sectors`,
        })))
      }

      if (!type || type === 'Faq') {
        let faqs = [...faqItems]
        if (query) faqs = faqs.filter((f: any) => f.question.toLowerCase().includes(query.toLowerCase()) || f.answer.toLowerCase().includes(query.toLowerCase()))
        results.push(...faqs.map((f: any) => ({
          id: f.id, type: 'FAQ',
          title: f.question, subtitle: f.answer.substring(0, 60) + (f.answer.length > 60 ? '...' : ''),
          description: f.category || '', url: `/faq`,
        })))
      }

      data = paged(results, page, pageSize)
    }

    // ========== PATIENTS ==========
    else if (url.match(/^\/patients\/[^/]+\/medical-history$/) && method === 'GET') {
      const id = url.split('/')[2]
      const items = patientMedicalHistory.filter((h: any) => h.patientId === id)
      data = items
    }
    else if (url.match(/^\/patients\/[^/]+\/visits$/) && method === 'GET') {
      const id = url.split('/')[2]
      const items = patientVisits.filter((v: any) => v.patientId === id)
      data = items
    }
    else if (url.match(/^\/patients\/[^/]+\/appointments$/) && method === 'GET') {
      const id = url.split('/')[2]
      const items = patientAppointments.filter((a: any) => a.patientId === id)
      data = items
    }
    else if (url.match(/^\/patients\/[^/]+$/) && method === 'GET') {
      const id = url.split('/')[2]
      const patient = patients.find((p: any) => p.id === id)
      data = patient || (status = 404, { title: 'Not found' })
    }
    else if (url.startsWith('/patients') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const phone = params.get('phone')
      const fileNumber = params.get('fileNumber')
      const identityNumber = params.get('identityNumber')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')
      let items = [...patients]
      if (phone) items = items.filter((p: any) => p.phone.includes(phone))
      if (fileNumber) items = items.filter((p: any) => p.fileNumber.toLowerCase().includes(fileNumber.toLowerCase()))
      if (identityNumber) items = items.filter((p: any) => p.identityNumber.includes(identityNumber))
      data = paged(items, page, pageSize)
    }

    // ========== AUDIT ==========
    else if (url.startsWith('/audit') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')
      const entityType = params.get('entityType') || ''
      const action = params.get('action') || ''
      let items = [...auditLogs]
      if (entityType) items = items.filter((a: any) => a.entityType.toLowerCase().includes(entityType.toLowerCase()))
      if (action) items = items.filter((a: any) => a.action.toLowerCase().includes(action.toLowerCase()))
      data = paged(items, page, pageSize)
    }

    // ========== ADMIN ==========
    else if (url === '/admin/stats') {
      data = {
        users: users.length,
        activeUsers: users.filter((u: any) => u.isActive).length,
        branches: branches.length,
        doctors: doctors.length,
        specialties: specialties.length,
        sectors: sectors.length,
        auditLogs: auditLogs.length,
        faqItems: faqItems.length,
      }
    }
    else if (url.match(/^\/admin\/users\/[^/]+$/) && method === 'GET') {
      const id = url.split('/')[3]
      data = users.find((u: any) => u.id === id) || (status = 404)
    }
    else if (url.match(/^\/admin\/users\/[^/]+$/) && method === 'PUT') {
      const id = url.split('/')[3]
      const idx = users.findIndex((u: any) => u.id === id)
      if (idx !== -1) {
        const body = config.data
        if (body.role) users[idx].role = body.role
        if (body.isActive !== undefined) users[idx] = { ...users[idx], isActive: body.isActive }
        if (body.branchId) (users[idx] as any).branchId = body.branchId
        data = users[idx]
      } else status = 404
    }
    else if (url.match(/^\/admin\/users\/[^/]+$/) && method === 'DELETE') {
      const id = url.split('/')[3]
      const idx = users.findIndex((u: any) => u.id === id)
      if (idx !== -1) { users.splice(idx, 1); status = 204; data = null }
      else status = 404
    }
    else if (url.startsWith('/admin/users') && method === 'GET') {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const email = params.get('email')
      const role = params.get('role')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')
      let items = [...users]
      if (email) items = items.filter((u: any) => u.email.toLowerCase().includes(email.toLowerCase()))
      if (role) items = items.filter((u: any) => u.role === role)
      data = paged(items, page, pageSize)
    }

    if (data !== null) {
      return Promise.reject({
        __mock: true,
        response: { status, data },
        config,
      })
    }

    return config
  })

  apiClient.interceptors.response.use(
    (response) => response,
    (error: any) => {
      if (error.__mock) {
        return Promise.resolve(error.response)
      }
      return Promise.reject(error)
    }
  )
}
