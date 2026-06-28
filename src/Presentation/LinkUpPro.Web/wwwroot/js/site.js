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
});
