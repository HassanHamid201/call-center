/**
 * Tadawi Doctor Info — shadcn/ui Design
 * Calendar, Booking Dialog, Interactions
 */

/* ========== CALENDAR ========== */

const arabicMonths = [
  'يناير','فبراير','مارس','إبريل','مايو','يونيو',
  'يوليو','أغسطس','سبتمبر','أكتوبر','نوفمبر','ديسمبر'
];

const availabilityByDay = { 0:'available', 1:'available', 2:'available', 3:'vacation', 4:'available', 5:'limited', 6:'vacation' };
const vacationStart = new Date(2026, 5, 5);
const vacationEnd   = new Date(2026, 5, 15);

let currentDate = new Date();
let selectedDate = null;

function inVacation(d) {
  const c = new Date(d.getFullYear(), d.getMonth(), d.getDate());
  return c >= vacationStart && c <= vacationEnd;
}
function isToday(d) {
  const t = new Date();
  return d.getDate()===t.getDate() && d.getMonth()===t.getMonth() && d.getFullYear()===t.getFullYear();
}
function getStatus(d) {
  if (inVacation(d)) return 'vacation';
  return availabilityByDay[(d.getDay()+1)%7] || 'available';
}

function renderCalendar() {
  const y = currentDate.getFullYear(), m = currentDate.getMonth();
  document.getElementById('calMonthYear').textContent = `${arabicMonths[m]} ${y}`;

  const first = new Date(y, m, 1);
  let start = (first.getDay() + 1) % 7;
  const daysInMonth = new Date(y, m+1, 0).getDate();
  const container = document.getElementById('calDays');
  container.innerHTML = '';

  for (let i = 0; i < start; i++) {
    const e = document.createElement('div');
    e.className = 'cal-cell other';
    container.appendChild(e);
  }

  for (let day = 1; day <= daysInMonth; day++) {
    const date = new Date(y, m, day);
    const status = getStatus(date);
    const cell = document.createElement('button');
    cell.className = `cal-cell ${status}`;
    if (isToday(date)) cell.classList.add('today');
    if (selectedDate && date.getDate()===selectedDate.getDate() && date.getMonth()===selectedDate.getMonth() && date.getFullYear()===selectedDate.getFullYear()) {
      cell.classList.add('selected');
    }
    cell.textContent = day;
    cell.type = 'button';
    cell.onclick = () => selectDate(date);
    container.appendChild(cell);
  }
}

function changeMonth(delta) {
  currentDate.setMonth(currentDate.getMonth() + delta);
  renderCalendar();
}

function selectDate(date) {
  const status = getStatus(date);
  if (status === 'vacation') {
    showToast('هذا اليوم في فترة إجازة الطبيب');
    return;
  }
  selectedDate = date;
  renderCalendar();

  const panel = document.getElementById('selectedDatePanel');
  const title = document.getElementById('selectedDateTitle');
  const slots = document.getElementById('timeSlots');

  const dayNames = ['السبت','الأحد','الإثنين','الثلاثاء','الأربعاء','الخميس','الجمعة'];
  title.textContent = `مواعيد ${dayNames[(date.getDay()+1)%7]} ${date.getDate()} ${arabicMonths[date.getMonth()]}`;

  let times = status==='limited'
    ? ['٩:٠٠ ص','٩:٣٠ ص','١٠:٠٠ ص','١٠:٣٠ ص','١١:٠٠ ص']
    : ['٩:٠٠ ص','٩:٣٠ ص','١٠:٠٠ ص','١٠:٣٠ ص','١١:٠٠ ص','١١:٣٠ ص','٤:٠٠ م','٤:٣٠ م','٥:٠٠ م','٥:٣٠ م','٦:٠٠ م','٦:٣٠ م','٧:٠٠ م','٧:٣٠ م','٨:٠٠ م'];

  const timeMap = {
    '٩:٠٠ ص':'09:00','٩:٣٠ ص':'09:30','١٠:٠٠ ص':'10:00','١٠:٣٠ ص':'10:30',
    '١١:٠٠ ص':'11:00','١١:٣٠ ص':'11:30','٤:٠٠ م':'16:00','٤:٣٠ م':'16:30',
    '٥:٠٠ م':'17:00','٥:٣٠ م':'17:30','٦:٠٠ م':'18:00','٦:٣٠ م':'18:30',
    '٧:٠٠ م':'19:00','٧:٣٠ م':'19:30','٨:٠٠ م':'20:00'
  };

  slots.innerHTML = times.map((t,i) => {
    const booked = i % 4 === 0;
    if (booked) return `<button type="button" class="time-slot booked" disabled>${t}</button>`;
    return `<button type="button" class="time-slot" onclick="pickTime('${timeMap[t]}')">${t}</button>`;
  }).join('');

  panel.style.display = 'block';
  panel.scrollIntoView({ behavior:'smooth', block:'nearest' });
}

