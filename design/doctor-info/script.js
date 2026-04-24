/**
 * Tadawi Doctor Info Page — Interactions
 * Calendar, Booking Modal, Form Handling
 */

// ============================================
// CALENDAR
// ============================================

const arabicMonths = [
  'يناير', 'فبراير', 'مارس', 'إبريل', 'مايو', 'يونيو',
  'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'
];

// Work week starts Saturday, ends Thursday
const workDays = ['السبت', 'الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة'];

// Sample availability data (day of week -> status)
const availabilityMap = {
  0: 'available',   // Saturday
  1: 'available',   // Sunday
  2: 'available',   // Monday
  3: 'vacation',    // Tuesday
  4: 'available',   // Wednesday
  5: 'limited',     // Thursday
  6: 'vacation',    // Friday
};

// Vacation period
const vacationStart = new Date(2026, 5, 5);  // June 5, 2026
const vacationEnd = new Date(2026, 5, 15);   // June 15, 2026

let currentCalDate = new Date();
let selectedDate = null;

function isDateInVacation(date) {
  const d = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  return d >= vacationStart && d <= vacationEnd;
}

function isToday(date) {
  const today = new Date();
  return date.getDate() === today.getDate() &&
         date.getMonth() === today.getMonth() &&
         date.getFullYear() === today.getFullYear();
}

function getDayStatus(date) {
  if (isDateInVacation(date)) return 'vacation';
  const dayOfWeek = date.getDay();
  // Convert JS day (0=Sunday) to our week (0=Saturday)
  const ourDay = (dayOfWeek + 1) % 7;
  return availabilityMap[ourDay] || 'available';
}

function renderCalendar() {
  const year = currentCalDate.getFullYear();
  const month = currentCalDate.getMonth();

  // Update header
  document.getElementById('calMonthYear').textContent = `${arabicMonths[month]} ${year}`;

  const firstDayOfMonth = new Date(year, month, 1);
  const lastDayOfMonth = new Date(year, month + 1, 0);
  const daysInMonth = lastDayOfMonth.getDate();

  // First day of week in this month (0=Sunday in JS)
  let startingDay = firstDayOfMonth.getDay();
  // Convert to our week starting Saturday
  startingDay = (startingDay + 1) % 7;

  const daysContainer = document.getElementById('calDays');
  daysContainer.innerHTML = '';

  // Empty cells for days before start of month
  for (let i = 0; i < startingDay; i++) {
    const emptyCell = document.createElement('div');
    emptyCell.className = 'cal-day other-month';
    daysContainer.appendChild(emptyCell);
  }

  // Days of month
  for (let day = 1; day <= daysInMonth; day++) {
    const date = new Date(year, month, day);
    const status = getDayStatus(date);
    const today = isToday(date);
    const isSelected = selectedDate &&
      date.getDate() === selectedDate.getDate() &&
      date.getMonth() === selectedDate.getMonth() &&
      date.getFullYear() === selectedDate.getFullYear();

    const cell = document.createElement('div');
    cell.className = `cal-day ${status}`;
    if (today) cell.classList.add('today');
    if (isSelected) cell.classList.add('selected');

    cell.innerHTML = `<span class="day-label">${day}</span><span class="day-status"></span>`;
    cell.onclick = () => selectDate(date);
    daysContainer.appendChild(cell);
  }
}

function changeMonth(delta) {
  currentCalDate.setMonth(currentCalDate.getMonth() + delta);
  renderCalendar();
}

