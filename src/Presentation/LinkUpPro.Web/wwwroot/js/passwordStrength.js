document.addEventListener('DOMContentLoaded', function () {
    const passwordInput = document.getElementById('passwordInput');
    const wrap = document.getElementById('passwordStrengthWrap');
    const bar = document.getElementById('passwordStrengthBar');
    const text = document.getElementById('passwordStrengthText');

    if (!passwordInput) return;

    passwordInput.addEventListener('input', function () {
        const val = passwordInput.value;
        if (val.length === 0) {
            wrap.style.display = 'none';
            return;
        }

        wrap.style.display = 'block';
        let strength = 0;
        
        if (val.length >= 8) strength += 1;
        if (val.match(/[a-z]+/)) strength += 1;
        if (val.match(/[A-Z]+/)) strength += 1;
        if (val.match(/[0-9]+/)) strength += 1;
        if (val.match(/[^a-zA-Z0-9]+/)) strength += 1; // Caracteres especiales

        bar.className = 'password-strength-fill';
        
        if (strength <= 2) {
            bar.classList.add('weak');
            text.textContent = 'Débil (Requiere mayúsculas, números y símbolos)';
            text.style.color = 'var(--accent-danger)';
        } else if (strength >= 3 && strength <= 4) {
            bar.classList.add('medium');
            text.textContent = 'Media (Agrega símbolos para mayor seguridad)';
            text.style.color = 'var(--accent-warning)';
        } else {
            bar.classList.add('strong');
            text.textContent = 'Fuerte';
            text.style.color = 'var(--accent-success)';
        }
    });
});
