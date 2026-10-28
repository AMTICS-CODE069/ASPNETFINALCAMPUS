// Dark mode toggle - preference is stored in a cookie (state management)
document.getElementById('themeToggle')?.addEventListener('click', function () {
    const html = document.documentElement;
    const next = html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
    html.setAttribute('data-bs-theme', next);
    document.cookie = 'theme=' + next + ';path=/;max-age=31536000';
});
