import { apiClient } from '@/api/client'
import {
  mockBranches,
  mockDoctors,
  mockSpecialties,
  mockSectors,
  mockUsers,
  mockAuditLogs,
} from './data'

let mockEnabled = false

export function enableMockApi() {
  if (mockEnabled) return
  mockEnabled = true

  apiClient.interceptors.request.use(async (config) => {
    const url = config.url || ''
    const method = config.method?.toUpperCase() || 'GET'

    await new Promise((r) => setTimeout(r, 150))

    let data: any = null
    let status = 200

    // Auth
    if (url === '/auth/login' && method === 'POST') {
      const body = config.data
      const user = mockUsers.find((u) => u.email === body.email)
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
      const newUser = {
        id: 'mock-' + Date.now(),
        email: body.email,
        firstName: body.firstName,
        lastName: body.lastName,
        role: 'CallCenterAgent',
        branchId: null,
      }
      mockUsers.push(newUser)
      status = 201
      data = newUser
    }
    else if (url === '/auth/me') {
      data = mockUsers[0]
    }

    // Branches
    else if (url.startsWith('/branches/') && url.length > '/branches/'.length) {
      const id = url.split('/')[2]
      const branch = mockBranches.find((b) => b.id === id)
      if (branch) {
        data = branch
      } else {
        status = 404
      }
    }
    else if (url.startsWith('/branches')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const city = params.get('city')
      const isActive = params.get('isActive')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')

      let items = [...mockBranches]
      if (city) items = items.filter((b) => b.city.toLowerCase().includes(city.toLowerCase()))
      if (isActive !== null) items = items.filter((b) => String(b.isActive) === isActive)

      const totalCount = items.length
      const paged = items.slice((page - 1) * pageSize, page * pageSize)
      data = { items: paged, totalCount, page, pageSize, totalPages: Math.ceil(totalCount / pageSize) || 1 }
    }

    // Doctors
    else if (url.startsWith('/doctors/') && url.length > '/doctors/'.length) {
      const id = url.split('/')[2]
      const doctor = mockDoctors.find((d) => d.id === id)
      if (doctor) {
        data = doctor
      } else {
        status = 404
      }
    }
    else if (url.startsWith('/doctors')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const name = params.get('name')
      const branchId = params.get('branchId')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')

      let items = [...mockDoctors]
      if (name) items = items.filter((d) => d.fullName.toLowerCase().includes(name.toLowerCase()))
      if (branchId) items = items.filter((d) => d.branchId === branchId)

      const totalCount = items.length
      const paged = items.slice((page - 1) * pageSize, page * pageSize)
      data = { items: paged, totalCount, page, pageSize, totalPages: Math.ceil(totalCount / pageSize) || 1 }
    }

    // Specialties
    else if (url.startsWith('/specialties')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '50')
      const totalCount = mockSpecialties.length
      const paged = mockSpecialties.slice((page - 1) * pageSize, page * pageSize)
      data = { items: paged, totalCount, page, pageSize, totalPages: Math.ceil(totalCount / pageSize) || 1 }
    }

    // Sectors
    else if (url.startsWith('/sectors')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '50')
      const totalCount = mockSectors.length
      const paged = mockSectors.slice((page - 1) * pageSize, page * pageSize)
      data = { items: paged, totalCount, page, pageSize, totalPages: Math.ceil(totalCount / pageSize) || 1 }
    }

    // Search
    else if (url.startsWith('/search')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const query = params.get('query')
      const city = params.get('city')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')

      let doctors = [...mockDoctors]
      let branches = [...mockBranches]

      if (query) {
        const q = query.toLowerCase()
        doctors = doctors.filter((d) => d.fullName.toLowerCase().includes(q) || (d.qualifications && d.qualifications.toLowerCase().includes(q)))
        branches = branches.filter((b) => b.name.toLowerCase().includes(q) || b.address.toLowerCase().includes(q))
      }
      if (city) {
        doctors = doctors.filter((d) => d.branchName.toLowerCase().includes(city.toLowerCase()))
        branches = branches.filter((b) => b.city.toLowerCase().includes(city.toLowerCase()))
      }

      const doctorResults = doctors.map((d) => ({
        id: d.id,
        type: 'Doctor',
        title: `Dr. ${d.fullName}`,
        subtitle: d.specialtyName,
        description: d.branchName,
        url: `/doctors/${d.id}`,
      }))

      const branchResults = branches.map((b) => ({
        id: b.id,
        type: 'Branch',
        title: b.name,
        subtitle: b.city,
        description: b.services.slice(0, 3).join(', '),
        url: `/branches/${b.id}`,
      }))

      const allResults = [...doctorResults, ...branchResults]
      const totalCount = allResults.length
      const paged = allResults.slice((page - 1) * pageSize, page * pageSize)
      data = { items: paged, totalCount, page, pageSize, totalPages: Math.ceil(totalCount / pageSize) || 1 }
    }

    // Audit
    else if (url.startsWith('/audit')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const entityType = params.get('entityType')
      const action = params.get('action')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '50')

      let items = [...mockAuditLogs]
      if (entityType) items = items.filter((a) => a.entityType.toLowerCase().includes(entityType.toLowerCase()))
      if (action) items = items.filter((a) => a.action.toLowerCase().includes(action.toLowerCase()))

      const totalCount = items.length
      const paged = items.slice((page - 1) * pageSize, page * pageSize)
      data = { items: paged, totalCount, page, pageSize, totalPages: Math.ceil(totalCount / pageSize) || 1 }
    }

    // Admin
    else if (url === '/admin/stats') {
      data = {
        users: mockUsers.length,
        activeUsers: mockUsers.filter((u) => u.role !== 'Admin').length,
        branches: mockBranches.length,
        doctors: mockDoctors.length,
        specialties: mockSpecialties.length,
        sectors: mockSectors.length,
        auditLogs: mockAuditLogs.length,
      }
    }
    else if (url.startsWith('/admin/users/') && url.length > '/admin/users/'.length) {
      const id = url.split('/')[3]
      const idx = mockUsers.findIndex((u) => u.id === id)
      if (method === 'PUT' && idx !== -1) {
        const body = config.data
        if (body.role) mockUsers[idx].role = body.role
        if (body.isActive !== undefined) mockUsers[idx] = { ...mockUsers[idx], isActive: body.isActive } as any
        if (body.branchId) mockUsers[idx].branchId = body.branchId
        data = mockUsers[idx]
      } else if (method === 'DELETE' && idx !== -1) {
        mockUsers.splice(idx, 1)
        status = 204
        data = null
      } else if (idx !== -1) {
        data = mockUsers[idx]
      } else {
        status = 404
      }
    }
    else if (url.startsWith('/admin/users')) {
      const params = new URLSearchParams(url.split('?')[1] || '')
      const email = params.get('email')
      const role = params.get('role')
      const page = parseInt(params.get('page') || '1')
      const pageSize = parseInt(params.get('pageSize') || '20')

      let items = [...mockUsers]
      if (email) items = items.filter((u) => u.email.toLowerCase().includes(email.toLowerCase()))
      if (role) items = items.filter((u) => u.role === role)

      const totalCount = items.length
      const paged = items.slice((page - 1) * pageSize, page * pageSize)
      data = { items: paged, totalCount, page, pageSize, totalPages: Math.ceil(totalCount / pageSize) || 1 }
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
