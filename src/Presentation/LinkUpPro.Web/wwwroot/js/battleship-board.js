// Gestor del tablero de Battleship 12x12

const BOARD_SIZE = 12;
let selectedShipSize = 0;
let currentDirection = 0; // 0: Horizontal, 1: Vertical
let placedShipsData = [];
let currentShipBtn = null;

$(document).ready(function () {
    if ($('#setupBoard').length) {
        initSetupBoard();
    }
    
    if ($('#attackBoard').length) {
        initAttackBoard();
    }
});

function initSetupBoard() {
    renderGrid('#setupBoard', true);
    
    // Cargar barcos ya colocados
    if (window.initialShips && window.initialShips.length > 0) {
        placedShipsData = window.initialShips;
        placedShipsData.forEach(ship => {
            markShipOnGrid(ship.StartX, ship.StartY, ship.Size, ship.Direction);
            disableShipButton(ship.Size);
        });
        updateShipsCount();
    }

    // Eventos UI
    $('.ship-select-btn').click(function () {
        if ($(this).hasClass('opacity-50')) return; // Ya colocado
        
        $('.ship-select-btn').removeClass('border-accent bg-accent/10').addClass('border-white/10 bg-white/5');
        $(this).removeClass('border-white/10 bg-white/5').addClass('border-accent bg-accent/10');
        
        selectedShipSize = parseInt($(this).data('size'));
        currentShipBtn = $(this);
    });

    $('#dirHorizontal').click(function() {
        currentDirection = 0;
        $(this).addClass('bg-accent/20 text-accent').removeClass('text-text-muted hover:text-white');
        $('#dirVertical').removeClass('bg-accent/20 text-accent').addClass('text-text-muted hover:text-white');
    });

    $('#dirVertical').click(function() {
        currentDirection = 1;
        $(this).addClass('bg-accent/20 text-accent').removeClass('text-text-muted hover:text-white');
        $('#dirHorizontal').removeClass('bg-accent/20 text-accent').addClass('text-text-muted hover:text-white');
    });

    // Hover effect en celdas
    $('#setupBoard .grid-cell').hover(
        function() {
            if (selectedShipSize === 0) return;
            let x = parseInt($(this).data('x'));
            let y = parseInt($(this).data('y'));
            
            let isValid = checkValidPlacement(x, y, selectedShipSize, currentDirection);
            
            for (let i = 0; i < selectedShipSize; i++) {
                let cx = currentDirection === 0 ? x + i : x;
                let cy = currentDirection === 1 ? y + i : y;
                
                let cell = $(`#setupBoard .grid-cell[data-x="${cx}"][data-y="${cy}"]`);
                if (cell.length) {
                    if (isValid) {
                        cell.addClass('bg-accent/40');
                    } else {
                        cell.addClass('bg-red-500/40 cursor-not-allowed');
                    }
                }
            }
        },
        function() {
            $('#setupBoard .grid-cell').removeClass('bg-accent/40 bg-red-500/40 cursor-not-allowed');
        }
    );

    // Click para colocar
    $('#setupBoard .grid-cell').click(function() {
        if (selectedShipSize === 0) {
            showError("Selecciona un barco primero.");
            return;
        }

        let x = parseInt($(this).data('x'));
        let y = parseInt($(this).data('y'));

        if (!checkValidPlacement(x, y, selectedShipSize, currentDirection)) {
            showError("Posición inválida o colisión con otro barco.");
            return;
        }

        // Llamada AJAX
        let token = $('input[name="__RequestVerificationToken"]').val();
        
        $.ajax({
            url: window.placeShipUrl,
            type: 'POST',
            data: {
                GameId: window.gameId,
                Size: selectedShipSize,
                Direction: currentDirection,
                StartX: x,
                StartY: y,
                __RequestVerificationToken: token
            },
            success: function(response) {
                if (response.success) {
                    hideError();
                    markShipOnGrid(x, y, selectedShipSize, currentDirection);
                    disableShipButton(selectedShipSize);
                    
                    placedShipsData.push(response.ship);
                    updateShipsCount();
                    
                    selectedShipSize = 0;
                    currentShipBtn = null;
                } else {
                    showError(response.message);
                }
            },
            error: function() {
                showError("Error de conexión al servidor.");
            }
        });
    });
}

