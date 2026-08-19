// Helper global para inicializar tooltips y popovers de Bootstrap si son necesarios
$(document).ready(function () {
    // Configuración global para AJAX
    $.ajaxSetup({
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("AJAX Error: ", textStatus, errorThrown);
        }
    });

    // Auto-dismiss alertas después de 5 segundos
    setTimeout(function() {
        $('.alert-custom:not(.no-auto-dismiss)').fadeOut('slow', function() {
            $(this).remove();
        });
    }, 5000);

    // Custom Password Visibility Toggle Global
    $('input[type="password"]').each(function() {
        if (!$(this).parent().hasClass('password-wrapper')) {
            $(this).wrap('<div class="password-wrapper" style="position: relative;"></div>');
            $(this).after(`
                <button type="button" class="password-toggle-btn" style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); background: transparent; border: none; color: #94A3B8; cursor: pointer; padding: 0; outline: none; z-index: 10; display: flex; align-items: center; justify-content: center;">
                    <svg class="eye-icon" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                </button>
            `);
            // Add padding to input so text doesn't hide behind icon
            $(this).css('padding-right', '40px');
        }
    });

    $(document).on('click', '.password-toggle-btn', function(e) {
        e.preventDefault(); // Prevent any form submission just in case
        
        const btn = $(this);
        const input = btn.siblings('input')[0];
        
        if (!input) return;

        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';
        
        if (isPassword) {
            btn.html('<svg class="eye-icon" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>');
            btn.css('color', '#00D4FF'); // Accent color when visible
        } else {
            btn.html('<svg class="eye-icon" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>');
            btn.css('color', '#94A3B8');
        }
    });
});