function selectDate(date) {
  const status = getDayStatus(date);
  if (status === 'vacation') {
    showToast('هذا اليوم في فترة إجازة الطبيب');
    return;
  }

  selectedDate = date;
  renderCalendar();

  // Show selected date panel
  const panel = document.getElementById('selectedDatePanel');
  const title = document.getElementById('selectedDateTitle');
  const timeSlotsContainer = document.getElementById('timeSlots');

  const dayName = workDays[(date.getDay() + 1) % 7];
  title.textContent = `مواعيد يوم ${dayName} ${date.getDate()} ${arabicMonths[date.getMonth()]}`;

  // Generate time slots based on day
  const ourDay = (date.getDay() + 1) % 7;
  let slots = [];

  if (status === 'limited') {
    // Morning only
    slots = ['٩:٠٠ ص', '٩:٣٠ ص', '١٠:٠٠ ص', '١٠:٣٠ ص', '١١:٠٠ ص'];
  } else {
    // Full day
    slots = [
      '٩:٠٠ ص', '٩:٣٠ ص', '١٠:٠٠ ص', '١٠:٣٠ ص', '١١:٠٠ ص', '١١:٣٠ ص',
      '٤:٠٠ م', '٤:٣٠ م', '٥:٠٠ م', '٥:٣٠ م', '٦:٠٠ م', '٦:٣٠ م', '٧:٠٠ م', '٧:٣٠ م', '٨:٠٠ م'
    ];
  }

  // Randomly mark some as booked
  timeSlotsContainer.innerHTML = slots.map((slot, i) => {
    const booked = i % 4 === 0; // every 4th slot booked
    if (booked) {
      return `<span class="time-slot booked">${slot}</span>`;
    }
    return `<span class="time-slot" onclick="selectTimeSlot(this, '${slot}')">${slot}</span>`;
  }).join('');

  panel.style.display = 'block';
  panel.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

function selectTimeSlot(el, time) {
  document.querySelectorAll('.time-slot').forEach(s => s.classList.remove('selected'));
  el.classList.add('selected');

  // Pre-fill booking modal
  const dateInput = document.getElementById('bookingDate');
  if (selectedDate && dateInput) {
    const y = selectedDate.getFullYear();
    const m = String(selectedDate.getMonth() + 1).padStart(2, '0');
    const d = String(selectedDate.getDate()).padStart(2, '0');
    dateInput.value = `${y}-${m}-${d}`;
  }

  const timeSelect = document.getElementById('bookingTime');
  if (timeSelect) {
    // Convert Arabic time to 24h for select
    const timeMap = {
      '٩:٠٠ ص': '09:00', '٩:٣٠ ص': '09:30', '١٠:٠٠ ص': '10:00', '١٠:٣٠ ص': '10:30',
      '١١:٠٠ ص': '11:00', '١١:٣٠ ص': '11:30', '٤:٠٠ م': '16:00', '٤:٣٠ م': '16:30',
      '٥:٠٠ م': '17:00', '٥:٣٠ م': '17:30', '٦:٠٠ م': '18:00', '٦:٣٠ م': '18:30',
      '٧:٠٠ م': '19:00', '٧:٣٠ م': '19:30', '٨:٠٠ م': '20:00'
    };
    if (timeMap[time]) timeSelect.value = timeMap[time];
  }

  openBookingModal();
}

// ============================================
// BOOKING MODAL
// ============================================

function openBookingModal() {
  const modal = document.getElementById('bookingModal');
  modal.classList.add('active');
  document.body.style.overflow = 'hidden';
}

function closeBookingModal() {
  const modal = document.getElementById('bookingModal');
  modal.classList.remove('active');
  document.body.style.overflow = '';
}

function closeBookingModalOnBackdrop(event) {
  if (event.target === event.currentTarget) {
    closeBookingModal();
  }
}

function submitBooking(event) {
  event.preventDefault();
  closeBookingModal();
  showSuccessModal();
}

// ============================================
// SUCCESS MODAL
// ============================================

function showSuccessModal() {
  const modal = document.getElementById('successModal');
  modal.classList.add('active');
  document.body.style.overflow = 'hidden';
}

function closeSuccessModal() {
  const modal = document.getElementById('successModal');
  modal.classList.remove('active');
  document.body.style.overflow = '';
  // Reset form
  document.getElementById('bookingForm')?.reset();
}

// ============================================
// TOAST
// ============================================

function showToast(message) {
  let toast = document.querySelector('.toast-notification');
  if (!toast) {
    toast = document.createElement('div');
    toast.className = 'toast-notification';
    toast.style.cssText = `
      position: fixed; bottom: 2rem; left: 50%;
      transform: translateX(-50%);
      background: var(--color-gray-800); color: #fff;
      padding: 0.75rem 1.5rem; border-radius: var(--radius-md);
      font-size: 0.875rem; font-weight: 600; z-index: 1000;
      opacity: 0; transition: opacity 0.3s ease, transform 0.3s ease;
      pointer-events: none; white-space: nowrap;
      font-family: var(--font-family);
    `;
    document.body.appendChild(toast);
  }
  toast.textContent = message;
  toast.style.opacity = '1';
  toast.style.transform = 'translateX(-50%) translateY(0)';
  clearTimeout(toast._hideTimer);
  toast._hideTimer = setTimeout(() => {
    toast.style.opacity = '0';
    toast.style.transform = 'translateX(-50%) translateY(10px)';
  }, 2500);
}

// ============================================
// INIT
// ============================================

document.addEventListener('DOMContentLoaded', () => {
  // Render calendar
  renderCalendar();

  // Close modal on Escape key
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
      closeBookingModal();
      closeSuccessModal();
    }
  });

  // Copy phone number to clipboard
  document.querySelectorAll('.info-value.link[href^="tel:"]').forEach(link => {
    link.addEventListener('click', (e) => {
      const phone = link.getAttribute('href').replace('tel:', '');
      if (navigator.clipboard) {
        e.preventDefault();
        navigator.clipboard.writeText(phone).then(() => {
          showToast(`تم نسخ الرقم: ${phone}`);
        });
      }
    });
  });

  // Copy email to clipboard
  document.querySelectorAll('.info-value.link[href^="mailto:"]').forEach(link => {
    link.addEventListener('click', (e) => {
      const email = link.getAttribute('href').replace('mailto:', '');
      if (navigator.clipboard) {
        e.preventDefault();
        navigator.clipboard.writeText(email).then(() => {
          showToast(`تم نسخ البريد: ${email}`);
        });
      }
    });
  });

  // Animate cards on scroll
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.style.opacity = '1';
        entry.target.style.transform = 'translateY(0)';
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.1 });

  document.querySelectorAll('.card, .hero-card, .alert-card').forEach(card => {
    card.style.opacity = '0';
    card.style.transform = 'translateY(12px)';
    card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
    observer.observe(card);
  });

  // Schedule row hover highlight
  document.querySelectorAll('.schedule-row:not(.schedule-header)').forEach(row => {
    row.addEventListener('mouseenter', () => { row.style.background = 'var(--color-gray-50)'; });
    row.addEventListener('mouseleave', () => { row.style.background = ''; });
  });
});
