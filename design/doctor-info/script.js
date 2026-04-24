/**
 * Tadawi Doctor Info Page — Interactions
 */

document.addEventListener('DOMContentLoaded', () => {

  // Smooth scroll for anchor links
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
      e.preventDefault();
      const target = document.querySelector(this.getAttribute('href'));
      if (target) {
        target.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    });
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
    row.addEventListener('mouseenter', () => {
      row.style.background = 'var(--color-gray-50)';
    });
    row.addEventListener('mouseleave', () => {
      row.style.background = '';
    });
  });

});

/**
 * Show a temporary toast notification
 */
function showToast(message) {
  let toast = document.querySelector('.toast-notification');
  if (!toast) {
    toast = document.createElement('div');
    toast.className = 'toast-notification';
    toast.style.cssText = `
      position: fixed;
      bottom: 2rem;
      left: 50%;
      transform: translateX(-50%);
      background: var(--color-gray-800);
      color: #fff;
      padding: 0.75rem 1.5rem;
      border-radius: var(--radius-md);
      font-size: 0.875rem;
      font-weight: 600;
      z-index: 1000;
      opacity: 0;
      transition: opacity 0.3s ease, transform 0.3s ease;
      pointer-events: none;
      white-space: nowrap;
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
