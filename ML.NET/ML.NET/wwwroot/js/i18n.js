const supportedLangs = ['zh', 'en'];
let messages = {};

async function loadMessages(lang) {
    if (!supportedLangs.includes(lang)) lang = 'zh';
    const res = await fetch(`/locale/${lang}.json`);
    messages = await res.json();
}

function applyI18n() {
    document.querySelectorAll('[data-i18n]').forEach(el => {
        const key = el.getAttribute('data-i18n');
        if (messages[key] !== undefined) el.textContent = messages[key];
    });
}

document.addEventListener('DOMContentLoaded', async () => {
    const sel = document.getElementById('langSelect');
    sel.addEventListener('change', async () => {
        await loadMessages(sel.value);
        applyI18n();
    });
    // 預設載入繁中
    await loadMessages(sel.value);
    applyI18n();
});
