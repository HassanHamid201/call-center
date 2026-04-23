import { useTranslation } from 'react-i18next'
import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import {
  Users, Building2, Stethoscope, Activity, ClipboardList,
  ShieldCheck, HelpCircle, Layers, BookOpen
} from 'lucide-react'
import { Link } from 'react-router-dom'

interface AdminStats {
  users: number
  activeUsers: number
  branches: number
  doctors: number
  specialties: number
  sectors: number
  auditLogs: number
  faqItems: number
}

function StatCard({ name, value, icon: Icon, href }: { name: string; value: number; icon: any; href?: string }) {
  const content = (
    <div className="card hover:shadow-md transition-shadow">
      <div className="flex items-center">
        <div className="flex-shrink-0 p-3 rounded-md bg-primary-100">
          <Icon className="h-6 w-6 text-primary-600" />
        </div>
        <div className="ms-5">
          <p className="text-sm font-medium text-gray-500">{name}</p>
          <p className="text-2xl font-semibold text-gray-900">{value}</p>
        </div>
      </div>
    </div>
  )

  if (href) return <Link to={href}>{content}</Link>
  return content
}

function PatientStatCard() {
  const { t } = useTranslation()
  const { data } = useQuery({
    queryKey: ['patients-count'],
    queryFn: async () => {
      const res = await apiClient.get('/patients?page=1&pageSize=1')
      return res.data as { totalCount: number }
    },
  })
  return (
    <StatCard
      name={t('admin.stats.patients')}
      value={data?.totalCount ?? 0}
      icon={Users}
      href="/admin/patients"
    />
  )
}

function ReferenceStatCard() {
  const { t } = useTranslation()
  const { data } = useQuery({
    queryKey: ['references-count'],
    queryFn: async () => {
      const res = await apiClient.get('/references/services/all?page=1&pageSize=1')
      return res.data as { totalCount: number }
    },
  })
  return (
    <StatCard
      name={t('admin.stats.references')}
      value={data?.totalCount ?? 0}
      icon={BookOpen}
      href="/admin/references"
    />
  )
}

function ManageCard({ title, description, href, icon: Icon }: { title: string; description: string; href: string; icon: any }) {
  return (
    <Link to={href} className="card hover:shadow-md transition-shadow flex items-start gap-4">
      <div className="flex-shrink-0 p-3 rounded-md bg-gray-100">
        <Icon className="h-6 w-6 text-gray-600" />
      </div>
      <div>
        <h3 className="text-base font-medium text-gray-900">{title}</h3>
        <p className="text-sm text-gray-500 mt-1">{description}</p>
      </div>
    </Link>
  )
}

export default function AdminDashboard() {
  const { t } = useTranslation()
  const { data: stats, isLoading } = useQuery({
    queryKey: ['admin-stats'],
    queryFn: async () => {
      const res = await apiClient.get('/admin/stats')
      return res.data as AdminStats
    },
  })

  if (isLoading) return <div className="p-8 text-center">{t('admin.loadingStats')}</div>

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center">
          <ShieldCheck className="h-7 w-7 me-2 text-primary-600" />
          {t('admin.title')}
        </h1>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard name={t('admin.stats.totalUsers')} value={stats?.users ?? 0} icon={Users} href="/admin/users" />
        <StatCard name={t('admin.stats.activeUsers')} value={stats?.activeUsers ?? 0} icon={Users} />
        <StatCard name={t('admin.stats.branches')} value={stats?.branches ?? 0} icon={Building2} href="/admin/branches" />
        <StatCard name={t('admin.stats.doctors')} value={stats?.doctors ?? 0} icon={Stethoscope} href="/admin/doctors" />
        <StatCard name={t('admin.stats.specialties')} value={stats?.specialties ?? 0} icon={Activity} href="/admin/specialties" />
        <StatCard name={t('admin.stats.sectors')} value={stats?.sectors ?? 0} icon={Layers} href="/admin/sectors" />
        <StatCard name={t('admin.stats.faqItems')} value={stats?.faqItems ?? 0} icon={HelpCircle} href="/admin/faq" />
        <StatCard name={t('admin.stats.auditLogs')} value={stats?.auditLogs ?? 0} icon={ClipboardList} href="/admin/audit" />
        <PatientStatCard />
        <ReferenceStatCard />
      </div>

      {/* Management Cards */}
      <div className="card">
        <h2 className="text-lg font-medium text-gray-900 mb-4">{t('admin.management.title')}</h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <ManageCard
            title={t('admin.management.userManagement')}
            description={t('admin.management.userManagementDesc')}
            href="/admin/users"
            icon={Users}
          />
          <ManageCard
            title={t('admin.management.branchManagement')}
            description={t('admin.management.branchManagementDesc')}
            href="/admin/branches"
            icon={Building2}
          />
          <ManageCard
            title={t('admin.management.doctorManagement')}
            description={t('admin.management.doctorManagementDesc')}
            href="/admin/doctors"
            icon={Stethoscope}
          />
          <ManageCard
            title={t('admin.management.specialtyManagement')}
            description={t('admin.management.specialtyManagementDesc')}
            href="/admin/specialties"
            icon={Activity}
          />
          <ManageCard
            title={t('admin.management.sectorManagement')}
            description={t('admin.management.sectorManagementDesc')}
            href="/admin/sectors"
            icon={Layers}
          />
          <ManageCard
            title={t('admin.management.faqManagement')}
            description={t('admin.management.faqManagementDesc')}
            href="/admin/faq"
            icon={HelpCircle}
          />
          <ManageCard
            title={t('admin.management.auditLogs')}
            description={t('admin.management.auditLogsDesc')}
            href="/admin/audit"
            icon={ClipboardList}
          />
          <ManageCard
            title={t('admin.management.patientManagement')}
            description={t('admin.management.patientManagementDesc')}
            href="/admin/patients"
            icon={Users}
          />
          <ManageCard
            title={t('admin.management.referenceManagement')}
            description={t('admin.management.referenceManagementDesc')}
            href="/admin/references"
            icon={BookOpen}
          />
        </div>
      </div>
    </div>
  )
}