function renderGrid(containerId, interactive) {
    let html = `<div class="grid grid-cols-13 gap-1 w-full" style="grid-template-columns: repeat(13, minmax(0, 1fr));">`;
    
    // Esquina superior izquierda
    html += `<div class="w-8 h-8 md:w-10 md:h-10"></div>`;
    
    // Letras columnas
    for (let i = 0; i < BOARD_SIZE; i++) {
        html += `<div class="w-8 h-8 md:w-10 md:h-10 flex items-center justify-center font-bold text-accent/50 text-sm">${String.fromCharCode(65 + i)}</div>`;
    }

    // Filas
    for (let y = 0; y < BOARD_SIZE; y++) {
        html += `<div class="w-8 h-8 md:w-10 md:h-10 flex items-center justify-center font-bold text-accent/50 text-sm">${y + 1}</div>`;
        
        for (let x = 0; x < BOARD_SIZE; x++) {
            let cursor = interactive ? "cursor-pointer hover:bg-white/10" : "";
            html += `<div class="grid-cell w-8 h-8 md:w-10 md:h-10 border border-white/10 bg-black/20 rounded-sm transition-colors ${cursor}" data-x="${x}" data-y="${y}"></div>`;
        }
    }
    html += `</div>`;
    $(containerId).html(html);
}

function checkValidPlacement(x, y, size, dir) {
    // Límites
    if (dir === 0 && x + size > BOARD_SIZE) return false;
    if (dir === 1 && y + size > BOARD_SIZE) return false;

    // Colisiones visuales locales
    for (let i = 0; i < size; i++) {
        let cx = dir === 0 ? x + i : x;
        let cy = dir === 1 ? y + i : y;
        let cell = $(`#setupBoard .grid-cell[data-x="${cx}"][data-y="${cy}"]`);
        if (cell.hasClass('has-ship')) return false;
    }
    return true;
}

function markShipOnGrid(x, y, size, dir) {
    for (let i = 0; i < size; i++) {
        let cx = dir === 0 ? x + i : x;
        let cy = dir === 1 ? y + i : y;
        let cell = $(`#setupBoard .grid-cell[data-x="${cx}"][data-y="${cy}"]`);
        
        // Estilos para que parezca un bloque continuo
        let classes = 'bg-accent/80 has-ship ';
        if (dir === 0) {
            if (i === 0) classes += 'rounded-l-full ';
            else if (i === size - 1) classes += 'rounded-r-full ';
        } else {
            if (i === 0) classes += 'rounded-t-full ';
            else if (i === size - 1) classes += 'rounded-b-full ';
        }
        
        cell.removeClass('bg-black/20').addClass(classes);
    }
}

function disableShipButton(size) {
    // Buscar un botón de este tamaño que no esté deshabilitado (para repetidos como el 3)
    let btn = $(`.ship-select-btn[data-size="${size}"]:not(.opacity-50)`).first();
    if (btn.length) {
        btn.removeClass('border-accent bg-accent/10').addClass('opacity-50 pointer-events-none cursor-not-allowed border-white/5 bg-black/20 text-text-muted');
        btn.find('div.bg-accent\\/80').removeClass('bg-accent/80').addClass('bg-white/20');
    }
}

function updateShipsCount() {
    let count = placedShipsData.length;
    $('#shipsCount').text(count);
    if (count === 5) {
        $('#confirmContainer').removeClass('hidden');
    }
}

function showError(msg) {
    $('#setupError').text(msg).removeClass('hidden');
    setTimeout(() => { hideError(); }, 5000);
}

function hideError() {
    $('#setupError').addClass('hidden');
}

// Para la pantalla de ataque
function initAttackBoard() {
    renderGrid('#attackBoard', true);
    renderGrid('#myBoard', false); // Solo lectura
    
    // Se llenarán por la vista Razor con llamadas a JS o render directo
}
