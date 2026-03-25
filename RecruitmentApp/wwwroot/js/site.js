// Auto-submit search form on input
document.querySelectorAll('.search-input').forEach(input => {
    let timer;
    input.addEventListener('input', function () {
        clearTimeout(timer);
        timer = setTimeout(() => this.closest('form').submit(), 400);
    });
});

// Dismiss alerts after 4 seconds
const alerts = document.querySelectorAll('.alert-success, .alert-error');
alerts.forEach(alert => {
    setTimeout(() => {
        alert.style.transition = 'opacity 0.5s';
        alert.style.opacity = '0';
        setTimeout(() => alert.remove(), 500);
    }, 4000);
});