function pickTime(time24) {
  const dateInput = document.getElementById('bookingDate');
  const timeSelect = document.getElementById('bookingTime');
  if (selectedDate && dateInput) {
    const y = selectedDate.getFullYear();
    const m = String(selectedDate.getMonth()+1).padStart(2,'0');
    const d = String(selectedDate.getDate()).padStart(2,'0');
    dateInput.value = `${y}-${m}-${d}`;
  }
  if (timeSelect) timeSelect.value = time24;
  openBookingModal();
}

/* ========== DIALOG ========== */

function openBookingModal() {
  document.getElementById('bookingModal').classList.add('open');
  document.body.style.overflow = 'hidden';
}
function closeBookingModal(e) {
  if (e && e.target !== e.currentTarget && e.type === 'click') return;
  document.getElementById('bookingModal').classList.remove('open');
  document.body.style.overflow = '';
}
function submitBooking(e) {
  e.preventDefault();
  closeBookingModal();
  document.getElementById('successModal').classList.add('open');
  document.body.style.overflow = 'hidden';
}
function closeSuccessModal() {
  document.getElementById('successModal').classList.remove('open');
  document.body.style.overflow = '';
  document.getElementById('bookingForm')?.reset();
}

/* ========== TOAST ========== */

function showToast(msg) {
  let t = document.querySelector('.toast');
  if (!t) {
    t = document.createElement('div');
    t.className = 'toast';
    document.body.appendChild(t);
  }
  t.textContent = msg;
  t.classList.add('show');
  clearTimeout(t._timer);
  t._timer = setTimeout(() => t.classList.remove('show'), 2500);
}

/* ========== INIT ========== */

document.addEventListener('DOMContentLoaded', () => {
  renderCalendar();

  document.addEventListener('keydown', e => {
    if (e.key === 'Escape') {
      closeBookingModal();
      closeSuccessModal();
    }
  });

  // Copy to clipboard
  document.querySelectorAll('a[href^="tel:"]').forEach(a => {
    a.addEventListener('click', e => {
      const num = a.getAttribute('href').replace('tel:','');
      if (navigator.clipboard) { e.preventDefault(); navigator.clipboard.writeText(num).then(()=>showToast(`تم نسخ: ${num}`)); }
    });
  });
  document.querySelectorAll('a[href^="mailto:"]').forEach(a => {
    a.addEventListener('click', e => {
      const em = a.getAttribute('href').replace('mailto:','');
      if (navigator.clipboard) { e.preventDefault(); navigator.clipboard.writeText(em).then(()=>showToast(`تم نسخ: ${em}`)); }
    });
  });

  // Scroll animations
  const obs = new IntersectionObserver(entries => {
    entries.forEach(en => {
      if (en.isIntersecting) {
        en.target.style.opacity = '1';
        en.target.style.transform = 'translateY(0)';
        obs.unobserve(en.target);
      }
    });
  }, { threshold: 0.1 });

  document.querySelectorAll('.card, .hero, .alert').forEach(el => {
    el.style.opacity = '0';
    el.style.transform = 'translateY(12px)';
    el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
    obs.observe(el);
  });
});
