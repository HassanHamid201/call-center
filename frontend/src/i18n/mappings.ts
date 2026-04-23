import i18n from './config'

export const classificationMap: Record<string, string> = {
  'أستشاري': 'Consultant',
  'أخصائي': 'Specialist',
  'بروفيسور': 'Professor',
  'طبيب عام': 'General Practitioner',
  'نائب': 'Resident',
}

export const availabilityMap: Record<string, string> = {
  'متواجد': 'Available',
  'إجازة': 'On Leave',
  'مكتفية': 'Full',
  'متواجد ويحتاج دعم': 'Available (Needs Support)',
  'تنبيه': 'Alert',
  'لايوجد عيادة': 'No Clinic',
}

export const insuranceMap: Record<string, string> = {
  'يقبل تأمين': 'Accepts Insurance',
  'لا يقبل تأمين': 'Does Not Accept Insurance',
  'الاطلاع على الملاحظة': 'See Notes',
}

export const clinicMechanismMap: Record<string, string> = {
  'الأولوية بقص الفاتورة': 'Priority by Invoice',
  'بالمواعيد': 'By Appointment',
  'توزيع الارقام قبل العياده': 'Numbers Distributed Before Clinic',
  'بالمواعيد والأرقام': 'Appointments & Numbers',
  'حجز موعد مسبق': 'Pre-booking Required',
  'قائمة الانتظار': 'Waiting List',
}

const maps: Record<string, Record<string, string>> = {
  classification: classificationMap,
  availability: availabilityMap,
  insurance: insuranceMap,
  clinicMechanism: clinicMechanismMap,
}

/**
 * Translate a backend Arabic value to English when in EN mode.
 * Returns the original value in AR mode.
 */
export function tValue(value: string | null | undefined, mapKey: string): string {
  if (!value) return ''
  if (i18n.language === 'ar') return value
  const map = maps[mapKey]
  return map?.[value] ?? value
}
